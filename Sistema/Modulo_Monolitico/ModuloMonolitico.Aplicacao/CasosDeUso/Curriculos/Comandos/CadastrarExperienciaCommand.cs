using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record CadastrarExperienciaCommand(
    Guid ContaId,
    Guid CurriculoId,
    string Empresa,
    string Cargo,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool EmAndamento,
    string? Descricao,
    string? Cidade
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarExperienciaCommandHandler : IRequestHandler<CadastrarExperienciaCommand, ResultadoDto<Guid>>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<CadastrarExperienciaCommandHandler> _logger;

    public CadastrarExperienciaCommandHandler(ICurriculoRepositorio repositorio, ILogger<CadastrarExperienciaCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarExperienciaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cadastrando experiência no currículo {CurriculoId}", request.CurriculoId);

        try
        {
            var entidade = new CurriculoExperiencia
            {
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

            var id = await _repositorio.CadastrarExperienciaAsync(entidade);
            _logger.LogInformation("Experiência {Id} cadastrada no currículo {CurriculoId}", id, request.CurriculoId);
            return ResultadoDto<Guid>.ComSucesso(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar experiência no currículo {CurriculoId}", request.CurriculoId);
            return ResultadoDto<Guid>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroCadastrarExperiencia);
        }
    }
}
