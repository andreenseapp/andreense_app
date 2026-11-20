using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record DeletarFisicoFotoAnuncioCommand(Guid FotoId, Guid UsuarioId) : IRequest<ResultadoDto>;

public class DeletarFisicoFotoAnuncioCommandHandler : IRequestHandler<DeletarFisicoFotoAnuncioCommand, ResultadoDto>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly IServicoArmazenamento _servicoArmazenamento;
    private readonly ILogger<DeletarFisicoFotoAnuncioCommandHandler> _logger;

    public DeletarFisicoFotoAnuncioCommandHandler(
        IAnuncioRepositorio repositorio,
        IServicoArmazenamento servicoArmazenamento,
        ILogger<DeletarFisicoFotoAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _servicoArmazenamento = servicoArmazenamento;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(DeletarFisicoFotoAnuncioCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removendo foto {FotoId} pelo usuário {UsuarioId}", comando.FotoId, comando.UsuarioId);

        var foto = await _repositorio.ObterFotoPorIdAsync(comando.FotoId);
        if (foto is null)
        {
            _logger.LogWarning("Foto {FotoId} não encontrada", comando.FotoId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
        }

        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(foto.AnuncioId);
        if (anuncio is null || anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning("Usuário {UsuarioId} tentou remover foto {FotoId} sem permissão", comando.UsuarioId, comando.FotoId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioSemPermissao);
        }

        try
        {
            var uri = new Uri(foto.UrlImagem);
            var chave = uri.AbsolutePath.TrimStart('/');
            await _servicoArmazenamento.DeletarFisicoArquivoAsync(chave);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar arquivo do storage para foto {FotoId}", comando.FotoId);
        }

        await _repositorio.DeletarFisicoFotoAsync(comando.FotoId);

        _logger.LogInformation("Foto {FotoId} removida com sucesso", comando.FotoId);
        return ResultadoDto.ComSucesso(MensagensSistema.Geral.Sucesso);
    }
}
