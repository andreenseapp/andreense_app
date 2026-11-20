using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarGruposSaaSQuery : IRequest<IEnumerable<GrupoUsuarioSaaS>>;

public class ListarGruposSaaSQueryHandler : IRequestHandler<ListarGruposSaaSQuery, IEnumerable<GrupoUsuarioSaaS>>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public ListarGruposSaaSQueryHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<GrupoUsuarioSaaS>> Handle(ListarGruposSaaSQuery comando, CancellationToken cancellationToken)
        => await _repo.ListarTodosAsync();
}



