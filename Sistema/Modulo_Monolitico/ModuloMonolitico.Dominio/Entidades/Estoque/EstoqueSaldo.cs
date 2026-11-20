using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EstoqueSaldo
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProdutoVariacaoId { get; set; }
    public Guid LocalEstoqueId { get; set; }
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
    public DateTime? DataFabricacao { get; set; }
    public decimal QuantidadeAtual { get; set; } = 0;
    public string Situacao { get; set; } = "disponivel";
    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimaAtualizacao { get; set; }
    public string? LocalNome { get; set; }
    public string? AreaNome { get; set; }
    public string? ProdutoNome { get; set; }
    public string? VariacaoNome { get; set; }
}

