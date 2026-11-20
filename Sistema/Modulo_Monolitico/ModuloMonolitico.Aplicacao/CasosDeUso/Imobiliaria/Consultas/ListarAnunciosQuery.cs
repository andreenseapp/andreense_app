using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ListarAnunciosQuery(FiltroAnuncio Filtro) : IRequest<ResultadoPaginado<AnuncioResumoDto>>;

public class ListarAnunciosQueryHandler : IRequestHandler<ListarAnunciosQuery, ResultadoPaginado<AnuncioResumoDto>>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<ListarAnunciosQueryHandler> _logger;

    public ListarAnunciosQueryHandler(IAnuncioRepositorio repositorio, ILogger<ListarAnunciosQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<AnuncioResumoDto>> Handle(ListarAnunciosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando anúncios imobiliários com filtros: TipoNegocio={TipoNegocio}, Bairro={Bairro}, Pagina={Pagina}",
            request.Filtro.TipoNegocio, request.Filtro.Bairro, request.Filtro.Pagina);

        return await _repositorio.ListarComFiltrosAsync(request.Filtro);
    }
}
