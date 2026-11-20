using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CancelarAgendamentoCommand(
    Guid Id,
    Guid EmpresaId,
    string Motivo,
    string CanceladoPor
) : IRequest<(bool Sucesso, string Mensagem)>;

public class CancelarAgendamentoCommandHandler : IRequestHandler<CancelarAgendamentoCommand, (bool, string)>
{
    private readonly IAgendamentoRepositorio _repo;
    private readonly IServicoEmail _servicoEmail;
    private const int AntecedenciaMinimaCancelamentoHoras = 2;

    public CancelarAgendamentoCommandHandler(IAgendamentoRepositorio repo, IServicoEmail servicoEmail)
    {
        _repo  = repo;
        _servicoEmail = servicoEmail;
    }

    public async Task<(bool, string)> Handle(CancelarAgendamentoCommand cmd, CancellationToken ct)
    {
        var agendamento = await _repo.ObterPorIdAsync(cmd.Id, cmd.EmpresaId);
        if (agendamento == null) return (false, MensagensSistema.Agendamentos.AgendamentoNaoEncontrado);
        if (agendamento.Situacao == "cancelado") return (false, MensagensSistema.Agendamentos.AgendamentoJaCancelado);
        if (agendamento.Situacao == "concluido") return (false, MensagensSistema.Agendamentos.AgendamentoJaConcluido);

        if (cmd.CanceladoPor == "cliente")
        {
            var horasRestantes = (agendamento.DataHoraInicio - DateTime.UtcNow).TotalHours;
            if (horasRestantes < AntecedenciaMinimaCancelamentoHoras)
                return (false, string.Format(MensagensSistema.Agendamentos.CancelamentoAntecedenciaMinima, AntecedenciaMinimaCancelamentoHoras));
        }

        await _repo.AtualizarSituacaoAsync(cmd.Id, cmd.EmpresaId, "cancelado", cmd.Motivo, cmd.CanceladoPor);

        if (!string.IsNullOrWhiteSpace(agendamento.ClienteEmail))
        {
            var dataFormatada = agendamento.DataHoraInicio.ToString("dd/MM/yyyy 'às' HH:mm");
            await _servicoEmail.EnviarEmailAsync(
                agendamento.ClienteEmail,
                MensagensSistema.Agendamentos.AssuntoEmailCancelado,
                string.Format(MensagensSistema.Agendamentos.CorpoEmailCancelado,
                    agendamento.ClienteNome ?? "cliente",
                    dataFormatada,
                    agendamento.NomeTipoServico,
                    string.IsNullOrWhiteSpace(cmd.Motivo) ? "" : $"<p>Motivo: {cmd.Motivo}</p>"));
        }

        return (true, MensagensSistema.Agendamentos.AgendamentoCanceladoComSucesso);
    }
}
