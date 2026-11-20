using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class FidelidadeLog
{
    public Guid Id { get; set; }
    public Guid SaldoId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? CodigoReferencia { get; set; }
    public DateTime DataInclusao { get; set; }
}

