using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ModuloMonolitico.Web.Servicos;

public class ConfiguracaoModel
{
    public int LoginMaximoTentativas { get; set; } = 10;
    public int PaginasListagemItensPorPagina { get; set; } = 50;
}

public class ServicoConfiguracaoApp(IWebHostEnvironment env)
{
    private readonly IWebHostEnvironment _env = env;
    private static readonly System.Threading.Lock _fileLock = new();

    private string ObterCaminhoArquivo()
    {
        return Path.Combine(_env.ContentRootPath, "appsettings.json");
    }

    public ConfiguracaoModel ObterConfiguracao()
    {
        string filePath = ObterCaminhoArquivo();
        if (!File.Exists(filePath)) return new ConfiguracaoModel();

        string jsonText;
        lock (_fileLock) { jsonText = File.ReadAllText(filePath); }

        if (JsonNode.Parse(jsonText) is not JsonObject root) return new ConfiguracaoModel();

        var model = new ConfiguracaoModel();

        if (root["ConfiguracoesLogin"] is JsonObject loginSection)
        {
            model.LoginMaximoTentativas = loginSection["MaximoTentativasLogin"]?.GetValue<int>() ?? 10;
        }

        if (root["ConfiguracoesPaginasListagem"] is JsonObject paginasSection)
        {
            model.PaginasListagemItensPorPagina = paginasSection["ItensPorPagina"]?.GetValue<int>() ?? 50;
        }

        return model;
    }

    public void AtualizarConfiguracao(ConfiguracaoModel model)
    {
        string filePath = ObterCaminhoArquivo();
        string jsonText;

        lock (_fileLock) { jsonText = File.Exists(filePath) ? File.ReadAllText(filePath) : "{}"; }

        var root = JsonNode.Parse(jsonText) as JsonObject ?? new JsonObject();

        var loginSection = root["ConfiguracoesLogin"] as JsonObject ?? new JsonObject();
        loginSection["MaximoTentativasLogin"] = model.LoginMaximoTentativas;
        root["ConfiguracoesLogin"] = loginSection;

        var paginasSection = root["ConfiguracoesPaginasListagem"] as JsonObject ?? new JsonObject();
        paginasSection["ItensPorPagina"] = model.PaginasListagemItensPorPagina;
        root["ConfiguracoesPaginasListagem"] = paginasSection;

        var writeOptions = new JsonSerializerOptions { WriteIndented = true };
        string outputText = root.ToJsonString(writeOptions);

        lock (_fileLock) { File.WriteAllText(filePath, outputText); }
    }
}
