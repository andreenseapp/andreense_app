using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarHorariosBloqueadosQuery(Guid EmpresaId, DateTime? De = null, DateTime? Ate = null)
    : IRequest<IEnumerable<HorarioBloqueado>>;

public class ListarHorariosBloqueadosQueryHandler : IRequestHandler<ListarHorariosBloqueadosQuery, IEnumerable<HorarioBloqueado>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarHorariosBloqueadosQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<HorarioBloqueado>> Handle(ListarHorariosBloqueadosQuery q, CancellationToken ct)
        => await _repo.ListarHorariosBloqueadosAsync(q.EmpresaId, q.De, q.Ate);
}



