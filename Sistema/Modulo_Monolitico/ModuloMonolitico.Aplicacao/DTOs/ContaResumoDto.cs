using System;

namespace ModuloMonolitico.Aplicacao.DTOs;
public class ContaResumoDto
{
    public Guid ContaId { get; set; }
    public string ContaNome { get; set; } = string.Empty;
    public string? EmailTitular { get; set; }
    public string? SituacaoAssinatura { get; set; }
    public string? PlanoNome { get; set; }
    public DateTime? DataVencimento { get; set; }
    public decimal? ValorContratado { get; set; }
    public Guid? AssinaturaId { get; set; }
}
