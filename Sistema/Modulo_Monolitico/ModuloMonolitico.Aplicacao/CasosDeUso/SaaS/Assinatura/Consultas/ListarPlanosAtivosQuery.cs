using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarPlanosAtivosQuery() : IRequest<IEnumerable<PlanoSaaS>>;

public class ListarPlanosAtivosQueryHandler : IRequestHandler<ListarPlanosAtivosQuery, IEnumerable<PlanoSaaS>>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ListarPlanosAtivosQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<PlanoSaaS>> Handle(ListarPlanosAtivosQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarPlanosAtivosAsync();
}
