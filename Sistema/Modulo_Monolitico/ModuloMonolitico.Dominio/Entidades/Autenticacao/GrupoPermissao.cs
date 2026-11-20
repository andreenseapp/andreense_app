using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class GrupoPermissao
{
    public Guid Id { get; set; }
    public Guid GrupoId { get; set; }
    public Guid RecursoId { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public bool AtalhoDashboard { get; set; } = false;
}

