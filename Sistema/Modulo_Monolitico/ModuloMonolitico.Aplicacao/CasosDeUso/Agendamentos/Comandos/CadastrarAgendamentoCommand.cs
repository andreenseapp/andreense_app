using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarAgendamentoCommand(
    Guid EmpresaId,
    Guid ProdutoVariacaoId,
    Guid? ProfissionalUsuarioId,
    Guid? ProfissionalContatoId,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    Guid? ClienteUsuarioId,
    string? ClienteNome,
    string? ClienteEmail,
    string? ClienteTelefone,
    string? Observacoes,
    Guid? OperadorInclusao,
    bool EnviaEmailConfirmacao = true
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarAgendamentoCommandHandler : IRequestHandler<CadastrarAgendamentoCommand, ResultadoDto<Guid>>
{
    private readonly IAgendamentoRepositorio _repo;
    private readonly IServicoEmail _servicoEmail;
    private readonly ILogger<CadastrarAgendamentoCommandHandler> _logger;

    public CadastrarAgendamentoCommandHandler(IAgendamentoRepositorio repo, IServicoEmail servicoEmail, ILogger<CadastrarAgendamentoCommandHandler> logger)
    {
        _repo  = repo;
        _servicoEmail = servicoEmail;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarAgendamentoCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation(
            "Cadastrando agendamento na empresa {EmpresaId} para {DataHoraInicio:yyyy-MM-dd HH:mm}.",
            cmd.EmpresaId,
            cmd.DataHoraInicio);

        var existeConflito = await _repo.ExisteConflitoHorarioAsync(
            cmd.EmpresaId, cmd.ProfissionalUsuarioId, cmd.ProfissionalContatoId, cmd.DataHoraInicio, cmd.DataHoraFim);

        if (existeConflito)
        {
            _logger.LogWarning(
                "Conflito de horário ao cadastrar agendamento na empresa {EmpresaId} para {DataHoraInicio:yyyy-MM-dd HH:mm} com profissional usuário {ProfissionalUsuarioId} / contato {ProfissionalContatoId}.",
                cmd.EmpresaId,
                cmd.DataHoraInicio,
                cmd.ProfissionalUsuarioId,
                cmd.ProfissionalContatoId);

            return ResultadoDto<Guid>.Falha(MensagensSistema.Agendamentos.ConflitoHorario);
        }

        var agendamento = new Agendamento
        {
            Id               = Guid.NewGuid(),
            EmpresaId        = cmd.EmpresaId,
            ProdutoVariacaoId = cmd.ProdutoVariacaoId,
            ProfissionalUsuarioId = cmd.ProfissionalUsuarioId,
            ProfissionalContatoId = cmd.ProfissionalContatoId,
            DataHoraInicio   = cmd.DataHoraInicio,
            DataHoraFim      = cmd.DataHoraFim,
            ClienteUsuarioId = cmd.ClienteUsuarioId,
            ClienteNome      = cmd.ClienteNome,
            ClienteEmail     = cmd.ClienteEmail,
            ClienteTelefone  = cmd.ClienteTelefone,
            Observacoes      = cmd.Observacoes,
            Situacao           = "pendente",
            DataInclusao     = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow,
            OperadorInclusao = cmd.OperadorInclusao
        };

        await _repo.CadastrarAsync(agendamento);

        _logger.LogInformation("Agendamento {AgendamentoId} cadastrado com sucesso na empresa {EmpresaId}.", agendamento.Id, cmd.EmpresaId);

        if (cmd.EnviaEmailConfirmacao && !string.IsNullOrWhiteSpace(cmd.ClienteEmail))
        {
            var dataFormatada = cmd.DataHoraInicio.ToString("dd/MM/yyyy 'às' HH:mm");
            await _servicoEmail.EnviarEmailAsync(
                cmd.ClienteEmail,
                MensagensSistema.Agendamentos.AssuntoEmailConfirmado,
                string.Format(MensagensSistema.Agendamentos.CorpoEmailConfirmado, cmd.ClienteNome ?? "cliente", dataFormatada));
        }

        return ResultadoDto<Guid>.ComSucesso(agendamento.Id);
    }
}




