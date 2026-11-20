# Migração em lote Fase 3: Pages/**/*.cshtml.css|js -> wwwroot/Contextos/
$ErrorActionPreference = 'Stop'
$web = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web"
$pages = Join-Path $web "Pages"
$wwwroot = Join-Path $web "wwwroot\Contextos"
$comumJs = "(function () { 'use strict'; document.documentElement.setAttribute('data-theme', localStorage.getItem('theme') || 'light'); })();"

# CSS de componentes com @(cid) — permanecem co-localizados (Razor scoped)
$skipCss = @(
    'Shared\Components\SeletorUsuario\Default.cshtml.css',
    'Shared\Components\SeletorProfissional\Default.cshtml.css',
    'Shared\Components\SeletorProdutoVariacao\Default.cshtml.css'
)

function Get-Context([string]$rel) {
    $parts = $rel -split '\\'
    if ($parts[0] -eq 'Shared') { return 'Shared' }
    if ($parts[0] -eq 'Index.cshtml.css' -or $parts[0] -eq 'Index.cshtml.js') { return 'Principal' }
    if ($parts[0] -match '\.cshtml\.(css|js)$') {
        if ($parts[0] -like 'Error*') { return 'Shared' }
        if ($parts[0] -like 'Index*') { return 'Principal' }
    }
    $first = $parts[0]
    if ($first -eq 'Index.cshtml.css') { return 'Principal' }
    $name = Split-Path $rel -Leaf
    if ($name -like 'Index.cshtml.*' -and $parts.Count -eq 1) { return 'Principal' }
    if ($name -like 'Error.cshtml.*') { return 'Shared' }
    return $first
}

function Get-AssetBaseName([string]$rel) {
    $dir = Split-Path $rel -Parent
    $file = Split-Path $rel -Leaf
    $base = $file -replace '\.cshtml\.(css|js)$', ''
    if (-not $dir) {
        if ($base -eq 'Index') { return 'index' }
        if ($base -eq 'Error') { return 'error' }
        return ($base.ToLower() -creplace '([a-z])([A-Z])', '$1-$2').ToLower()
    }
    $segments = @($dir -split '\\')
    if ($segments[0] -eq 'Shared') {
        if ($segments[1] -eq 'Layouts') {
            return ($base -replace '^_', '').ToLower().Replace('_', '-')
        }
        if ($segments[1] -eq 'Partials') {
            return ($base -replace '^_', '').ToLower().Replace('_', '-')
        }
        if ($segments[1] -eq 'Components') {
            $comp = $segments[2].ToLower()
            return "$comp-default"
        }
    }
    if ($segments[0] -eq 'InfraAdmin' -and $segments -contains 'SaaS') {
        $saasParts = @($segments | Where-Object { $_ -ne 'InfraAdmin' -and $_ -ne 'SaaS' })
        $nameParts = @('saas') + $saasParts + @($base)
        return (($nameParts | ForEach-Object { $_.ToLower() }) -join '-' -creplace '_', '-')
    }
    $leaf = $segments[-1]
    if ($base -eq 'Index') { return "$($leaf.ToLower())-index" }
    if ($base -eq 'Novo') { return "$($leaf.ToLower())-novo" }
    if ($base -eq 'Editar') { return "$($leaf.ToLower())-editar" }
    if ($base -eq 'Detalhes') { return "$($leaf.ToLower())-detalhes" }
    if ($base -eq 'Entrada') { return 'entrada' }
    if ($base -eq 'Saida') { return 'saida' }
    if ($base -eq 'Usuarios') { return "$($leaf.ToLower())-usuarios" }
    if ($base -eq 'Criar') { return "$($leaf.ToLower())-criar" }
    return ($base.ToLower() -creplace '([a-z])([A-Z])', '$1-$2').ToLower()
}

$mapping = @{}  # old web path -> new web path

foreach ($ext in @('css', 'js')) {
    Get-ChildItem $pages -Recurse -Filter "*.cshtml.$ext" -File | ForEach-Object {
        $rel = $_.FullName.Substring($pages.Length + 1)
        if ($ext -eq 'css' -and ($skipCss -contains $rel)) { return }

        $ctx = Get-Context $rel
        $name = Get-AssetBaseName $rel
        $destDir = Join-Path $wwwroot "$ctx\$ext"
        New-Item -ItemType Directory -Force -Path $destDir | Out-Null
        $destFile = Join-Path $destDir "$name.$ext"
        Copy-Item $_.FullName $destFile -Force

        $oldWeb = "~/Pages/" + ($rel -replace '\\', '/')
        $newWeb = "~/Contextos/$ctx/$ext/$name.$ext"
        $mapping[$oldWeb] = $newWeb
    }
}

# comum.js por contexto
Get-ChildItem $wwwroot -Directory | ForEach-Object {
    $ctx = $_.Name
    if ($ctx -eq 'Shared') { return }
    $jsDir = Join-Path $_.FullName 'js'
    if (-not (Test-Path $jsDir)) { return }
    $comumPath = Join-Path $jsDir 'comum.js'
    if (-not (Test-Path $comumPath)) {
        Set-Content -Encoding UTF8 $comumPath $comumJs
    }
}

# Referências quebradas conhecidas
$broken = @{
    '~/Pages/ERP/Estoque/' = '~/Contextos/Estoque/'
    '~/Pages/ERP/UnidadesMedida/' = '~/Contextos/Estoque/'
    '~/Pages/ERP/ContextosPreco/' = '~/Contextos/Precos/'
    '~/Pages/Administracao/Equipe/' = '~/Contextos/Base/'
    '~/Pages/MeuPerfil.cshtml.js' = '~/Contextos/Perfil/js/meu-perfil.js'
    '~/Pages/MeuPerfilDesativar.cshtml.js' = '~/Contextos/Perfil/js/meu-perfil-desativar.js'
    '~/Pages/MeuPerfil.cshtml.css' = '~/Contextos/Perfil/css/meu-perfil.css'
    '~/Pages/MeuPerfilDesativar.cshtml.css' = '~/Contextos/Perfil/css/meu-perfil-desativar.css'
}

Get-ChildItem $pages -Recurse -Filter "*.cshtml" -File | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    $orig = $content

    foreach ($k in $broken.Keys) {
        $content = $content.Replace($k, $broken[$k])
    }
    foreach ($k in ($mapping.Keys | Sort-Object { $_.Length } -Descending)) {
        $content = $content.Replace($k, $mapping[$k])
    }

    # Substitui styles bundle por CSS explícito em páginas standalone
    if ($content -match 'ModuloMonolitico\.Web\.styles\.css' -and $content -match 'Layout = null') {
        $relCshtml = $_.FullName.Substring($pages.Length + 1)
        $cssRel = $relCshtml -replace '\.cshtml$', '.cshtml.css'
        $cssRel2 = $relCshtml -replace '\\[^\\]+\.cshtml$', '\Index.cshtml.css'
        $cssWeb = $null
        foreach ($try in @($cssRel, $cssRel2)) {
            $tryWeb = '~/Pages/' + ($try -replace '\\', '/')
            if ($mapping.ContainsKey($tryWeb)) { $cssWeb = $mapping[$tryWeb]; break }
        }
        if ($cssWeb) {
            $content = $content -replace '\s*<link rel="stylesheet" href="~/ModuloMonolitico\.Web\.styles\.css" asp-append-version="true"\s*/>\s*', "`n  <link rel=`"stylesheet`" href=`"$cssWeb`" asp-append-version=`"true`" />`n"
        }
    }

    # Theme-only script -> comum.js (standalone)
    if ($content -match 'Layout = null') {
        $relCshtml = $_.FullName.Substring($pages.Length + 1)
        $ctx = Get-Context $relCshtml
        if ($ctx -ne 'Shared') {
            $jsRel = $relCshtml -replace '\.cshtml$', '.cshtml.js'
            $jsWeb = '~/Pages/' + ($jsRel -replace '\\', '/')
            if ($mapping.ContainsKey($jsWeb)) {
                $jsPath = Join-Path $pages ($jsRel -replace '/', '\')
                if (Test-Path $jsPath) {
                    $jsContent = Get-Content $jsPath -Raw
                    if ($jsContent.Trim() -match '^document\.documentElement\.setAttribute\(''data-theme''') {
                        $comum = "~/Contextos/$ctx/js/comum.js"
                        $content = $content.Replace($mapping[$jsWeb], $comum)
                    }
                }
            }
        }
    }

    # @section Styles vazio -> adicionar CSS se página usa _AppLayout/_ModuleLayout
    if ($content -match '@section Styles \{\s*\}' -or $content -match '@section Styles \{\s*\r?\n\s*\}') {
        $relCshtml = $_.FullName.Substring($pages.Length + 1)
        $cssRel = $relCshtml -replace '\.cshtml$', '.cshtml.css'
        $cssWeb = '~/Pages/' + ($cssRel -replace '\\', '/')
        if ($mapping.ContainsKey($cssWeb)) {
            $link = "  <link rel=`"stylesheet`" href=`"$($mapping[$cssWeb])`" asp-append-version=`"true`" />"
            $content = $content -replace '@section Styles \{\s*\}', "@section Styles {`n$link`n}"
        }
    }

    if ($content -ne $orig) {
        Set-Content -Path $_.FullName -Value $content -Encoding UTF8 -NoNewline
    }
}

# Layouts compartilhados
$appLayout = Join-Path $pages 'Shared\Layouts\_AppLayout.cshtml'
if (Test-Path $appLayout) {
    $c = Get-Content $appLayout -Raw -Encoding UTF8
    $c = $c -replace '<link rel="stylesheet" href="~/ModuloMonolitico\.Web\.styles\.css" asp-append-version="true"\s*/>', @'
<link rel="stylesheet" href="~/Contextos/Shared/css/app-layout.css" asp-append-version="true" />
  <link rel="stylesheet" href="~/ModuloMonolitico.Web.styles.css" asp-append-version="true" />
'@
    Set-Content $appLayout $c -Encoding UTF8 -NoNewline
}

$moduleLayout = Join-Path $pages 'Shared\Layouts\_ModuleLayout.cshtml'
if (Test-Path $moduleLayout) {
    $c = Get-Content $moduleLayout -Raw -Encoding UTF8
    if ($c -notmatch 'ModuloMonolitico\.Web\.styles') {
        $c = $c -replace '(design-system\.css" asp-append-version="true"\s*/>)', "`$1`n  <link rel=`"stylesheet`" href=`"~/Contextos/Shared/css/module-layout.css`" asp-append-version=`"true`" />`n  <link rel=`"stylesheet`" href=`"~/ModuloMonolitico.Web.styles.css`" asp-append-version=`"true`" />"
        Set-Content $moduleLayout $c -Encoding UTF8 -NoNewline
    }
}

# Remover órfãos (exceto seletores com Razor)
$removed = 0
Get-ChildItem $pages -Recurse -Filter "*.cshtml.css" -File | ForEach-Object {
    $rel = $_.FullName.Substring($pages.Length + 1)
    if ($skipCss -contains $rel) { return }
    Remove-Item $_.FullName -Force
    $removed++
}
Get-ChildItem $pages -Recurse -Filter "*.cshtml.js" -File | ForEach-Object {
    Remove-Item $_.FullName -Force
    $removed++
}

Write-Host "Mapeamentos: $($mapping.Count)"
Write-Host "Arquivos removidos de Pages: $removed"
