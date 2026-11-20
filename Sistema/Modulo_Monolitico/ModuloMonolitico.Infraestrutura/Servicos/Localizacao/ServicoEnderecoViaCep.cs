using System.Text.Json;
using System.Text.Json.Serialization;
using ModuloMonolitico.Dominio.Servicos;

namespace ModuloMonolitico.Infraestrutura.Servicos.Localizacao;
public class ServicoEnderecoViaCep : IServicoEndereco
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://viacep.com.br/ws/{0}/json/";

    public ServicoEnderecoViaCep(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoEndereco> BuscarPorCepAsync(string cep)
    {
        var cepDigitos = new string(cep.Where(char.IsDigit).ToArray());

        if (cepDigitos.Length != 8)
            return ResultadoEndereco.NaoEncontrado;

        try
        {
            var url = string.Format(BaseUrl, cepDigitos);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ResultadoEndereco.NaoEncontrado;

            var json = await response.Content.ReadAsStringAsync();
            var viaCepDto = JsonSerializer.Deserialize<ViaCepDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (viaCepDto is null || viaCepDto.Erro)
                return ResultadoEndereco.NaoEncontrado;

            return new ResultadoEndereco(
                Cep:          viaCepDto.Cep ?? string.Empty,
                Logradouro:   viaCepDto.Logradouro ?? string.Empty,
                Complemento:  viaCepDto.Complemento ?? string.Empty,
                Bairro:       viaCepDto.Bairro ?? string.Empty,
                Cidade:       viaCepDto.Localidade ?? string.Empty,
                Uf:           viaCepDto.Uf ?? string.Empty,
                Encontrado:   true
            );
        }
        catch
        {
            return ResultadoEndereco.NaoEncontrado;
        }
    }
    private sealed class ViaCepDto
    {
        public string? Cep        { get; init; }
        public string? Logradouro { get; init; }
        public string? Complemento{ get; init; }
        public string? Bairro     { get; init; }
        public string? Localidade { get; init; }
        public string? Uf         { get; init; }

        [JsonPropertyName("erro")]
        public bool Erro { get; init; }
    }
}


