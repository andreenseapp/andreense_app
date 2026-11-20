using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;

public record ListarAnunciosVeiculosQuery(FiltroAnuncioVeiculo Filtro) : IRequest<ResultadoPaginado<AnuncioVeiculoResumoDto>>;

public class ListarAnunciosVeiculosQueryHandler : IRequestHandler<ListarAnunciosVeiculosQuery, ResultadoPaginado<AnuncioVeiculoResumoDto>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<ListarAnunciosVeiculosQueryHandler> _logger;

    public ListarAnunciosVeiculosQueryHandler(IAnuncioVeiculoRepositorio repositorio, ILogger<ListarAnunciosVeiculosQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<AnuncioVeiculoResumoDto>> Handle(ListarAnunciosVeiculosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando anúncios de veículos — Marca={Marca}, Modelo={Modelo}, Pagina={Pagina}",
            request.Filtro.Marca, request.Filtro.Modelo, request.Filtro.Pagina);

        return await _repositorio.ListarComFiltrosAsync(request.Filtro);
    }
}
