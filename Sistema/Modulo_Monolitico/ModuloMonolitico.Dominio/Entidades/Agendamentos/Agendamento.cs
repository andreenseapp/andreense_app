using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class Agendamento
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProdutoVariacaoId { get; set; }
    public Guid? ProfissionalUsuarioId { get; set; }
    public Guid? ProfissionalContatoId { get; set; }

    public Guid? ClienteUsuarioId { get; set; }
    public string? ClienteNome { get; set; }
    public string? ClienteEmail { get; set; }
    public string? ClienteTelefone { get; set; }

    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public string Situacao { get; set; } = "pendente";

    public string? Observacoes { get; set; }
    public string? MotivoCancelamento { get; set; }
    public string? CanceladoPor { get; set; }

    public decimal? PrecoCobrado { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public Guid? OperadorInclusao { get; set; }
    public string? NomeTipoServico { get; set; }
    public string? CorHexadecimalServico { get; set; }
    public string? NomeProfissional { get; set; }
    public int DuracaoMinutos { get; set; }

    public string RotuloSituacao => Situacao switch
    {
        "pendente" => "Pendente",
        "confirmado" => "Confirmado",
        "concluido" => "Concluído",
        "cancelado" => "Cancelado",
        "faltou" => "Faltou",
        _ => Situacao
    };

    public string ClasseCssSituacao => Situacao switch
    {
        "pendente" => "situacao-pendente",
        "confirmado" => "situacao-confirmado",
        "concluido" => "situacao-concluido",
        "cancelado" => "situacao-cancelado",
        "faltou" => "situacao-faltou",
        _ => ""
    };
}

