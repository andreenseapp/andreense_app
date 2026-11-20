using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record CadastrarFotoAnuncioVeiculoCommand(
    Guid AnuncioVeiculoId,
    Guid UsuarioId,
    byte[] Bytes,
    string NomeArquivo,
    string ContentType) : IRequest<ResultadoDto<string>>;

public class CadastrarFotoAnuncioVeiculoCommandHandler : IRequestHandler<CadastrarFotoAnuncioVeiculoCommand, ResultadoDto<string>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly IServicoProcessamentoImagem _processamento;
    private readonly IServicoArmazenamento _armazenamento;
    private readonly ILogger<CadastrarFotoAnuncioVeiculoCommandHandler> _logger;

    public CadastrarFotoAnuncioVeiculoCommandHandler(
        IAnuncioVeiculoRepositorio repositorio,
        IServicoProcessamentoImagem processamento,
        IServicoArmazenamento armazenamento,
        ILogger<CadastrarFotoAnuncioVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _processamento = processamento;
        _armazenamento = armazenamento;
        _logger = logger;
    }

    public async Task<ResultadoDto<string>> Handle(CadastrarFotoAnuncioVeiculoCommand comando, CancellationToken cancellationToken)
    {
        var totalFotos = await _repositorio.ContarFotosAsync(comando.AnuncioVeiculoId);
        if (totalFotos >= 20)
            return ResultadoDto<string>.Falha(MensagensSistema.Veiculos.LimiteFotosAtingido);

        var fotoId = Guid.NewGuid();
        byte[] bytesProcessados;
        string tipoConteudoFinal;
        string extensaoFinal;

        try
        {
            (bytesProcessados, tipoConteudoFinal, extensaoFinal) = await _processamento.OtimizarParaWebAsync(comando.Bytes);
        }
        catch (Exception)
        {
            return ResultadoDto<string>.Falha("A foto enviada é inválida ou não pôde ser processada.");
        }

        var chave = ChavesSistema.StorageVeiculos(comando.AnuncioVeiculoId, fotoId, extensaoFinal);

        string url;
        using (var stream = new MemoryStream(bytesProcessados))
        {
            url = await _armazenamento.EnviarArquivoAsync(stream, chave, tipoConteudoFinal);
        }

        var foto = new AnuncioVeiculoFoto
        {
            Id = fotoId,
            AnuncioVeiculoId = comando.AnuncioVeiculoId,
            UrlImagem = url,
            Ordem = (short)(totalFotos + 1)
        };

        await _repositorio.CadastrarFotoAsync(foto);

        _logger.LogInformation("Foto {FotoId} adicionada ao anúncio {AnuncioId}", fotoId, comando.AnuncioVeiculoId);

        return ResultadoDto<string>.ComSucesso(url, MensagensSistema.Geral.Sucesso);
    }
}
