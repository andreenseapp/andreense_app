using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;

public record ListarCurriculosQuery(Guid ContaId, FiltroCurriculo Filtro) : IRequest<ResultadoPaginado<CurriculoResumoDto>>;

public class ListarCurriculosQueryHandler : IRequestHandler<ListarCurriculosQuery, ResultadoPaginado<CurriculoResumoDto>>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<ListarCurriculosQueryHandler> _logger;

    public ListarCurriculosQueryHandler(ICurriculoRepositorio repositorio, ILogger<ListarCurriculosQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<CurriculoResumoDto>> Handle(ListarCurriculosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando currículos com filtros: Area={Area}, Cargo={Cargo}, Pagina={Pagina}",
            request.Filtro.AreaAtuacao, request.Filtro.CargoDesejado, request.Filtro.Pagina);

        return await _repositorio.ListarComFiltrosAsync(request.ContaId, request.Filtro);
    }
}
