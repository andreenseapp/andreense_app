# Gera Manifesto_Mapa_Contextos_Pastas.html
$ErrorActionPreference = "Stop"
$Root = "C:\Projeto\Projeto"
$Mono = Join-Path $Root "Sistema\Modulo_Monolitico"
$Out = Join-Path $Root "Documentação\Manifestos\Manifesto_Mapa_Contextos_Pastas.html"

$Layers = @{
    web_pages    = Join-Path $Mono "ModuloMonolitico.Web\Pages"
    web_contextos= Join-Path $Mono "ModuloMonolitico.Web\wwwroot\Contextos"
    app          = Join-Path $Mono "ModuloMonolitico.Aplicacao\CasosDeUso"
    dom_repo     = Join-Path $Mono "ModuloMonolitico.Dominio\InterfacesDeRepositorio"
    dom_ent      = Join-Path $Mono "ModuloMonolitico.Dominio\Entidades"
    infra_repo   = Join-Path $Mono "ModuloMonolitico.Infraestrutura\AcessoADados\Repositorios"
    infra_di     = Join-Path $Mono "ModuloMonolitico.Infraestrutura\InjecaoDependencia\Contextos"
}

function Get-Dirs($path) {
    if (-not (Test-Path $path)) { return @() }
    Get-ChildItem -Directory $path | Select-Object -ExpandProperty Name
}
function Test-RelDir($base, $rel) { Test-Path (Join-Path $base ($rel -replace '/','\')) }
function Count-Cs($base, $rel) {
    $p = Join-Path $base ($rel -replace '/','\')
    if (-not (Test-Path $p)) { return 0 }
    return (Get-ChildItem $p -Recurse -Filter *.cs -EA SilentlyContinue).Count
}
function Count-PageAssets($base, $rels) {
    $css = 0; $js = 0
    foreach ($rel in $rels) {
        $p = Join-Path $base ($rel -replace '/','\')
        if (Test-Path $p) {
            $css += (Get-ChildItem $p -Recurse -Filter *.css -EA SilentlyContinue).Count
            $js  += (Get-ChildItem $p -Recurse -Filter *.js -EA SilentlyContinue).Count
        }
    }
    return @{ css = $css; js = $js }
}
function Count-CtxAssets($base, $name) {
    $p = Join-Path $base $name
    if (-not (Test-Path $p)) { return @{ css = 0; js = 0; empty = $false } }
    $css = (Get-ChildItem $p -Recurse -Filter *.css -EA SilentlyContinue).Count
    $js  = (Get-ChildItem $p -Recurse -Filter *.js -EA SilentlyContinue).Count
    return @{ css = $css; js = $js; empty = ($css + $js -eq 0) }
}
function Cell($ok, $detail = "", $warn = $false) {
    if ($ok -and $warn) { return "<span class=`"warn`">⚠ $detail</span>" }
    if ($ok) { if ($detail) { return "<span class=`"ok`">✔ $detail</span>" } else { return '<span class="ok">✔</span>' } }
    return '<span class="miss">✘</span>'
}

$Contexts = [ordered]@{
    "Agendamentos" = @{ rota="/Contextos/Agendamentos"; web=@("Agendamentos"); app=@("Agendamentos"); dom=@("Agendamentos"); infra=@("Agendamentos"); di="AgendamentosSetup"; asset="Agendamentos" }
    "Api" = @{ rota="/Contextos/Api"; web=@("Api"); app=@(); dom=@(); infra=@(); di=$null; asset="Api"; nota="Endpoints auxiliares" }
    "AreaServicos" = @{ rota="/Contextos/AreaServicos"; web=@("AreaServicos"); app=@(); dom=@(); infra=@(); di="ServicosSetup"; asset="AreaServicos"; nota="Pasta Web AreaServicos ausente no disco" }
    "Autenticacao" = @{ rota="/Contextos/Autenticacao"; web=@("Auth"); app=@("Auth"); dom=@("Auth"); infra=@("Autenticacao"); di="AutenticacaoSetup"; asset="Auth"; nota="Pages=Auth · Infra=Autenticacao · Asset=Auth" }
    "Auth (Grupos)" = @{ rota="/Contextos/Auth/Grupos"; web=@("Auth/Grupos"); app=@("Auth/Grupos"); dom=@("Auth"); infra=@("Autenticacao"); di="AutenticacaoSetup"; asset="Auth" }
    "Base" = @{ rota="/Contextos/Base"; web=@("Base"); app=@(); dom=@("Base"); infra=@("Base"); di=$null; asset="Base"; nota="Entidades transversais" }
    "Cidadania" = @{ rota="/Contextos/Cidadania"; web=@("Cidadania","Cidade"); app=@("Cidadania"); dom=@("Cidadania"); infra=@("Cidadania"); di="CidadaniaSetup"; asset="Cidadania"; nota="App/Cidadania pasta vazia (0 .cs)" }
    "Contatos" = @{ rota="/Contextos/Contatos"; web=@("Contatos"); app=@("Contatos"); dom=@("Contatos"); infra=@("Contatos"); di="ContatosSetup"; asset="Contatos" }
    "Empresas" = @{ rota="/Contextos/Empresas"; web=@("Empresas"); app=@("Empresas"); dom=@("Empresas"); infra=@("Empresas"); di="EmpresasSetup"; asset="Empresas" }
    "ERP" = @{ rota="/Contextos/ERP"; web=@("ERP"); app=@("ERP"); dom=@("ERP"); infra=@("Erp"); di="ErpSetup"; asset="ERP"; nota="Infra=Erp (case)" }
    "Estoque" = @{ rota="/Contextos/Estoque"; web=@("Estoque","ERP/Estoque","ERP/UnidadesMedida"); app=@("Estoque"); dom=@("Estoque"); infra=@("Estoque","Erp"); di="EstoqueSetup"; asset="Estoque"; nota="ProdutoRepositorio em Infra/Estoque" }
    "Fidelizacao" = @{ rota="/Contextos/Fidelizacao"; web=@("Fidelizacao"); app=@("Fidelizacao"); dom=@("Fidelizacao"); infra=@("Fidelizacao"); di="FidelizacaoSetup"; asset="Fidelizacao" }
    "Financeiro" = @{ rota="/Contextos/Financeiro"; web=@("Financeiro"); app=@("Financeiro"); dom=@(); infra=@("Financeiro"); di="FinanceiroSetup"; asset="Financeiro"; nota="App/Financeiro vazia · Infra tem pasta" }
    "Fiscal" = @{ rota="/Contextos/Fiscal"; web=@("Fiscal"); app=@("Fiscal"); dom=@("Fiscal"); infra=@("Fiscal"); di="FiscalSetup"; asset="Fiscal"; nota="App/Fiscal vazia (0 .cs)" }
    "Geral" = @{ rota="/Contextos/Geral"; web=@("Geral"); app=@("Geral"); dom=@("Gerais"); infra=@("Geral"); di="GeralSetup"; asset="Geral"; nota="Dom=Gerais (plural)" }
    "Gestao" = @{ rota="/Contextos/Gestao"; web=@("Gestao"); app=@("Gestao"); dom=@("Gestao"); infra=@("Gestao"); di="GestaoSetup"; asset="Gestao"; nota="DocumentoRepositorio em Infra/Gestao" }
    "HelpDesk" = @{ rota="/Contextos/HelpDesk"; web=@("HelpDesk"); app=@("HelpDesk"); dom=@(); infra=@("HelpDesk"); di="HelpDeskSetup"; asset="HelpDesk"; nota="App/HelpDesk vazia" }
    "InfraAdmin" = @{ rota="/Contextos/InfraAdmin"; web=@("InfraAdmin"); app=@("InfraAdmin"); dom=@("InfraAdmin"); infra=@("Gestao"); di=$null; asset="InfraAdmin"; nota="Repo Documento em Infra/Gestao" }
    "Logistica" = @{ rota="/Contextos/Logistica"; web=@("Logistica"); app=@("Logistica"); dom=@(); infra=@("Logistica"); di="LogisticaSetup"; asset="Logistica"; nota="App/Logistica vazia" }
    "Mensagens" = @{ rota="/Contextos/Mensagens"; web=@("Mensagens"); app=@(); dom=@("Gerais"); infra=@("Geral"); di="GeralSetup"; asset="Mensagens"; nota="MensagensRepositorio em Infra/Geral" }
    "Notificacoes" = @{ rota="/Contextos/Notificacoes"; web=@("Notificacoes"); app=@(); dom=@("Gerais"); infra=@("Geral"); di="GeralSetup"; asset="Notificacoes" }
    "Perfil" = @{ rota="/Contextos/Perfil"; web=@("Perfil"); app=@(); dom=@(); infra=@(); di=$null; asset="Perfil" }
    "Precos" = @{ rota="/Contextos/Precos"; web=@("Precos","ERP/ContextosPreco"); app=@("Precos"); dom=@("Precos"); infra=@("Precos"); di="PrecosSetup"; asset="Precos" }
    "Principal" = @{ rota="/Contextos/Principal/Hub"; web=@("Principal"); app=@(); dom=@(); infra=@(); di=$null; asset="Principal" }
    "SaaS" = @{ rota="/Contextos/SaaS"; web=@("SaaS","InfraAdmin/SaaS"); app=@("SaaS"); dom=@("SaaS"); infra=@("SaaS"); di="SaaSSetup"; asset="SaaS" }
    "Servicos" = @{ rota="(Agendamentos)"; web=@(); app=@(); dom=@("Servicos"); infra=@(); di="ServicosSetup"; asset="Servicos"; nota="Entidade Prestador · asset folder existe vazia" }
    "Varejo" = @{ rota="/Contextos/Varejo"; web=@("Varejo"); app=@("Varejo"); dom=@(); infra=@("Varejo"); di="VarejoSetup"; asset="Varejo"; nota="App/Varejo vazia" }
}

$webAll = Get-Dirs $Layers.web_pages
$appAll = Get-Dirs $Layers.app
$domAll = Get-Dirs $Layers.dom_repo
$infraAll = Get-Dirs $Layers.infra_repo
$ctxAll = Get-Dirs $Layers.web_contextos

$mappedWeb = [System.Collections.Generic.HashSet[string]]::new()
$mappedApp = [System.Collections.Generic.HashSet[string]]::new()
$mappedDom = [System.Collections.Generic.HashSet[string]]::new()
$mappedInfra = [System.Collections.Generic.HashSet[string]]::new()

$rows = ""
$fullStack = 0
$withGaps = 0
$ctxEmpty = 0

foreach ($ctx in $Contexts.Keys) {
    $c = $Contexts[$ctx]
    $wPaths = $c.web; $aPaths = $c.app; $dPaths = $c.dom; $iPaths = $c.infra
    foreach ($p in $wPaths) { [void]$mappedWeb.Add(($p -split '/')[0]) }
    foreach ($p in $aPaths) { [void]$mappedApp.Add(($p -split '/')[0]) }
    foreach ($p in $dPaths) { [void]$mappedDom.Add($p) }
    foreach ($p in $iPaths) { [void]$mappedInfra.Add($p) }

    $webOk = ($wPaths | Where-Object { Test-RelDir $Layers.web_pages $_ }).Count -gt 0
    $webLbl = ($wPaths | Where-Object { Test-RelDir $Layers.web_pages $_ })[0]
    if (-not $webLbl) { $webLbl = ($wPaths -join ", ") }

    $appCount = 0
    foreach ($p in $aPaths) { $appCount += Count-Cs $Layers.app $p }
    $appOk = $appCount -gt 0

    $domRepoOk = ($dPaths | Where-Object { Test-RelDir $Layers.dom_repo $_ }).Count -gt 0
    $domEntOk  = ($dPaths | Where-Object { Test-RelDir $Layers.dom_ent $_ }).Count -gt 0
    $infraOk   = ($iPaths | Where-Object { Test-RelDir $Layers.infra_repo $_ }).Count -gt 0
    $diOk = $false
    if ($c.di) { $diOk = Test-Path (Join-Path $Layers.infra_di $c.di) }

    $assetName = if ($c.asset) { $c.asset } else { ($ctx -split ' ')[0] }
    $assetFolder = $assetName -in $ctxAll
    $pgA = Count-PageAssets $Layers.web_pages $wPaths
    $cxA = Count-CtxAssets $Layers.web_contextos $assetName
    $assetOk = $assetFolder -and ($cxA.css + $cxA.js -gt 0)
    $assetEmpty = $assetFolder -and ($cxA.css + $cxA.js -eq 0)
    if ($assetEmpty) { $ctxEmpty++ }
    $assetDetail = "pasta:$assetFolder pg:$($pgA.css)c/$($pgA.js)j ctx:$($cxA.css)c/$($cxA.js)j"

    $gaps = @()
    $uiOnly = @("Api","Base","Financeiro","HelpDesk","Logistica","Perfil","Principal","Varejo","AreaServicos","Mensagens","Notificacoes")
    if ($webOk -and -not $appOk -and $ctx -notin $uiOnly) { $gaps += "Sem CasosDeUso" }
    if ($webOk -and -not $domRepoOk -and $dPaths.Count -gt 0) { $gaps += "Sem IRepositorio" }
    if ($webOk -and -not $infraOk -and $iPaths.Count -gt 0) { $gaps += "Sem Repositorio" }
    if (($webOk -or $appOk) -and $c.di -and -not $diOk) { $gaps += "Sem DI Setup" }
    if ($webOk -and -not $assetFolder) { $gaps += "Sem pasta Contextos" }
    elseif ($webOk -and $assetEmpty) { $gaps += "Contextos vazio" }
    if ($appOk -and -not $webOk -and $wPaths.Count -gt 0) { $gaps += "App sem Pages" }
    foreach ($p in $aPaths) { if ((Test-RelDir $Layers.app $p) -and (Count-Cs $Layers.app $p) -eq 0) { $gaps += "App pasta vazia" } }

    if ($gaps.Count -gt 0) { $withGaps++ }
    if ($webOk -and $appOk -and $domRepoOk -and $infraOk) { $fullStack++ }

    $nota = if ($c.nota) { "<br><small class=`"muted`">$($c.nota)</small>" } else { "" }
    $gapHtml = if ($gaps.Count -gt 0) { ($gaps | Select-Object -Unique) -join ", " } else { "—" }
    $rowClass = if ($gaps.Count -gt 0) { "gap-warn" } else { "" }
    $diLbl = if ($diOk) { ($c.di -replace 'Setup','') } else { "" }

    $rows += @"
          <tr class="$rowClass">
            <td><strong>$ctx</strong>$nota<br><small class="muted">$($c.rota)</small><br><small class="layer-path">Web: $($wPaths -join ', ')</small></td>
            <td>$(Cell $webOk $webLbl)</td>
            <td>$(Cell $appOk "$appCount arq")</td>
            <td>$(Cell $domRepoOk 'Repo') $(Cell $domEntOk 'Ent')</td>
            <td>$(Cell $infraOk ($iPaths -join ',')) $(Cell $diOk $diLbl)</td>
            <td>$(Cell $assetFolder $assetDetail $(-not $assetOk -and $assetFolder))</td>
            <td>$gapHtml</td>
          </tr>
"@
}

function OrphanBlock($title, $all, $mapped, $skip) {
    $orphans = $all | Where-Object { $_ -notin $mapped -and $_ -notin $skip } | Sort-Object
    if (-not $orphans) { return "" }
    $lis = ($orphans | ForEach-Object { "<li><code>$_</code></li>" }) -join ""
    return "<div class=`"alert-card`"><div class=`"alert-title`">⚠ $title</div><ul class=`"orphan-list`">$lis</ul></div>"
}

$orphans = (OrphanBlock "Web/Pages fora do mapa" $webAll $mappedWeb @("Shared","Error")) +
           (OrphanBlock "Aplicacao/CasosDeUso fora do mapa" $appAll $mappedApp @()) +
           (OrphanBlock "Dominio/InterfacesDeRepositorio fora do mapa" $domAll $mappedDom @()) +
           (OrphanBlock "Infraestrutura/Repositorios fora do mapa" $infraAll $mappedInfra @("Base"))

$total = $Contexts.Count
$now = Get-Date -Format "dd/MM/yyyy HH:mm"
$ctxFolderCount = $ctxAll.Count

$html = @"
<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Mapa de Contextos e Pastas — Monolito</title>
  <style>
    @import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700&display=swap');
    body { background:#090d16; color:#e2e8f0; font-family:'Outfit',sans-serif; margin:0; padding:40px 20px; display:flex; justify-content:center; }
    .developer-dashboard { background:#0f172a; padding:32px; border-radius:16px; border:1px solid #1e293b; max-width:1440px; width:100%; box-shadow:0 20px 40px rgba(0,0,0,.4); }
    .dashboard-header { display:flex; justify-content:space-between; align-items:center; border-bottom:2px solid #1e293b; padding-bottom:24px; margin-bottom:24px; flex-wrap:wrap; gap:16px; }
    .header-info h2 { margin:0 0 6px; font-size:28px; font-weight:700; background:linear-gradient(135deg,#38bdf8,#818cf8); -webkit-background-clip:text; -webkit-text-fill-color:transparent; }
    .header-info p { margin:0; font-size:14px; color:#94a3b8; }
    .stats-container { display:flex; gap:16px; flex-wrap:wrap; }
    .stat-badge { background:rgba(30,41,59,.7); border:1px solid #334155; padding:10px 20px; border-radius:12px; text-align:center; min-width:90px; }
    .stat-val { display:block; font-size:24px; font-weight:700; color:#38bdf8; }
    .stat-lbl { font-size:11px; color:#64748b; text-transform:uppercase; letter-spacing:.5px; }
    .legend-row { display:flex; flex-wrap:wrap; gap:16px; margin-bottom:24px; background:rgba(15,23,42,.6); padding:16px; border-radius:10px; border:1px solid rgba(255,255,255,.02); font-size:13px; color:#cbd5e1; }
    .ok { color:#34d399; font-weight:600; }
    .miss { color:#f87171; font-weight:700; }
    .warn { color:#fbbf24; font-weight:600; }
    .muted { color:#64748b; font-size:11px; }
    .gap-warn { background:rgba(245,158,11,.04); }
    .hub-section { background:rgba(30,41,59,.4); border:1px solid #1e293b; border-radius:12px; margin-bottom:24px; overflow:hidden; }
    .hub-header { background:rgba(30,41,59,.8); padding:16px 20px; border-bottom:1px solid #1e293b; }
    .hub-title { font-size:18px; font-weight:600; }
    .table-container { padding:16px; overflow-x:auto; }
    .dashboard-table { width:100%; border-collapse:collapse; font-size:13px; }
    .dashboard-table th { text-align:left; padding:12px 14px; color:#64748b; font-weight:600; border-bottom:1px solid #1e293b; text-transform:uppercase; font-size:10px; letter-spacing:.5px; white-space:nowrap; }
    .dashboard-table td { padding:12px 14px; border-bottom:1px solid rgba(30,41,59,.5); vertical-align:top; }
    .dashboard-table tr:hover { background:rgba(255,255,255,.02); }
    .alert-card { background:rgba(239,68,68,.08); border:1px solid rgba(239,68,68,.2); border-radius:12px; padding:16px 20px; margin-bottom:16px; }
    .alert-title { font-weight:600; color:#f87171; margin-bottom:8px; }
    .orphan-list { margin:0; padding-left:20px; color:#fca5a5; font-size:13px; }
    .orphan-list code { background:rgba(0,0,0,.3); padding:2px 6px; border-radius:4px; }
    .note { font-size:12px; color:#94a3b8; margin-top:24px; line-height:1.7; }
    .layer-path { font-family:Consolas,monospace; font-size:10px; color:#64748b; }
  </style>
</head>
<body>
<div class="developer-dashboard">
  <div class="dashboard-header">
    <div class="header-info">
      <h2>Mapa de Contextos × Pastas</h2>
      <p>Domínio → Infraestrutura → Aplicação → Web — detecta pastas ausentes, vazias ou com nomenclatura divergente</p>
      <p class="layer-path">Gerado em $now · ModuloMonolitico · Script: Scripts_Auxiliares/gerar_mapa_contextos_pastas.ps1</p>
    </div>
    <div class="stats-container">
      <div class="stat-badge"><span class="stat-val">$total</span><span class="stat-lbl">Contextos</span></div>
      <div class="stat-badge"><span class="stat-val">$fullStack</span><span class="stat-lbl">Stack completa</span></div>
      <div class="stat-badge"><span class="stat-val">$withGaps</span><span class="stat-lbl">Com lacunas</span></div>
      <div class="stat-badge"><span class="stat-val">$ctxFolderCount</span><span class="stat-lbl">Pastas Contextos</span></div>
      <div class="stat-badge"><span class="stat-val">$ctxEmpty</span><span class="stat-lbl">Contextos vazios</span></div>
    </div>
  </div>

  <div class="legend-row">
    <span><span class="ok">✔</span> Presente</span>
    <span><span class="miss">✘</span> Ausente</span>
    <span><span class="warn">⚠</span> Pasta existe mas vazia / nome divergente</span>
    <span>pg = assets co-localizados em Pages · ctx = wwwroot/Contextos</span>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">📁 Matriz por Bounded Context</div></div>
    <div class="table-container">
      <table class="dashboard-table">
        <thead>
          <tr>
            <th>Contexto / Rota</th>
            <th>Web Pages</th>
            <th>Aplicação</th>
            <th>Domínio</th>
            <th>Infraestrutura</th>
            <th>Assets</th>
            <th>Lacunas</th>
          </tr>
        </thead>
        <tbody>
$rows
        </tbody>
      </table>
    </div>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">🔍 Pastas órfãs (existem no disco, fora do mapa)</div></div>
    <div class="table-container">
      $(if ($orphans) { $orphans } else { '<p class="muted">Nenhuma pasta órfã detectada.</p>' })
    </div>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">📐 Caminhos físicos por camada</div></div>
    <div class="table-container">
      <table class="dashboard-table">
        <thead><tr><th>Camada</th><th>Padrão de pasta</th></tr></thead>
        <tbody>
          <tr><td>Domínio — Interfaces</td><td class="layer-path">ModuloMonolitico.Dominio/InterfacesDeRepositorio/{Contexto}/</td></tr>
          <tr><td>Domínio — Entidades</td><td class="layer-path">ModuloMonolitico.Dominio/Entidades/{Contexto}/</td></tr>
          <tr><td>Infra — Repositories</td><td class="layer-path">ModuloMonolitico.Infraestrutura/AcessoADados/Repositorios/{Contexto}/</td></tr>
          <tr><td>Infra — DI</td><td class="layer-path">ModuloMonolitico.Infraestrutura/InjecaoDependencia/Contextos/{Contexto}Setup.cs</td></tr>
          <tr><td>Aplicação</td><td class="layer-path">ModuloMonolitico.Aplicacao/CasosDeUso/{Contexto}/Comandos|Consultas/</td></tr>
          <tr><td>Web — Pages</td><td class="layer-path">ModuloMonolitico.Web/Pages/{Contexto}/</td></tr>
          <tr><td>Web — Assets (alvo)</td><td class="layer-path">ModuloMonolitico.Web/wwwroot/Contextos/{Contexto}/css|js/</td></tr>
        </tbody>
      </table>
    </div>
  </div>

  <p class="note">
    <strong>Principais inconsistências detectadas:</strong><br>
    • <code>Auth</code> (Web/App/Dom/Asset) ↔ <code>Autenticacao</code> (Infra) — rota <code>/Contextos/Autenticacao</code><br>
    • <code>ERP</code> ↔ <code>Erp</code> (Infra) — Produto em Estoque/, demais em Erp/<br>
    • <code>Geral</code> ↔ <code>Gerais</code> (Dom) — Mensagens/Notificações/Auditoria<br>
    • <code>wwwroot/Contextos/</code> — $ctxFolderCount pastas criadas, todas <strong>sem arquivos</strong> (assets ainda em Pages co-localizados)<br>
    • Pastas App vazias (0 .cs): Cidadania, Financeiro, Fiscal, HelpDesk, Logistica, Varejo<br>
    • Infra tem pastas Financeiro/HelpDesk/Logistica/Varejo sem Domínio/App correspondente<br>
    • Rotas: <code>ModuloMonolitico.Web/Core/ModuloRotas.Paginas.cs</code>
  </p>
</div>
</body>
</html>
"@

$dir = Split-Path $Out -Parent
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
[System.IO.File]::WriteAllText($Out, $html, [System.Text.UTF8Encoding]::new($false))
Write-Host "OK: $Out ($($html.Length) bytes)"
