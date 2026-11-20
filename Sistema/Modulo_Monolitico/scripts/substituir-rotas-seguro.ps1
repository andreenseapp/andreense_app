$ErrorActionPreference = 'Stop'
$root = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages"
$excludeFiles = @(
    '_ContextoStandaloneHead.cshtml',
    '_AuthHead.cshtml'
)

$map = [ordered]@{
    '/InfraAdmin/SaaS/Planos/Editar' = 'ModuloRotas.InfraAdmin.SaaS_Planos_Editar'
    '/InfraAdmin/SaaS/Planos/Novo' = 'ModuloRotas.InfraAdmin.SaaS_Planos_Novo'
    '/InfraAdmin/SaaS/Planos/Index' = 'ModuloRotas.InfraAdmin.SaaS_Planos'
    '/InfraAdmin/GruposSaaS/Usuarios' = 'ModuloRotas.InfraAdmin.GruposSaaS_Usuarios'
    '/InfraAdmin/GruposSaaS/Editar' = 'ModuloRotas.InfraAdmin.GruposSaaS_Editar'
    '/InfraAdmin/GruposSaaS/Novo' = 'ModuloRotas.InfraAdmin.GruposSaaS_Novo'
    '/InfraAdmin/GruposSaaS/Index' = 'ModuloRotas.InfraAdmin.GruposSaaS'
    '/InfraAdmin/Grupos/Editar' = 'ModuloRotas.InfraAdmin.Grupos_Editar'
    '/InfraAdmin/Grupos/Novo' = 'ModuloRotas.InfraAdmin.Grupos_Novo'
    '/InfraAdmin/Grupos/Index' = 'ModuloRotas.InfraAdmin.Grupos'
    '/InfraAdmin/HistoricoLogin/Index' = 'ModuloRotas.InfraAdmin.HistoricoLogin'
    '/InfraAdmin/HistoricoLogin' = 'ModuloRotas.InfraAdmin.HistoricoLogin'
    '/Configuracoes/Fiscal/Parametros/Editar' = 'ModuloRotas.Base.Fiscal_Parametros'
    '/Configuracoes/Fiscal/Parametros/Novo' = 'ModuloRotas.Base.Fiscal_Parametros_Novo'
    '/Configuracoes/Fiscal/Parametros' = 'ModuloRotas.Base.Fiscal_Parametros'
    '/Configuracoes/Fiscal' = 'ModuloRotas.Base.Fiscal_Certificados'
    '/Configuracoes' = 'ModuloRotas.Base.Configuracoes'
    '/Base/Vinculos/Solicitar' = 'ModuloRotas.Base.Vinculos_Solicitar'
    '/Base/Vinculos/Index' = 'ModuloRotas.Base.Vinculos'
    '/Base/Vinculos' = 'ModuloRotas.Base.Vinculos'
    '/Base/Vendedores/Detalhes' = 'ModuloRotas.Base.Vendedores_Detalhes'
    '/Base/Vendedores/Novo' = 'ModuloRotas.Base.Vendedores_Novo'
    '/Base/Vendedores/Editar' = 'ModuloRotas.Base.Vendedores_Editar'
    '/Base/Vendedores/Index' = 'ModuloRotas.Base.Vendedores'
    '/Base/Vendedores' = 'ModuloRotas.Base.Vendedores'
    '/Base/GrupoUsuario/Editar' = 'ModuloRotas.Base.GrupoUsuario_Editar'
    '/Base/GrupoUsuario/Novo' = 'ModuloRotas.Base.GrupoUsuario_Novo'
    '/Base/GrupoUsuario/Index' = 'ModuloRotas.Base.GrupoUsuario'
    '/Base/Estoque/Produtos/Index' = 'ModuloRotas.ERP.Produtos'
    '/TiposVendedor/Editar' = 'ModuloRotas.Base.TiposVendedor_Editar'
    '/TiposVendedor/Novo' = 'ModuloRotas.Base.TiposVendedor_Novo'
    '/TiposVendedor/Index' = 'ModuloRotas.Base.TiposVendedor'
    '/AreaCliente/LerQRCode' = 'ModuloRotas.AreaCliente.LerQRCode'
    '/AreaCliente/MeuCashback' = 'ModuloRotas.AreaCliente.MeuCashback'
    '/AreaCliente/MeusCartoes' = 'ModuloRotas.AreaCliente.MeusCartoes'
    '/AreaCliente/MeusAgendamentos/Novo' = 'ModuloRotas.AreaCliente.MeusAgendamentos_Novo'
    '/AreaCliente/MeusAgendamentos/Detalhes' = 'ModuloRotas.AreaCliente.MeusAgendamentos_Detalhes'
    '/AreaCliente/MeusAgendamentos/Index' = 'ModuloRotas.AreaCliente.MeusAgendamentos'
    '/AreaCliente/Index' = 'ModuloRotas.AreaCliente.Index'
    '/AreaServicos/Agendamentos/Configuracao' = 'ModuloRotas.AreaServicos.Agendamentos_Configuracao'
    '/AreaServicos/Agendamentos/Detalhes' = 'ModuloRotas.AreaServicos.Agendamentos_Detalhes'
    '/AreaServicos/Agendamentos/Novo' = 'ModuloRotas.AreaServicos.Agendamentos_Novo'
    '/AreaServicos/Agendamentos/Index' = 'ModuloRotas.AreaServicos.Agendamentos'
    '/AreaServicos/Index' = 'ModuloRotas.AreaServicos.Index'
    '/Perfil/MeusVinculos' = 'ModuloRotas.Perfil.MeusVinculos'
    '/ERP/Produtos/Novo' = 'ModuloRotas.ERP.Produtos_Novo'
    '/ERP/Produtos/Index' = 'ModuloRotas.ERP.Produtos'
    '/ERP/Index' = 'ModuloRotas.ERP.Index'
    '/CRM/Contatos/Novo' = 'ModuloRotas.CRM.Contatos_Novo'
    '/CRM/Contatos/Index' = 'ModuloRotas.CRM.Contatos'
    '/Contatos/Novo' = 'ModuloRotas.CRM.Contatos_Novo'
    '/Contatos' = 'ModuloRotas.CRM.Contatos'
    '/Cashback/Novo' = 'ModuloRotas.AreaServicos.Cashback_Novo'
    '/Cashback/Index' = 'ModuloRotas.AreaServicos.Cashback'
    '/Fidelidade/Novo' = 'ModuloRotas.AreaServicos.Fidelidade_Novo'
    '/Fidelidade/Index' = 'ModuloRotas.AreaServicos.Fidelidade'
    '/Notificacoes/Index' = 'ModuloRotas.Notificacoes.Index'
    '/Mensagens/Index' = 'ModuloRotas.Mensagens.Index'
    '/Meu-Perfil' = 'ModuloRotas.MeuPerfil.Index'
    '/Principal/Index' = 'ModuloRotas.Principal.Hub'
    '/Index' = 'ModuloRotas.Raiz.Index'
}

function Apply-SafeMap([string]$content, [string]$mode) {
    foreach ($key in $map.Keys) {
        $expr = $map[$key]
        $escaped = [regex]::Escape($key)
        if ($mode -eq 'cshtml') {
            $content = [regex]::Replace($content, "href=`"$escaped`"", "href=`"@$expr`"")
            $content = [regex]::Replace($content, "UrlVoltar = `"$escaped`"", "UrlVoltar = $expr")
            $content = [regex]::Replace($content, "UrlBotaoAcao = `"$escaped`"", "UrlBotaoAcao = $expr")
            $content = [regex]::Replace($content, "urlVoltar = `"$escaped`"", "urlVoltar = $expr")
            $content = [regex]::Replace($content, "urlNovo = `"$escaped`"", "urlNovo = $expr")
            $content = [regex]::Replace($content, "urlLimpar = `"$escaped`"", "urlLimpar = $expr")
            $content = [regex]::Replace($content, "urlEditar = `"$escaped`"", "urlEditar = $expr")
            $content = [regex]::Replace($content, "href=`"$escaped`"", "href=`"@$expr`"")
        }
        if ($mode -eq 'cs') {
            $content = [regex]::Replace($content, "RedirectToPage\(`"$escaped`"\)", "ModuloRotas.Redirecionar($expr)")
            $content = [regex]::Replace($content, "RedirectToPage\(`"$escaped`",", "ModuloRotas.Redirecionar($expr,")
            $content = [regex]::Replace($content, "UrlVoltar = `"$escaped`"", "UrlVoltar = $expr")
        }
    }
    return $content
}

$updated = 0
Get-ChildItem -Path $root -Recurse -Include *.cshtml,*.cs -File | ForEach-Object {
    if ($excludeFiles -contains $_.Name) { return }
    $mode = if ($_.Extension -eq '.cshtml') { 'cshtml' } else { 'cs' }
    $orig = [IO.File]::ReadAllText($_.FullName)
    $new = Apply-SafeMap $orig $mode
    if ($new -ne $orig) {
        [IO.File]::WriteAllText($_.FullName, $new)
        $updated++
        Write-Output $_.FullName
    }
}
Write-Output "Total updated: $updated"
