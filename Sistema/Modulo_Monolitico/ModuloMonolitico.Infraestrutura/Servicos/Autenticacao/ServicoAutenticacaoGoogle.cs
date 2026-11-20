using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModuloMonolitico.Dominio.Servicos;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public sealed class ServicoAutenticacaoGoogle : IServicoAutenticacaoGoogle
{
    private const string LogTag = "LOG :: [ServicoAutenticacaoGoogle]";

    private readonly ConfiguracaoAutenticacaoGoogle _config;
    private readonly ILogger<ServicoAutenticacaoGoogle> _logger;

    public ServicoAutenticacaoGoogle(
        IOptions<ConfiguracaoAutenticacaoGoogle> config,
        ILogger<ServicoAutenticacaoGoogle> logger)
    {
        _config = config.Value;
        _logger = logger;

        if (_config.EstaConfigurado)
            _logger.LogInformation("{LogTag} Serviço habilitado. IdCliente configurado.", LogTag);
        else
            _logger.LogWarning("{LogTag} Google Auth não configurado. Preencha Google:ClientId e Google:ClientSecret em appsettings.Development.json.", LogTag);
    }

    public bool Habilitado => _config.EstaConfigurado;

    public async Task<DadosGoogleUsuario?> ValidarTokenIdentidadeAsync(string tokenIdentidade)
    {
        if (!_config.EstaConfigurado)
        {
            _logger.LogWarning("{LogTag} Tentativa de validar token com serviço não configurado.", LogTag);
            return null;
        }

        if (string.IsNullOrWhiteSpace(tokenIdentidade))
        {
            _logger.LogWarning("{LogTag} tokenIdentidade recebido nulo ou vazio.", LogTag);
            return null;
        }

        try
        {
            _logger.LogInformation("{LogTag} Validando id_token do Google...", LogTag);

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config.IdCliente }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenIdentidade, settings);

            _logger.LogInformation("{LogTag} Token validado com sucesso. Email: {Email} | Sub: {Sub}",
                LogTag, payload.Email, payload.Subject);

            return new DadosGoogleUsuario(
                Email: payload.Email,
                Nome: payload.Name ?? payload.Email,
                UrlFoto: payload.Picture,
                IdGoogle: payload.Subject
            );
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("{LogTag} id_token inválido: {Mensagem}", LogTag, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{LogTag} Erro inesperado ao validar id_token.", LogTag);
            return null;
        }
    }
}
