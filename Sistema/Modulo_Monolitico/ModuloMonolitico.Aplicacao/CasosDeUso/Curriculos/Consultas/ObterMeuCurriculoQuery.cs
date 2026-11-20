using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Consultas;

public record ObterMeuCurriculoQuery(Guid ContaId, Guid UsuarioId) : IRequest<CurriculoCompletoDto?>;

public class ObterMeuCurriculoQueryHandler : IRequestHandler<ObterMeuCurriculoQuery, CurriculoCompletoDto?>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<ObterMeuCurriculoQueryHandler> _logger;

    public ObterMeuCurriculoQueryHandler(ICurriculoRepositorio repositorio, ILogger<ObterMeuCurriculoQueryHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<CurriculoCompletoDto?> Handle(ObterMeuCurriculoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo currículo completo do usuário {UsuarioId}", request.UsuarioId);
        return await _repositorio.ObterCompletoDoUsuarioAsync(request.ContaId, request.UsuarioId);
    }
}
