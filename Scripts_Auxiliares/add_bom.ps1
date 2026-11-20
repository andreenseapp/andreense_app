$utf8Bom = New-Object System.Text.UTF8Encoding $True
$files = Get-ChildItem -Path "c:\Projeto\Projeto\Sistema\Modulo_Monolitico" -Recurse -Include *.cs,*.cshtml
foreach ($file in $files) {
    # Read text, let .NET handle the BOM if it exists
    $content = [System.IO.File]::ReadAllText($file.FullName)
    # Write back WITH BOM
    [System.IO.File]::WriteAllText($file.FullName, $content, $utf8Bom)
}
Write-Host "All files converted to UTF-8 with BOM"
