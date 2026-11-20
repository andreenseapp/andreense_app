using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterAgendamentoQuery(Guid Id, Guid EmpresaId) : IRequest<Agendamento?>;

public class ObterAgendamentoQueryHandler : IRequestHandler<ObterAgendamentoQuery, Agendamento?>
{
    private readonly IAgendamentoRepositorio _repo;
    public ObterAgendamentoQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<Agendamento?> Handle(ObterAgendamentoQuery q, CancellationToken ct)
        => await _repo.ObterPorIdAsync(q.Id, q.EmpresaId);
}

public record ObterAgendamentoPorClienteQuery(Guid Id, Guid ClienteUsuarioId) : IRequest<Agendamento?>;

public class ObterAgendamentoPorClienteQueryHandler : IRequestHandler<ObterAgendamentoPorClienteQuery, Agendamento?>
{
    private readonly IAgendamentoRepositorio _repo;
    public ObterAgendamentoPorClienteQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<Agendamento?> Handle(ObterAgendamentoPorClienteQuery q, CancellationToken ct)
        => await _repo.ObterPorIdEClienteAsync(q.Id, q.ClienteUsuarioId);
}



