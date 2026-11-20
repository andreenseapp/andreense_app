using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Dominio.Servicos;
using ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;
using ModuloMonolitico.Infraestrutura.Servicos.Localizacao;
using ModuloMonolitico.Infraestrutura.Servicos.Sistema;
using ModuloMonolitico.Infraestrutura.AcessoADados.Contexto;
using ModuloMonolitico.Infraestrutura.Servicos.Email;
using ModuloMonolitico.Infraestrutura.Servicos.Storage;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Middlewares;
using ModuloMonolitico.Web.Servicos;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando o Modulo Monolitico (Andreense ERP)...");

    var builder = WebApplication.CreateBuilder(args);

    var culturaPtBr = new System.Globalization.CultureInfo("pt-BR");
    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culturaPtBr;
    System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culturaPtBr;

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
        options.SupportedCultures = new[] { culturaPtBr };
        options.SupportedUICultures = new[] { culturaPtBr };
    });

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddRazorPages(options => 
    {
        options.ConfigurarRotasCanonicas();
    })
    .AddMvcOptions(options =>
    {
        options.ModelMetadataDetailsProviders.Add(new ModuloMonolitico.Web.Core.Providers.MensagensPadraoValidacaoProvider());
        
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"O valor '{x}' não é válido para {y}.");
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => $"Um valor para a propriedade '{x}' não foi fornecido.");
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Um valor é obrigatório.");
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "O corpo da requisição é obrigatório.");
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => $"O valor '{x}' não é válido.");
        options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor fornecido é inválido.");
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "O campo deve ser um número.");
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => $"O valor fornecido é inválido para {x}.");
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"O valor '{x}' é inválido.");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => $"O campo {x} deve ser um número.");
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => $"O valor não pode ser nulo.");
    });
    builder.Services.AddControllers();

builder.Services.Configure<OpcoesDiagnosticoDesempenho>(
    builder.Configuration.GetSection("DiagnosticoDesempenho"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ModuloMonolitico.Dominio.InterfacesDeServico.IRegistradorDesempenhoRequisicao, RegistradorDesempenhoRequisicao>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ModuloMonolitico.Aplicacao.Comportamentos.MedicaoDesempenhoPipelineBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(LoginCommandValidador).Assembly);
ValidatorOptions.Global.LanguageManager.Culture = culturaPtBr;

var connectionString = builder.Configuration.GetConnectionString("BancoPrincipal")
    ?? throw new InvalidOperationException("ConnectionString 'BancoPrincipal' não encontrada no appsettings.");

builder.Services.AddDbContext<ModuloMonolitico.Infraestrutura.AcessoADados.Contexto.DbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddContextoAutenticacao(connectionString);
builder.Services.AddContextoEmpresas(connectionString);
builder.Services.AddContextoSaaS(connectionString);

builder.Services.AddContextoGestao(connectionString);
builder.Services.AddContextoInfraAdmin(connectionString);
builder.Services.AddContextoContatos(connectionString);
builder.Services.AddContextoAgendamentos(connectionString);
builder.Services.AddContextoCidadania(connectionString);
builder.Services.AddContextoImobiliaria(connectionString);
builder.Services.AddContextoVeiculos(connectionString);
builder.Services.AddContextoCurriculos(connectionString);

builder.Services.AddContextoProduto(connectionString);
builder.Services.AddContextoEstoque(connectionString);
builder.Services.AddContextoPrecos(connectionString);

builder.Services.AddContextoErp(connectionString);
builder.Services.AddContextoServicos(connectionString);
builder.Services.AddContextoFiscal(connectionString);

builder.Services.AddContextoFidelizacao(connectionString);
builder.Services.AddContextoGeral(connectionString);
builder.Services.AddContextoFinanceiro(connectionString);
builder.Services.AddContextoLogistica(connectionString);
builder.Services.AddContextoVarejo(connectionString);
builder.Services.AddContextoHelpDesk(connectionString);
builder.Services.AddScoped<IServicoEmail, ServicoEmailSmtp>();

var dbOrigem = connectionString.Contains("supabase.com") ? "?? SUPABASE (NUVEM)" : "?? LOCALHOST (DOCKER/POSTGRES)";
Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine($"?? Andreense ERP iniciado no ambiente: {builder.Environment.EnvironmentName.ToUpper()}");
Console.WriteLine($"?? Banco de Dados Ativo: {dbOrigem}");
Console.WriteLine(new string('=', 60) + "\n");

builder.Services.AddHttpClient<IServicoEndereco, ServicoEnderecoViaCep>();
builder.Services.AddSingleton<IServicoToken, ServicoToken>();
builder.Services.AddSingleton<IServicoImpressaoDigital, ServicoImpressaoDigital>();
builder.Services.Configure<ModuloMonolitico.Infraestrutura.Configuracoes.CloudflareConfig>(builder.Configuration.GetSection("CloudflareR2"));
builder.Services.AddScoped<ModuloMonolitico.Dominio.InterfacesDeServico.IServicoArmazenamento, ModuloMonolitico.Infraestrutura.Servicos.Storage.ServicoArmazenamentoCloudflare>();


builder.Services.Configure<ConfiguracaoAutenticacaoGoogle>(builder.Configuration.GetSection("Google"));
builder.Services.AddSingleton<IServicoAutenticacaoGoogle, ServicoAutenticacaoGoogle>();

var googleClientId = builder.Configuration["Google:ClientId"] ?? string.Empty;
if (!string.IsNullOrWhiteSpace(googleClientId) && !googleClientId.StartsWith("PREENCHER"))
    Log.Information("[Google] Login com Google habilitado. ClientId configurado.");
else
    Log.Warning("[Google] Login com Google N?O configurado. Preencha Google:ClientId em appsettings.Development.json. Botão ficar? desabilitado na UI.");

builder.Services.Configure<OpcoesUsuarioMaster>(
    builder.Configuration.GetSection(OpcoesUsuarioMaster.Secao));
builder.Services.Configure<OpcoesAutenticacao>(
    builder.Configuration.GetSection(OpcoesAutenticacao.Secao));
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IServicoLimitacaoRequisicoes, ServicoLimitacaoRequisicoes>();
builder.Services.AddScoped<IServicoCookieAutenticacao, ServicoCookieAutenticacao>();
builder.Services.AddScoped<IServicoIdentidadeJwt, ServicoIdentidadeJwt>();

builder.Services.AddSingleton<ModuloMonolitico.Infraestrutura.Servicos.Autenticacao.ServicoRastreadorSessao>();
builder.Services.AddSingleton<ModuloMonolitico.Web.Servicos.ServicoConfiguracaoApp>();

var jwtChave = builder.Configuration["Jwt:Chave"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtChave)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "andreense-erp",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "andreense-usuarios",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies[EsquemaCookieAutenticacao.TokenAcesso];
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var rastreadorSessao = context.HttpContext.RequestServices.GetRequiredService<ModuloMonolitico.Infraestrutura.Servicos.Autenticacao.ServicoRastreadorSessao>();
            var principal = context.Principal;

            if (principal != null)
            {
                var idSessao = ModuloMonolitico.Web.Core.ExtensoesClaimsPrincipal.ObterIdSessao(principal);
                var usuarioId = ModuloMonolitico.Web.Core.ExtensoesClaimsPrincipal.ObterUsuarioId(principal);

                if (!string.IsNullOrEmpty(idSessao))
                {
                    if (rastreadorSessao.ObterSessao(idSessao)?.Revogada is true)
                    {
                        logger.LogWarning("Token rejeitado para o usuário ID {UserId}: A sessão foi encerrada por um administrador.", usuarioId);
                        context.HttpContext.Items["SessaoRevogada"] = true;
                        context.Fail("Sessão revogada.");
                        rastreadorSessao.RemoverSessao(idSessao);
                        return Task.CompletedTask;
                    }

                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconhecido";
                    var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();
                    string navegador = "Desconhecido";
                    
                    if (!string.IsNullOrEmpty(userAgent))
                    {
                        if (userAgent.Contains("Edg/")) navegador = "Edge";
                        else if (userAgent.Contains("OPR/") || userAgent.Contains("Opera")) navegador = "Opera";
                        else if (userAgent.Contains("Chrome/")) navegador = "Chrome";
                        else if (userAgent.Contains("Firefox/")) navegador = "Firefox";
                        else if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome/")) navegador = "Safari";
                        else navegador = "Navegador Web";
                    }

                    var nome = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Usuário";
                    var login = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Desconhecido";
                    var grupo = ModuloMonolitico.Web.Core.ExtensoesClaimsPrincipal.ObterGrupoCodigo(principal);

                    rastreadorSessao.AtualizarSessao(idSessao, usuarioId, nome, login, grupo, "Padrão", ip, navegador);
                }
            }

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            
            var request = context.HttpContext.Request;
            bool isApi = request.Path.StartsWithSegments("/Api", StringComparison.OrdinalIgnoreCase) || 
                         request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
            bool isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                          request.Headers["Accept"].ToString().Contains("application/json");

            if (isApi || isAjax)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var mensagem = MensagensSistema.Autenticacao.SessaoExpirada;
                return context.Response.WriteAsync(
                    $"{{\"sucesso\":false,\"codigo\":\"SESSAO_EXPIRADA\",\"mensagem\":{System.Text.Json.JsonSerializer.Serialize(mensagem)}}}");
            }

            var redirectUri = ModuloRotas.Autenticacao.Login;
            
            if (context.HttpContext.Items.ContainsKey("SessaoRevogada"))
                redirectUri += "?exp=2";
            else if (context.AuthenticateFailure != null)
                redirectUri += "?exp=1";

            context.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

    app.UseSubdomainRouting();

    app.UseSecurityHeaders(builder.Configuration);

    app.UseRouting();

    app.UseMedicaoDesempenho();

    app.UseRequestLocalization();

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, _, ex) =>
        {
            if (ex != null)
                return Serilog.Events.LogEventLevel.Error;

            var path = httpContext.Request.Path.Value ?? string.Empty;

            if (path.Contains("/InfraAdmin/Logs", StringComparison.OrdinalIgnoreCase))
                return Serilog.Events.LogEventLevel.Verbose;

            if (path.StartsWith("/img/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase))
            {
                return Serilog.Events.LogEventLevel.Verbose;
            }

            return Serilog.Events.LogEventLevel.Information;
        };
    });

    app.UseAuthentication();
    app.UseMiddleware<JwtUsuarioContextMiddleware>();
    app.UseMiddleware<JwtIdentidadeValidacaoMiddleware>();
    app.UseAuthorization();
    app.UseMiddleware<LogContextMiddleware>();
app.UseAssinaturaCheck();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.MapGet("/api/auditoria", async (string tabela, string registroId, IAuditoriaRepositorio repo, HttpContext httpContext, IServicoIdentidadeJwt identidade) =>
{
    if (!identidade.TentarResolverUsuarioId(httpContext.User, out var usuarioId)
        || !identidade.EhUsuarioMaster(usuarioId, identidade.ObterEmail(httpContext.User)))
        return Results.Forbid();

    var logs = await repo.ListarLogsPorRegistroAsync(tabela, registroId);
    return Results.Ok(logs);
}).RequireAuthorization();

app.MapPost("/api/preferencias-tela", async (ModuloMonolitico.Web.Endpoints.RequisicaoAtualizarPreferencia request, IMediator mediator, HttpContext httpContext, IServicoIdentidadeJwt identidade) =>
{
    if (!identidade.TentarResolverUsuarioId(httpContext.User, out var usuarioId))
        return Results.Unauthorized();
    
    var empresaIdString = httpContext.User.FindFirst("empresa_ativa_id")?.Value;
    if (string.IsNullOrEmpty(empresaIdString) || !Guid.TryParse(empresaIdString, out var empresaId))
        return Results.BadRequest("Empresa ativa não identificada.");

    var command = new ModuloMonolitico.Aplicacao.CasosDeUso.Base.Comandos.AtualizarPreferenciaTelaCommand(usuarioId, empresaId, request.CaminhoTela, request.TabelaId, request.ColunasOcultasJson);
    await mediator.Send(command);

    return Results.Ok();
}).RequireAuthorization();

    await ServicoSincronizarRecursosSistema.ExecutarAsync(connectionString);

    using (var conn = new Npgsql.NpgsqlConnection(connectionString))
    {
        await conn.OpenAsync();
        var sqlTabela = """
            CREATE TABLE IF NOT EXISTS preferencias_tela_usuario (
                id UUID PRIMARY KEY,
                usuario_id UUID NOT NULL,
                empresa_id UUID NOT NULL,
                tela_path VARCHAR(255) NOT NULL,
                tabela_id VARCHAR(100) NOT NULL,
                colunas_ocultas_json TEXT NOT NULL DEFAULT '[]',
                data_atualizacao TIMESTAMP NOT NULL
            );
            """;
        using var cmd = new Npgsql.NpgsqlCommand(sqlTabela, conn);
        await cmd.ExecuteNonQueryAsync();

        var sqlImobMigracao = """
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS aceita_pet BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_churrasqueira BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_salao_festas BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_quadra BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_academia BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_portaria_24h BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_varanda BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_playground BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_agua_separada BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_luz_separada BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_entrada_separada BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_cobertura BOOLEAN NOT NULL DEFAULT FALSE;
            ALTER TABLE IF EXISTS imobiliaria_imoveis ADD COLUMN IF NOT EXISTS situacao_obra VARCHAR(20) NOT NULL DEFAULT 'pronto';
            ALTER TABLE IF EXISTS imobiliaria_anuncios ADD COLUMN IF NOT EXISTS codigo_publico VARCHAR(30);

            CREATE TABLE IF NOT EXISTS imobiliaria_amenidades (
                id UUID PRIMARY KEY,
                codigo VARCHAR(50) NOT NULL UNIQUE,
                nome VARCHAR(120) NOT NULL,
                categoria VARCHAR(30) NOT NULL,
                ordem INT NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS imobiliaria_imovel_amenidades (
                imovel_id UUID NOT NULL REFERENCES imobiliaria_imoveis(id) ON DELETE CASCADE,
                amenidade_id UUID NOT NULL REFERENCES imobiliaria_amenidades(id) ON DELETE CASCADE,
                PRIMARY KEY (imovel_id, amenidade_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS ix_imobiliaria_anuncios_codigo_publico
                ON imobiliaria_anuncios (codigo_publico)
                WHERE codigo_publico IS NOT NULL;

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_data_inclusao
                ON imobiliaria_anuncios (data_inclusao DESC);

            CREATE TABLE IF NOT EXISTS imobiliaria_buscas_recentes (
                id UUID PRIMARY KEY,
                usuario_id UUID NOT NULL REFERENCES usuarios(id),
                chave_filtro VARCHAR(64) NOT NULL,
                filtro_json JSONB NOT NULL,
                rotulo VARCHAR(200) NOT NULL,
                data_inclusao TIMESTAMP NOT NULL,
                data_ultima_busca TIMESTAMP NOT NULL
            );

            CREATE UNIQUE INDEX IF NOT EXISTS ix_imobiliaria_buscas_recentes_usuario_chave
                ON imobiliaria_buscas_recentes (usuario_id, chave_filtro);

            CREATE INDEX IF NOT EXISTS ix_imobiliaria_buscas_recentes_usuario_data
                ON imobiliaria_buscas_recentes (usuario_id, data_ultima_busca DESC);

            CREATE TABLE IF NOT EXISTS imoveis_favoritos (
                usuario_id UUID NOT NULL,
                imovel_id UUID NOT NULL,
                data_criacao TIMESTAMP WITH TIME ZONE NOT NULL,
                PRIMARY KEY (usuario_id, imovel_id)
            );

            CREATE TABLE IF NOT EXISTS imoveis_visualizados (
                usuario_id UUID NOT NULL,
                anuncio_id UUID NOT NULL,
                data_visualizacao TIMESTAMP WITH TIME ZONE NOT NULL,
                PRIMARY KEY (usuario_id, anuncio_id)
            );

            CREATE INDEX IF NOT EXISTS ix_imoveis_visualizados_usuario
                ON imoveis_visualizados (usuario_id);

            ALTER TABLE IF EXISTS imobiliaria_anuncios
                ADD COLUMN IF NOT EXISTS eh_destaque BOOLEAN NOT NULL DEFAULT FALSE;

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_destaque
                ON imobiliaria_anuncios (status, tipo_negocio, eh_destaque DESC, data_inclusao DESC);

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_fotos_anuncio_ordem
                ON imobiliaria_fotos (anuncio_id, ordem);

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_listagem
                ON imobiliaria_anuncios (status, tipo_negocio, data_inclusao DESC);

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_imovel_id
                ON imobiliaria_anuncios (imovel_id);

            CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_conta_id
                ON imobiliaria_anuncios (conta_id);

            CREATE INDEX IF NOT EXISTS idx_empresas_conta_id
                ON empresas (conta_id);
            """;
        using var cmdImob = new Npgsql.NpgsqlCommand(sqlImobMigracao, conn);
        await cmdImob.ExecuteNonQueryAsync();

        foreach (var amenidade in ModuloMonolitico.Dominio.Constantes.AmenidadesImobiliariaDefinicoes.Todas)
        {
            using var cmdSeed = new Npgsql.NpgsqlCommand("""
                INSERT INTO imobiliaria_amenidades (id, codigo, nome, categoria, ordem)
                VALUES (@id, @codigo, @nome, @categoria, @ordem)
                ON CONFLICT (codigo) DO NOTHING
                """, conn);
            cmdSeed.Parameters.AddWithValue("id", Guid.NewGuid());
            cmdSeed.Parameters.AddWithValue("codigo", amenidade.Codigo);
            cmdSeed.Parameters.AddWithValue("nome", amenidade.Nome);
            cmdSeed.Parameters.AddWithValue("categoria", amenidade.Categoria);
            cmdSeed.Parameters.AddWithValue("ordem", amenidade.Ordem);
            await cmdSeed.ExecuteNonQueryAsync();
        }

        var sqlMigrarBools = """
            INSERT INTO imobiliaria_imovel_amenidades (imovel_id, amenidade_id)
            SELECT i.id, a.id
            FROM imobiliaria_imoveis i
            CROSS JOIN LATERAL (
                VALUES
                    (i.tem_elevador, 'elevador'),
                    (i.tem_piscina, 'piscina'),
                    (i.aceita_pet, 'aceita_pet'),
                    (i.tem_churrasqueira, 'churrasqueira'),
                    (i.tem_salao_festas, 'salao_festas'),
                    (i.tem_quadra, 'quadra'),
                    (i.tem_academia, 'academia'),
                    (i.tem_portaria_24h, 'portaria_24h'),
                    (i.tem_varanda, 'varanda'),
                    (i.tem_playground, 'playground'),
                    (i.tem_agua_separada, 'agua_separada'),
                    (i.tem_luz_separada, 'luz_separada'),
                    (i.tem_entrada_separada, 'entrada_separada'),
                    (i.tem_cobertura, 'cobertura')
            ) AS m(ativo, codigo)
            INNER JOIN imobiliaria_amenidades a ON a.codigo = m.codigo
            WHERE m.ativo = TRUE
            ON CONFLICT DO NOTHING;
            """;
        using var cmdMigrarBools = new Npgsql.NpgsqlCommand(sqlMigrarBools, conn);
        await cmdMigrarBools.ExecuteNonQueryAsync();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro catastrófico ao iniciar o aplicativo.");
}
finally
{
    Log.CloseAndFlush();
}













