using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class HorarioBloqueado
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid? ProfissionalUsuarioId { get; set; }
    public Guid? ProfissionalContatoId { get; set; }
    public string? NomeProfissional { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string? Motivo { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}

