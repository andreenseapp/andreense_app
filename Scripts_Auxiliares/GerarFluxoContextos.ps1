$base = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico"
$existingFile = Get-ChildItem -Path "C:\Projeto\Projeto" -Recurse -Filter "Manifesto_Fluxo_Contextos.html" | Select-Object -First 1
$out = $existingFile.FullName

function gf($path, $ext="*.cs") {
    if (!(Test-Path $path)) { return [string[]]@() }
    $r = Get-ChildItem $path -Recurse -File -Filter $ext | Where-Object { $_.Name -notlike "_*" } | Select-Object -ExpandProperty Name
    if ($null -eq $r) { return [string[]]@() }
    return @($r)
}

function col($title, $subtitle, $color, $files) {
    $h  = '<div class="col" style="border-color:' + $color + '44">'
    $h += '<div class="col-title" style="color:' + $color + '">' + $title + '</div>'
    $h += '<div class="col-path">' + $subtitle + '</div>'
    if ($files.Count -eq 0) {
        $h += '<div class="empty">&#8212; ausente</div>'
    } else {
        foreach ($f in $files) { $h += '<div class="fname">' + $f + '</div>' }
    }
    $h += '</div>'
    return $h
}

$ctxs = @("Agendamentos","Api","AreaServicos","Autenticacao","Base","Cidadania",
    "Contatos","ERP","Empresas","Estoque","Fidelizacao","Financeiro","Fiscal",
    "Geral","Gestao","HelpDesk","InfraAdmin","Logistica","Mensagens",
    "Notificacoes","Perfil","Precos","Principal","SaaS","Servicos","Varejo")

$sb = [System.Text.StringBuilder]::new()

foreach ($ctx in $ctxs) {
    $rk = if ($ctx -eq "ERP") { "Erp" } else { $ctx }
    $dk = if ($ctx -eq "ERP") { "Erp" } else { $ctx }

    $IR  = gf "$base\ModuloMonolitico.Dominio\InterfacesDeRepositorio\$ctx"
    $EN  = gf "$base\ModuloMonolitico.Dominio\Entidades\$ctx"
    $RP  = gf "$base\ModuloMonolitico.Infraestrutura\AcessoADados\Repositorios\$rk"
    $dp  = "$base\ModuloMonolitico.Infraestrutura\InjecaoDependencia\Contextos\${dk}Setup.cs"
    $DI  = if (Test-Path $dp) { [string[]]@("${dk}Setup.cs") } else { [string[]]@() }
    $CU  = gf "$base\ModuloMonolitico.Aplicacao\CasosDeUso\$ctx"
    $PG  = @(gf "$base\ModuloMonolitico.Web\Pages\$ctx" "*.cshtml" | Where-Object { $_ -notlike "_*" })
    $CSS = gf "$base\ModuloMonolitico.Web\wwwroot\Contextos\$ctx" "*.css"
    $JS  = gf "$base\ModuloMonolitico.Web\wwwroot\Contextos\$ctx" "*.js"

    $allLayers = @($IR, $EN, $RP, $DI, $CU, $PG)
    $complete = ($allLayers | Where-Object { $_.Count -eq 0 }).Count -eq 0
    $bc = if ($complete) { '#34d399' } else { '#334155' }
    $badge = if ($complete) { '<span class="badge-ok">Stack completa</span>' } else { '<span class="badge-partial">Stack parcial</span>' }

    $card  = '<div class="ctx-card" style="border-color:' + $bc + '">'
    $card += '<div class="ctx-header"><span class="ctx-name">' + $ctx + '</span>' + $badge + '</div>'
    $card += '<div class="flow">'

    $card += '<div class="group">'
    $card += '<div class="group-title" style="background:#6366f115;color:#a5b4fc;border-color:#6366f125">Dominio</div>'
    $card += '<div class="group-cols">'
    $card += col 'Interfaces de Repositorio' "Dominio/InterfacesDeRepositorio/$ctx/" '#818cf8' $IR
    $card += col 'Entidades' "Dominio/Entidades/$ctx/" '#818cf8' $EN
    $card += '</div></div>'

    $card += '<div class="arrow">&#10140;</div>'

    $card += '<div class="group">'
    $card += '<div class="group-title" style="background:#14b8a615;color:#5eead4;border-color:#14b8a625">Infraestrutura</div>'
    $card += '<div class="group-cols">'
    $card += col 'Repositorios' "Infraestrutura/Repositorios/$rk/" '#2dd4bf' $RP
    $card += col 'DI Setup' "InjecaoDependencia/Contextos/" '#2dd4bf' $DI
    $card += '</div></div>'

    $card += '<div class="arrow">&#10140;</div>'

    $card += '<div class="group">'
    $card += '<div class="group-title" style="background:#f59e0b15;color:#fcd34d;border-color:#f59e0b25">Aplicacao — MediatR</div>'
    $card += '<div class="group-cols">'
    $card += col 'Casos de Uso' "Aplicacao/CasosDeUso/$ctx/" '#fbbf24' $CU
    $card += '</div></div>'

    $card += '<div class="arrow">&#10140;</div>'

    $card += '<div class="group">'
    $card += '<div class="group-title" style="background:#38bdf815;color:#7dd3fc;border-color:#38bdf825">Web</div>'
    $card += '<div class="group-cols">'
    $card += col 'Pages (.cshtml)' "Web/Pages/$ctx/" '#38bdf8' $PG
    $card += col 'Assets CSS' "wwwroot/Contextos/$ctx/" '#38bdf8' $CSS
    $card += col 'Assets JS' "wwwroot/Contextos/$ctx/" '#38bdf8' $JS
    $card += '</div></div>'

    $card += '</div></div>'
    [void]$sb.AppendLine($card)
}

$ts  = Get-Date -Format "dd/MM/yyyy HH:mm"
$css = 'body{background:#060b14;color:#e2e8f0;font-family:system-ui,sans-serif;padding:32px 20px;max-width:1900px;margin:0 auto}h1{font-size:26px;font-weight:800;background:linear-gradient(135deg,#38bdf8,#818cf8);-webkit-background-clip:text;-webkit-text-fill-color:transparent;margin-bottom:6px}.sub{font-size:13px;color:#475569;margin-bottom:28px}.ctx-card{background:#0c1220;border:2px solid;border-radius:14px;margin-bottom:32px;overflow:hidden}.ctx-header{display:flex;align-items:center;gap:14px;padding:16px 22px;background:#0f172a;border-bottom:1px solid #1e293b}.ctx-name{font-size:22px;font-weight:800;color:#f1f5f9;letter-spacing:-.3px}.badge-ok{background:rgba(52,211,153,.15);border:1px solid rgba(52,211,153,.3);color:#34d399;font-size:12px;padding:4px 14px;border-radius:20px;font-weight:700}.badge-partial{background:rgba(100,116,139,.12);border:1px solid #334155;color:#64748b;font-size:12px;padding:4px 14px;border-radius:20px}.flow{display:flex;align-items:flex-start;padding:16px;gap:2px;overflow-x:auto}.arrow{font-size:24px;color:#1e3a4a;padding:0 4px;margin-top:48px;flex-shrink:0}.group{flex:1;min-width:180px}.group-title{font-size:11px;font-weight:800;text-transform:uppercase;letter-spacing:1px;padding:7px 10px;border-radius:8px;border:1px solid;margin-bottom:8px;text-align:center}.group-cols{display:flex;gap:6px;align-items:flex-start}.col{flex:1;background:#070d18;border:1px solid;border-radius:10px;padding:12px 14px;min-width:130px}.col-title{font-size:12px;font-weight:700;margin-bottom:4px}.col-path{font-size:10px;color:#2d3f55;font-family:monospace;margin-bottom:10px;word-break:break-all;line-height:1.4;border-bottom:1px solid #111c2e;padding-bottom:8px}.fname{font-size:13px;color:#94a3b8;padding:5px 0;border-bottom:1px solid #0d1626;font-family:monospace;word-break:break-all;line-height:1.4}.fname:last-child{border-bottom:none}.empty{font-size:13px;color:#1e3a4a;font-style:italic;padding:6px 0}'

$html  = '<!DOCTYPE html><html lang="pt-BR"><head><meta charset="UTF-8"><title>Fluxo de Contextos</title><style>' + $css + '</style></head><body>'
$html += '<h1>Fluxo de Contextos por Camada</h1>'
$html += '<p class="sub">Gerado em ' + $ts + ' &mdash; cada card mostra os arquivos reais de cada camada: Dominio &#10140; Infraestrutura &#10140; Aplicacao &#10140; Web</p>'
$html += $sb.ToString()
$html += '</body></html>'

[System.IO.File]::WriteAllText($out, $html, [System.Text.Encoding]::UTF8)
Write-Host "OK: $out"
