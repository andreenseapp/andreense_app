using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.Imobiliaria.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ListarBuscasRecentesQuery(Guid UsuarioId) : IRequest<IReadOnlyList<BuscaRecenteDto>>;

public class ListarBuscasRecentesQueryHandler : IRequestHandler<ListarBuscasRecentesQuery, IReadOnlyList<BuscaRecenteDto>>
{
    private readonly IBuscaRecenteRepositorio _repositorio;
    private readonly ILogger<ListarBuscasRecentesQueryHandler> _logger;

    public ListarBuscasRecentesQueryHandler(
        IBuscaRecenteRepositorio repositorio,
        ILogger<ListarBuscasRecentesQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BuscaRecenteDto>> Handle(ListarBuscasRecentesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando buscas recentes do usuário {UsuarioId}.", request.UsuarioId);

        if (request.UsuarioId == Guid.Empty)
            return [];

        var itens = await _repositorio.ListarPorUsuarioAsync(request.UsuarioId, BuscaRecenteFiltroHelper.Limite);
        return itens
            .Select(b => BuscaRecenteFiltroHelper.MontarDto(
                b.Id,
                b.ChaveFiltro,
                b.FiltroJson,
                b.Rotulo,
                b.DataUltimaBusca))
            .ToList();
    }
}
