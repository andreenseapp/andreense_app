using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class CurriculoHabilidade
{
    public Guid Id { get; set; }
    public Guid CurriculoId { get; set; }
    public Guid ContaId { get; set; }
    public string Habilidade { get; set; } = string.Empty;
}
