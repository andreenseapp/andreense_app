using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record CadastrarFotoAnuncioCommand(
    Guid AnuncioId,
    Guid UsuarioId,
    byte[] BytesFoto,
    string NomeArquivo,
    string TipoConteudo
) : IRequest<ResultadoDto>;

public class CadastrarFotoAnuncioCommandHandler : IRequestHandler<CadastrarFotoAnuncioCommand, ResultadoDto>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly IServicoArmazenamento _servicoArmazenamento;
    private readonly IServicoProcessamentoImagem _servicoProcessamentoImagem;
    private readonly ILogger<CadastrarFotoAnuncioCommandHandler> _logger;

    public CadastrarFotoAnuncioCommandHandler(
        IAnuncioRepositorio repositorio,
        IServicoArmazenamento servicoArmazenamento,
        IServicoProcessamentoImagem servicoProcessamentoImagem,
        ILogger<CadastrarFotoAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _servicoArmazenamento = servicoArmazenamento;
        _servicoProcessamentoImagem = servicoProcessamentoImagem;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(CadastrarFotoAnuncioCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adicionando foto ao anúncio {AnuncioId} pelo usuário {UsuarioId}", comando.AnuncioId, comando.UsuarioId);

        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioId);
        if (anuncio is null)
        {
            _logger.LogWarning("Anúncio {AnuncioId} não encontrado para adicionar foto", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
        }

        if (anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning("Usuário {UsuarioId} tentou adicionar foto ao anúncio {AnuncioId} sem permissão", comando.UsuarioId, comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioSemPermissao);
        }

        var totalFotos = await _repositorio.ContarFotosAsync(comando.AnuncioId);
        if (totalFotos >= 15)
        {
            _logger.LogWarning("Limite de fotos atingido para o anúncio {AnuncioId}", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.LimiteAFotos);
        }

        byte[] bytesOtimizados;
        string tipoConteudoFinal;
        string extensaoFinal;

        try
        {
            (bytesOtimizados, tipoConteudoFinal, extensaoFinal) = await _servicoProcessamentoImagem.OtimizarParaWebAsync(comando.BytesFoto);
            _logger.LogInformation("Foto otimizada: {TamanhoOriginal}KB → {TamanhoOtimizado}KB para anúncio {AnuncioId}",
                comando.BytesFoto.Length / 1024, bytesOtimizados.Length / 1024, comando.AnuncioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao otimizar foto para o anúncio {AnuncioId}", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.FotoInvalida);
        }

        var chave = ChavesSistema.StorageImobiliaria(comando.AnuncioId, Guid.NewGuid(), extensaoFinal);

        string url;
        try
        {
            using var stream = new MemoryStream(bytesOtimizados);
            url = await _servicoArmazenamento.EnviarArquivoAsync(stream, chave, tipoConteudoFinal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload de foto para o anúncio {AnuncioId}", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.ErroUploadFoto);
        }

        var foto = new AnuncioFoto
        {
            Id = Guid.NewGuid(),
            AnuncioId = comando.AnuncioId,
            UrlImagem = url,
            Ordem = (short)totalFotos
        };

        await _repositorio.CadastrarFotoAsync(foto);

        _logger.LogInformation("Foto adicionada com sucesso ao anúncio {AnuncioId}", comando.AnuncioId);

        return ResultadoDto.ComSucesso(MensagensSistema.Geral.Sucesso);
    }
}
