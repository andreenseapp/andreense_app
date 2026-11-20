using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarContatosPaginadoQuery(
    Guid ContaId,
    int Pagina,
    int TamanhoPagina,
    string? Busca = null,
    string? Tipo = null,
    DateTime? Desde = null,
    string? OrdenarPor = null,
    string? OrdenarDirecao = null,
    string? Papel = null) : IRequest<ResultadoPaginado<Contato>>;

public class ListarContatosPaginadoQueryHandler
    : IRequestHandler<ListarContatosPaginadoQuery, ResultadoPaginado<Contato>>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public ListarContatosPaginadoQueryHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public Task<ResultadoPaginado<Contato>> Handle(ListarContatosPaginadoQuery comando, CancellationToken cancellationToken)
        => _contatoRepositorio.ListarPaginadoAsync(
            comando.ContaId,
            comando.Pagina,
            comando.TamanhoPagina,
            comando.Busca,
            comando.Tipo,
            comando.Desde,
            comando.OrdenarPor,
            comando.OrdenarDirecao,
            comando.Papel);
}
