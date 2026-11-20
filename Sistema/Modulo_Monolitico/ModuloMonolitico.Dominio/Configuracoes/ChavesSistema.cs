using System;

namespace ModuloMonolitico.Dominio.Configuracoes;

public static class ChavesSistema
{
    public static string CookieFiltros(string sufixo, Guid usuarioId) => $"iltros_{sufixo}_{usuarioId}";
    
    public static string CacheIdentidade(Guid usuarioId) => $"identidade:{usuarioId}";
    
    public static string SessaoAdministrador(string sessionId) => $"Suporte_{sessionId}_Administrador";
    
    public static string HtmlHiddenId(string campo) => $"hidden_{campo.Replace(".", "_")}";
    
    public static string HtmlComponentId(string prefixo) => $"{prefixo}_{Guid.NewGuid():N}";

    public static string CacheAutenticacaoPrefixo() => "identidade:";

    public static string PayloadCashback(string dados) => $"andreense:cashback:{dados}";
    
    public static string PayloadFidelidade(string dados) => $"andreense:fidelidade:{dados}";
    
    public static string StoragePerfil(Guid usuarioId, string extensao) => $"perfil/{usuarioId}{extensao}";
    public static string StorageAchadosPerdidos(Guid id, string extensao) => $"achados-perdidos/{id}{extensao}";
    public static string StorageImobiliaria(Guid anuncioId, Guid fotoId, string extensao) => $"imobiliaria/{anuncioId}/{fotoId}{extensao}";
    public static string StorageVeiculos(Guid anuncioId, Guid fotoId, string extensao) => $"veiculos/{anuncioId}/{fotoId}{extensao}";
    public static string StorageLocalBuscasRecentesImoveis() => "andreense_imoveis_buscas_recentes";
    public static string StorageLocalAnunciosVisualizadosImoveis() => "andreense_imoveis_anuncios_visualizados";
    public static string StorageLocalBuscasRecentesVeiculos() => "andreense_veiculos_buscas_recentes";
    
    public static string ArquivoRelatorioContatos(DateTime data) => $"Relatorio_Contatos_{data:yyyyMMdd_HHmmss}.csv";
    
    public static string CookieDiagnosticoDesempenho() => "andreense_diag_desempenho";
    public static string FormAgendamentoAtivo(int dia) => $"ativo_{dia}";
    public static string FormAgendamentoInicio(int dia) => $"inicio_{dia}";
    public static string FormAgendamentoFim(int dia) => $"fim_{dia}";
    public static string FormAgendamentoIntervalo(int dia) => $"intervalo_{dia}";
    public static string PrestadorIdComposto(string tipo, object id) => $"{tipo}|{id}";

    public static void ExtrairPrestadorIdComposto(string? prestadorIdComposto, out Guid? usuarioId, out Guid? contatoId)
    {
        usuarioId = null;
        contatoId = null;
        if (string.IsNullOrEmpty(prestadorIdComposto))
            return;

        var partes = prestadorIdComposto.Split('|');
        if (partes.Length != 2 || !Guid.TryParse(partes[1], out var id))
            return;

        if (partes[0] == "U")
            usuarioId = id;
        else if (partes[0] == "C")
            contatoId = id;
    }

    public static void ExtrairTipoEIdPrestador(string? prestadorIdComposto, out string? tipo, out Guid? id)
    {
        tipo = null;
        id = null;
        if (string.IsNullOrEmpty(prestadorIdComposto))
            return;

        var partes = prestadorIdComposto.Split('|');
        if (partes.Length != 2 || !Guid.TryParse(partes[1], out var parsedId))
            return;

        tipo = partes[0];
        id = parsedId;
    }

    public static bool ExtrairTipoETokenQrCode(string? payload, out string tipo, out string token)
    {
        tipo = string.Empty;
        token = string.Empty;
        if (string.IsNullOrEmpty(payload))
            return false;

        var partes = payload.Split(':');
        if (partes.Length < 3 || partes[0] != "andreense")
            return false;

        tipo = partes[1];
        token = partes[2];
        return true;
    }

    public static string LimparNumeroWhatsapp(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return string.Empty;

        return new string(telefone.Where(char.IsDigit).ToArray());
    }

    public static string NormalizarUrlSite(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var urlLimpa = url.Trim();
        return urlLimpa.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || urlLimpa.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? urlLimpa
            : $"https://{urlLimpa}";
    }
}