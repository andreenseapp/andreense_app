$ErrorActionPreference = 'Stop'
$base = "C:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages"

# Mapa legado -> expressao ModuloRotas (rotas mais longas primeiro)
$routeMap = [ordered]@{
    '/InfraAdmin/Documentos/Index' = 'ModuloRotas.InfraAdmin.Documentos'
    '/InfraAdmin/Documentos/Novo' = 'ModuloRotas.InfraAdmin.Documentos_Novo'
    '/InfraAdmin/Documentos/Editar' = 'ModuloRotas.InfraAdmin.Documentos_Editar'
    '/InfraAdmin/HistoricoLogin/Index' = 'ModuloRotas.InfraAdmin.HistoricoLogin'
    '/InfraAdmin/UltimosConectados/Index' = 'ModuloRotas.InfraAdmin.UltimosConectados'
    '/InfraAdmin/GruposSaaS/Novo' = 'ModuloRotas.InfraAdmin.GruposSaaS_Novo'
    '/InfraAdmin/GruposSaaS/Index' = 'ModuloRotas.InfraAdmin.GruposSaaS'
    '/InfraAdmin/Grupos/Novo' = 'ModuloRotas.InfraAdmin.Grupos_Novo'
    '/InfraAdmin/Grupos/Index' = 'ModuloRotas.InfraAdmin.Grupos'
    '/InfraAdmin/SaaS/Planos/Novo' = 'ModuloRotas.InfraAdmin.SaaS_Planos_Novo'
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
    '/Base/Vendedores/Novo' = 'ModuloRotas.Base.Vendedores_Novo'
    '/Base/Vendedores/Index' = 'ModuloRotas.Base.Vendedores'
    '/Base/Vendedores/Editar' = 'ModuloRotas.Base.Vendedores_Editar'
    '/Base/GrupoUsuario/Novo' = 'ModuloRotas.Base.GrupoUsuario_Novo'
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
    '/Autenticacao/ValidarEmail' = 'ModuloRotas.Autenticacao.ValidarEmail'
    '/Autenticacao/Login' = 'ModuloRotas.Autenticacao.Login'
    '/Autenticacao/Sair' = 'ModuloRotas.Autenticacao.Sair'
}

function Convert-LegacyPathToHref([string]$path) {
    if ($path -match '^([^#?]+)(\?[^#]*)?(#.*)?$') {
        $basePath = $matches[1]
        $query = if ($matches[2]) { $matches[2] } else { '' }
        $hash = if ($matches[3]) { $matches[3] } else { '' }
        foreach ($key in $routeMap.Keys) {
            if ($basePath -eq $key) {
                $expr = $routeMap[$key]
                if ($query -or $hash) {
                    $extra = $query + $hash
                    return "href=`"@($expr + `"$extra`")`""
                }
                return "href=`"@$expr`""
            }
        }
    }
    return "href=`"$path`""
}

function Convert-TailToHref([string]$tail) {
    if ($tail -match '^~') { return "href=`"$tail`"" }
    if ($tail -match '^@ModuloRotas\.') { return "href=`"$tail`"" }
    if ($tail -match '^@Model\.') { return "href=`"$tail`"" }
    if ($tail -match '^@\(User\.') { return "href=`"$tail`"" }
    if ($tail -match '^@Url\.Content') { return "href=`"$tail`"" }
    if ($tail -match '^/') { return Convert-LegacyPathToHref $tail }
    if ($tail -match '^filtro=') {
        return "href=`"@(ModuloRotas.Notificacoes.Index + `"?$tail`")`""
    }
    return "href=`"$tail`""
}

function Repair-CorruptedHrefs([string]$content) {
    $pattern = 'href="@\(ModuloRotas\.Raiz\.Index \+ "(?:\?@\(ModuloRotas\.[\w.]+\s+\+\s+")*\?([^"]+)"'
    $evaluator = {
        param($m)
        Convert-TailToHref $m.Groups[1].Value
    }
    return [regex]::Replace($content, $pattern, $evaluator)
}

function Repair-BrokenTernaries([string]$content) {
    $content = [regex]::Replace($content, '\?\s*ModuloRotas\.([\w.]+)"', '? ModuloRotas.$1')
    $content = $content.Replace(': $"/Contatos?Papel=', ': $"{ModuloRotas.CRM.Contatos}?Papel=')
    $content = $content.Replace('$"/Contatos/', '$"{ModuloRotas.CRM.Contatos}/')
    $content = $content.Replace('$"/Contatos?', '$"{ModuloRotas.CRM.Contatos}?')
    return $content
}

$updated = 0
Get-ChildItem -Path $base -Recurse -Include *.cshtml,*.cs -File | ForEach-Object {
    $orig = [IO.File]::ReadAllText($_.FullName)
    $new = Repair-CorruptedHrefs $orig
    $new = Repair-BrokenTernaries $new
    if ($new -ne $orig) {
        [IO.File]::WriteAllText($_.FullName, $new)
        $updated++
        Write-Output $_.FullName
    }
}
Write-Output "Repaired files: $updated"
