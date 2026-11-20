using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ContaAssinatura
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid PlanoId { get; set; }
    public string Situacao { get; set; } = SituacaoAssinatura.Trial;

    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime DataVencimento { get; set; }
    public decimal ValorContratado { get; set; }

    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public string? PlanoNome { get; set; }
}

