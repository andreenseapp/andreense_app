using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class Conta
{
    public Guid Id { get; set; }
    public string IdentificadorPortal { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
}

