using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record DeletarFormacaoCommand(Guid ContaId, Guid CurriculoId, Guid Id) : IRequest<ResultadoDto>;

public class DeletarFormacaoCommandHandler : IRequestHandler<DeletarFormacaoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<DeletarFormacaoCommandHandler> _logger;

    public DeletarFormacaoCommandHandler(ICurriculoRepositorio repositorio, ILogger<DeletarFormacaoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarFormacaoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deletando formação {Id}", request.Id);
        try
        {
            await _repositorio.DeletarFisicoFormacaoAsync(request.ContaId, request.CurriculoId, request.Id);
            _logger.LogInformation("Formação {Id} deletada com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar formação {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroDeletarFormacao);
        }
    }
}
