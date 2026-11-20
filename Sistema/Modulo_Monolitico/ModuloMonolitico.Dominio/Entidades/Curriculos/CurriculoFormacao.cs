using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class CurriculoFormacao
{
    public Guid Id { get; set; }
    public Guid CurriculoId { get; set; }
    public Guid ContaId { get; set; }
    public string Instituicao { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;
    public string Grau { get; set; } = "graduacao";
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public bool EmAndamento { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}
