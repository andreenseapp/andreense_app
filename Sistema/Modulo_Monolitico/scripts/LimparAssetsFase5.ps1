# Fase 5 — Remove assets orfaos apos migracao Fase 3 + rename Fase 4
$ErrorActionPreference = 'Stop'
$web = 'C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web'
$pages = Join-Path $web 'Pages'
$wwwroot = Join-Path $web 'wwwroot\Contextos'

$keepCss = @(
    'Shared\Components\SeletorUsuario\Default.cshtml.css',
    'Shared\Components\SeletorProfissional\Default.cshtml.css',
    'Shared\Components\SeletorProdutoVariacao\Default.cshtml.css'
)

$removedCss = 0
$removedJs = 0

Get-ChildItem $pages -Recurse -Filter '*.cshtml.css' -File | ForEach-Object {
    $rel = $_.FullName.Substring($pages.Length + 1)
    if ($keepCss -contains $rel) { return }
    Remove-Item $_.FullName -Force
    $removedCss++
}

Get-ChildItem $pages -Recurse -Filter '*.cshtml.js' -File | ForEach-Object {
    Remove-Item $_.FullName -Force
    $removedJs++
}

$removedDirs = @()
foreach ($dir in @('Auth', '_rename_Autenticacao')) {
    $path = Join-Path $wwwroot $dir
    if (Test-Path $path) {
        Remove-Item $path -Recurse -Force
        $removedDirs += $dir
    }
}

Write-Host "Removidos: $removedCss .cshtml.css, $removedJs .cshtml.js"
Write-Host "Pastas wwwroot removidas: $($removedDirs -join ', ')"
