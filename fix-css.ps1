$utf8 = New-Object System.Text.UTF8Encoding $false

# 1. SeletorProdutoVariacao
$srcCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorProdutoVariacao\Default.cshtml.css"
$destCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\css\seletorprodutovariacao-default.css"
Move-Item -Path $srcCss -Destination $destCss -Force
$cssContent = [System.IO.File]::ReadAllText($destCss, $utf8)
$cssContent = [Regex]::Replace($cssContent, '(?<=\bspv-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
[System.IO.File]::WriteAllText($destCss, $cssContent, $utf8)

$htmlPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorProdutoVariacao\Default.cshtml"
$htmlContent = [System.IO.File]::ReadAllText($htmlPath, $utf8)
$htmlContent = [Regex]::Replace($htmlContent, '(?<=\bspv-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
if ($htmlContent -notmatch "seletorprodutovariacao-default.css") {
    $htmlContent = $htmlContent -replace '(?s)(<script src="~/Contextos/Shared/js/seletorprodutovariacao-default.js".*?>\s*</script>)', "<link href=`"~/Contextos/Shared/css/seletorprodutovariacao-default.css`" rel=`"stylesheet`" asp-append-version=`"true`" />`n`$1"
}
[System.IO.File]::WriteAllText($htmlPath, $htmlContent, $utf8)

$jsPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\js\seletorprodutovariacao-default.js"
$jsContent = [System.IO.File]::ReadAllText($jsPath, $utf8)
$jsContent = [Regex]::Replace($jsContent, '(?<=\bspv-[a-zA-Z0-9-]+)-\$\{CID\}', '')
$jsContent = [Regex]::Replace($jsContent, '(?<=\bspv-[a-zA-Z0-9-]+)-'' \+ CID', "'")
[System.IO.File]::WriteAllText($jsPath, $jsContent, $utf8)


# 2. SeletorProfissional
$srcCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorProfissional\Default.cshtml.css"
$destCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\css\seletorprofissional-default.css"
Move-Item -Path $srcCss -Destination $destCss -Force
$cssContent = [System.IO.File]::ReadAllText($destCss, $utf8)
$cssContent = [Regex]::Replace($cssContent, '(?<=\bspr-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
[System.IO.File]::WriteAllText($destCss, $cssContent, $utf8)

$htmlPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorProfissional\Default.cshtml"
$htmlContent = [System.IO.File]::ReadAllText($htmlPath, $utf8)
$htmlContent = [Regex]::Replace($htmlContent, '(?<=\bspr-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
if ($htmlContent -notmatch "seletorprofissional-default.css") {
    $htmlContent = $htmlContent -replace '(?s)(<script src="~/Contextos/Shared/js/seletorprofissional-default.js".*?>\s*</script>)', "<link href=`"~/Contextos/Shared/css/seletorprofissional-default.css`" rel=`"stylesheet`" asp-append-version=`"true`" />`n`$1"
}
[System.IO.File]::WriteAllText($htmlPath, $htmlContent, $utf8)

$jsPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\js\seletorprofissional-default.js"
$jsContent = [System.IO.File]::ReadAllText($jsPath, $utf8)
$jsContent = [Regex]::Replace($jsContent, '(?<=\bspr-[a-zA-Z0-9-]+)-\$\{CID\}', '')
$jsContent = [Regex]::Replace($jsContent, '(?<=\bspr-[a-zA-Z0-9-]+)-'' \+ CID', "'")
[System.IO.File]::WriteAllText($jsPath, $jsContent, $utf8)


# 3. SeletorUsuario
$srcCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorUsuario\Default.cshtml.css"
$destCss = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\css\seletorusuario-default.css"
Move-Item -Path $srcCss -Destination $destCss -Force
$cssContent = [System.IO.File]::ReadAllText($destCss, $utf8)
$cssContent = [Regex]::Replace($cssContent, '(?<=\bseu-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
[System.IO.File]::WriteAllText($destCss, $cssContent, $utf8)

$htmlPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared\Components\SeletorUsuario\Default.cshtml"
$htmlContent = [System.IO.File]::ReadAllText($htmlPath, $utf8)
$htmlContent = [Regex]::Replace($htmlContent, '(?<=\bseu-[a-zA-Z0-9-]+)-@\(?cid\)?', '')
if ($htmlContent -notmatch "seletorusuario-default.css") {
    $htmlContent = $htmlContent -replace '(?s)(<script src="~/Contextos/Shared/js/seletorusuario-default.js".*?>\s*</script>)', "<link href=`"~/Contextos/Shared/css/seletorusuario-default.css`" rel=`"stylesheet`" asp-append-version=`"true`" />`n`$1"
}
[System.IO.File]::WriteAllText($htmlPath, $htmlContent, $utf8)

$jsPath = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\wwwroot\Contextos\Shared\js\seletorusuario-default.js"
$jsContent = [System.IO.File]::ReadAllText($jsPath, $utf8)
$jsContent = [Regex]::Replace($jsContent, '(?<=\bseu-[a-zA-Z0-9-]+)-\$\{CID\}', '')
$jsContent = [Regex]::Replace($jsContent, '(?<=\bseu-[a-zA-Z0-9-]+)-'' \+ CID', "'")
[System.IO.File]::WriteAllText($jsPath, $jsContent, $utf8)

Write-Host "Done!"
