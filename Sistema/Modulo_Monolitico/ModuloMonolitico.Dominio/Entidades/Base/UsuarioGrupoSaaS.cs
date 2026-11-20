using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class UsuarioGrupoSaaS
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid GrupoSaaSId { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public string? NomeUsuario { get; set; }
    public string? EmailUsuario { get; set; }
    public string? NomeGrupo { get; set; }
}

