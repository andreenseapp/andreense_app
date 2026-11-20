using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record IdiomaItem(string Idioma, string Nivel);

public record SalvarIdiomasCommand(Guid ContaId, Guid CurriculoId, IList<IdiomaItem> Idiomas) : IRequest<ResultadoDto>;

public class SalvarIdiomasCommandHandler : IRequestHandler<SalvarIdiomasCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<SalvarIdiomasCommandHandler> _logger;

    public SalvarIdiomasCommandHandler(ICurriculoRepositorio repositorio, ILogger<SalvarIdiomasCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(SalvarIdiomasCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salvando {Qtd} idiomas no currículo {CurriculoId}", request.Idiomas.Count, request.CurriculoId);

        try
        {
            var entidades = request.Idiomas
                .Where(i => !string.IsNullOrWhiteSpace(i.Idioma))
                .Select(i => new CurriculoIdioma { CurriculoId = request.CurriculoId, ContaId = request.ContaId, Idioma = i.Idioma, Nivel = i.Nivel });

            await _repositorio.SubstituirIdiomasAsync(request.CurriculoId, request.ContaId, entidades);
            _logger.LogInformation("Idiomas do currículo {CurriculoId} salvos com sucesso", request.CurriculoId);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar idiomas do currículo {CurriculoId}", request.CurriculoId);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroSalvarIdiomas);
        }
    }
}
