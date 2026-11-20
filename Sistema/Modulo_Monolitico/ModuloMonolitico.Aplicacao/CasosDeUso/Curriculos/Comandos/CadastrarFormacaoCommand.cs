using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record CadastrarFormacaoCommand(
    Guid ContaId,
    Guid CurriculoId,
    string Instituicao,
    string Curso,
    string Grau,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool EmAndamento
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarFormacaoCommandHandler : IRequestHandler<CadastrarFormacaoCommand, ResultadoDto<Guid>>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<CadastrarFormacaoCommandHandler> _logger;

    public CadastrarFormacaoCommandHandler(ICurriculoRepositorio repositorio, ILogger<CadastrarFormacaoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarFormacaoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cadastrando formação no currículo {CurriculoId}", request.CurriculoId);

        try
        {
            var entidade = new CurriculoFormacao
            {
                CurriculoId = request.CurriculoId,
                ContaId = request.ContaId,
                Instituicao = request.Instituicao,
                Curso = request.Curso,
                Grau = request.Grau,
                DataInicio = request.DataInicio,
                DataFim = request.DataFim,
                EmAndamento = request.EmAndamento
            };

            var id = await _repositorio.CadastrarFormacaoAsync(entidade);
            _logger.LogInformation("Formação {Id} cadastrada no currículo {CurriculoId}", id, request.CurriculoId);
            return ResultadoDto<Guid>.ComSucesso(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar formação no currículo {CurriculoId}", request.CurriculoId);
            return ResultadoDto<Guid>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroCadastrarFormacao);
        }
    }
}
