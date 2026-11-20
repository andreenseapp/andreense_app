using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class CurriculoIdioma
{
    public Guid Id { get; set; }
    public Guid CurriculoId { get; set; }
    public Guid ContaId { get; set; }
    public string Idioma { get; set; } = string.Empty;
    public string Nivel { get; set; } = "basico";
}
