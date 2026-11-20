using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarProdutosPaginadoQuery(
    Guid ContaId,
    int Pagina,
    int TamanhoPagina,
    string? Busca = null,
    string? Tipo = null,
    string? OrdenarPor = null,
    string? OrdenarDirecao = null) : IRequest<ResultadoPaginado<Produto>>;

public class ListarProdutosPaginadoQueryHandler
    : IRequestHandler<ListarProdutosPaginadoQuery, ResultadoPaginado<Produto>>
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public ListarProdutosPaginadoQueryHandler(IProdutoRepositorio produtoRepositorio)
        => _produtoRepositorio = produtoRepositorio;

    public Task<ResultadoPaginado<Produto>> Handle(ListarProdutosPaginadoQuery comando, CancellationToken cancellationToken)
        => _produtoRepositorio.ListarPaginadoAsync(
            comando.ContaId, comando.Pagina, comando.TamanhoPagina,
            comando.Busca, comando.Tipo, comando.OrdenarPor, comando.OrdenarDirecao);
}
