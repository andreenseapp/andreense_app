using SkiaSharp;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.Servicos.Imagem;

public class ServicoProcessamentoImagem : IServicoProcessamentoImagem
{
    private const int QualidadeWebP = 82;

    public Task<(byte[] Bytes, string TipoConteudo, string Extensao)> OtimizarParaWebAsync(byte[] bytesOriginais, int larguraMaxima = 1920)
    {
        using var streamOriginal = new MemoryStream(bytesOriginais);
        using var bitmap = SKBitmap.Decode(streamOriginal);

        if (bitmap is null)
            throw new InvalidOperationException("Não foi possível decodificar a imagem enviada.");

        using var bitmapOtimizado = RedimensionarSeNecessario(bitmap, larguraMaxima);

        using var streamSaida = new MemoryStream();
        bitmapOtimizado.Encode(streamSaida, SKEncodedImageFormat.Webp, QualidadeWebP);

        return Task.FromResult((streamSaida.ToArray(), "image/webp", ".webp"));
    }

    private static SKBitmap RedimensionarSeNecessario(SKBitmap original, int larguraMaxima)
    {
        if (original.Width <= larguraMaxima)
            return original.Copy();

        var proporcao = (float)larguraMaxima / original.Width;
        var novaAltura = (int)(original.Height * proporcao);

        var info = new SKImageInfo(larguraMaxima, novaAltura, SKColorType.Rgba8888, SKAlphaType.Premul);
        var redimensionado = new SKBitmap(info);

        original.ScalePixels(redimensionado, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

        return redimensionado;
    }
}
