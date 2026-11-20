using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record AlternarDestaqueAnuncioVeiculoCommand(
    Guid AnuncioVeiculoId,
    Guid UsuarioId,
    bool EhAdministradorMaster = false) : IRequest<ResultadoDto<bool>>;

public class AlternarDestaqueAnuncioVeiculoCommandHandler : IRequestHandler<AlternarDestaqueAnuncioVeiculoCommand, ResultadoDto<bool>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<AlternarDestaqueAnuncioVeiculoCommandHandler> _logger;

    public AlternarDestaqueAnuncioVeiculoCommandHandler(
        IAnuncioVeiculoRepositorio repositorio,
        ILogger<AlternarDestaqueAnuncioVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<bool>> Handle(AlternarDestaqueAnuncioVeiculoCommand comando, CancellationToken cancellationToken)
    {
        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioVeiculoId);
        if (anuncio is null)
            return ResultadoDto<bool>.Falha(MensagensSistema.Veiculos.AnuncioNaoEncontrado);

        if (!comando.EhAdministradorMaster && anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning(
                "Tentativa não autorizada de alterar destaque do anúncio {AnuncioId} pelo usuário {UsuarioId}",
                comando.AnuncioVeiculoId, comando.UsuarioId);
            return ResultadoDto<bool>.Falha(MensagensSistema.Veiculos.AnuncioSemPermissao);
        }

        if (anuncio.Situacao != "ativo" && !anuncio.EhDestaque)
            return ResultadoDto<bool>.Falha(MensagensSistema.Veiculos.DestaqueSomenteAtivo);

        var novoValor = !anuncio.EhDestaque;
        await _repositorio.AlternarDestaqueAsync(comando.AnuncioVeiculoId, novoValor);

        _logger.LogInformation(
            "Destaque do anúncio {AnuncioId} alterado para {EhDestaque} pelo usuário {UsuarioId}",
            comando.AnuncioVeiculoId, novoValor, comando.UsuarioId);

        return ResultadoDto<bool>.ComSucesso(
            novoValor,
            novoValor ? MensagensSistema.Veiculos.DestaqueAtivado : MensagensSistema.Veiculos.DestaqueDesativado);
    }
}
