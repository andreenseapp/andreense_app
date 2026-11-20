using System.Net;
using System.Text;

var monolito = @"C:\Projeto\Projeto\Sistema\Modulo_Monolitico";
var output = @"C:\Projeto\Projeto\Documentação\Manifestos\Manifesto_Mapa_Contextos_Pastas.html";

var layers = new (string Key, string Title, string Desc, Func<string, string> PathFn)[]
{
    ("dominio_iface", "1 · Domínio — Interfaces de Repositório",
        "Contratos que a aplicação enxerga. Pasta: ModuloMonolitico.Dominio/InterfacesDeRepositorio/{Contexto}/",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Dominio", "InterfacesDeRepositorio", ctx)),
    ("dominio_ent", "2 · Domínio — Entidades",
        "Modelos de negócio. Pasta: ModuloMonolitico.Dominio/Entidades/{Contexto}/ (ou subpastas)",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Dominio", "Entidades", ctx)),
    ("infra_repo", "3 · Infraestrutura — Repositórios (Dapper)",
        "Implementação SQL. Pasta: ModuloMonolitico.Infraestrutura/AcessoADados/Repositorios/{Contexto}/",
        ctx => ResolveInfraRepo(ctx)),
    ("infra_di", "4 · Infraestrutura — Injeção de Dependência",
        "Registro no container. Arquivo: ModuloMonolitico.Infraestrutura/InjecaoDependencia/Contextos/{Contexto}Setup.cs",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Infraestrutura", "InjecaoDependencia", "Contextos", $"{ctx}Setup.cs")),
    ("aplicacao", "5 · Aplicação — Casos de Uso",
        "Comandos, consultas MediatR. Pasta: ModuloMonolitico.Aplicacao/CasosDeUso/{Contexto}/",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Aplicacao", "CasosDeUso", ctx)),
    ("web_pages", "6 · Web — Razor Pages",
        "Telas .cshtml. Pasta: ModuloMonolitico.Web/Pages/{Contexto}/",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Web", "Pages", ctx)),
    ("web_assets", "7 · Web — Assets estáticos",
        "CSS/JS do contexto. Pasta: ModuloMonolitico.Web/wwwroot/Contextos/{Contexto}/",
        ctx => Path.Combine(monolito, "ModuloMonolitico.Web", "wwwroot", "Contextos", ctx)),
};

// Contextos Web/Pages (exceto Shared) + extras conhecidos
var webContexts = Directory.Exists(Path.Combine(monolito, "ModuloMonolitico.Web", "Pages"))
    ? Directory.GetDirectories(Path.Combine(monolito, "ModuloMonolitico.Web", "Pages"))
        .Select(Path.GetFileName)
        .Where(n => n is not null && n != "Shared")
        .Cast<string>()
        .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
        .ToList()
    : new List<string>();

// Aliases: nome da pasta Web → pastas reais em outras camadas
var aliases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
{
    ["ERP"] = ["ERP", "Erp"],
    ["Api"] = ["Api"],
    ["Mensagens"] = ["Mensagens", "Geral"],
    ["Notificacoes"] = ["Notificacoes", "Geral"],
    ["Estoque"] = ["Estoque", "Erp"],
    ["Base"] = ["Base", "Empresas"],
    ["AreaServicos"] = ["AreaServicos", "Servicos"],
    ["Perfil"] = ["Perfil"],
    ["Principal"] = ["Principal"],
};

var crossLinks = new Dictionary<string, List<(string Layer, string Path, string Note)>>(StringComparer.OrdinalIgnoreCase)
{
    ["Mensagens"] = [("dominio_iface", "InterfacesDeRepositorio/Geral/IMensagensRepositorio.cs", "Repositório fica em Geral, não em Mensagens")],
    ["Notificacoes"] = [("dominio_iface", "InterfacesDeRepositorio/Geral/INotificacaoRepositorio.cs", "Repositório fica em Geral")],
    ["Estoque"] = [
        ("aplicacao", "CasosDeUso/Produto/Consultas/ListarProdutosAtivosQuery.cs", "Movimentação de estoque consome produtos do contexto Produto"),
        ("dominio_iface", "InterfacesDeRepositorio/Produto/IProdutoRepositorio.cs", "Estoque usa IProdutoRepositorio — contrato no contexto Produto"),
    ],
    ["Gestao"] = [("infra_repo", "Repositorios/InfraAdmin/DocumentoRepositorio.cs", "DocumentoRepositorio foi movido para InfraAdmin")],
    ["SaaS"] = [("web_pages", "Pages/InfraAdmin/SaaS/", "Telas admin SaaS ficam dentro de InfraAdmin/SaaS")],
    ["Servicos"] = [("dominio_ent", "Entidades/Servicos/", "Entidade Prestador — sem pasta Pages própria")],
    ["ERP"] = [
        ("contexto transversal", "#ctx-Produto", "Menu 'Produtos' aponta para Pages/Produto/ — contexto próprio, não subpasta de ERP"),
    ],
    ["AreaServicos"] = [
        ("contexto transversal", "#ctx-Produto", "Menu 'Catálogo de Serviços' aponta para Pages/Produto/Index.cshtml"),
        ("web_pages", "Pages/Agendamentos/Configuracoes/", "Consome produtos tipo servico para agenda"),
    ],
    ["Agendamentos"] = [
        ("contexto transversal", "#ctx-Produto", "Serviços agendáveis = produtos com tipo_item=servico (contexto Produto)"),
        ("dominio_iface", "InterfacesDeRepositorio/Produto/IProdutoRepositorio.cs", "AgendamentoRepositorio lista serviços via tabela erp_produtos"),
        ("web_pages", "Pages/Produto/Index.cshtml", "Link 'Cadastrar serviço' em Configurações aponta para contexto Produto"),
    ],
    ["Produto"] = [
        ("menu compartilhado", "MenuSistemaDefinicoes.cs", "Aparece no menu ERP ('Produtos') e Área de Serviços ('Catálogo de Serviços') — mesma tela, permissão Produto.Visualizar"),
    ],
};

var sb = new StringBuilder();
sb.AppendLine("<!DOCTYPE html>");
sb.AppendLine("<html lang=\"pt-BR\">");
sb.AppendLine("<head>");
sb.AppendLine("  <meta charset=\"UTF-8\">");
sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
sb.AppendLine("  <title>Árvore genealógica — Contextos × Pastas — Monolito</title>");
sb.AppendLine(Css());
sb.AppendLine("</head>");
sb.AppendLine("<body>");

sb.AppendLine("<div class=\"page\">");
sb.AppendLine("  <header class=\"hero\">");
sb.AppendLine("    <h1>Árvore genealógica dos contextos</h1>");
sb.AppendLine("    <p class=\"lead\">Cada linha é um <strong>contexto de negócio</strong> (nome da pasta em <code>Web/Pages</code>).");
sb.AppendLine("    Da esquerda para a direita você vê como a pasta desse contexto aparece — ou não — em cada camada do monolito.</p>");
sb.AppendLine($"    <p class=\"meta\">Gerado em {DateTime.Now:dd/MM/yyyy HH:mm} · {webContexts.Count} contextos em Web/Pages · caminho base: <code>Sistema/Modulo_Monolitico/</code></p>");
sb.AppendLine("  </header>");

sb.AppendLine("  <section class=\"how\">");
sb.AppendLine("    <h2>Como ler esta árvore</h2>");
sb.AppendLine("    <div class=\"how-grid\">");
foreach (var layer in layers)
{
    sb.AppendLine($"      <div class=\"how-card\"><span class=\"how-num\">{layer.Title.Split('·')[0].Trim()}</span><strong>{layer.Title.Split('·', 2)[1].Trim()}</strong><p>{layer.Desc}</p></div>");
}
sb.AppendLine("    </div>");
    sb.AppendLine("    <p class=\"how-note\">Setas horizontais ligam a <em>mesma linha genealógica</em> do contexto. Caixas tracejadas = pasta ou arquivo ausente nesta camada.");
    sb.AppendLine("    Ligações cruzadas (contexto A usa pasta B) aparecem em bloco separado abaixo da linha, com caminho completo.</p>");
    sb.AppendLine("    <p class=\"how-note\"><strong>Contextos transversais:</strong> alguns contextos (ex.: <code>Produto</code>) aparecem no menu de <em>vários módulos</em> (ERP e Área de Serviços),");
    sb.AppendLine("    mas têm stack própria alinhada em todas as camadas — veja o contexto <a href=\"#ctx-Produto\">Produto</a>.</p>");
    sb.AppendLine("  </section>");

sb.AppendLine(RenderProdutoHubSection(monolito));

// Exemplo detalhado: Autenticacao primeiro
sb.AppendLine("  <nav class=\"toc\">");
sb.AppendLine("    <h2>Índice — ir direto ao contexto</h2>");
sb.AppendLine("    <div class=\"toc-grid\">");
sb.AppendLine("      <a class=\"toc-link toc-link--shared\" href=\"#ctx-Produto\">Produto (hub transversal ERP + Serviços)</a>");
sb.AppendLine("      <a class=\"toc-link toc-link--example\" href=\"#exemplo\">Autenticacao (exemplo completo)</a>");
foreach (var ctx in webContexts.Where(c => !c.Equals("Autenticacao", StringComparison.OrdinalIgnoreCase)))
    sb.AppendLine($"      <a class=\"toc-link\" href=\"#ctx-{ctx}\">{WebUtility.HtmlEncode(ctx)}</a>");
sb.AppendLine("    </div>");
sb.AppendLine("  </nav>");

sb.AppendLine("  <section class=\"example\" id=\"exemplo\">");
sb.AppendLine("    <h2>Exemplo completo — contexto <code>Autenticacao</code></h2>");
sb.AppendLine("    <p>Este é o modelo de leitura. Todas as linhas abaixo seguem o mesmo padrão: sete colunas, nomes reais de pastas e arquivos.</p>");
sb.AppendLine(RenderTreeRow("Autenticacao", layers, aliases, crossLinks, highlight: true));
sb.AppendLine("  </section>");

sb.AppendLine("  <section class=\"all-contexts\">");
sb.AppendLine("    <h2>Todos os contextos (Web/Pages)</h2>");
foreach (var ctx in webContexts.Where(c => !c.Equals("Autenticacao", StringComparison.OrdinalIgnoreCase)))
{
    sb.AppendLine($"    <article class=\"context-block\" id=\"ctx-{ctx}\">");
    sb.AppendLine($"      <h3 class=\"context-name\">Contexto: <code>{WebUtility.HtmlEncode(ctx)}</code></h3>");
    sb.AppendLine(RenderTreeRow(ctx, layers, aliases, crossLinks, highlight: false));
    sb.AppendLine("    </article>");
}
sb.AppendLine("  </section>");

sb.AppendLine("  <section class=\"orphans\">");
sb.AppendLine("    <h2>Pastas fora do par Web/Pages (transversais)</h2>");
sb.AppendLine("    <p>Existem no disco mas não têm pasta homônima em <code>Web/Pages</code>:</p>");
sb.AppendLine("    <ul class=\"orphan-list\">");
foreach (var extra in new[] { "Servicos", "Geral" })
    sb.AppendLine($"      <li><code>{extra}</code> — {DescribeOrphan(extra, monolito)}</li>");
sb.AppendLine("    </ul>");
sb.AppendLine("  </section>");

sb.AppendLine("</div>");
sb.AppendLine("</body></html>");

File.WriteAllText(output, sb.ToString(), new UTF8Encoding(true));
Console.WriteLine("Gerado: " + output);

static string ResolveInfraRepo(string ctx)
{
    var basePath = @"C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Infraestrutura\AcessoADados\Repositorios";
    foreach (var name in new[] { ctx, ctx.Equals("ERP", StringComparison.OrdinalIgnoreCase) ? "Erp" : ctx })
    {
        var p = Path.Combine(basePath, name);
        if (Directory.Exists(p)) return p;
    }
    return Path.Combine(basePath, ctx);
}

static string RenderTreeRow(string ctx, (string Key, string Title, string Desc, Func<string, string> PathFn)[] layers,
    Dictionary<string, string[]> aliases, Dictionary<string, List<(string, string, string)>> crossLinks, bool highlight)
{
    var sb = new StringBuilder();
    var rowClass = highlight ? "gene-row gene-row--example" : "gene-row";
    sb.AppendLine($"    <div class=\"{rowClass}\">");

    for (var i = 0; i < layers.Length; i++)
    {
        var layer = layers[i];
        if (i > 0) sb.AppendLine("      <div class=\"gene-arrow\" aria-hidden=\"true\">→</div>");

        var paths = aliases.TryGetValue(ctx, out var al) ? al.Distinct().ToArray() : [ctx];
        var nodes = new List<string>();
        foreach (var pName in paths)
        {
            var path = layer.PathFn(pName);
            nodes.Add(RenderNode(layer.Key, path, pName, ctx));
        }
        sb.AppendLine("      <div class=\"gene-col\">");
        sb.AppendLine($"        <div class=\"gene-col-title\">{WebUtility.HtmlEncode(layer.Title)}</div>");
        sb.AppendLine(string.Join("\n", nodes));
        sb.AppendLine("      </div>");
    }

    sb.AppendLine("    </div>");

    if (crossLinks.TryGetValue(ctx, out var links))
    {
        sb.AppendLine("    <div class=\"cross-links\">");
        sb.AppendLine("      <div class=\"cross-links-title\">Ligações cruzadas (este contexto usa pastas de outro nome)</div>");
        sb.AppendLine("      <ul>");
        foreach (var (layer, path, note) in links)
        {
            if (path.StartsWith('#'))
                sb.AppendLine($"        <li><strong>{WebUtility.HtmlEncode(layer)}</strong> → <a href=\"{path}\">{WebUtility.HtmlEncode(note)}</a></li>");
            else
                sb.AppendLine($"        <li><strong>{WebUtility.HtmlEncode(layer)}</strong> → <code>{WebUtility.HtmlEncode(path)}</code> — {WebUtility.HtmlEncode(note)}</li>");
        }
        sb.AppendLine("      </ul>");
        sb.AppendLine("    </div>");
    }

    return sb.ToString();
}

static string RenderNode(string layerKey, string path, string folderName, string webContext)
{
    var rel = path.Replace(@"C:\Projeto\Projeto\Sistema\Modulo_Monolitico\", "").Replace('\\', '/');
    var exists = layerKey == "infra_di" ? File.Exists(path) : Directory.Exists(path) || File.Exists(path);

    if (!exists)
    {
        return $"""
            <div class="gene-node gene-node--missing">
              <div class="node-status">Ausente</div>
              <div class="node-path">{WebUtility.HtmlEncode(rel)}</div>
              <div class="node-detail">Não existe no disco para o contexto <code>{WebUtility.HtmlEncode(webContext)}</code></div>
            </div>
            """;
    }

    var sb = new StringBuilder();
    sb.AppendLine("<div class=\"gene-node gene-node--ok\">");
    sb.AppendLine("  <div class=\"node-status\">Presente</div>");
    sb.AppendLine($"  <div class=\"node-path\">{WebUtility.HtmlEncode(rel)}</div>");

    if (File.Exists(path))
    {
        sb.AppendLine("  <ul class=\"node-files\">");
        sb.AppendLine($"    <li><code>{WebUtility.HtmlEncode(Path.GetFileName(path))}</code></li>");
        sb.AppendLine("  </ul>");
    }
    else
    {
        var dirs = SafeEnumDirs(path);
        var files = SafeEnumFiles(path);
        var recursiveLayers = new[] { "aplicacao", "web_pages", "web_assets", "dominio_iface", "dominio_ent", "infra_repo" };

        if (recursiveLayers.Contains(layerKey))
        {
            var allFiles = ListFilesRecursive(path, maxDepth: layerKey is "web_pages" ? 4 : 3);
            if (allFiles.Count > 0)
            {
                sb.AppendLine("  <div class=\"node-subtitle\">Todos os arquivos</div>");
                sb.AppendLine("  <ul class=\"node-files node-files--full\">");
                foreach (var f in allFiles)
                    sb.AppendLine($"    <li><code>{WebUtility.HtmlEncode(f)}</code></li>");
                sb.AppendLine("  </ul>");
            }
        }
        else if (dirs.Count > 0 || files.Count > 0)
        {
            if (dirs.Count > 0)
            {
                sb.AppendLine("  <div class=\"node-subtitle\">Subpastas</div>");
                sb.AppendLine("  <ul class=\"node-dirs\">");
                foreach (var d in dirs)
                    sb.AppendLine($"    <li><code>{WebUtility.HtmlEncode(d)}</code></li>");
                sb.AppendLine("  </ul>");
            }
            if (files.Count > 0)
            {
                sb.AppendLine("  <div class=\"node-subtitle\">Arquivos</div>");
                sb.AppendLine("  <ul class=\"node-files\">");
                foreach (var f in files)
                    sb.AppendLine($"    <li><code>{WebUtility.HtmlEncode(f)}</code></li>");
                sb.AppendLine("  </ul>");
            }
        }
    }

    sb.AppendLine("</div>");
    return sb.ToString();
}

static List<string> ListFilesRecursive(string root, int maxDepth, string prefix = "", int depth = 0)
{
    var result = new List<string>();
    if (!Directory.Exists(root) || depth > maxDepth) return result;
    foreach (var file in Directory.GetFiles(root).OrderBy(f => f))
    {
        var name = Path.GetFileName(file);
        if (name is ".DS_Store") continue;
        result.Add(string.IsNullOrEmpty(prefix) ? name : $"{prefix}{name}");
    }
    if (depth >= maxDepth) return result;
    foreach (var dir in Directory.GetDirectories(root).OrderBy(d => d))
    {
        var dirName = Path.GetFileName(dir) + "/";
        var subPrefix = string.IsNullOrEmpty(prefix) ? dirName : prefix + dirName;
        result.AddRange(ListFilesRecursive(dir, maxDepth, subPrefix, depth + 1));
    }
    return result;
}

static List<string> SafeEnumDirs(string path) =>
    Directory.Exists(path)
        ? Directory.GetDirectories(path).Select(Path.GetFileName).Where(n => n != null).Cast<string>().OrderBy(n => n).ToList()
        : [];

static List<string> SafeEnumFiles(string path) =>
    Directory.Exists(path)
        ? Directory.GetFiles(path).Select(Path.GetFileName).Where(n => n != null).Cast<string>().OrderBy(n => n).ToList()
        : [];

static string RenderProdutoHubSection(string monolito)
{
    var sb = new StringBuilder();
    sb.AppendLine("  <section class=\"shared-cap\" id=\"hub-produto\">");
    sb.AppendLine("    <h2>Contexto <code>Produto</code> — hub transversal</h2>");
    sb.AppendLine("    <p class=\"shared-lead\">Produto físico (mercadoria), insumo e serviço agendável (<code>tipo_item=servico</code>) são a <strong>mesma entidade</strong> (<code>Produto</code> + <code>ProdutoVariacao</code>).");
    sb.AppendLine("    O contexto tem <strong>stack própria alinhada</strong> em todas as camadas — não é subpasta de ERP nem Estoque.</p>");

    sb.AppendLine("    <div class=\"shared-block\">");
    sb.AppendLine("      <h3>Onde aparece no menu (mesma tela, dois módulos)</h3>");
    sb.AppendLine("      <table class=\"menu-table\">");
    sb.AppendLine("        <thead><tr><th>Módulo do menu</th><th>Texto no menu</th><th>Rota canônica</th><th>Arquivo físico</th></tr></thead>");
    sb.AppendLine("        <tbody>");
    sb.AppendLine("          <tr><td><code>ERP</code></td><td>Produtos</td><td><code>/Contextos/Produto</code></td><td><code>Pages/Produto/Index.cshtml</code></td></tr>");
    sb.AppendLine("          <tr><td><code>Área de Serviços</code></td><td>Catálogo de Serviços</td><td><code>/Contextos/Produto</code></td><td><code>Pages/Produto/Index.cshtml</code> (mesmo arquivo)</td></tr>");
    sb.AppendLine("        </tbody></table>");
    sb.AppendLine("      <p class=\"shared-note\">Permissões: <code>Produto.Visualizar</code>, <code>Produto.Cadastrar</code>, <code>Produto.Editar</code>. Rotas legadas <code>/ERP/Produtos/*</code> redirecionam para <code>/Contextos/Produto/*</code>.</p>");
    sb.AppendLine("    </div>");

    sb.AppendLine("    <div class=\"shared-block\">");
    sb.AppendLine("      <h3>Quem consome o contexto Produto (sem tela própria de produto)</h3>");
    sb.AppendLine("      <ul class=\"consumer-list\">");
    sb.AppendLine("        <li><a href=\"#ctx-Agendamentos\">Agendamentos</a> — Configurações → serviços agendáveis = produtos tipo servico</li>");
    sb.AppendLine("        <li><a href=\"#ctx-Estoque\">Estoque</a> — movimentação usa <code>ListarProdutosAtivosQuery</code> e <code>IProdutoRepositorio</code></li>");
    sb.AppendLine("        <li><code>Api/SeletorProdutoVariacao</code> — componente de busca reutilizável</li>");
    sb.AppendLine("        <li><a href=\"#ctx-Precos\">Precos</a> — contextos de preço vinculados a variações de produto</li>");
    sb.AppendLine("      </ul>");
    sb.AppendLine("      <p class=\"shared-note\">Árvore genealógica completa do contexto: <a href=\"#ctx-Produto\">#ctx-Produto</a></p>");
    sb.AppendLine("    </div>");
    sb.AppendLine("  </section>");
    return sb.ToString();
}

static string DescribeOrphan(string name, string monolito)
{
    var parts = new List<string>();
    var iface = Path.Combine(monolito, "ModuloMonolitico.Dominio", "InterfacesDeRepositorio", name);
    var ent = Path.Combine(monolito, "ModuloMonolitico.Dominio", "Entidades", name);
    var repo = Path.Combine(monolito, "ModuloMonolitico.Infraestrutura", "AcessoADados", "Repositorios", name);
    if (Directory.Exists(ent)) parts.Add($"Entidades/{name}");
    if (Directory.Exists(repo)) parts.Add($"Repositorios/{name}");
    if (Directory.Exists(iface)) parts.Add($"InterfacesDeRepositorio/{name}");
    return parts.Count > 0 ? string.Join(", ", parts) : "verificar disco";
}

static string Css() => """
  <style>
    @import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap');
    * { box-sizing: border-box; }
    body {
      margin: 0;
      background: #090d16;
      color: #f4f4f5;
      font-family: 'Outfit', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      font-size: 16px;
      line-height: 1.65;
      font-weight: 400;
      -webkit-font-smoothing: antialiased;
    }
    a {
      color: #fde68a;
      text-decoration: underline;
      text-decoration-color: rgba(253, 230, 138, 0.45);
      text-underline-offset: 3px;
      font-weight: 500;
    }
    a:hover { color: #fef08a; text-decoration-color: #fef08a; }
    .toc-link, .toc-link:hover { text-decoration: none; }
    code {
      font-family: 'JetBrains Mono', ui-monospace, monospace;
      font-size: 0.86em;
      font-weight: 400;
      background: rgba(255,255,255,.09);
      padding: 2px 7px;
      border-radius: 5px;
      color: #fafafa;
    }
    .page { max-width: 100%; padding: 32px 24px 80px; }
    .hero { max-width: 960px; margin-bottom: 48px; }
    .hero h1 {
      font-size: 2.1rem;
      font-weight: 700;
      margin: 0 0 12px;
      color: #fafafa;
      letter-spacing: -0.02em;
    }
    .lead { color: #94a3b8; font-size: 1.08rem; max-width: 820px; font-weight: 400; }
    .meta { color: #64748b; font-size: 0.95rem; margin-top: 16px; }
    h2 { font-size: 1.4rem; margin: 0 0 16px; color: #e2e8f0; font-weight: 600; letter-spacing: -0.01em; }
    h3.context-name {
      font-size: 1.2rem;
      font-weight: 600;
      margin: 0 0 20px;
      padding: 14px 18px;
      background: #1c1c1f;
      border-left: 4px solid #71717a;
      border-radius: 0 10px 10px 0;
      color: #fafafa;
      letter-spacing: -0.01em;
    }
    h3.context-name code { font-size: 1em; color: #fafafa; background: rgba(255,255,255,.1); }
    .how { margin-bottom: 56px; }
    .how-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 12px; margin-bottom: 20px; }
    .how-card { background: #1c1c1f; border: 1px solid #3f3f46; border-radius: 12px; padding: 16px 18px; }
    .how-num { display: inline-block; background: rgba(255,255,255,.08); color: #e4e4e7; font-weight: 600; font-size: 0.78rem; padding: 3px 10px; border-radius: 6px; margin-bottom: 10px; }
    .how-card strong { display: block; margin-bottom: 6px; color: #f1f5f9; font-size: 1rem; font-weight: 600; }
    .how-card p { margin: 0; font-size: 0.92rem; color: #94a3b8; line-height: 1.55; }
    .how-note { color: #94a3b8; font-size: 0.98rem; max-width: 900px; line-height: 1.7; }
    .toc { margin-bottom: 48px; padding: 22px 26px; background: #1c1c1f; border: 1px solid #3f3f46; border-radius: 14px; }
    .toc h2 { font-size: 1.15rem; margin-bottom: 16px; }
    .toc-grid { display: flex; flex-wrap: wrap; gap: 10px; }
    .toc-link {
      display: inline-block;
      padding: 9px 16px;
      background: #27272a;
      border: 1px solid #52525b;
      border-radius: 10px;
      color: #f4f4f5;
      text-decoration: none;
      font-size: 0.95rem;
      font-weight: 500;
      transition: background .15s, border-color .15s, color .15s;
    }
    .toc-link:hover { background: #3f3f46; border-color: #52525b; color: #fff; }
    .toc-link--example { border-color: #52525b; background: rgba(255,255,255,.06); }
    .toc-link--shared { border-color: #71717a; background: rgba(255,255,255,.04); color: #fafafa; }
    .shared-cap { margin-bottom: 64px; padding-bottom: 48px; border-bottom: 2px solid #27272a; }
    .shared-lead { color: #e4e4e7; max-width: 900px; margin-bottom: 28px; font-size: 1.05rem; line-height: 1.7; }
    .shared-block { background: #1c1c1f; border: 1px solid #3f3f46; border-radius: 14px; padding: 22px 26px; margin-bottom: 20px; }
    .shared-block h3 { margin: 0 0 14px; font-size: 1.1rem; color: #f1f5f9; font-weight: 600; }
    .shared-block--pending { border-color: rgba(251,191,36,.35); background: rgba(251,191,36,.04); }
    .shared-block--pending h3 { color: #fcd34d; }
    .shared-note { font-size: 0.95rem; color: #94a3b8; margin: 12px 0 0; line-height: 1.6; }
    .gene-row--shared { background: rgba(255,255,255,.02); border: 1px solid #3f3f46; border-radius: 14px; padding: 16px 8px; margin-top: 16px; }
    .gene-node--warn { background: #1a1a0f; border: 1px solid #854d0e; border-radius: 10px; padding: 12px 14px; margin-top: 8px; }
    .menu-table { width: 100%; border-collapse: collapse; font-size: 0.95rem; margin-top: 8px; }
    .menu-table th { text-align: left; padding: 10px 12px; color: #a1a1aa; border-bottom: 1px solid #3f3f46; font-size: 0.8rem; font-weight: 600; text-transform: uppercase; letter-spacing: .03em; }
    .menu-table td { padding: 11px 12px; border-bottom: 1px solid rgba(63,63,70,.6); vertical-align: top; color: #e4e4e7; }
    .consumer-list, .option-list { color: #cbd5e1; line-height: 1.8; padding-left: 22px; font-size: 0.98rem; }
    .option-list li { margin-bottom: 8px; }
    .example { margin-bottom: 64px; padding-bottom: 48px; border-bottom: 2px solid #3f3f46; }
    .example > p { color: #d4d4d8; margin-bottom: 24px; font-size: 1rem; }
    .context-block { margin-bottom: 56px; padding-bottom: 40px; border-bottom: 1px solid #3f3f46; }
    .gene-row { display: flex; align-items: stretch; gap: 0; overflow-x: auto; padding-bottom: 8px; min-height: 120px; }
    .gene-row--example { background: rgba(255,255,255,.03); border: 1px solid #3f3f46; border-radius: 14px; padding: 16px 8px; }
    .gene-arrow { flex: 0 0 36px; display: flex; align-items: center; justify-content: center; color: #475569; font-size: 1.5rem; font-weight: 300; padding-top: 48px; }
    .gene-col { flex: 0 0 300px; min-width: 280px; max-width: 340px; display: flex; flex-direction: column; gap: 8px; }
    .gene-col-title {
      font-family: 'Outfit', sans-serif;
      font-size: 0.82rem;
      font-weight: 600;
      text-transform: none;
      letter-spacing: 0;
      color: #94a3b8;
      padding: 0 4px 8px;
      border-bottom: 1px solid #3f3f46;
      min-height: 52px;
      line-height: 1.45;
    }
    .gene-node { border-radius: 12px; padding: 14px 16px; flex: 1; }
    .gene-node--ok { background: #222225; border: 1px solid #52525b; }
    .gene-node--missing { background: #1f1818; border: 1px dashed #7f1d1d; opacity: .95; }
    .node-status {
      font-family: 'Outfit', sans-serif;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: .05em;
      margin-bottom: 10px;
    }
    .gene-node--ok .node-status { color: #86efac; }
    .gene-node--missing .node-status { color: #f87171; }
    .node-path {
      font-family: 'Outfit', sans-serif;
      font-size: 0.88rem;
      font-weight: 400;
      color: #e4e4e7;
      word-break: break-word;
      margin-bottom: 10px;
      line-height: 1.5;
    }
    .node-detail { font-size: 0.92rem; color: #94a3b8; line-height: 1.55; }
    .node-subtitle {
      font-family: 'Outfit', sans-serif;
      font-size: 0.78rem;
      font-weight: 600;
      color: #64748b;
      text-transform: none;
      letter-spacing: 0;
      margin: 10px 0 6px;
    }
    .node-files--full { max-height: 420px; overflow-y: auto; }
    .node-dirs, .node-files {
      margin: 0;
      padding-left: 20px;
      font-family: 'Outfit', sans-serif;
      font-size: 0.92rem;
      color: #cbd5e1;
      line-height: 1.65;
    }
    .node-files code { font-size: 0.9em; }
    .node-files li { margin-bottom: 3px; }
    .node-files li.dir { color: #f4f4f5; }
    .node-files code { color: #fafafa; }
    .node-subtree { margin-top: 10px; padding-top: 8px; border-top: 1px solid rgba(255,255,255,.06); }
    .node-subtree-path { font-size: 0.88rem; color: #e4e4e7; margin-bottom: 4px; font-weight: 600; }
    .cross-links {
      margin: 16px 0 0 4px;
      padding: 16px 20px;
      background: #222225;
      border: 1px solid #52525b;
      border-radius: 12px;
      font-family: 'Outfit', sans-serif;
      font-size: 0.98rem;
      line-height: 1.7;
    }
    .cross-links-title {
      font-weight: 600;
      font-size: 1rem;
      color: #fafafa;
      margin-bottom: 10px;
    }
    .cross-links ul { margin: 0; padding-left: 22px; color: #f4f4f5; }
    .cross-links li { margin-bottom: 6px; }
    .cross-links strong { color: #e4e4e7; font-weight: 600; }
    .cross-links code { color: #fafafa; }
    .orphans { margin-top: 48px; }
    .orphan-list { color: #cbd5e1; line-height: 1.8; font-size: 0.98rem; }
    @media (max-width: 900px) {
      .gene-col { flex: 0 0 240px; min-width: 220px; }
      body { font-size: 15px; }
    }
  </style>
  """;
