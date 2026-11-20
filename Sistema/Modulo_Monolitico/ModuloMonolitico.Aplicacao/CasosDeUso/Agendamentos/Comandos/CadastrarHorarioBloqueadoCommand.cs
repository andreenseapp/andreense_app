using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarHorarioBloqueadoCommand(
    Guid EmpresaId,
    Guid? ProfissionalUsuarioId,
    Guid? ProfissionalContatoId,
    DateTime DataInicio,
    DateTime DataFim,
    string? Motivo
) : IRequest<bool>;

public class CadastrarHorarioBloqueadoCommandHandler : IRequestHandler<CadastrarHorarioBloqueadoCommand, bool>
{
    private readonly IAgendamentoRepositorio _repo;
    public CadastrarHorarioBloqueadoCommandHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<bool> Handle(CadastrarHorarioBloqueadoCommand cmd, CancellationToken ct)
    {
        await _repo.CadastrarHorarioBloqueadoAsync(new HorarioBloqueado
        {
            Id = Guid.NewGuid(), EmpresaId = cmd.EmpresaId, 
            ProfissionalUsuarioId = cmd.ProfissionalUsuarioId, ProfissionalContatoId = cmd.ProfissionalContatoId,
            DataInicio = cmd.DataInicio, DataFim = cmd.DataFim, Motivo = cmd.Motivo
        });
        return true;
    }
}



