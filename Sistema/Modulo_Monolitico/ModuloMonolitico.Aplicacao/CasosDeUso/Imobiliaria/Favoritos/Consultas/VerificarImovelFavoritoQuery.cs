using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;

public record VerificarImovelFavoritoQuery(Guid UsuarioId, Guid ImovelId) : IRequest<bool>;

public class VerificarImovelFavoritoQueryHandler : IRequestHandler<VerificarImovelFavoritoQuery, bool>
{
    private readonly IImovelFavoritoRepositorio _repositorio;

    public VerificarImovelFavoritoQueryHandler(IImovelFavoritoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> Handle(VerificarImovelFavoritoQuery request, CancellationToken cancellationToken)
    {
        if (request.UsuarioId == Guid.Empty || request.ImovelId == Guid.Empty)
            return false;

        var favorito = await _repositorio.ObterAsync(request.UsuarioId, request.ImovelId);
        return favorito != null;
    }
}
