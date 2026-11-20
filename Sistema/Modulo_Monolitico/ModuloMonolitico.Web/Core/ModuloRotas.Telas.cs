namespace ModuloMonolitico.Web.Core;

public static partial class ModuloRotas
{
    public static class Administracao
    {
        public const string Equipe = "/Contextos/Base/Equipe";
    }

    public static class AreaServicos
    {
        public const string Index = "/Contextos/AreaServicos";
    }

    public static class Agendamentos
    {
        public const string Index = "/Contextos/Agendamentos/Index";
        public const string MeusAgendamentos = "/Contextos/Agendamentos/MeusAgendamentos";
        public const string Painel = "/Contextos/Agendamentos/Painel";
        public const string Configuracoes = "/Contextos/Agendamentos/Configuracoes";
        public const string Painel_Detalhes = "/Contextos/Agendamentos/Painel/Detalhes";
        public const string Painel_Novo = "/Contextos/Agendamentos/Painel/Novo";
        public const string MeusAgendamentos_Detalhes = "/Contextos/Agendamentos/MeusAgendamentos/Detalhes";
        public const string MeusAgendamentos_Novo = "/Contextos/Agendamentos/MeusAgendamentos/Novo";
    }

    public static class Api
    {
        public const string Cep = "/Contextos/Api/Cep";
        public const string SeletorProdutoVariacao = "/Contextos/Api/SeletorProdutoVariacao";
        public const string SeletorProfissional = "/Contextos/Api/SeletorProfissional";
        public const string SeletorUsuario = "/Contextos/Api/SeletorUsuario";
    }

    public static class Autenticacao
    {
        public const string Cadastro = "/Contextos/Autenticacao/Cadastro";
        public const string EsqueciSenha = "/Contextos/Autenticacao/EsqueciSenha";
        public const string Login = "/Contextos/Autenticacao/Login";
        public const string LoginGoogle = "/Contextos/Autenticacao/LoginGoogle";
        public const string RedefinirSenha = "/Contextos/Autenticacao/RedefinirSenha";
        public const string RenovarToken = "/Contextos/Autenticacao/RenovarToken";
        public const string RedefinirAdministrador = "/Contextos/Autenticacao/RedefinirAdministrador";
        public const string Sair = "/Contextos/Autenticacao/Sair";
        public const string ValidarEmail = "/Contextos/Autenticacao/ValidarEmail";
        public const string Grupos = "/Contextos/Autenticacao/Grupos";
        public const string Grupos_Editar = "/Contextos/Autenticacao/Grupos/Editar";
        public const string Grupos_Novo = "/Contextos/Autenticacao/Grupos/Novo";
    }

    public static class Base
    {
    }

    public static class Cidadania
    {
        public const string Index = "/Contextos/Cidadania";
        public const string AchadosPerdidos = "/Contextos/Cidadania/AchadosPerdidos";
        public const string CodigoBarras = "/Contextos/Cidadania/CodigoBarras";
        public const string EtiquetaEnvio = "/Contextos/Cidadania/EtiquetaEnvio";
    }

    public static class Imoveis
    {
        public const string Index = "/Imoveis";
        public const string Detalhes = "/Imoveis/Detalhes";
        public const string Anunciante = "/Imoveis/Anunciante";
        public const string BuscasRecentes = "/Imoveis/BuscasRecentes";
        public const string Favoritos = "/Imoveis/Favoritos";
        public const string Favoritar = "/Imoveis/Acoes/Favoritar";
        public const string RegistrarVisualizacao = "/Imoveis/Acoes/RegistrarVisualizacao";
        public const string ContextoUsuario = "/Imoveis/Acoes/ContextoUsuario";
        public const string AlternarDestaque = "/Imoveis/Acoes/AlternarDestaque";
    }

    public static class Imobiliaria
    {
        public const string Index = Imoveis.Index;
        public const string Detalhes = Imoveis.Detalhes;
        public const string Anunciante = Imoveis.Anunciante;
        public const string BuscasRecentes = Imoveis.BuscasRecentes;
        public const string Favoritos = Imoveis.Favoritos;
        public const string MeusAnuncios = "/Contextos/Imobiliaria/MeusAnuncios";
        public const string MeusAnuncios_Novo = "/Contextos/Imobiliaria/MeusAnuncios/Novo";
        public const string MeusAnuncios_Editar = "/Contextos/Imobiliaria/MeusAnuncios/Editar";
        public const string MeusAnuncios_Perfil = "/Contextos/Imobiliaria/MeusAnuncios/Perfil";
    }

    public static class Veiculos
    {
        public const string Index = "/Veiculos";
        public const string Detalhes = "/Veiculos/Detalhes";
        public const string Vendedor = "/Veiculos/Vendedor";
        public const string BuscasRecentes = "/Veiculos/BuscasRecentes";
        public const string Favoritos = "/Veiculos/Favoritos";
        public const string Favoritar = "/Veiculos/Acoes/Favoritar";
        public const string ContextoUsuario = "/Veiculos/Acoes/ContextoUsuario";
        public const string AlternarDestaque = "/Veiculos/Acoes/AlternarDestaque";
    }

    public static class VeiculosSaaS
    {
        public const string Index = Veiculos.Index;
        public const string Detalhes = Veiculos.Detalhes;
        public const string Vendedor = Veiculos.Vendedor;
        public const string BuscasRecentes = Veiculos.BuscasRecentes;
        public const string Favoritos = Veiculos.Favoritos;
        public const string MeusAnuncios = "/Contextos/Veiculos/MeusAnuncios";
        public const string MeusAnuncios_Novo = "/Contextos/Veiculos/MeusAnuncios/Novo";
        public const string MeusAnuncios_Editar = "/Contextos/Veiculos/MeusAnuncios/Editar";
        public const string MeusAnuncios_Perfil = "/Contextos/Veiculos/MeusAnuncios/Perfil";
    }

    public static class Curriculos
    {
        public const string Index = "/Curriculos";
        public const string Detalhes = "/Curriculos/Detalhes";
        public const string Candidato = "/Curriculos/Candidato";
        public const string MeuCurriculo = "/Contextos/MeuCurriculo";
        public const string MeuCurriculo_DadosProfissionais = "/Contextos/MeuCurriculo/DadosProfissionais";
        public const string MeuCurriculo_Experiencias = "/Contextos/MeuCurriculo/Experiencias";
        public const string MeuCurriculo_Experiencias_Novo = "/Contextos/MeuCurriculo/Experiencias/Novo";
        public const string MeuCurriculo_Experiencias_Editar = "/Contextos/MeuCurriculo/Experiencias/Editar";
        public const string MeuCurriculo_Formacoes = "/Contextos/MeuCurriculo/Formacoes";
        public const string MeuCurriculo_Formacoes_Novo = "/Contextos/MeuCurriculo/Formacoes/Novo";
        public const string MeuCurriculo_Formacoes_Editar = "/Contextos/MeuCurriculo/Formacoes/Editar";
        public const string MeuCurriculo_Cursos = "/Contextos/MeuCurriculo/Cursos";
        public const string MeuCurriculo_Cursos_Novo = "/Contextos/MeuCurriculo/Cursos/Novo";
        public const string MeuCurriculo_Cursos_Editar = "/Contextos/MeuCurriculo/Cursos/Editar";
        public const string MeuCurriculo_Idiomas = "/Contextos/MeuCurriculo/Idiomas";
        public const string MeuCurriculo_Habilidades = "/Contextos/MeuCurriculo/Habilidades";
    }

    public static class Contatos
    {
        public const string Index = "/Contextos/Contatos";
        public const string Cadastro = "/Contextos/Contatos/Cadastro";
        public const string Cadastro_Detalhes = "/Contextos/Contatos/Cadastro/Detalhes";
        public const string Cadastro_Editar = "/Contextos/Contatos/Cadastro/Editar";
        public const string Cadastro_Novo = "/Contextos/Contatos/Cadastro/Novo";
        public const string Cadastro_Relatorio = "/Contextos/Contatos/Cadastro/Relatorio";
        public const string Vinculos = "/Contextos/Contatos/Vinculos";
        public const string Vinculos_Solicitar = "/Contextos/Contatos/Vinculos/Solicitar";
    }

    public static class Empresas
    {
        public const string ResponderConviteEquipe = "/Contextos/Empresas/Convite/ResponderConviteEquipe";
        public const string Index = "/Contextos/Empresas";
        public const string Empresas_Detalhes = "/Contextos/Empresas/Detalhes";
        public const string Empresas_Editar = "/Contextos/Empresas/Editar";
        public const string Empresas_Selecionar = "/Contextos/Empresas/Selecionar";
    }

    public static class ERP
    {
        public const string Index = "/Contextos/ERP";
        public const string Categorias = "/Contextos/ERP/Categorias";
        public const string Marcas = "/Contextos/ERP/Marcas";
    }

    public static class Produto
    {
        public const string Index = "/Contextos/Produto";
        public const string Editar = "/Contextos/Produto/Editar";
        public const string Novo = "/Contextos/Produto/Novo";
    }

    public static class Estoque
    {
        public const string Index = "/Contextos/Estoque";
        public const string Entrada = "/Contextos/Estoque/Entrada";
        public const string Saida = "/Contextos/Estoque/Saida";
        public const string UnidadesMedida = "/Contextos/Estoque/UnidadesMedida";
    }

    public static class Precos
    {
        public const string Index = "/Contextos/Precos";
        public const string Editar = "/Contextos/Precos/Editar";
    }

    public static class Error
    {
        public const string Index = "/Contextos/Error";
    }

    public static class Fidelizacao
    {
        public const string Cashback = "/Contextos/Fidelizacao/Cashback";
        public const string Cashback_Editar = "/Contextos/Fidelizacao/Cashback/Editar";
        public const string Cashback_Gerar = "/Contextos/Fidelizacao/Cashback/Gerar";
        public const string Cashback_Novo = "/Contextos/Fidelizacao/Cashback/Novo";
        public const string Fidelidade = "/Contextos/Fidelizacao/Fidelidade";
        public const string Fidelidade_Editar = "/Contextos/Fidelizacao/Fidelidade/Editar";
        public const string Fidelidade_Novo = "/Contextos/Fidelizacao/Fidelidade/Novo";
        public const string Fidelidade_Validar = "/Contextos/Fidelizacao/Fidelidade/Validar";
        public const string Index = "/Contextos/Fidelizacao";
        public const string LerQRCode = "/Contextos/Fidelizacao/LerQRCode";
        public const string MeuCashback = "/Contextos/Fidelizacao/MeuCashback";
        public const string MeusCartoes = "/Contextos/Fidelizacao/MeusCartoes";
    }

    public static class Financeiro
    {
        public const string Index = "/Contextos/Financeiro";
    }

    public static class Fiscal
    {
        public const string Index = "/Contextos/Fiscal";
        public const string Certificados = "/Contextos/Fiscal/Certificados";
        public const string Parametros = "/Contextos/Fiscal/Parametros";
        public const string Parametros_Editar_idguid = "/Contextos/Fiscal/Parametros/Editar/{id:guid}";
        public const string Parametros_Novo = "/Contextos/Fiscal/Parametros/Novo";
    }

    public static class Gestao
    {
        public const string Configuracoes = "/Contextos/Gestao/Configuracoes";
        public const string TiposVendedor = "/Contextos/Gestao/TiposVendedor";
        public const string TiposVendedor_Editar = "/Contextos/Gestao/TiposVendedor/Editar";
        public const string TiposVendedor_Novo = "/Contextos/Gestao/TiposVendedor/Novo";
        public const string Vendedores = "/Contextos/Gestao/Vendedores";
        public const string Vendedores_Detalhes = "/Contextos/Gestao/Vendedores/Detalhes";
        public const string Vendedores_Editar = "/Contextos/Gestao/Vendedores/Editar";
        public const string Vendedores_Novo = "/Contextos/Gestao/Vendedores/Novo";
    }

    public static class Geral
    {
        public const string SeletorDemo = "/Contextos/Geral/SeletorDemo";
    }

    public static class HelpDesk
    {
        public const string Index = "/Contextos/HelpDesk";
    }

    public static class InfraAdmin
    {
        public const string Index = "/Contextos/InfraAdmin";
        public const string Documentos = "/Contextos/InfraAdmin/Documentos";
        public const string Documentos_Editar = "/Contextos/InfraAdmin/Documentos/Editar";
        public const string Documentos_Novo = "/Contextos/InfraAdmin/Documentos/Novo";
        public const string Grupos = "/Contextos/InfraAdmin/Grupos";
        public const string Grupos_Editar = "/Contextos/InfraAdmin/Grupos/Editar";
        public const string Grupos_Novo = "/Contextos/InfraAdmin/Grupos/Novo";
        public const string GruposSaaS = "/Contextos/InfraAdmin/GruposSaaS";
        public const string GruposSaaS_Editar = "/Contextos/InfraAdmin/GruposSaaS/Editar";
        public const string GruposSaaS_Novo = "/Contextos/InfraAdmin/GruposSaaS/Novo";
        public const string GruposSaaS_Usuarios = "/Contextos/InfraAdmin/GruposSaaS/Usuarios";
        public const string HistoricoLogin = "/Contextos/InfraAdmin/HistoricoLogin";
        public const string Logs = "/Contextos/InfraAdmin/Logs";
        public const string SaaS_Comunicados = "/Contextos/InfraAdmin/SaaS/Comunicados";
        public const string SaaS_Comunicados_Cadastrar = "/Contextos/InfraAdmin/SaaS/Comunicados/Cadastrar";
        public const string SaaS_Comunicados_Detalhes = "/Contextos/InfraAdmin/SaaS/Comunicados/Detalhes";
        public const string SaaS_Contas = "/Contextos/InfraAdmin/SaaS/Contas";
        public const string SaaS_Faturas = "/Contextos/InfraAdmin/SaaS/Faturas";
        public const string SaaS_Planos = "/Contextos/InfraAdmin/SaaS/Planos";
        public const string SaaS_Planos_Editar = "/Contextos/InfraAdmin/SaaS/Planos/Editar";
        public const string SaaS_Planos_Novo = "/Contextos/InfraAdmin/SaaS/Planos/Novo";
        public const string Sessao = "/Contextos/InfraAdmin/Sessao";
        public const string UltimosConectados = "/Contextos/InfraAdmin/UltimosConectados";
    }

    public static class Logistica
    {
        public const string Index = "/Contextos/Logistica";
    }

    public static class Mensagens
    {
        public const string Index = "/Contextos/Mensagens";
    }

    public static class Notificacoes
    {
        public const string Index = "/Contextos/Notificacoes";
    }

    public static class Perfil
    {
        public const string MeuPerfil = "/Contextos/MeuPerfil";
        public const string MeuPerfilDesativar = "/Contextos/MeuPerfilDesativar";
        public const string MeusVinculos = "/Contextos/Perfil/MeusVinculos";
    }

    public static class Principal
    {
        public const string Hub = "/Principal/Index";
    }

    public static class SaaS
    {
        public const string Comunicados_Pendentes = "/Contextos/SaaS/Comunicados/Pendentes";
        public const string Suspensa = "/Contextos/SaaS/Suspensa";
    }

    public static class Varejo
    {
        public const string Index = "/Contextos/Varejo";
    }
}







