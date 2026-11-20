namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public enum FormatoCodigoBarras
{
    Codabar,
    Code11,
    Code39,
    Code39Extended,
    Code93,
    Code128,
    EAN8,
    EAN13,
    GS1_128,
    Interleaved2of5,
    ISBN10_ISBN13,
    MSIPlessey,
    PostNet,
    Standard2of5,
    UPCA,
    UPCE,
    UPCExtension2Digits,
    UPCExtension5Digits
}

public interface IServicoGeradorCodigoBarras
{
    byte[] GerarCodigoBarras(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true);
    string GerarCodigoBarrasComoBase64(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true);
    MemoryStream GerarCodigoBarrasComoStream(string conteudo, FormatoCodigoBarras formato, int width = 300, int height = 120, bool incluirRotulo = true);
}
