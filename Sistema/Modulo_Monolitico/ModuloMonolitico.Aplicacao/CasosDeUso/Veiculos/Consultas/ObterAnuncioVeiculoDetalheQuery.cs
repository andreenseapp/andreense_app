using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;

public record ObterAnuncioVeiculoDetalheQuery(Guid AnuncioVeiculoId) : IRequest<AnuncioVeiculoDetalheDto?>;

public class ObterAnuncioVeiculoDetalheQueryHandler : IRequestHandler<ObterAnuncioVeiculoDetalheQuery, AnuncioVeiculoDetalheDto?>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<ObterAnuncioVeiculoDetalheQueryHandler> _logger;

    public ObterAnuncioVeiculoDetalheQueryHandler(IAnuncioVeiculoRepositorio repositorio, ILogger<ObterAnuncioVeiculoDetalheQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<AnuncioVeiculoDetalheDto?> Handle(ObterAnuncioVeiculoDetalheQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo detalhe do anúncio de veículo {AnuncioId}", request.AnuncioVeiculoId);

        var dto = await _repositorio.ObterDetalheAsync(request.AnuncioVeiculoId);

        if (dto is null)
        {
            _logger.LogWarning("Anúncio de veículo {AnuncioId} não encontrado", request.AnuncioVeiculoId);
            return null;
        }

        return dto;
    }
}
