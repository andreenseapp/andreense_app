using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EmpresaTransportador : IContatoPapel
{
    public Guid ContatoId { get; set; }
    public Guid ContaId { get; set; }
    public string? Observacao { get; set; }
}

