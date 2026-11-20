using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record SalvarHabilidadesCommand(Guid ContaId, Guid CurriculoId, IList<string> Habilidades) : IRequest<ResultadoDto>;

public class SalvarHabilidadesCommandHandler : IRequestHandler<SalvarHabilidadesCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<SalvarHabilidadesCommandHandler> _logger;

    public SalvarHabilidadesCommandHandler(ICurriculoRepositorio repositorio, ILogger<SalvarHabilidadesCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(SalvarHabilidadesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salvando {Qtd} habilidades no currículo {CurriculoId}", request.Habilidades.Count, request.CurriculoId);

        try
        {
            var habilidadesFiltradas = request.Habilidades.Where(h => !string.IsNullOrWhiteSpace(h));
            await _repositorio.SubstituirHabilidadesAsync(request.CurriculoId, request.ContaId, habilidadesFiltradas);
            _logger.LogInformation("Habilidades do currículo {CurriculoId} salvas com sucesso", request.CurriculoId);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar habilidades do currículo {CurriculoId}", request.CurriculoId);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroSalvarHabilidades);
        }
    }
}
