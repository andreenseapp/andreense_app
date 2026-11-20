using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record RegistrarVisualizacaoAnuncioCommand(Guid UsuarioId, Guid AnuncioId) : IRequest;

public class RegistrarVisualizacaoAnuncioCommandHandler : IRequestHandler<RegistrarVisualizacaoAnuncioCommand>
{
    private readonly IAnuncioVisualizadoRepositorio _repositorio;
    private readonly IAnuncioRepositorio _anuncioRepositorio;
    private readonly ILogger<RegistrarVisualizacaoAnuncioCommandHandler> _logger;

    public RegistrarVisualizacaoAnuncioCommandHandler(
        IAnuncioVisualizadoRepositorio repositorio,
        IAnuncioRepositorio anuncioRepositorio,
        ILogger<RegistrarVisualizacaoAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _anuncioRepositorio = anuncioRepositorio;
        _logger = logger;
    }

    public async Task Handle(RegistrarVisualizacaoAnuncioCommand request, CancellationToken cancellationToken)
    {
        var anuncio = await _anuncioRepositorio.ObterAnuncioPorIdAsync(request.AnuncioId);
        if (anuncio is null || anuncio.Situacao != "ativo")
            return;

        await _repositorio.RegistrarAsync(new AnuncioVisualizado
        {
            UsuarioId = request.UsuarioId,
            AnuncioId = request.AnuncioId,
            DataVisualizacao = DateTime.UtcNow
        });

        _logger.LogInformation("Visualização registrada: anúncio {AnuncioId} pelo usuário {UsuarioId}",
            request.AnuncioId, request.UsuarioId);
    }
}
