using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Favoritos.Consultas;

public record ObterImoveisFavoritosQuery(Guid UsuarioId) : IRequest<List<Guid>>;

public class ObterImoveisFavoritosQueryHandler : IRequestHandler<ObterImoveisFavoritosQuery, List<Guid>>
{
    private readonly IImovelFavoritoRepositorio _repositorio;

    public ObterImoveisFavoritosQueryHandler(IImovelFavoritoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Guid>> Handle(ObterImoveisFavoritosQuery request, CancellationToken cancellationToken)
    {
        if (request.UsuarioId == Guid.Empty)
            return new List<Guid>();

        return await _repositorio.ObterIdsFavoritosDoUsuarioAsync(request.UsuarioId);
    }
}
