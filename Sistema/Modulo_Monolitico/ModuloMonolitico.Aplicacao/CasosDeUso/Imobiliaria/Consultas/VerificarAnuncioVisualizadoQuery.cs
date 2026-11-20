using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record VerificarAnuncioVisualizadoQuery(Guid UsuarioId, Guid AnuncioId) : IRequest<bool>;

public class VerificarAnuncioVisualizadoQueryHandler : IRequestHandler<VerificarAnuncioVisualizadoQuery, bool>
{
    private readonly IAnuncioVisualizadoRepositorio _repositorio;

    public VerificarAnuncioVisualizadoQueryHandler(IAnuncioVisualizadoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> Handle(VerificarAnuncioVisualizadoQuery request, CancellationToken cancellationToken)
    {
        return await _repositorio.FoiVisualizadoAsync(request.UsuarioId, request.AnuncioId);
    }
}
