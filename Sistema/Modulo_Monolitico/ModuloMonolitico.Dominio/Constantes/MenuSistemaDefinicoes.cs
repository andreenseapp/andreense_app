namespace ModuloMonolitico.Dominio.Constantes;

public class MenuItemDefinicao
{
    public string ChavePermissao { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string SvgIcone { get; set; } = string.Empty;
    public string CorHexadecimal { get; set; } = "#3b82f6";
    public bool EmBreve { get; set; } = false;
}

public class CategoriaMenuDefinicao
{
    public string Nome { get; set; } = string.Empty;
    public string SvgIcone { get; set; } = string.Empty;
    public List<MenuItemDefinicao> Itens { get; set; } = new();
}

public class ModuloMenuDefinicao
{
    public string Nome { get; set; } = string.Empty;
    public string ModuloRaiz { get; set; } = string.Empty;
    public List<CategoriaMenuDefinicao> Categorias { get; set; } = new();
}

public static class MenuSistemaDefinicoes
{
    public const string CorAzul = "#3b82f6";
    public const string CorRoxo = "#8b5cf6";
    public const string CorVerde = "#10b981";
    public const string CorLaranja = "#f59e0b";
    public const string CorIndigo = "#6366f1";
    public const string CorCinza = "#94a3b8";
    public const string IconeCadastros = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"></path><circle cx=\"9\" cy=\"7\" r=\"4\"></circle></svg>";
    public const string IconeEstoque = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z\"></path><polyline points=\"3.27 6.96 12 12.01 20.73 6.96\"></polyline><line x1=\"12\" y1=\"22.08\" x2=\"12\" y2=\"12\"></line></svg>";
    public const string IconePDV = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><circle cx=\"12\" cy=\"12\" r=\"10\"></circle><circle cx=\"12\" cy=\"12\" r=\"4\"></circle><line x1=\"4.93\" y1=\"4.93\" x2=\"9.17\" y2=\"9.17\"></line><line x1=\"14.83\" y1=\"14.83\" x2=\"19.07\" y2=\"19.07\"></line><line x1=\"14.83\" y1=\"9.17\" x2=\"19.07\" y2=\"4.93\"></line><line x1=\"4.93\" y1=\"19.07\" x2=\"9.17\" y2=\"14.83\"></line></svg>";
    public const string IconeCampanhas = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><path d=\"M22 11.08V12a10 10 0 1 1-5.93-9.14\"></path><polyline points=\"22 4 12 14.01 9 11.01\"></polyline></svg>";
    public const string IconeConfig = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><circle cx=\"12\" cy=\"12\" r=\"3\"></circle><path d=\"M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z\"></path></svg>";

    public const string IconeEmpresa = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M3 21h18M3 7v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V7M9 3h6v4H9z\"/><line x1=\"9\" y1=\"11\" x2=\"15\" y2=\"11\"/><line x1=\"9\" y1=\"15\" x2=\"15\" y2=\"15\"/></svg>";
    public const string IconeCalendario = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><line x1=\"16\" y1=\"2\" x2=\"16\" y2=\"6\"/><line x1=\"8\" y1=\"2\" x2=\"8\" y2=\"6\"/><line x1=\"3\" y1=\"10\" x2=\"21\" y2=\"10\"/></svg>";
    public const string SvgEmpresas = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M3 21h18M3 7v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V7M9 3h6v4H9z\"/><line x1=\"9\" y1=\"11\" x2=\"15\" y2=\"11\"/><line x1=\"9\" y1=\"15\" x2=\"15\" y2=\"15\"/></svg>";
    public const string SvgContatos = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/><path d=\"M23 21v-2a4 4 0 0 0-3-3.87\"/><path d=\"M16 3.13a4 4 0 0 1 0 7.75\"/></svg>";
    public const string SvgVendedores = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"></path><circle cx=\"9\" cy=\"7\" r=\"4\"></circle><path d=\"M23 21v-2a4 4 0 0 0-3-3.87\"></path><path d=\"M16 3.13a4 4 0 0 1 0 7.75\"></path></svg>";
    public const string SvgProduto = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z\"></path><polyline points=\"3.27 6.96 12 12.01 20.73 6.96\"></polyline><line x1=\"12\" y1=\"22.08\" x2=\"12\" y2=\"12\"></line></svg>";
    public const string SvgEstoque = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M5 8h14M5 8a2 2 0 1 0 0-4h14a2 2 0 1 0 0 4M5 8v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8M10 12h4\"/></svg>";
    public const string SvgCashback = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><rect x=\"2\" y=\"4\" width=\"20\" height=\"16\" rx=\"2\" ry=\"2\"></rect><line x1=\"6\" y1=\"8\" x2=\"6.01\" y2=\"8\"></line><line x1=\"10\" y1=\"8\" x2=\"10.01\" y2=\"8\"></line></svg>";
    public const string SvgFidelidade = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M16 13H8\"/><path d=\"M16 17H8\"/><path d=\"M10 9H8\"/></svg>";
    public const string SvgConfig = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"></path><circle cx=\"9\" cy=\"7\" r=\"4\"></circle><path d=\"M23 21v-2a4 4 0 0 0-3-3.87\"></path><path d=\"M16 3.13a4 4 0 0 1 0 7.75\"></path></svg>";
    public const string SvgCalendario = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><line x1=\"16\" y1=\"2\" x2=\"16\" y2=\"6\"/><line x1=\"8\" y1=\"2\" x2=\"8\" y2=\"6\"/><line x1=\"3\" y1=\"10\" x2=\"21\" y2=\"10\"/></svg>";

    public static CategoriaMenuDefinicao ObterCategoriaEmpresa() => new CategoriaMenuDefinicao
    {
        Nome = "Empresa",
        SvgIcone = IconeEmpresa,
        Itens = new List<MenuItemDefinicao>
        {
            new MenuItemDefinicao { ChavePermissao = "ERP.Empresas.Visualizar", Titulo = "Minhas Empresas", Url = "/Empresas/Index", Descricao = "Gerencie as filiais e os CNPJs da sua conta.", SvgIcone = SvgEmpresas, CorHexadecimal = CorAzul },
            new MenuItemDefinicao { ChavePermissao = "Base.Fiscal.Certificados", Titulo = "Certificados", Url = "/Configuracoes/Fiscal/Certificados", Descricao = "Gerencie certificados digitais A1.", SvgIcone = SvgConfig, CorHexadecimal = CorIndigo },
            new MenuItemDefinicao { ChavePermissao = "Base.Fiscal.Parametros.Visualizar", Titulo = "Parâmetros de Emissão", Url = "/Configuracoes/Fiscal/Parametros", Descricao = "Talonário, ambiente e regras de SEFAZ.", SvgIcone = SvgConfig, CorHexadecimal = CorIndigo }
        }
    };

    public static CategoriaMenuDefinicao ObterCategoriaPessoas() => new CategoriaMenuDefinicao
    {
        Nome = "Pessoas",
        SvgIcone = IconeCadastros,
        Itens = new List<MenuItemDefinicao>
        {
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Clientes", Url = "/Contatos?Papel=cliente", Descricao = "Visualize e gerencie seus clientes.", SvgIcone = SvgContatos, CorHexadecimal = CorAzul },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Fornecedores", Url = "/Contatos?Papel=fornecedor", Descricao = "Visualize e gerencie seus fornecedores.", SvgIcone = SvgContatos, CorHexadecimal = CorLaranja },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Transportadoras", Url = "/Contatos?Papel=transportador", Descricao = "Visualize e gerencie suas transportadoras.", SvgIcone = SvgContatos, CorHexadecimal = CorIndigo },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Colaboradores", Url = "/Contatos?Papel=funcionario", Descricao = "Visualize e gerencie seus colaboradores e funcionários.", SvgIcone = SvgContatos, CorHexadecimal = CorVerde },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Contatos Geral", Url = "/Contatos", Descricao = "Busca avan?ada e gestão de múltiplos pap?is.", SvgIcone = SvgContatos, CorHexadecimal = CorCinza }
        }
    };

    public static CategoriaMenuDefinicao ObterCategoriaPessoasCRM() => new CategoriaMenuDefinicao
    {
        Nome = "Pessoas",
        SvgIcone = IconeCadastros,
        Itens = new List<MenuItemDefinicao>
        {
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Leads", Url = "/Contatos?Papel=lead", Descricao = "Visualize e gerencie seus leads comerciais.", SvgIcone = SvgContatos, CorHexadecimal = CorRoxo },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Prospects", Url = "/Contatos?Papel=prospect", Descricao = "Visualize e gerencie seus prospects.", SvgIcone = SvgContatos, CorHexadecimal = CorRoxo },
            new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Funcionários", Url = "/Contatos?Papel=funcionario", Descricao = "Visualize e gerencie seus funcionários.", SvgIcone = SvgContatos, CorHexadecimal = CorVerde },
            new MenuItemDefinicao { ChavePermissao = "CRM.Vendedores.Visualizar", Titulo = "Vendedores", Url = "/Base/Vendedores/Index", Descricao = "Cadastre sua equipe de vendas e defina comissões.", SvgIcone = SvgVendedores, CorHexadecimal = CorRoxo }
        }
    };

    public static CategoriaMenuDefinicao ObterCategoriaEquipe() => new CategoriaMenuDefinicao
    {
        Nome = "Equipe",
        SvgIcone = IconeConfig,
        Itens = new List<MenuItemDefinicao>
        {
            new MenuItemDefinicao { ChavePermissao = "Base.GrupoUsuario.Gerenciar", Titulo = "Grupos de Usuários", Url = "/Base/GrupoUsuario/Index", Descricao = "Defina cargos e configure telas e atalhos.", SvgIcone = SvgConfig, CorHexadecimal = CorIndigo },
            new MenuItemDefinicao { ChavePermissao = "Base.Equipe.Gerenciar", Titulo = "Gestão de Equipe", Url = "/Administracao/Equipe/Index", Descricao = "Gerencie convites e usuários da sua empresa.", SvgIcone = SvgContatos, CorHexadecimal = CorIndigo }
        }
    };

    public static List<ModuloMenuDefinicao> Modulos { get; } = new()
    {
        new ModuloMenuDefinicao
        {
            Nome = "ERP",
            ModuloRaiz = "ERP",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                ObterCategoriaEmpresa(),
                ObterCategoriaPessoas(),
                ObterCategoriaEquipe(),
                new CategoriaMenuDefinicao
                {
                    Nome = "Estoque",
                    SvgIcone = IconeEstoque,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "Produto.Visualizar", Titulo = "Produtos", Url = "/Produto/Index", Descricao = "Gerencie seu catélogo, preços e variações.", SvgIcone = SvgProduto, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.UnidadesMedida.Visualizar", Titulo = "Unidade de Medida", Url = "/ERP/UnidadesMedida/Index", Descricao = "Gerencie KG, LT, UN, CX, etc.", SvgIcone = SvgProduto, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Categorias.Visualizar", Titulo = "Grupo / Categoria", Url = "/ERP/Categorias/Index", Descricao = "Organize seus produtos hierarquicamente.", SvgIcone = SvgProduto, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Marcas.Visualizar", Titulo = "Marca", Url = "/ERP/Marcas/Index", Descricao = "Gerencie as marcas dos produtos.", SvgIcone = SvgProduto, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Estoque.Visualizar", Titulo = "Posição de Estoque", Url = "/ERP/Estoque/Index", Descricao = "Consulte saldos, entradas e saídas de mercadorias.", SvgIcone = SvgEstoque, CorHexadecimal = CorLaranja }
                    }
                }
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "Área de Serviços",
            ModuloRaiz = "AreaServicos",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                ObterCategoriaEmpresa(),
                new CategoriaMenuDefinicao
                {
                    Nome = "Pessoas",
                    SvgIcone = IconeCadastros,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Clientes", Url = "/Contatos?Papel=cliente", Descricao = "Visualize e gerencie seus clientes.", SvgIcone = SvgContatos, CorHexadecimal = CorAzul },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Colaboradores", Url = "/Contatos?Papel=funcionario", Descricao = "Colaboradores aptos para realizar serviços.", SvgIcone = SvgContatos, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Contatos.Visualizar", Titulo = "Contatos Geral", Url = "/Contatos", Descricao = "Busca avan?ada e gestão de múltiplos pap?is.", SvgIcone = SvgContatos, CorHexadecimal = CorCinza }
                    }
                },
                ObterCategoriaEquipe(),
                new CategoriaMenuDefinicao
                {
                    Nome = "Agenda e Marcações",
                    SvgIcone = IconeCalendario,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "Servicos.Agendamentos.Visualizar", Titulo = "Agendamentos", Url = "/Contextos/Agendamentos/Painel", Descricao = "Gerencie a agenda da sua empresa com calendário visual.", SvgIcone = SvgCalendario, CorHexadecimal = CorIndigo },
                        new MenuItemDefinicao { ChavePermissao = "Servicos.Agendamentos.Configurar", Titulo = "Configurações", Url = "/Contextos/Agendamentos/Configuracoes", Descricao = "Defina horários, serviços e profissionais disponíveis.", SvgIcone = SvgConfig, CorHexadecimal = CorIndigo }
                    }
                },
                new CategoriaMenuDefinicao
                {
                    Nome = "Serviços e Produtos",
                    SvgIcone = IconeEstoque,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "Produto.Visualizar", Titulo = "Catélogo de Serviços", Url = "/Produto/Index", Descricao = "Gerencie seus serviços e produtos para agendamento.", SvgIcone = SvgProduto, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "ERP.Estoque.Visualizar", Titulo = "Posição de Estoque", Url = "/ERP/Estoque/Index", Descricao = "Consulte saldos, entradas e saídas.", SvgIcone = SvgEstoque, CorHexadecimal = CorLaranja }
                    }
                },
                new CategoriaMenuDefinicao
                {
                    Nome = "Fidelidade e Campanhas",
                    SvgIcone = IconeCampanhas,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "Servicos.PDV.Cashback", Titulo = "Conceder Cashback", Url = "/Cashback/Gerar", Descricao = "Gere um QR Code para creditar saldo na carteira do seu cliente.", SvgIcone = SvgCashback, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "Servicos.PDV.Fidelidade", Titulo = "Validar Fidelidade", Url = "/Fidelidade/Validar", Descricao = "Gere um QR Code para computar uma visita no cartão do cliente.", SvgIcone = SvgFidelidade, CorHexadecimal = CorLaranja },
                        new MenuItemDefinicao { ChavePermissao = "Servicos.Campanhas.Cashback", Titulo = "Regras de Cashback", Url = "/Cashback/Index", Descricao = "Configure os percentuais de retorno e validade do saldo.", SvgIcone = SvgCashback, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "Servicos.Campanhas.Fidelidade", Titulo = "Cartões Fidelidade", Url = "/Fidelidade/Index", Descricao = "Crie cart?es de selos, prêmios e acompanhe os resgates.", SvgIcone = SvgFidelidade, CorHexadecimal = CorLaranja }
                    }
                }
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "Imóveis",
            ModuloRaiz = "Imobiliaria",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                new CategoriaMenuDefinicao
                {
                    Nome = "Painel Imobiliário",
                    SvgIcone = IconeCadastros,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Buscar Imóveis", Url = "/Imoveis", Descricao = "Busque e visualize imóveis disponíveis na região.", SvgIcone = IconeEmpresa, CorHexadecimal = CorAzul },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Meus Imóveis", Url = "/Contextos/Imobiliaria/MeusAnuncios", Descricao = "Gerencie e publique seus próprios anúncios de imóveis.", SvgIcone = IconeCadastros, CorHexadecimal = CorVerde }
                    }
                }
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "Veículos",
            ModuloRaiz = "Veiculos",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                new CategoriaMenuDefinicao
                {
                    Nome = "Painel de Veículos",
                    SvgIcone = IconeCadastros,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Buscar Veículos", Url = "/Veiculos", Descricao = "Busque e visualize veículos disponíveis na região.", SvgIcone = IconeEmpresa, CorHexadecimal = CorAzul },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Meus Veículos", Url = "/Contextos/Veiculos/MeusAnuncios", Descricao = "Gerencie e publique seus próprios anúncios de veículos.", SvgIcone = IconeCadastros, CorHexadecimal = CorVerde }
                    }
                }
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "CRM",
            ModuloRaiz = "CRM",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                ObterCategoriaPessoasCRM(),
                ObterCategoriaEquipe()
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "Administração",
            ModuloRaiz = "InfraAdmin",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                new CategoriaMenuDefinicao
                {
                    Nome = "SaaS e Contas",
                    SvgIcone = IconeEmpresa,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Planos", Url = "/Contextos/InfraAdmin/SaaS/Planos", Descricao = "Gerencie os planos de assinatura.", SvgIcone = SvgConfig, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Contas SaaS", Url = "/Contextos/InfraAdmin/SaaS/Contas", Descricao = "Gerencie as contas ativas no sistema.", SvgIcone = SvgConfig, CorHexadecimal = CorAzul },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Faturas", Url = "/Contextos/InfraAdmin/SaaS/Faturas", Descricao = "Controle de cobranças e faturamento.", SvgIcone = SvgConfig, CorHexadecimal = CorLaranja }
                    }
                },
                new CategoriaMenuDefinicao
                {
                    Nome = "Segurança e Auditoria",
                    SvgIcone = IconeConfig,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Grupos SaaS", Url = "/Contextos/InfraAdmin/GruposSaaS", Descricao = "Níveis de acesso administrativo.", SvgIcone = SvgConfig, CorHexadecimal = CorIndigo },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Histórico de Logins", Url = "/Contextos/InfraAdmin/HistoricoLogin", Descricao = "Auditoria de acessos e logins.", SvgIcone = SvgConfig, CorHexadecimal = CorCinza },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Últimos Conectados", Url = "/Contextos/InfraAdmin/UltimosConectados", Descricao = "Usuários recentemente online.", SvgIcone = SvgConfig, CorHexadecimal = CorCinza },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Inspetor de Sessão", Url = "/Contextos/InfraAdmin/Sessao", Descricao = "Claims e token JWT da sessão Master.", SvgIcone = SvgConfig, CorHexadecimal = CorCinza },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Logs do Sistema", Url = "/Contextos/InfraAdmin/Logs", Descricao = "Visualização de logs de depuração.", SvgIcone = SvgConfig, CorHexadecimal = CorRoxo }
                    }
                },
                new CategoriaMenuDefinicao
                {
                    Nome = "Conteúdo",
                    SvgIcone = IconeCampanhas,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Comunicados SaaS", Url = "/Contextos/InfraAdmin/SaaS/Comunicados", Descricao = "Envio de mensagens globais para usuários.", SvgIcone = IconeCampanhas, CorHexadecimal = CorVerde },
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Documentos Legais", Url = "/Contextos/InfraAdmin/Documentos", Descricao = "Políticas de privacidade, Termos de uso, etc.", SvgIcone = SvgConfig, CorHexadecimal = CorCinza }
                    }
                }
            }
        },
        new ModuloMenuDefinicao
        {
            Nome = "Geral",
            ModuloRaiz = "*",
            Categorias = new List<CategoriaMenuDefinicao>
            {
                new CategoriaMenuDefinicao
                {
                    Nome = "Configurações",
                    SvgIcone = IconeConfig,
                    Itens = new List<MenuItemDefinicao>
                    {
                        new MenuItemDefinicao { ChavePermissao = "", Titulo = "Gerais", Url = "#", Descricao = "Configurações gerais do sistema.", SvgIcone = SvgConfig, CorHexadecimal = CorCinza, EmBreve = true }
                    }
                }
            }
        }
    };
    public static List<MenuItemDefinicao> ObterTodosItensPlanificados()
    {
        return Modulos.SelectMany(m => m.Categorias).SelectMany(c => c.Itens).ToList();
    }
}
