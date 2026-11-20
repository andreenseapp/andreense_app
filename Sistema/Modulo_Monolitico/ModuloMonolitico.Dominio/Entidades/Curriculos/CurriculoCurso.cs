using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class CurriculoCurso
{
    public Guid Id { get; set; }
    public Guid CurriculoId { get; set; }
    public Guid ContaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Instituicao { get; set; }
    public int? CargaHoraria { get; set; }
    public short? AnoConclusao { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}
