param(
    [string]$path = "C:\Projeto\Projeto"
)

Write-Host "Iniciando correção de encoding no diretório: $path"

# Encoding UTF-8 com BOM
$utf8Bom = New-Object System.Text.UTF8Encoding $True

# Dicionário de substituições conhecidas
$replacements = @{
    # Double encoding ANSI/UTF-8
    "Ã¡" = "á"
    "Ã¢" = "â"
    "Ã£" = "ã"
    "Ã§" = "ç"
    "Ã©" = "é"
    "Ãª" = "ê"
    "Ã­" = "í"
    "Ã³" = "ó"
    "Ã´" = "ô"
    "Ãµ" = "õ"
    "Ãº" = "ú"
    "Ã " = "À"
    "Ã\x81" = "Á"
    "Ã‡" = "Ç"
    "Ã‰" = "É"
    "ÃŠ" = "Ê"
    "Ã“" = "Ó"
    "Ã”" = "Ô"
    "Ã•" = "Õ"
    "Ãš" = "Ú"
    "Ã\x8D" = "Í"
    "Ãrea" = "Área"

    # Substituições para quando caracteres acentuados foram substituídos por U+FFFD e salvos
    "Gesto" = "Gestão"
    "Gestï¿½o" = "Gestão"
    "Gesto" = "Gestão"
    "avanada" = "avançada"
    "avanï¿½ada" = "avançada"
    "avanada" = "avançada"
    "emisso" = "emissão"
    "emissï¿½o" = "emissão"
    "emisso" = "emissão"
    "rpidas" = "rápidas"
    "rï¿½pidas" = "rápidas"
    "rpidas" = "rápidas"
    "Aplica" = "Aplicaç"
    "Aplicaï¿½" = "Aplicaç"
    "Aplica" = "Aplicaç"
    "No" = "Não"
    "Nï¿½o" = "Não"
    "No" = "Não"
}

# Obtém todos os arquivos .cs e .cshtml
$files = Get-ChildItem -Path $path -Recurse -Include *.cs,*.cshtml -File

$count = 0

foreach ($file in $files) {
    try {
        # Lê o conteúdo bruto como bytes para evitar mais problemas de conversão durante a leitura inicial se estiver bagunçado,
        # mas como é texto corrompido, ler como UTF-8 é geralmente seguro para os textos double-encoded.
        $content = [System.IO.File]::ReadAllText($file.FullName, $utf8Bom)
        
        $modified = $false
        $newContent = $content

        # Checa se o conteúdo contém U+FFFD \uFFFD ou a representação literal ""
        if ($newContent.Contains([char]0xFFFD)) {
            $newContent = $newContent -replace "Gest$([char]0xFFFD)o", "Gestão"
            $newContent = $newContent -replace "Gest$([char]0xFFFD)", "Gestão"
            $newContent = $newContent -replace "avan$([char]0xFFFD)ada", "avançada"
            $newContent = $newContent -replace "emiss$([char]0xFFFD)o", "emissão"
            $newContent = $newContent -replace "r$([char]0xFFFD)pidas", "rápidas"
            $newContent = $newContent -replace "Aplica$([char]0xFFFD)", "Aplicaç"
            $newContent = $newContent -replace "N$([char]0xFFFD)o", "Não"
            $modified = $true
        }

        # Substitui baseado no dicionário
        foreach ($key in $replacements.Keys) {
            # Faz um replace exato (case-sensitive para evitar quebrar mais coisas)
            if ($newContent.Contains($key)) {
                $newContent = $newContent.Replace($key, $replacements[$key])
                $modified = $true
            }
        }
        
        # Também temos que garantir que qualquer palavra "Gesto avançada" (já substituído em parte por avanada) seja consertado
        if ($newContent -match "\bGesto avan") {
            $newContent = $newContent.Replace("Gesto avan", "Gestão avan")
            $modified = $true
        }

        if ($newContent -match "\bemisso de NFs") {
            $newContent = $newContent.Replace("emisso de NFs", "emissão de NFs")
            $modified = $true
        }

        if ($newContent -match "vendas rpidas") {
            $newContent = $newContent.Replace("vendas rpidas", "vendas rápidas")
            $modified = $true
        }

        if ($newContent -match "Aplicaes") {
            $newContent = $newContent.Replace("Aplicaes", "Aplicações")
            $modified = $true
        }

        if ($newContent -match "Acesso a todos os mdulos") {
            $newContent = $newContent.Replace("mdulos", "módulos")
            $modified = $true
        }
        
        # Salva o arquivo sempre, mesmo que não tenha feito replace de string corrompida, 
        # para garantir a assinatura BOM (UTF-8 com BOM).
        [System.IO.File]::WriteAllText($file.FullName, $newContent, $utf8Bom)
        
        if ($modified) {
            Write-Host "Corrigido e salvo (BOM): $($file.FullName)"
            $count++
        } else {
            #Write-Host "Salvo com BOM: $($file.FullName)"
        }
    }
    catch {
        Write-Warning "Erro ao processar $($file.FullName): $_"
    }
}

Write-Host "Processo concluído. $count arquivos tiveram strings corrompidas corrigidas."
Write-Host "Todos os $($files.Count) arquivos avaliados foram salvos em UTF-8 com BOM."
