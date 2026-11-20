using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record AlternarDestaqueAnuncioCommand(
    Guid AnuncioId,
    Guid UsuarioId,
    bool EhAdministradorMaster = false) : IRequest<ResultadoDto<bool>>;

public class AlternarDestaqueAnuncioCommandHandler : IRequestHandler<AlternarDestaqueAnuncioCommand, ResultadoDto<bool>>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<AlternarDestaqueAnuncioCommandHandler> _logger;

    public AlternarDestaqueAnuncioCommandHandler(IAnuncioRepositorio repositorio, ILogger<AlternarDestaqueAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<bool>> Handle(AlternarDestaqueAnuncioCommand comando, CancellationToken cancellationToken)
    {
        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioId);
        if (anuncio is null)
            return ResultadoDto<bool>.Falha(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);

        if (!comando.EhAdministradorMaster && anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning(
                "Tentativa não autorizada de alterar destaque do anúncio {AnuncioId} pelo usuário {UsuarioId}",
                comando.AnuncioId, comando.UsuarioId);
            return ResultadoDto<bool>.Falha(MensagensSistema.Imobiliaria.AnuncioSemPermissao);
        }

        if (anuncio.Situacao != "ativo" && !anuncio.EhDestaque)
            return ResultadoDto<bool>.Falha(MensagensSistema.Imobiliaria.DestaqueSomenteAtivo);

        var novoValor = !anuncio.EhDestaque;
        await _repositorio.AlternarDestaqueAsync(comando.AnuncioId, novoValor);

        _logger.LogInformation(
            "Destaque do anúncio {AnuncioId} alterado para {EhDestaque} pelo usuário {UsuarioId} (master={EhMaster})",
            comando.AnuncioId, novoValor, comando.UsuarioId, comando.EhAdministradorMaster);

        return ResultadoDto<bool>.ComSucesso(
            novoValor,
            novoValor ? MensagensSistema.Imobiliaria.DestaqueAtivado : MensagensSistema.Imobiliaria.DestaqueDesativado);
    }
}
