using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ProdutoVariacao
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid ContaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string? CodigoSku { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; } = 0;
    public decimal PesoLiquido { get; set; } = 0;
    public decimal PesoBruto { get; set; } = 0;
    public decimal? AlturaCm { get; set; }
    public decimal? LarguraCm { get; set; }
    public decimal? ProfundidadeCm { get; set; }
    public string? UnidadeTributavel { get; set; }
    public decimal FatorConversao { get; set; } = 1;
    public int PrazoEntregaDias { get; set; } = 0;
    public decimal LoteMultiplo { get; set; } = 1;
    public int DuracaoMinutos { get; set; } = 60;
    public string CorHexadecimal { get; set; } = "#6366f1";

    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}

