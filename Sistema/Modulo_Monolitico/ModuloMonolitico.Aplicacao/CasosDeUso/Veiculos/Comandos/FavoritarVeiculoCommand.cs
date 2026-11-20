using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record FavoritarVeiculoCommand(Guid UsuarioId, Guid AnuncioVeiculoId) : IRequest;

public class FavoritarVeiculoCommandHandler : IRequestHandler<FavoritarVeiculoCommand>
{
    private readonly IVeiculoFavoritoRepositorio _repositorio;
    private readonly ILogger<FavoritarVeiculoCommandHandler> _logger;

    public FavoritarVeiculoCommandHandler(
        IVeiculoFavoritoRepositorio repositorio,
        ILogger<FavoritarVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task Handle(FavoritarVeiculoCommand comando, CancellationToken cancellationToken)
    {
        var ehFavoritado = await _repositorio.EhFavoritadoAsync(comando.UsuarioId, comando.AnuncioVeiculoId);

        if (ehFavoritado)
        {
            await _repositorio.DesfavoritarAsync(comando.UsuarioId, comando.AnuncioVeiculoId);
            _logger.LogInformation("Usuário {UsuarioId} desfavoritou anúncio {AnuncioId}", comando.UsuarioId, comando.AnuncioVeiculoId);
        }
        else
        {
            await _repositorio.FavoritarAsync(new VeiculoFavorito
            {
                UsuarioId = comando.UsuarioId,
                AnuncioVeiculoId = comando.AnuncioVeiculoId
            });
            _logger.LogInformation("Usuário {UsuarioId} favoritou anúncio {AnuncioId}", comando.UsuarioId, comando.AnuncioVeiculoId);
        }
    }
}
