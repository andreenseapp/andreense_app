using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarSituacaoAgendamentoCommand(
    Guid Id,
    Guid EmpresaId,
    string NovaSituacao,
    string? Motivo = null
) : IRequest<bool>;

public class AtualizarSituacaoAgendamentoCommandHandler : IRequestHandler<AtualizarSituacaoAgendamentoCommand, bool>
{
    private readonly IAgendamentoRepositorio _repo;
    private readonly IServicoEmail _servicoEmail;

    public AtualizarSituacaoAgendamentoCommandHandler(IAgendamentoRepositorio repo, IServicoEmail servicoEmail)
    {
        _repo  = repo;
        _servicoEmail = servicoEmail;
    }

    public async Task<bool> Handle(AtualizarSituacaoAgendamentoCommand cmd, CancellationToken ct)
    {
        var agendamento = await _repo.ObterPorIdAsync(cmd.Id, cmd.EmpresaId);
        if (agendamento == null) return false;

        await _repo.AtualizarSituacaoAsync(cmd.Id, cmd.EmpresaId, cmd.NovaSituacao, cmd.Motivo, canceladoPor: null);

        if (!string.IsNullOrWhiteSpace(agendamento.ClienteEmail) && cmd.NovaSituacao == "confirmado")
        {
            var dataFormatada = agendamento.DataHoraInicio.ToString("dd/MM/yyyy 'às' HH:mm");
            await _servicoEmail.EnviarEmailAsync(
                agendamento.ClienteEmail,
                MensagensSistema.Agendamentos.AssuntoEmailSituacaoAtualizada,
                string.Format(MensagensSistema.Agendamentos.CorpoEmailSituacaoAtualizada,
                    agendamento.ClienteNome ?? "cliente",
                    dataFormatada,
                    agendamento.NomeTipoServico,
                    agendamento.NomeProfissional));
        }

        return true;
    }
}
