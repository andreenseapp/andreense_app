using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class CurriculoExperiencia
{
    public Guid Id { get; set; }
    public Guid CurriculoId { get; set; }
    public Guid ContaId { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public bool EmAndamento { get; set; }
    public string? Descricao { get; set; }
    public string? Cidade { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}
