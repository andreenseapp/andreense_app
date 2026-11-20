using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record AtualizarFormacaoCommand(
    Guid Id, Guid ContaId, Guid CurriculoId,
    string Instituicao, string Curso, string Grau,
    DateOnly DataInicio, DateOnly? DataFim, bool EmAndamento
) : IRequest<ResultadoDto>;

public class AtualizarFormacaoCommandHandler : IRequestHandler<AtualizarFormacaoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<AtualizarFormacaoCommandHandler> _logger;

    public AtualizarFormacaoCommandHandler(ICurriculoRepositorio repositorio, ILogger<AtualizarFormacaoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AtualizarFormacaoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando formação {Id}", request.Id);
        try
        {
            var entidade = new CurriculoFormacao
            {
                Id = request.Id, CurriculoId = request.CurriculoId, ContaId = request.ContaId,
                Instituicao = request.Instituicao, Curso = request.Curso, Grau = request.Grau,
                DataInicio = request.DataInicio, DataFim = request.DataFim, EmAndamento = request.EmAndamento
            };
            await _repositorio.AtualizarFormacaoAsync(entidade);
            _logger.LogInformation("Formação {Id} atualizada com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar formação {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroAtualizarFormacao);
        }
    }
}
