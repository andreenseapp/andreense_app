using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarProximosAgendamentosClienteQuery(Guid ClienteUsuarioId) : IRequest<IEnumerable<Agendamento>>;

public class ListarProximosAgendamentosClienteQueryHandler : IRequestHandler<ListarProximosAgendamentosClienteQuery, IEnumerable<Agendamento>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarProximosAgendamentosClienteQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<Agendamento>> Handle(ListarProximosAgendamentosClienteQuery q, CancellationToken ct)
    {
        return await _repo.ListarProximosPorClienteAsync(q.ClienteUsuarioId);
    }
}

public record ListarHistoricoAgendamentosClienteQuery(Guid ClienteUsuarioId, int Pagina = 1, int TamanhoPagina = 20) : IRequest<ResultadoPaginado<Agendamento>>;

public class ListarHistoricoAgendamentosClienteQueryHandler : IRequestHandler<ListarHistoricoAgendamentosClienteQuery, ResultadoPaginado<Agendamento>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarHistoricoAgendamentosClienteQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<ResultadoPaginado<Agendamento>> Handle(ListarHistoricoAgendamentosClienteQuery q, CancellationToken ct)
    {
        return await _repo.ListarHistoricoPorClienteAsync(q.ClienteUsuarioId, q.Pagina, q.TamanhoPagina);
    }
}
