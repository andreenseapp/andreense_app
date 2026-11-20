using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ConfiguracaoHorario
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid? ProfissionalUsuarioId { get; set; }
    public Guid? ProfissionalContatoId { get; set; }
    public int DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public int IntervaloMinutos { get; set; } = 30;
    public bool Ativo { get; set; } = true;

    public string NomeDiaSemana => DiaSemana switch
    {
        0 => "Domingo",
        1 => "Segunda",
        2 => "Terça",
        3 => "Quarta",
        4 => "Quinta",
        5 => "Sexta",
        6 => "Sábado",
        _ => "?"
    };
}

