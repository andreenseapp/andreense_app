using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarConfiguracoesHorariosCommand(
    Guid EmpresaId,
    IEnumerable<ConfiguracaoHorario> Horarios,
    Guid? ProfissionalUsuarioId = null,
    Guid? ProfissionalContatoId = null
) : IRequest<bool>;

public class AtualizarConfiguracoesHorariosCommandHandler : IRequestHandler<AtualizarConfiguracoesHorariosCommand, bool>
{
    private readonly IAgendamentoRepositorio _repo;
    public AtualizarConfiguracoesHorariosCommandHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<bool> Handle(AtualizarConfiguracoesHorariosCommand cmd, CancellationToken ct)
    {
        await _repo.AtualizarConfiguracoesHorariosAsync(
            cmd.EmpresaId, cmd.Horarios,
            cmd.ProfissionalUsuarioId, cmd.ProfissionalContatoId);
        return true;
    }
}



