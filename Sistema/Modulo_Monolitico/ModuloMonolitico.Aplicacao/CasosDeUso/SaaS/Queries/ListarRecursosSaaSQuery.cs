using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarRecursosSaaSQuery : IRequest<IEnumerable<RecursoSistemaSaaS>>;

public class ListarRecursosSaaSQueryHandler : IRequestHandler<ListarRecursosSaaSQuery, IEnumerable<RecursoSistemaSaaS>>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public ListarRecursosSaaSQueryHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<RecursoSistemaSaaS>> Handle(ListarRecursosSaaSQuery comando, CancellationToken cancellationToken)
        => await _repo.ListarTodosRecursosAsync();
}



