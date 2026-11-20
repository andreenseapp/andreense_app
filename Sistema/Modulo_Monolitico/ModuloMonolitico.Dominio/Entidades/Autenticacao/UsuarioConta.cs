using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class UsuarioConta
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid ContaId { get; set; }
    public bool EhTitular { get; set; }
    public bool Ativo { get; set; } = true;
}

