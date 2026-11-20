using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class GrupoPermissaoSaaS
{
    public Guid Id { get; set; }
    public Guid GrupoSaaSId { get; set; }
    public Guid RecursoSaaSId { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
}

