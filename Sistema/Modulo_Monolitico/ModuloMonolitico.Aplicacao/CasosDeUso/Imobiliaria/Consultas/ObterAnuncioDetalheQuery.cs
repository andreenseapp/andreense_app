using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ObterAnuncioDetalheQuery(Guid AnuncioId) : IRequest<AnuncioDetalheDto?>;

public class ObterAnuncioDetalheQueryHandler : IRequestHandler<ObterAnuncioDetalheQuery, AnuncioDetalheDto?>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<ObterAnuncioDetalheQueryHandler> _logger;
    private static readonly Random _random = new();

    public ObterAnuncioDetalheQueryHandler(IAnuncioRepositorio repositorio, ILogger<ObterAnuncioDetalheQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<AnuncioDetalheDto?> Handle(ObterAnuncioDetalheQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo detalhe do anúncio {AnuncioId}", request.AnuncioId);

        var dto = await _repositorio.ObterDetalheAsync(request.AnuncioId);

        if (dto is null)
        {
            _logger.LogWarning("Anúncio {AnuncioId} não encontrado", request.AnuncioId);
            return null;
        }

        if (dto.Latitude.HasValue && dto.Longitude.HasValue && dto.Logradouro is null)
            AplicarOfuscacaoCoordenadas(dto);

        return dto;
    }

    private static void AplicarOfuscacaoCoordenadas(AnuncioDetalheDto dto)
    {
        var jitterLat = (decimal)(_random.NextDouble() * 0.004 - 0.002);
        var jitterLng = (decimal)(_random.NextDouble() * 0.004 - 0.002);
        dto.Latitude = dto.Latitude + jitterLat;
        dto.Longitude = dto.Longitude + jitterLng;
    }
}
