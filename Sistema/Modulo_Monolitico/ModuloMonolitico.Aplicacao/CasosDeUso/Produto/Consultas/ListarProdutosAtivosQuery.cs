using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarProdutosAtivosQuery(Guid ContaId) : IRequest<IEnumerable<Produto>>;

public class ListarProdutosAtivosQueryHandler : IRequestHandler<ListarProdutosAtivosQuery, IEnumerable<Produto>>
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public ListarProdutosAtivosQueryHandler(IProdutoRepositorio produtoRepositorio)
        => _produtoRepositorio = produtoRepositorio;

    public async Task<IEnumerable<Produto>> Handle(ListarProdutosAtivosQuery comando, CancellationToken cancellationToken)
    {
        var produtos = await _produtoRepositorio.ListarPorContaIdAsync(comando.ContaId);
        return produtos.Where(p => p.Ativo);
    }
}
