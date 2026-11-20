using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record AtualizarSituacaoAnuncioCommand(Guid AnuncioId, Guid UsuarioId, string NovaSituacao) : IRequest<ResultadoDto>;

public class AtualizarSituacaoAnuncioCommandHandler : IRequestHandler<AtualizarSituacaoAnuncioCommand, ResultadoDto>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<AtualizarSituacaoAnuncioCommandHandler> _logger;

    private static readonly string[] SituacoesPermitidas = ["ativo", "pausado", "vendido", "locado"];

    public AtualizarSituacaoAnuncioCommandHandler(IAnuncioRepositorio repositorio, ILogger<AtualizarSituacaoAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AtualizarSituacaoAnuncioCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alterando status do anúncio {AnuncioId} para '{NovaSituacao}' pelo usuário {UsuarioId}",
            comando.AnuncioId, comando.NovaSituacao, comando.UsuarioId);

        if (!Array.Exists(SituacoesPermitidas, s => s == comando.NovaSituacao))
        {
            _logger.LogWarning("Status inválido '{NovaSituacao}' informado para anúncio {AnuncioId}", comando.NovaSituacao, comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.ErroAtualizarSituacao);
        }

        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioId);
        if (anuncio is null)
        {
            _logger.LogWarning("Anúncio {AnuncioId} não encontrado para alterar status", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
        }

        if (anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning("Usuário {UsuarioId} tentou alterar status do anúncio {AnuncioId} sem permissão", comando.UsuarioId, comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioSemPermissao);
        }

        await _repositorio.AtualizarSituacaoAsync(comando.AnuncioId, comando.NovaSituacao);

        _logger.LogInformation("Status do anúncio {AnuncioId} alterado para '{NovaSituacao}' com sucesso", comando.AnuncioId, comando.NovaSituacao);
        return ResultadoDto.ComSucesso(MensagensSistema.Geral.Sucesso);
    }
}
