using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarAgendamentosQuery(
    Guid EmpresaId,
    DateTime De,
    DateTime Ate,
    Guid? ProfissionalUsuarioId = null,
    Guid? ProfissionalContatoId = null
) : IRequest<IEnumerable<Agendamento>>;

public class ListarAgendamentosQueryHandler : IRequestHandler<ListarAgendamentosQuery, IEnumerable<Agendamento>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarAgendamentosQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<Agendamento>> Handle(ListarAgendamentosQuery q, CancellationToken ct)
        => await _repo.ListarPorPeriodoAsync(q.EmpresaId, q.De, q.Ate, q.ProfissionalUsuarioId, q.ProfissionalContatoId);
}



