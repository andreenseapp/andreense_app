using System;

namespace ModuloMonolitico.Aplicacao.DTOs;
public class HorarioDisponivel
{
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public Guid? ProfissionalUsuarioId { get; set; }
    public Guid? ProfissionalContatoId { get; set; }
    public string NomeProfissional { get; set; } = string.Empty;
    public string HoraFormatada => DataHoraInicio.ToString("HH:mm");
}
