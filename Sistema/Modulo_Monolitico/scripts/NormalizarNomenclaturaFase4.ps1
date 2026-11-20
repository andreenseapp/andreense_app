# Fase 4 — Normalizacao Auth/Autenticacao, Erp/ERP, Gerais/Geral
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent

function Rename-FolderCaseSensitive {
    param([string]$Path, [string]$NewName)
    if (-not (Test-Path $Path)) { return }
    $parent = Split-Path $Path -Parent
    $current = Split-Path $Path -Leaf
    if ($current -eq $NewName) { return }
    $temp = Join-Path $parent "_rename_$NewName"
    if (Test-Path $temp) { Remove-Item $temp -Recurse -Force }
    Move-Item -LiteralPath $Path -Destination $temp
    Move-Item -LiteralPath $temp -Destination (Join-Path $parent $NewName)
    Write-Host "Renomeado: $Path -> $NewName"
}

$folderRenames = @(
    @{ From = Join-Path $root 'ModuloMonolitico.Infraestrutura\AcessoADados\Repositorios\Erp'; To = 'ERP' },
    @{ From = Join-Path $root 'ModuloMonolitico.Dominio\InterfacesDeRepositorio\Gerais'; To = 'Geral' },
    @{ From = Join-Path $root 'ModuloMonolitico.Dominio\InterfacesDeRepositorio\Auth'; To = 'Autenticacao' },
    @{ From = Join-Path $root 'ModuloMonolitico.Dominio\Entidades\Auth'; To = 'Autenticacao' },
    @{ From = Join-Path $root 'ModuloMonolitico.Aplicacao\CasosDeUso\Auth'; To = 'Autenticacao' },
    @{ From = Join-Path $root 'ModuloMonolitico.Web\Pages\Auth'; To = 'Autenticacao' },
    @{ From = Join-Path $root 'ModuloMonolitico.Web\wwwroot\Contextos\Auth'; To = 'Autenticacao' }
)

foreach ($r in $folderRenames) {
    Rename-FolderCaseSensitive -Path $r.From -NewName $r.To
}

$extensions = @('*.cs', '*.cshtml', '*.js', '*.css', '*.md', '*.html', '*.ps1', '*.json')
$files = Get-ChildItem -Path $root -Recurse -Include $extensions -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git)\\' }

$replacements = @(
    @{ Old = 'Repositorios.ERP'; New = 'Repositorios.ERP' },
    @{ Old = 'ModuloMonolitico.Web.Pages.Autenticacao'; New = 'ModuloMonolitico.Web.Pages.Autenticacao' },
    @{ Old = 'Pages.Autenticacao'; New = 'Pages.Autenticacao' },
    @{ Old = 'ModuloRotas.Autenticacao'; New = 'ModuloRotas.Autenticacao' },
    @{ Old = '~/Contextos/Autenticacao/'; New = '~/Contextos/Autenticacao/' },
    @{ Old = 'Contextos/Autenticacao/'; New = 'Contextos/Autenticacao/' },
    @{ Old = '"/Autenticacao/'; New = '"/Autenticacao/' },
    @{ Old = "'/Autenticacao/"; New = "'/Autenticacao/" },
    @{ Old = ' — /Auth/'; New = ' — /Autenticacao/' },
    @{ Old = 'Path = "/Contextos/Autenticacao"'; New = 'Path = "/Contextos/Autenticacao"' }
)

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    $original = $content
    foreach ($r in $replacements) {
        $content = $content.Replace($r.Old, $r.New)
    }
    if ($content -ne $original) {
        [System.IO.File]::WriteAllText($file.FullName, $content)
        Write-Host "Atualizado: $($file.FullName)"
    }
}

Write-Host 'Fase 4 concluida.'
