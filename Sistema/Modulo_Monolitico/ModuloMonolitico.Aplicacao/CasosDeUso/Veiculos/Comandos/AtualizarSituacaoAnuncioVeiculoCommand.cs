using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record AtualizarSituacaoAnuncioVeiculoCommand(
    Guid AnuncioVeiculoId,
    Guid UsuarioId,
    string NovaSituacao,
    bool EhAdministradorMaster = false) : IRequest<ResultadoDto<bool>>;

public class AtualizarSituacaoAnuncioVeiculoCommandHandler : IRequestHandler<AtualizarSituacaoAnuncioVeiculoCommand, ResultadoDto<bool>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<AtualizarSituacaoAnuncioVeiculoCommandHandler> _logger;

    public AtualizarSituacaoAnuncioVeiculoCommandHandler(
        IAnuncioVeiculoRepositorio repositorio,
        ILogger<AtualizarSituacaoAnuncioVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<bool>> Handle(AtualizarSituacaoAnuncioVeiculoCommand comando, CancellationToken cancellationToken)
    {
        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioVeiculoId);
        if (anuncio is null)
            return ResultadoDto<bool>.Falha(MensagensSistema.Veiculos.AnuncioNaoEncontrado);

        if (!comando.EhAdministradorMaster && anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning(
                "Tentativa não autorizada de alterar situação do anúncio {AnuncioId} pelo usuário {UsuarioId}",
                comando.AnuncioVeiculoId, comando.UsuarioId);
            return ResultadoDto<bool>.Falha(MensagensSistema.Veiculos.AnuncioSemPermissao);
        }

        await _repositorio.AtualizarSituacaoAsync(comando.AnuncioVeiculoId, comando.NovaSituacao);

        _logger.LogInformation(
            "Situação do anúncio {AnuncioId} alterada para {NovaSituacao} pelo usuário {UsuarioId}",
            comando.AnuncioVeiculoId, comando.NovaSituacao, comando.UsuarioId);

        return ResultadoDto<bool>.ComSucesso(true, MensagensSistema.Geral.Sucesso);
    }
}
