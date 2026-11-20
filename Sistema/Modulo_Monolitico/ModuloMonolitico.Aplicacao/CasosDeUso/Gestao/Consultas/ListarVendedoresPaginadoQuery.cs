using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarVendedoresPaginadoQuery(
    Guid ContaId,
    int Pagina,
    int TamanhoPagina,
    string? Busca,
    string? OrdenarPor,
    string? OrdenarDirecao) : IRequest<ResultadoPaginado<Vendedor>>;

public class ListarVendedoresPaginadoQueryHandler
    : IRequestHandler<ListarVendedoresPaginadoQuery, ResultadoPaginado<Vendedor>>
{
    private readonly IVendedorRepositorio _vendedorRepositorio;

    public ListarVendedoresPaginadoQueryHandler(IVendedorRepositorio vendedorRepositorio)
        => _vendedorRepositorio = vendedorRepositorio;

    public Task<ResultadoPaginado<Vendedor>> Handle(
        ListarVendedoresPaginadoQuery comando,
        CancellationToken cancellationToken)
        => _vendedorRepositorio.ListarPaginadoAsync(
            comando.ContaId,
            comando.Pagina,
            comando.TamanhoPagina,
            comando.Busca,
            comando.OrdenarPor,
            comando.OrdenarDirecao);
}
