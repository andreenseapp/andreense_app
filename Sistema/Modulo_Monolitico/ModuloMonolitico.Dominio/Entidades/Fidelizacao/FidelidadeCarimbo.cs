using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class FidelidadeCarimbo
{
    public Guid Id { get; set; }
    public Guid SaldoId { get; set; }
    public string Situacao { get; set; } = "valido";
    public string? CodigoReferencia { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; }
}

