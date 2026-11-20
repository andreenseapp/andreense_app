using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Consultas;

public record ListarMeusAnunciosVeiculosQuery(Guid UsuarioId) : IRequest<IEnumerable<AnuncioVeiculoResumoDto>>;

public class ListarMeusAnunciosVeiculosQueryHandler : IRequestHandler<ListarMeusAnunciosVeiculosQuery, IEnumerable<AnuncioVeiculoResumoDto>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<ListarMeusAnunciosVeiculosQueryHandler> _logger;

    public ListarMeusAnunciosVeiculosQueryHandler(IAnuncioVeiculoRepositorio repositorio, ILogger<ListarMeusAnunciosVeiculosQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<IEnumerable<AnuncioVeiculoResumoDto>> Handle(ListarMeusAnunciosVeiculosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando anúncios de veículos do usuário {UsuarioId}", request.UsuarioId);
        return await _repositorio.ListarPorUsuarioAsync(request.UsuarioId);
    }
}
