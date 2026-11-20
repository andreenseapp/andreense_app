using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Comandos;

public record AlternarImovelFavoritoCommand(Guid UsuarioId, Guid ImovelId) : IRequest<bool>;

public class AlternarImovelFavoritoCommandHandler : IRequestHandler<AlternarImovelFavoritoCommand, bool>
{
    private readonly IImovelFavoritoRepositorio _repositorio;
    private readonly ILogger<AlternarImovelFavoritoCommandHandler> _logger;

    public AlternarImovelFavoritoCommandHandler(IImovelFavoritoRepositorio repositorio, ILogger<AlternarImovelFavoritoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<bool> Handle(AlternarImovelFavoritoCommand request, CancellationToken cancellationToken)
    {
        var favorito = await _repositorio.ObterAsync(request.UsuarioId, request.ImovelId);

        if (favorito != null)
        {
            await _repositorio.RemoverAsync(favorito);
            _logger.LogInformation("Imóvel {ImovelId} desfavoritado pelo usuário {UsuarioId}", request.ImovelId, request.UsuarioId);
            return false;
        }
        else
        {
            favorito = new ImovelFavorito
            {
                UsuarioId = request.UsuarioId,
                ImovelId = request.ImovelId,
                DataCriacao = DateTime.UtcNow
            };
            
            await _repositorio.CadastrarAsync(favorito);
            _logger.LogInformation("Imóvel {ImovelId} favoritado pelo usuário {UsuarioId}", request.ImovelId, request.UsuarioId);
            return true;
        }
    }
}
