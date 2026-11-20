using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class GrupoUsuarioSaaS
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public List<RecursoSistemaSaaS> Recursos { get; set; } = new();
}

