using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record CadastrarCursoCommand(
    Guid ContaId, Guid CurriculoId,
    string Nome, string? Instituicao, int? CargaHoraria, short? AnoConclusao
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarCursoCommandHandler : IRequestHandler<CadastrarCursoCommand, ResultadoDto<Guid>>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<CadastrarCursoCommandHandler> _logger;

    public CadastrarCursoCommandHandler(ICurriculoRepositorio repositorio, ILogger<CadastrarCursoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarCursoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cadastrando curso no currículo {CurriculoId}", request.CurriculoId);
        try
        {
            var entidade = new CurriculoCurso
            {
                CurriculoId = request.CurriculoId, ContaId = request.ContaId,
                Nome = request.Nome, Instituicao = request.Instituicao,
                CargaHoraria = request.CargaHoraria, AnoConclusao = request.AnoConclusao
            };
            var id = await _repositorio.CadastrarCursoAsync(entidade);
            _logger.LogInformation("Curso {Id} cadastrado no currículo {CurriculoId}", id, request.CurriculoId);
            return ResultadoDto<Guid>.ComSucesso(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar curso no currículo {CurriculoId}", request.CurriculoId);
            return ResultadoDto<Guid>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroCadastrarCurso);
        }
    }
}
