using MediatR;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;

public record ListarAnunciosFavoritosQuery(Guid UsuarioId, int Pagina = 1, int TamanhoPagina = 12) : IRequest<ResultadoPaginado<AnuncioResumoDto>>;

public class ListarAnunciosFavoritosQueryHandler : IRequestHandler<ListarAnunciosFavoritosQuery, ResultadoPaginado<AnuncioResumoDto>>
{
    private readonly IAnuncioRepositorio _repositorio;

    public ListarAnunciosFavoritosQueryHandler(IAnuncioRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoPaginado<AnuncioResumoDto>> Handle(ListarAnunciosFavoritosQuery request, CancellationToken cancellationToken)
    {
        if (request.UsuarioId == Guid.Empty)
            return new ResultadoPaginado<AnuncioResumoDto>();

        var pagina = request.Pagina < 1 ? 1 : request.Pagina;
        var tamanho = request.TamanhoPagina < 1 ? 12 : request.TamanhoPagina;

        return await _repositorio.ListarPorFavoritosDoUsuarioAsync(request.UsuarioId, pagina, tamanho);
    }
}
