using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ListarAnunciosDestaqueQuery(string? TipoNegocio, int Limite = 6) : IRequest<IEnumerable<AnuncioResumoDto>>;

public class ListarAnunciosDestaqueQueryHandler : IRequestHandler<ListarAnunciosDestaqueQuery, IEnumerable<AnuncioResumoDto>>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<ListarAnunciosDestaqueQueryHandler> _logger;

    public ListarAnunciosDestaqueQueryHandler(IAnuncioRepositorio repositorio, ILogger<ListarAnunciosDestaqueQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<IEnumerable<AnuncioResumoDto>> Handle(ListarAnunciosDestaqueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando anúncios em destaque: TipoNegocio={TipoNegocio}, Limite={Limite}",
            request.TipoNegocio, request.Limite);

        return await _repositorio.ListarDestaquesAsync(request.TipoNegocio, request.Limite);
    }
}
