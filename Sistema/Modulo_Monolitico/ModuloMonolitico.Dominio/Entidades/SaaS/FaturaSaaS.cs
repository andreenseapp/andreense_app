using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class FaturaSaaS
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string MesReferencia { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Situacao { get; set; } = SituacaoFatura.Pendente;

    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? Observacoes { get; set; }
    public Guid? OperadorBaixa { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}

