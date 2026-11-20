using Microsoft.Extensions.Configuration;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public sealed class ConfiguracaoAutenticacaoGoogle
{
    [ConfigurationKeyName("ClientId")]
    public string IdCliente { get; set; } = string.Empty;

    [ConfigurationKeyName("ClientSecret")]
    public string SegredoCliente { get; set; } = string.Empty;

    public bool EstaConfigurado =>
        !string.IsNullOrWhiteSpace(IdCliente) &&
        !IdCliente.StartsWith("PREENCHER");
}
