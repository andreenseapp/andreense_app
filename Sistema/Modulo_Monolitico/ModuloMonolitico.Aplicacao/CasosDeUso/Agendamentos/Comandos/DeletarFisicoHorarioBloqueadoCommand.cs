using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarFisicoHorarioBloqueadoCommand(Guid Id, Guid EmpresaId) : IRequest<bool>;

public class DeletarFisicoHorarioBloqueadoCommandHandler : IRequestHandler<DeletarFisicoHorarioBloqueadoCommand, bool>
{
    private readonly IAgendamentoRepositorio _repo;
    public DeletarFisicoHorarioBloqueadoCommandHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<bool> Handle(DeletarFisicoHorarioBloqueadoCommand cmd, CancellationToken ct)
    {
        await _repo.DeletarFisicoHorarioBloqueadoAsync(cmd.Id, cmd.EmpresaId);
        return true;
    }
}



