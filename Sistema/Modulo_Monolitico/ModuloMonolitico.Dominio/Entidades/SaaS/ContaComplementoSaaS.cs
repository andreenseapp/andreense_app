using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ContaComplementoSaaS
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid RecursoId { get; set; }
    public int QuantidadeAdicional { get; set; } = 0;
    public decimal PrecoNegociado { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataFim { get; set; }
    public string? RecursoChave { get; set; }
    public string? RecursoNome { get; set; }
}

