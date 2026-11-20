using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EmpresaCliente : IContatoPapel
{
    public Guid ContatoId { get; set; }
    public Guid ContaId { get; set; }
    public decimal LimiteCredito { get; set; }
    public decimal DescontoPadrao { get; set; }
    public string? Observacao { get; set; }
}

