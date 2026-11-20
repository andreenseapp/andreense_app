# DEPRECATED — corrompe assets (~/css, Url.Content). Use substituir-rotas-seguro.ps1 + reparar-rotas-corrompidas.ps1
$shared = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages\Shared"

# Ordem: rotas mais longas primeiro
$map = [ordered]@{
    '/InfraAdmin/Documentos/Index' = 'ModuloRotas.InfraAdmin.Documentos'
    '/InfraAdmin/Documentos/Novo' = 'ModuloRotas.InfraAdmin.Documentos_Novo'
    '/InfraAdmin/Documentos/Editar' = 'ModuloRotas.InfraAdmin.Documentos_Editar'
    '/InfraAdmin/HistoricoLogin/Index' = 'ModuloRotas.InfraAdmin.HistoricoLogin'
    '/InfraAdmin/UltimosConectados/Index' = 'ModuloRotas.InfraAdmin.UltimosConectados'
    '/InfraAdmin/GruposSaaS/Index' = 'ModuloRotas.InfraAdmin.GruposSaaS'
    '/InfraAdmin/Grupos/Index' = 'ModuloRotas.InfraAdmin.Grupos'
    '/InfraAdmin/SaaS/Planos' = 'ModuloRotas.InfraAdmin.SaaS_Planos'
    '/InfraAdmin/SaaS/Contas' = 'ModuloRotas.InfraAdmin.SaaS_Contas'
    '/InfraAdmin/SaaS/Faturas' = 'ModuloRotas.InfraAdmin.SaaS_Faturas'
    '/InfraAdmin/Sessao/Index' = 'ModuloRotas.InfraAdmin.Sessao'
    '/InfraAdmin/Logs/Index' = 'ModuloRotas.InfraAdmin.Logs'
    '/InfraAdmin/Index' = 'ModuloRotas.InfraAdmin.Index'
    '/Configuracoes/Fiscal/Parametros/Novo' = 'ModuloRotas.Base.Fiscal_Parametros_Novo'
    '/Configuracoes/Fiscal/Parametros/Editar' = 'ModuloRotas.Base.Fiscal_Parametros'
    '/Configuracoes/Fiscal/Parametros' = 'ModuloRotas.Base.Fiscal_Parametros'
    '/Configuracoes/Fiscal' = 'ModuloRotas.Base.Fiscal_Certificados'
    '/Configuracoes' = 'ModuloRotas.Base.Configuracoes'
    '/Base/Fiscal/Parametros/Index' = 'ModuloRotas.Base.Fiscal_Parametros'
    '/Base/Empresas/Detalhes' = 'ModuloRotas.Base.Empresas_Detalhes'
    '/Base/Empresas/Selecionar' = 'ModuloRotas.Base.Empresas_Selecionar'
    '/Base/Empresas/Index' = 'ModuloRotas.Base.Empresas'
    '/Base/Empresas/Editar' = 'ModuloRotas.Base.Empresas_Editar'
    '/Base/Vendedores/Index' = 'ModuloRotas.Base.Vendedores'
    '/Base/Vendedores/Editar' = 'ModuloRotas.Base.Vendedores_Editar'
    '/Base/GrupoUsuario/Index' = 'ModuloRotas.Base.GrupoUsuario'
    '/AreaServicos/Agendamentos/Configuracao' = 'ModuloRotas.AreaServicos.Agendamentos_Configuracao'
    '/AreaServicos/Agendamentos/Detalhes' = 'ModuloRotas.AreaServicos.Agendamentos_Detalhes'
    '/AreaServicos/Agendamentos/Novo' = 'ModuloRotas.AreaServicos.Agendamentos_Novo'
    '/AreaServicos/Agendamentos/Index' = 'ModuloRotas.AreaServicos.Agendamentos'
    '/AreaServicos/Index' = 'ModuloRotas.AreaServicos.Index'
    '/AreaCliente/MeusAgendamentos/Novo' = 'ModuloRotas.AreaCliente.MeusAgendamentos_Novo'
    '/AreaCliente/MeusAgendamentos/Detalhes' = 'ModuloRotas.AreaCliente.MeusAgendamentos_Detalhes'
    '/AreaCliente/MeusAgendamentos/Index' = 'ModuloRotas.AreaCliente.MeusAgendamentos'
    '/AreaCliente/Index' = 'ModuloRotas.AreaCliente.Index'
    '/ERP/ContextosPreco/Index' = 'ModuloRotas.ERP.ContextosPreco'
    '/ERP/ContextosPreco/Editar' = 'ModuloRotas.ERP.ContextosPreco_Editar'
    '/ERP/Produtos/Index' = 'ModuloRotas.ERP.Produtos'
    '/ERP/Produtos/Novo' = 'ModuloRotas.ERP.Produtos_Novo'
    '/ERP/Produtos/Editar' = 'ModuloRotas.ERP.Produtos_Editar'
    '/ERP/Estoque/Index' = 'ModuloRotas.ERP.Estoque'
    '/ERP/Estoque/Entrada' = 'ModuloRotas.ERP.Estoque_Entrada'
    '/ERP/Estoque/Saida' = 'ModuloRotas.ERP.Estoque_Saida'
    '/ERP/Index' = 'ModuloRotas.ERP.Index'
    '/CRM/Contatos/Index' = 'ModuloRotas.CRM.Contatos'
    '/Contatos/Relatorio' = 'ModuloRotas.CRM.Contatos_Relatorio'
    '/Contatos/Detalhes' = 'ModuloRotas.CRM.Contatos_Detalhes'
    '/Contatos/Editar' = 'ModuloRotas.CRM.Contatos_Editar'
    '/Contatos/Novo' = 'ModuloRotas.CRM.Contatos_Novo'
    '/Contatos' = 'ModuloRotas.CRM.Contatos'
    '/Cidade/AchadosPerdidos' = 'ModuloRotas.Cidade.AchadosPerdidos'
    '/Cidade/CodigoBarras' = 'ModuloRotas.Cidade.CodigoBarras'
    '/Cidade/EtiquetaEnvio' = 'ModuloRotas.Cidade.EtiquetaEnvio'
    '/Cidade/Index' = 'ModuloRotas.Cidade.Index'
    '/Fidelidade/Validar' = 'ModuloRotas.AreaServicos.Fidelidade_Validar'
    '/Fidelidade/Editar' = 'ModuloRotas.AreaServicos.Fidelidade_Editar'
    '/Fidelidade/Novo' = 'ModuloRotas.AreaServicos.Fidelidade_Novo'
    '/Fidelidade/Index' = 'ModuloRotas.AreaServicos.Fidelidade'
    '/Cashback/Gerar' = 'ModuloRotas.AreaServicos.Cashback_Gerar'
    '/Cashback/Editar' = 'ModuloRotas.AreaServicos.Cashback_Editar'
    '/Cashback/Novo' = 'ModuloRotas.AreaServicos.Cashback_Novo'
    '/Cashback/Index' = 'ModuloRotas.AreaServicos.Cashback'
    '/Meu-Perfil/Desativar' = 'ModuloRotas.MeuPerfilDesativar.Index'
    '/Meu-Perfil' = 'ModuloRotas.MeuPerfil.Index'
    '/Notificacoes/Index' = 'ModuloRotas.Notificacoes.Index'
    '/Mensagens/Index' = 'ModuloRotas.Mensagens.Index'
    '/Financeiro/Index' = 'ModuloRotas.Financeiro.Index'
    '/Logistica/Index' = 'ModuloRotas.Logistica.Index'
    '/HelpDesk/Index' = 'ModuloRotas.HelpDesk.Index'
    '/Varejo/Index' = 'ModuloRotas.Varejo.Index'
    '/Vendedores/Index' = 'ModuloRotas.Base.Vendedores'
    '/Empresas/Index' = 'ModuloRotas.Base.Empresas'
    '/Empresas/Editar' = 'ModuloRotas.Base.Empresas_Editar'
    '/Empresas/Selecionar' = 'ModuloRotas.Base.Empresas_Selecionar'
    '/CRM/Index' = 'ModuloRotas.CRM.Index'
    '/Fiscal/Index' = 'ModuloRotas.Fiscal.Index'
    '/Testes/Index' = 'ModuloRotas.Testes.Index'
    '/Principal/Index' = 'ModuloRotas.Principal.Hub'
    '/MeuPerfil' = 'ModuloRotas.MeuPerfil.Index'
    '/Index' = 'ModuloRotas.Raiz.Index'
}

function Apply-Map([string]$content, [string]$mode) {
    foreach ($key in $map.Keys) {
        $expr = $map[$key]
        if ($mode -eq 'cshtml') {
            $content = $content.Replace("href=`"$key", "href=`"@$expr")
            $content = $content.Replace("href=`"$key?", "href=`"@($expr + `"?")
            $content = $content.Replace("url = `"$key`"", "url = $expr")
            $content = $content.Replace("UrlVoltar = `"$key`"", "UrlVoltar = $expr")
            $content = $content.Replace("UrlBotaoAcao = `"$key`"", "UrlBotaoAcao = $expr")
            $content = $content.Replace("urlNovo = `"$key", "urlNovo = $expr")
            $content = $content.Replace("urlVoltar = `"$key", "urlVoltar = $expr")
            $content = $content.Replace("urlLimpar = `"$key", "urlLimpar = $expr")
            $content = $content.Replace("urlEditar = `"$key", "urlEditar = $expr")
            $content = $content.Replace("? `"$key", "? $expr")
            $content = $content.Replace(": `"$key", ": $expr")
        }
        if ($mode -eq 'cs') {
            $content = $content.Replace("RedirectToPage(`"$key`")", "ModuloRotas.Redirecionar($expr)")
            $content = $content.Replace("RedirectToPage(`"$key`",", "ModuloRotas.Redirecionar($expr,")
            $content = $content.Replace("context.Result = RedirectToPage(`"$key`")", "context.Result = ModuloRotas.Redirecionar($expr)")
        }
    }
    return $content
}

$updated = 0
Get-ChildItem -Path @($root, $shared) -Recurse -Include *.cshtml,*.cs -File | ForEach-Object {
    $mode = if ($_.Extension -eq '.cshtml') { 'cshtml' } else { 'cs' }
    $orig = [IO.File]::ReadAllText($_.FullName)
    $new = Apply-Map $orig $mode
    if ($new -ne $orig) {
        [IO.File]::WriteAllText($_.FullName, $new)
        $updated++
        Write-Output $_.FullName
    }
}
Write-Output "Total updated: $updated"
