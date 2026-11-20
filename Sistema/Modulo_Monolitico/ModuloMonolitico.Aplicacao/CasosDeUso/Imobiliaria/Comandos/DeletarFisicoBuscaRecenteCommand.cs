using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record DeletarFisicoBuscaRecenteCommand(Guid Id, Guid UsuarioId) : IRequest<ResultadoDto>;

public class DeletarFisicoBuscaRecenteCommandHandler : IRequestHandler<DeletarFisicoBuscaRecenteCommand, ResultadoDto>
{
    private readonly IBuscaRecenteRepositorio _repositorio;
    private readonly ILogger<DeletarFisicoBuscaRecenteCommandHandler> _logger;

    public DeletarFisicoBuscaRecenteCommandHandler(
        IBuscaRecenteRepositorio repositorio,
        ILogger<DeletarFisicoBuscaRecenteCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarFisicoBuscaRecenteCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Removendo busca recente {BuscaId} do usuário {UsuarioId}.",
            comando.Id,
            comando.UsuarioId);

        var removida = await _repositorio.DeletarFisicoAsync(comando.Id, comando.UsuarioId);
        if (!removida)
        {
            _logger.LogWarning(
                "Busca recente {BuscaId} não encontrada para o usuário {UsuarioId}.",
                comando.Id,
                comando.UsuarioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.BuscaRecenteNaoEncontrada);
        }

        return ResultadoDto.ComSucesso(MensagensSistema.Imobiliaria.BuscaRecenteRemovida);
    }
}
