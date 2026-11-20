using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarServicosAgendaQuery(Guid ContaId, bool ApenasAtivos = true) : IRequest<IEnumerable<ModuloMonolitico.Dominio.Entidades.Servicos.ServicoAgendavel>>;

public class ListarServicosAgendaQueryHandler : IRequestHandler<ListarServicosAgendaQuery, IEnumerable<ModuloMonolitico.Dominio.Entidades.Servicos.ServicoAgendavel>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ListarServicosAgendaQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<ModuloMonolitico.Dominio.Entidades.Servicos.ServicoAgendavel>> Handle(ListarServicosAgendaQuery q, CancellationToken ct)
        => await _repo.ListarServicosDaContaAsync(q.ContaId, q.ApenasAtivos);
}



