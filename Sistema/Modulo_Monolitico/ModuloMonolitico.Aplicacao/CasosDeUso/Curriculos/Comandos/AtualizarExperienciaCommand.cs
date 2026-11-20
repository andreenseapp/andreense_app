using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record AtualizarExperienciaCommand(
    Guid Id,
    Guid ContaId,
    Guid CurriculoId,
    string Empresa,
    string Cargo,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool EmAndamento,
    string? Descricao,
    string? Cidade
) : IRequest<ResultadoDto>;

public class AtualizarExperienciaCommandHandler : IRequestHandler<AtualizarExperienciaCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<AtualizarExperienciaCommandHandler> _logger;

    public AtualizarExperienciaCommandHandler(ICurriculoRepositorio repositorio, ILogger<AtualizarExperienciaCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AtualizarExperienciaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando experiência {Id} do currículo {CurriculoId}", request.Id, request.CurriculoId);

        try
        {
            var entidade = new CurriculoExperiencia
            {
                Id = request.Id,
                CurriculoId = request.CurriculoId,
                ContaId = request.ContaId,
                Empresa = request.Empresa,
                Cargo = request.Cargo,
                DataInicio = request.DataInicio,
                DataFim = request.DataFim,
                EmAndamento = request.EmAndamento,
                Descricao = request.Descricao,
                Cidade = request.Cidade
            };

            await _repositorio.AtualizarExperienciaAsync(entidade);
            _logger.LogInformation("Experiência {Id} atualizada com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar experiência {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroAtualizarExperiencia);
        }
    }
}
