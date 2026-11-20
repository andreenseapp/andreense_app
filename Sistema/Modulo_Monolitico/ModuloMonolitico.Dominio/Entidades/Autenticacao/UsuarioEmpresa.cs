using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class UsuarioEmpresa
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid? GrupoId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

