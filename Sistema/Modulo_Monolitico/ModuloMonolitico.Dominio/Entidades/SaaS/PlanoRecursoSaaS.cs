using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class PlanoRecursoSaaS
{
    public Guid PlanoId { get; set; }
    public Guid RecursoId { get; set; }
    public string Valor { get; set; } = "1";
    public string? RecursoChave { get; set; }
    public string? RecursoTipo { get; set; }
}

