$files = Get-ChildItem -Path "c:\Projeto\Projeto\Sistema\Modulo_Monolitico" -Recurse -Include *.cs,*.cshtml
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match "Ã") {
        Write-Host "Fixing $($file.FullName)"
        $bytes = [System.Text.Encoding]::GetEncoding(1252).GetBytes($content)
        $fixedContent = [System.Text.Encoding]::UTF8.GetString($bytes)
        
        # Save fixed content
        $utf8NoBom = New-Object System.Text.UTF8Encoding $False
        [System.IO.File]::WriteAllText($file.FullName, $fixedContent, $utf8NoBom)
    }
}
