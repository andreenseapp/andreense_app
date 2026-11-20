using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarCategoriasQuery(Guid ContaId) : IRequest<IEnumerable<CategoriaDto>>;

public class ListarCategoriasQueryHandler : IRequestHandler<ListarCategoriasQuery, IEnumerable<CategoriaDto>>
{
    private readonly ICategoriaRepositorio _categoriaRepositorio;

    public ListarCategoriasQueryHandler(ICategoriaRepositorio categoriaRepositorio)
        => _categoriaRepositorio = categoriaRepositorio;

    public Task<IEnumerable<CategoriaDto>> Handle(ListarCategoriasQuery comando, CancellationToken cancellationToken)
        => _categoriaRepositorio.ListarPorContaAsync(comando.ContaId);
}
