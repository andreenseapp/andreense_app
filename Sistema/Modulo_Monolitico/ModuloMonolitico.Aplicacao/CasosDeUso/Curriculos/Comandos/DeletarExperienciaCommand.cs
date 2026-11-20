using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record DeletarExperienciaCommand(Guid ContaId, Guid CurriculoId, Guid Id) : IRequest<ResultadoDto>;

public class DeletarExperienciaCommandHandler : IRequestHandler<DeletarExperienciaCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<DeletarExperienciaCommandHandler> _logger;

    public DeletarExperienciaCommandHandler(ICurriculoRepositorio repositorio, ILogger<DeletarExperienciaCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarExperienciaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deletando experiência {Id} do currículo {CurriculoId}", request.Id, request.CurriculoId);

        try
        {
            await _repositorio.DeletarFisicoExperienciaAsync(request.ContaId, request.CurriculoId, request.Id);
            _logger.LogInformation("Experiência {Id} deletada com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar experiência {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroDeletarExperiencia);
        }
    }
}
