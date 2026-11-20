using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuloMonolitico.Dominio.Constantes;

public static class AmenidadesImobiliariaDefinicoes
{
    public const string CategoriaImovel = "imovel";
    public const string CategoriaCondominio = "condominio";
    public const string CategoriaSeguranca = "seguranca";
    public const string CategoriaOutros = "outros";

    public static readonly IReadOnlyList<AmenidadeDefinicao> Todas =
    [
        new("elevador", "Elevador", CategoriaImovel, 10),
        new("varanda", "Varanda / Sacada", CategoriaImovel, 20),
        new("cobertura", "Cobertura", CategoriaImovel, 30),
        new("armarios_embutidos", "Armários embutidos", CategoriaImovel, 40),
        new("ar_condicionado", "Ar-condicionado", CategoriaImovel, 50),
        new("mobiliado", "Mobiliado", CategoriaImovel, 60),
        new("closet", "Closet", CategoriaImovel, 70),
        new("area_servico", "Área de serviço", CategoriaImovel, 80),
        new("lavabo", "Lavabo", CategoriaImovel, 90),
        new("churrasqueira", "Churrasqueira", CategoriaImovel, 100),
        new("agua_separada", "Água separada", CategoriaImovel, 110),
        new("luz_separada", "Luz separada", CategoriaImovel, 120),
        new("entrada_separada", "Entrada separada", CategoriaImovel, 130),
        new("aceita_pet", "Aceita pet", CategoriaImovel, 140),
        new("aquecimento", "Aquecimento", CategoriaImovel, 150),
        new("lareira", "Lareira", CategoriaImovel, 160),
        new("escritorio", "Escritório", CategoriaImovel, 170),
        new("jardim", "Jardim", CategoriaImovel, 180),
        new("quintal", "Quintal", CategoriaImovel, 190),
        new("despensa", "Despensa", CategoriaImovel, 200),
        new("dependencia_empregada", "Dependência de empregada", CategoriaImovel, 210),
        new("gas_encanado", "Gás encanado", CategoriaImovel, 220),
        new("interfone", "Interfone", CategoriaImovel, 230),
        new("piscina", "Piscina", CategoriaCondominio, 10),
        new("salao_festas", "Salão de festas", CategoriaCondominio, 20),
        new("quadra", "Quadra esportiva", CategoriaCondominio, 30),
        new("academia", "Academia", CategoriaCondominio, 40),
        new("playground", "Playground", CategoriaCondominio, 50),
        new("espaco_gourmet", "Espaço gourmet", CategoriaCondominio, 60),
        new("sauna", "Sauna", CategoriaCondominio, 70),
        new("brinquedoteca", "Brinquedoteca", CategoriaCondominio, 80),
        new("lavanderia", "Lavanderia", CategoriaCondominio, 90),
        new("bicicletario", "Bicicletário", CategoriaCondominio, 100),
        new("salao_jogos", "Salão de jogos", CategoriaCondominio, 110),
        new("coworking", "Coworking", CategoriaCondominio, 120),
        new("piscina_aquecida", "Piscina aquecida", CategoriaCondominio, 130),
        new("spa", "Spa", CategoriaCondominio, 140),
        new("sala_cinema", "Sala de cinema", CategoriaCondominio, 150),
        new("portaria_24h", "Portaria 24h", CategoriaSeguranca, 10),
        new("vigilancia", "Vigilância", CategoriaSeguranca, 20),
        new("cameras", "Câmeras de segurança", CategoriaSeguranca, 30),
        new("cerca_eletrica", "Cerca elétrica", CategoriaSeguranca, 40),
        new("alarme", "Alarme", CategoriaSeguranca, 50),
        new("controle_acesso", "Controle de acesso", CategoriaSeguranca, 60),
        new("proximo_metro", "Próximo ao metrô", CategoriaOutros, 10),
    ];

    public static readonly IReadOnlyDictionary<string, string> ColunaAntigaParaCodigo =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["tem_elevador"] = "elevador",
            ["tem_piscina"] = "piscina",
            ["aceita_pet"] = "aceita_pet",
            ["tem_churrasqueira"] = "churrasqueira",
            ["tem_salao_festas"] = "salao_festas",
            ["tem_quadra"] = "quadra",
            ["tem_academia"] = "academia",
            ["tem_portaria_24h"] = "portaria_24h",
            ["tem_varanda"] = "varanda",
            ["tem_playground"] = "playground",
            ["tem_agua_separada"] = "agua_separada",
            ["tem_luz_separada"] = "luz_separada",
            ["tem_entrada_separada"] = "entrada_separada",
            ["tem_cobertura"] = "cobertura",
        };

    public static string NomeCategoria(string categoria) => categoria switch
    {
        CategoriaImovel => "Do imóvel",
        CategoriaCondominio => "Do condomínio",
        CategoriaSeguranca => "Segurança",
        CategoriaOutros => "Outros",
        _ => categoria
    };

    public static IEnumerable<IGrouping<string, AmenidadeDefinicao>> AgruparPorCategoria() =>
        Todas.GroupBy(a => a.Categoria).OrderBy(g => g.Key switch
        {
            CategoriaImovel => 1,
            CategoriaCondominio => 2,
            CategoriaSeguranca => 3,
            CategoriaOutros => 4,
            _ => 9
        });
}

public sealed record AmenidadeDefinicao(string Codigo, string Nome, string Categoria, int Ordem);
