using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record DeletarFisicoTodasBuscasRecentesCommand(Guid UsuarioId) : IRequest<ResultadoDto>;

public class DeletarFisicoTodasBuscasRecentesCommandHandler : IRequestHandler<DeletarFisicoTodasBuscasRecentesCommand, ResultadoDto>
{
    private readonly IBuscaRecenteRepositorio _repositorio;
    private readonly ILogger<DeletarFisicoTodasBuscasRecentesCommandHandler> _logger;

    public DeletarFisicoTodasBuscasRecentesCommandHandler(
        IBuscaRecenteRepositorio repositorio,
        ILogger<DeletarFisicoTodasBuscasRecentesCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarFisicoTodasBuscasRecentesCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Limpando todas as buscas recentes do usuário {UsuarioId}.", comando.UsuarioId);

        await _repositorio.DeletarFisicoTodasAsync(comando.UsuarioId);

        return ResultadoDto.ComSucesso(MensagensSistema.Imobiliaria.BuscasRecentesLimpas);
    }
}
