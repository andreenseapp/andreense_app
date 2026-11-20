using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class UsuarioProfissao
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
}

