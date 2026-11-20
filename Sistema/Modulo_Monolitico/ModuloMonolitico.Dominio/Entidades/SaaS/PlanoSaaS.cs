using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class PlanoSaaS
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;

    public List<PlanoRecursoSaaS> Recursos { get; set; } = new();
}

