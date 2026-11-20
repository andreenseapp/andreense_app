using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record AtualizarCursoCommand(
    Guid Id, Guid ContaId, Guid CurriculoId,
    string Nome, string? Instituicao, int? CargaHoraria, short? AnoConclusao
) : IRequest<ResultadoDto>;

public class AtualizarCursoCommandHandler : IRequestHandler<AtualizarCursoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<AtualizarCursoCommandHandler> _logger;

    public AtualizarCursoCommandHandler(ICurriculoRepositorio repositorio, ILogger<AtualizarCursoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando curso {Id}", request.Id);
        try
        {
            var entidade = new CurriculoCurso
            {
                Id = request.Id, CurriculoId = request.CurriculoId, ContaId = request.ContaId,
                Nome = request.Nome, Instituicao = request.Instituicao,
                CargaHoraria = request.CargaHoraria, AnoConclusao = request.AnoConclusao
            };
            await _repositorio.AtualizarCursoAsync(entidade);
            _logger.LogInformation("Curso {Id} atualizado com sucesso", request.Id);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar curso {Id}", request.Id);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroAtualizarCurso);
        }
    }
}
