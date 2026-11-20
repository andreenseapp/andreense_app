using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record DeletarCursoCommand(Guid ContaId, Guid CurriculoId, Guid Id) : IRequest<ResultadoDto>;

public class DeletarCursoCommandHandler : IRequestHandler<DeletarCursoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<DeletarCursoCommandHandler> _logger;

    public DeletarCursoCommandHandler(ICurriculoRepositorio repositorio, ILogger<DeletarCursoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarCursoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deletando curso {Id}", request.Id);
        try
        {
            await _repositorio.DeletarFisicoCursoAsync(request.ContaId, request.CurriculoId, request.Id);
            _logger.LogInformation("Curso {Id} deletado com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar curso {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroDeletarCurso);
        }
    }
}
