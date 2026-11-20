using System.Text.RegularExpressions;
using BarcodeStandard;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using SkiaSharp;

namespace ModuloMonolitico.Infraestrutura.Servicos.Sistema;

public class ServicoGeradorCodigoBarras : IServicoGeradorCodigoBarras
{
    public byte[] GerarCodigoBarras(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true)
    {
        if (string.IsNullOrWhiteSpace(conteudo))
        {
            throw new ArgumentException("O conteúdo do código de barras não pode ser nulo ou vazio.", nameof(conteudo));
        }
        ValidarConteudo(conteudo, formato);

        try
        {
            using var barcode = new Barcode
            {
                IncludeLabel = incluirRotulo,
                Alignment = AlignmentPositions.Center
            };

            var mappedType = MapearFormato(formato);
            using var imagem = barcode.Encode(mappedType, conteudo, SKColors.Black, SKColors.White, width, height);
            using var dadosImagem = imagem.Encode(SKEncodedImageFormat.Png, 100);
            return dadosImagem.ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro interno ao gerar o código de barras no formato {formato}: {ex.Message}", ex);
        }
    }

    public string GerarCodigoBarrasComoBase64(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true)
    {
        var bytes = GerarCodigoBarras(conteudo, formato, width, height, incluirRotulo);
        return Convert.ToBase64String(bytes);
    }

    public MemoryStream GerarCodigoBarrasComoStream(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true)
    {
        var bytes = GerarCodigoBarras(conteudo, formato, width, height, incluirRotulo);
        return new MemoryStream(bytes);
    }

    private static void ValidarConteudo(string conteudo, FormatoCodigoBarras formato)
    {
        switch (formato)
        {
            case FormatoCodigoBarras.EAN13:
                if (!Regex.IsMatch(conteudo, @"^\d{12,13}$"))
                    throw new ArgumentException("O formato EAN-13 exige exatamente 12 ou 13 caracteres numéricos.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.EAN8:
                if (!Regex.IsMatch(conteudo, @"^\d{7,8}$"))
                    throw new ArgumentException("O formato EAN-8 exige exatamente 7 ou 8 caracteres numéricos.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.UPCA:
                if (!Regex.IsMatch(conteudo, @"^\d{11,12}$"))
                    throw new ArgumentException("O formato UPC-A exige exatamente 11 ou 12 caracteres numéricos.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.UPCE:
                if (!Regex.IsMatch(conteudo, @"^\d{6,8}$"))
                    throw new ArgumentException("O formato UPC-E exige exatamente de 6 a 8 caracteres numéricos.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.PostNet:
                if (!Regex.IsMatch(conteudo, @"^\d+$"))
                    throw new ArgumentException("O formato PostNet exige apenas caracteres numéricos.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.ISBN10_ISBN13:
                if (!Regex.IsMatch(conteudo, @"^[0-9xX-]+$"))
                    throw new ArgumentException("O formato ISBN exige apenas números, hifens ou caractere 'X'/'x'.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.Codabar:
                if (!Regex.IsMatch(conteudo, @"^[0-9a-dA-D\-\$\/\.\+\:]+$"))
                    throw new ArgumentException("O formato Codabar aceita apenas números e os caracteres: A, B, C, D, -, $, /, ., +, :", nameof(conteudo));
                break;

            case FormatoCodigoBarras.Code39:
                if (!Regex.IsMatch(conteudo, @"^[0-9A-Z\-\.\ \$\/\+\%]+$"))
                    throw new ArgumentException("O formato Code39 padrão aceita apenas números, letras maiúsculas e os caracteres: espaço, -, ., $, /, +, %", nameof(conteudo));
                break;

            case FormatoCodigoBarras.Code11:
                if (!Regex.IsMatch(conteudo, @"^[0-9\-]+$"))
                    throw new ArgumentException("O formato Code11 aceita apenas números e hifens.", nameof(conteudo));
                break;

            case FormatoCodigoBarras.Interleaved2of5:
            case FormatoCodigoBarras.Standard2of5:
                if (!Regex.IsMatch(conteudo, @"^\d+$"))
                    throw new ArgumentException($"O formato {formato} exige apenas caracteres numéricos.", nameof(conteudo));
                break;
        }
    }

    private static BarcodeStandard.Type MapearFormato(FormatoCodigoBarras formato)
    {
        return formato switch
        {
            FormatoCodigoBarras.Codabar => BarcodeStandard.Type.Codabar,
            FormatoCodigoBarras.Code11 => BarcodeStandard.Type.Code11,
            FormatoCodigoBarras.Code39 => BarcodeStandard.Type.Code39,
            FormatoCodigoBarras.Code39Extended => BarcodeStandard.Type.Code39Extended,
            FormatoCodigoBarras.Code93 => BarcodeStandard.Type.Code93,
            FormatoCodigoBarras.Code128 => BarcodeStandard.Type.Code128,
            FormatoCodigoBarras.EAN8 => BarcodeStandard.Type.Ean8,
            FormatoCodigoBarras.EAN13 => BarcodeStandard.Type.Ean13,
            FormatoCodigoBarras.GS1_128 => BarcodeStandard.Type.Code128,
            FormatoCodigoBarras.Interleaved2of5 => BarcodeStandard.Type.Interleaved2Of5,
            FormatoCodigoBarras.ISBN10_ISBN13 => BarcodeStandard.Type.Isbn,
            FormatoCodigoBarras.MSIPlessey => BarcodeStandard.Type.MsiMod10,
            FormatoCodigoBarras.PostNet => BarcodeStandard.Type.PostNet,
            FormatoCodigoBarras.Standard2of5 => BarcodeStandard.Type.Standard2Of5,
            FormatoCodigoBarras.UPCA => BarcodeStandard.Type.UpcA,
            FormatoCodigoBarras.UPCE => BarcodeStandard.Type.UpcE,
            FormatoCodigoBarras.UPCExtension2Digits => BarcodeStandard.Type.UpcSupplemental2Digit,
            FormatoCodigoBarras.UPCExtension5Digits => BarcodeStandard.Type.UpcSupplemental5Digit,
            _ => throw new ArgumentOutOfRangeException(nameof(formato), $"Formato {formato} não mapeado ou suportado.")
        };
    }
}
