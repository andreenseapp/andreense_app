using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModuloMonolitico.Dominio.Imobiliaria.Servicos;

public static class BuscaRecenteFiltroHelper
{
    public const int Limite = 8;

    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static FiltroAnuncio Normalizar(FiltroAnuncio filtro)
    {
        var amenidades = (filtro.CodigosAmenidade ?? [])
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new FiltroAnuncio
        {
            TipoNegocio = NormalizarTexto(filtro.TipoNegocio),
            TipoImovel = NormalizarTexto(filtro.TipoImovel),
            Bairro = NormalizarTexto(filtro.Bairro),
            QuartosMinimo = filtro.QuartosMinimo,
            BanheirosMinimo = filtro.BanheirosMinimo,
            VagasMinimo = filtro.VagasMinimo,
            ValorMinimo = filtro.ValorMinimo,
            ValorMaximo = filtro.ValorMaximo,
            ValorCondominioMinimo = filtro.ValorCondominioMinimo,
            ValorCondominioMaximo = filtro.ValorCondominioMaximo,
            AreaMinima = filtro.AreaMinima,
            AreaMaxima = filtro.AreaMaxima,
            SituacaoObra = NormalizarTexto(filtro.SituacaoObra),
            CodigoPublico = NormalizarTexto(filtro.CodigoPublico),
            CodigosAmenidade = amenidades,
            BuscarPorPacote = filtro.BuscarPorPacote,
            Pagina = 1,
            TamanhoPagina = 12
        };
    }

    public static bool TemCriterioSalvavel(FiltroAnuncio filtro)
    {
        var n = Normalizar(filtro);
        return !string.IsNullOrWhiteSpace(n.TipoImovel)
            || !string.IsNullOrWhiteSpace(n.Bairro)
            || n.QuartosMinimo.HasValue
            || n.BanheirosMinimo.HasValue
            || n.VagasMinimo.HasValue
            || n.ValorMinimo.HasValue
            || n.ValorMaximo.HasValue
            || n.ValorCondominioMinimo.HasValue
            || n.ValorCondominioMaximo.HasValue
            || n.AreaMinima.HasValue
            || n.AreaMaxima.HasValue
            || !string.IsNullOrWhiteSpace(n.SituacaoObra)
            || !string.IsNullOrWhiteSpace(n.CodigoPublico)
            || n.CodigosAmenidade.Count > 0
            || n.BuscarPorPacote
            || string.Equals(n.TipoNegocio, "aluguel", StringComparison.OrdinalIgnoreCase);
    }

    public static string SerializarJson(FiltroAnuncio filtro)
    {
        var n = Normalizar(filtro);
        var snapshot = new BuscaRecenteSnapshot
        {
            TipoNegocio = n.TipoNegocio,
            TipoImovel = n.TipoImovel,
            Bairro = n.Bairro,
            QuartosMinimo = n.QuartosMinimo,
            BanheirosMinimo = n.BanheirosMinimo,
            VagasMinimo = n.VagasMinimo,
            ValorMinimo = n.ValorMinimo,
            ValorMaximo = n.ValorMaximo,
            ValorCondominioMinimo = n.ValorCondominioMinimo,
            ValorCondominioMaximo = n.ValorCondominioMaximo,
            AreaMinima = n.AreaMinima,
            AreaMaxima = n.AreaMaxima,
            SituacaoObra = n.SituacaoObra,
            CodigoPublico = n.CodigoPublico,
            CodigosAmenidade = n.CodigosAmenidade,
            BuscarPorPacote = n.BuscarPorPacote ? true : null
        };
        return JsonSerializer.Serialize(snapshot, OpcoesJson);
    }

    public static FiltroAnuncio? DesserializarJson(string filtroJson)
    {
        if (string.IsNullOrWhiteSpace(filtroJson))
            return null;

        try
        {
            var snapshot = JsonSerializer.Deserialize<BuscaRecenteSnapshot>(filtroJson, OpcoesJson);
            if (snapshot is null)
                return null;

            return Normalizar(new FiltroAnuncio
            {
                TipoNegocio = snapshot.TipoNegocio,
                TipoImovel = snapshot.TipoImovel,
                Bairro = snapshot.Bairro,
                QuartosMinimo = snapshot.QuartosMinimo,
                BanheirosMinimo = snapshot.BanheirosMinimo,
                VagasMinimo = snapshot.VagasMinimo,
                ValorMinimo = snapshot.ValorMinimo,
                ValorMaximo = snapshot.ValorMaximo,
                ValorCondominioMinimo = snapshot.ValorCondominioMinimo,
                ValorCondominioMaximo = snapshot.ValorCondominioMaximo,
                AreaMinima = snapshot.AreaMinima,
                AreaMaxima = snapshot.AreaMaxima,
                SituacaoObra = snapshot.SituacaoObra,
                CodigoPublico = snapshot.CodigoPublico,
                CodigosAmenidade = snapshot.CodigosAmenidade ?? [],
                BuscarPorPacote = snapshot.BuscarPorPacote == true
            });
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static string GerarChave(FiltroAnuncio filtro)
    {
        var json = SerializarJson(filtro);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string GerarRotulo(FiltroAnuncio filtro)
    {
        var n = Normalizar(filtro);
        var partes = new List<string>();

        if (string.Equals(n.TipoNegocio, "aluguel", StringComparison.OrdinalIgnoreCase))
            partes.Add("Alugar");
        else if (string.Equals(n.TipoNegocio, "venda", StringComparison.OrdinalIgnoreCase))
            partes.Add("Comprar");

        if (!string.IsNullOrWhiteSpace(n.TipoImovel))
            partes.Add(RotuloTipoImovel(n.TipoImovel));

        if (!string.IsNullOrWhiteSpace(n.Bairro))
            partes.Add(n.Bairro);

        if (n.QuartosMinimo.HasValue)
            partes.Add($"{n.QuartosMinimo}+ quartos");

        if (n.BanheirosMinimo.HasValue)
            partes.Add($"{n.BanheirosMinimo}+ banheiros");

        if (n.VagasMinimo.HasValue)
            partes.Add($"{n.VagasMinimo}+ vagas");

        if (n.ValorMinimo.HasValue || n.ValorMaximo.HasValue)
            partes.Add(RotuloFaixa("R$", n.ValorMinimo, n.ValorMaximo));

        if (n.AreaMinima.HasValue || n.AreaMaxima.HasValue)
            partes.Add(RotuloFaixa("m²", n.AreaMinima, n.AreaMaxima));

        if (!string.IsNullOrWhiteSpace(n.SituacaoObra))
            partes.Add(RotuloSituacaoObra(n.SituacaoObra));

        if (!string.IsNullOrWhiteSpace(n.CodigoPublico))
            partes.Add(n.CodigoPublico);

        if (n.CodigosAmenidade.Count > 0)
            partes.Add($"{n.CodigosAmenidade.Count} amenidade(s)");

        if (n.BuscarPorPacote)
            partes.Add("Pacote");

        if (partes.Count == 0)
            return MensagensSistema.Imobiliaria.TituloBuscasRecentes;

        var rotulo = string.Join(" · ", partes);
        return rotulo.Length <= 200 ? rotulo : rotulo[..197] + "...";
    }

    public static string MontarQueryString(FiltroAnuncio filtro)
    {
        var n = Normalizar(filtro);
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(n.TipoNegocio))
            parts.Add($"Filtro.TipoNegocio={Uri.EscapeDataString(n.TipoNegocio)}");
        if (!string.IsNullOrWhiteSpace(n.TipoImovel))
            parts.Add($"Filtro.TipoImovel={Uri.EscapeDataString(n.TipoImovel)}");
        if (!string.IsNullOrWhiteSpace(n.Bairro))
            parts.Add($"Filtro.Bairro={Uri.EscapeDataString(n.Bairro)}");
        if (n.QuartosMinimo.HasValue)
            parts.Add($"Filtro.QuartosMinimo={n.QuartosMinimo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.BanheirosMinimo.HasValue)
            parts.Add($"Filtro.BanheirosMinimo={n.BanheirosMinimo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.VagasMinimo.HasValue)
            parts.Add($"Filtro.VagasMinimo={n.VagasMinimo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.ValorMinimo.HasValue)
            parts.Add($"Filtro.ValorMinimo={n.ValorMinimo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.ValorMaximo.HasValue)
            parts.Add($"Filtro.ValorMaximo={n.ValorMaximo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.ValorCondominioMinimo.HasValue)
            parts.Add($"Filtro.ValorCondominioMinimo={n.ValorCondominioMinimo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.ValorCondominioMaximo.HasValue)
            parts.Add($"Filtro.ValorCondominioMaximo={n.ValorCondominioMaximo.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.AreaMinima.HasValue)
            parts.Add($"Filtro.AreaMinima={n.AreaMinima.Value.ToString(CultureInfo.InvariantCulture)}");
        if (n.AreaMaxima.HasValue)
            parts.Add($"Filtro.AreaMaxima={n.AreaMaxima.Value.ToString(CultureInfo.InvariantCulture)}");
        if (!string.IsNullOrWhiteSpace(n.SituacaoObra))
            parts.Add($"Filtro.SituacaoObra={Uri.EscapeDataString(n.SituacaoObra)}");
        if (!string.IsNullOrWhiteSpace(n.CodigoPublico))
            parts.Add($"Filtro.CodigoPublico={Uri.EscapeDataString(n.CodigoPublico)}");
        if (n.BuscarPorPacote)
            parts.Add("Filtro.BuscarPorPacote=true");
        foreach (var codigo in n.CodigosAmenidade)
            parts.Add($"Filtro.CodigosAmenidade={Uri.EscapeDataString(codigo)}");

        return string.Join("&", parts);
    }

    public static BuscaRecenteDto MontarDto(Guid id, string chaveFiltro, string filtroJson, string rotulo, DateTime dataUltimaBusca)
    {
        var filtro = DesserializarJson(filtroJson) ?? new FiltroAnuncio();
        return new BuscaRecenteDto
        {
            Id = id,
            ChaveFiltro = chaveFiltro,
            FiltroJson = filtroJson,
            Rotulo = string.IsNullOrWhiteSpace(rotulo) ? GerarRotulo(filtro) : rotulo,
            QueryString = MontarQueryString(filtro),
            DataUltimaBusca = dataUltimaBusca
        };
    }

    private static string? NormalizarTexto(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static string RotuloTipoImovel(string tipo) => tipo.ToLowerInvariant() switch
    {
        "apartamento" => "Apartamento",
        "casa" => "Casa",
        "kitnet" => "Kitnet/Studio",
        "comercial" => "Comercial",
        _ => tipo
    };

    private static string RotuloSituacaoObra(string situacao) => situacao.ToLowerInvariant() switch
    {
        "pronto" => "Pronto",
        "em_construcao" => "Em construção",
        "na_planta" => "Na planta",
        _ => situacao
    };

    private static string RotuloFaixa(string unidade, decimal? minimo, decimal? maximo)
    {
        if (minimo.HasValue && maximo.HasValue)
            return $"{FormatarNumero(minimo.Value)}–{FormatarNumero(maximo.Value)} {unidade}";
        if (minimo.HasValue)
            return $"de {FormatarNumero(minimo.Value)} {unidade}";
        return $"até {FormatarNumero(maximo!.Value)} {unidade}";
    }

    private static string FormatarNumero(decimal valor) =>
        valor % 1 == 0
            ? ((long)valor).ToString("N0", CultureInfo.GetCultureInfo("pt-BR"))
            : valor.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));

    private sealed class BuscaRecenteSnapshot
    {
        public string? TipoNegocio { get; set; }
        public string? TipoImovel { get; set; }
        public string? Bairro { get; set; }
        public short? QuartosMinimo { get; set; }
        public short? BanheirosMinimo { get; set; }
        public short? VagasMinimo { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public decimal? ValorCondominioMinimo { get; set; }
        public decimal? ValorCondominioMaximo { get; set; }
        public decimal? AreaMinima { get; set; }
        public decimal? AreaMaxima { get; set; }
        public string? SituacaoObra { get; set; }
        public string? CodigoPublico { get; set; }
        public List<string>? CodigosAmenidade { get; set; }
        public bool? BuscarPorPacote { get; set; }
    }
}
