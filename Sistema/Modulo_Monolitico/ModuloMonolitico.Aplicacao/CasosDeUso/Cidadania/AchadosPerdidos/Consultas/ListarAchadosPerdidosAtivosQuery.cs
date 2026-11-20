using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarAchadosPerdidosAtivosQuery() : IRequest<IEnumerable<AchadoPerdido>>;

public class ListarAchadosPerdidosAtivosQueryHandler : IRequestHandler<ListarAchadosPerdidosAtivosQuery, IEnumerable<AchadoPerdido>>
{
    private readonly IAchadosPerdidosRepositorio _repositorio;

    public ListarAchadosPerdidosAtivosQueryHandler(IAchadosPerdidosRepositorio repositorio)
        => _repositorio = repositorio;

    public Task<IEnumerable<AchadoPerdido>> Handle(ListarAchadosPerdidosAtivosQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarAtivosAsync();
}
