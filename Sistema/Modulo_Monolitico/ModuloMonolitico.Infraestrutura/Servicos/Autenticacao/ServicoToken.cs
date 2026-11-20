using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public class ServicoToken : IServicoToken
{
    private readonly string _chaveSecreta;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiracaoMinutos;

    public ServicoToken(IConfiguration configuration)
    {
        _chaveSecreta = configuration["Jwt:Chave"] ?? throw new ArgumentException("Chave JWT ausente");
        _issuer = configuration["Jwt:Issuer"] ?? "andreense-erp";
        _audience = configuration["Jwt:Audience"] ?? "andreense-usuarios";
        _expiracaoMinutos = int.TryParse(configuration["Jwt:ExpiracaoTokenAcessoMinutos"], out var min) ? min : 15;
    }

    public string GerarTokenAcesso(Usuario usuario, Guid contaId, IList<Guid> idsEmpresas, string hashImpressaoDigital, ContaDireitosPlanoDto? direitosPlano = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("SessionId", Guid.NewGuid().ToString()),
            new("conta_id", contaId.ToString()),
            new("email_validado", usuario.EmailValidado.ToString().ToLower()),
            new("fingerprint", hashImpressaoDigital)
        };
        claims.Add(new Claim("empresas", JsonSerializer.Serialize(idsEmpresas)));
        if (direitosPlano != null)
        {
            claims.Add(new Claim("saas_status", direitosPlano.SituacaoAssinatura));
            claims.Add(new Claim("saas_features", JsonSerializer.Serialize(direitosPlano.Recursos)));
            if (direitosPlano.Vencimento.HasValue)
                claims.Add(new Claim("saas_vencimento", direitosPlano.Vencimento.Value.ToString("O")));
        }
        else
        {
            claims.Add(new Claim("saas_status", SituacaoAssinatura.Trial));
            claims.Add(new Claim("saas_features", "{}"));
        }

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_chaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiracaoMinutos),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GerarTokenRenovacao()
    {
        var bytesAleatorios = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytesAleatorios);
        return Convert.ToBase64String(bytesAleatorios);
    }

    public string GerarHashTokenRenovacao(string tokenRenovacao)
    {
        using var sha256 = SHA256.Create();
        var bytesHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenRenovacao));
        return Convert.ToBase64String(bytesHash);
    }

    public ClaimsPrincipal? ValidarTokenAcesso(string tokenAcesso)
    {
        var manipuladorToken = new JwtSecurityTokenHandler();
        var chave = Encoding.UTF8.GetBytes(_chaveSecreta);

        var parametrosValidacao = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chave),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var entidade = manipuladorToken.ValidateToken(tokenAcesso, parametrosValidacao, out _);
            return entidade;
        }
        catch
        {
            return null;
        }
    }
}
