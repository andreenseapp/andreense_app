using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class GrupoUsuario
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool EhAdministrador { get; set; } = false;
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public List<RecursoSistema> Recursos { get; set; } = new();
}

