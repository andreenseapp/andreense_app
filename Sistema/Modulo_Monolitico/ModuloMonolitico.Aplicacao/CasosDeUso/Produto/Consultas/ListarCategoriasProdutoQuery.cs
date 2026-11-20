using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarCategoriasProdutoQuery(Guid ContaId) : IRequest<IEnumerable<string>>;

public class ListarCategoriasProdutoQueryHandler : IRequestHandler<ListarCategoriasProdutoQuery, IEnumerable<string>>
{
    private readonly IProdutoRepositorio _repositorio;

    public ListarCategoriasProdutoQueryHandler(IProdutoRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<IEnumerable<string>> Handle(ListarCategoriasProdutoQuery comando, CancellationToken cancellationToken)
    {
        return await _repositorio.ListarCategoriasAtivasAsync(comando.ContaId);
    }
}
