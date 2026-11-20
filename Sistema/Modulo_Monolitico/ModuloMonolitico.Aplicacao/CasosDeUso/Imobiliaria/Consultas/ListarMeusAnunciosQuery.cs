using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ListarMeusAnunciosQuery(Guid UsuarioId) : IRequest<IEnumerable<AnuncioResumoDto>>;

public class ListarMeusAnunciosQueryHandler : IRequestHandler<ListarMeusAnunciosQuery, IEnumerable<AnuncioResumoDto>>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<ListarMeusAnunciosQueryHandler> _logger;

    public ListarMeusAnunciosQueryHandler(IAnuncioRepositorio repositorio, ILogger<ListarMeusAnunciosQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<IEnumerable<AnuncioResumoDto>> Handle(ListarMeusAnunciosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando anúncios do usuário {UsuarioId}", request.UsuarioId);
        return await _repositorio.ListarPorUsuarioAsync(request.UsuarioId);
    }
}
