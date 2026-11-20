using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ObterAnunciosVisualizadosQuery(Guid UsuarioId) : IRequest<List<Guid>>;

public class ObterAnunciosVisualizadosQueryHandler : IRequestHandler<ObterAnunciosVisualizadosQuery, List<Guid>>
{
    private readonly IAnuncioVisualizadoRepositorio _repositorio;

    public ObterAnunciosVisualizadosQueryHandler(IAnuncioVisualizadoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Guid>> Handle(ObterAnunciosVisualizadosQuery request, CancellationToken cancellationToken)
    {
        return await _repositorio.ObterIdsVisualizadosDoUsuarioAsync(request.UsuarioId);
    }
}
