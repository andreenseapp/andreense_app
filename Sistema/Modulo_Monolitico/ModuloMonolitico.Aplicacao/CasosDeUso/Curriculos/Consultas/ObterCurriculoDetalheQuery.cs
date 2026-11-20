using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;

public record ObterCurriculoDetalheQuery(Guid ContaId, Guid CurriculoId) : IRequest<CurriculoDetalheDto?>;

public class ObterCurriculoDetalheQueryHandler : IRequestHandler<ObterCurriculoDetalheQuery, CurriculoDetalheDto?>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<ObterCurriculoDetalheQueryHandler> _logger;

    public ObterCurriculoDetalheQueryHandler(ICurriculoRepositorio repositorio, ILogger<ObterCurriculoDetalheQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<CurriculoDetalheDto?> Handle(ObterCurriculoDetalheQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo detalhe público do currículo {CurriculoId}", request.CurriculoId);
        return await _repositorio.ObterDetalhePublicoAsync(request.ContaId, request.CurriculoId);
    }
}
