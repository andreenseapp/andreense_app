using Microsoft.Extensions.Options;
using ModuloMonolitico.Web.Options;

namespace ModuloMonolitico.Web.Servicos;

public class ServicoCookieAutenticacao : IServicoCookieAutenticacao
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OpcoesAutenticacao _auth;

    public ServicoCookieAutenticacao(
        IHttpContextAccessor httpContextAccessor,
        IOptions<OpcoesAutenticacao> authOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _auth = authOptions.Value;
    }

    public void DefinirTokensAutenticacao(string tokenAcesso, string tokenRenovacao, bool manterConectado = false)
    {
        var response = ObterResponse();
        response.Cookies.Append(EsquemaCookieAutenticacao.TokenAcesso, tokenAcesso, CriarOpcoesTokenAcesso(manterConectado));
        response.Cookies.Append(EsquemaCookieAutenticacao.TokenRenovacao, tokenRenovacao, CriarOpcoesTokenRenovacao(manterConectado));
    }

    public void DefinirTokenAcesso(string tokenAcesso, bool manterConectado = false) =>
        ObterResponse().Cookies.Append(
            EsquemaCookieAutenticacao.TokenAcesso,
            tokenAcesso,
            CriarOpcoesTokenAcesso(manterConectado));

    public void DefinirEmpresaAtiva(Guid empresaId, string? nome = null)
    {
        var response = ObterResponse();
        var secure = EhHttps();

        response.Cookies.Append(EsquemaCookieAutenticacao.EmpresaAtivaId, empresaId.ToString(), new CookieOptions
        {
            HttpOnly = false,
            Secure = secure,
            SameSite = SameSiteMode.Strict,
            Path = EsquemaCookieAutenticacao.PathRaiz,
            Expires = DateTimeOffset.UtcNow.Add(EsquemaCookieAutenticacao.EmpresaAtivaDuracao)
        });

        if (!string.IsNullOrEmpty(nome))
        {
            response.Cookies.Append(EsquemaCookieAutenticacao.EmpresaAtivaNome, Uri.EscapeDataString(nome), new CookieOptions
            {
                HttpOnly = false,
                Secure = secure,
                SameSite = SameSiteMode.Strict,
                Path = EsquemaCookieAutenticacao.PathRaiz,
                Expires = DateTimeOffset.UtcNow.Add(EsquemaCookieAutenticacao.EmpresaAtivaDuracao)
            });
        }
    }

    public void DefinirThrottleEmailValidacao() =>
        ObterResponse().Cookies.Append(EsquemaCookieAutenticacao.EmailCodigoEnviado, "true", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.Add(EsquemaCookieAutenticacao.EmailCodigoThrottleDuracao),
            HttpOnly = true,
            Secure = EhHttps(),
            SameSite = SameSiteMode.Strict,
            Path = EsquemaCookieAutenticacao.PathRaiz
        });

    public void LimparSessao()
    {
        var response = ObterResponse();

        var opcoesExclusaoSessao = new[]
        {
            new CookieOptions { Path = EsquemaCookieAutenticacao.PathRaiz },
            new CookieOptions { Path = EsquemaCookieAutenticacao.PathRaiz, Secure = true },
            new CookieOptions { Path = EsquemaCookieAutenticacao.PathRaiz, Domain = "localhost" },
            new CookieOptions { Path = EsquemaCookieAutenticacao.PathRaiz, Domain = "localhost", Secure = true },
            new CookieOptions { Path = EsquemaCookieAutenticacao.PathRaiz, Domain = ".localhost" },
            new CookieOptions { Path = "/Contextos/Autenticacao" },
            new CookieOptions { Path = "/Base/Empresas" }
        };

        foreach (var opt in opcoesExclusaoSessao)
        {
            response.Cookies.Delete(EsquemaCookieAutenticacao.TokenAcesso, opt);
            response.Cookies.Delete(EsquemaCookieAutenticacao.EmpresaAtivaId, opt);
            response.Cookies.Delete(EsquemaCookieAutenticacao.EmpresaAtivaNome, opt);
        }

        foreach (var opt in new[]
        {
            new CookieOptions { Path = EsquemaCookieAutenticacao.CaminhoTokenRenovacao },
            new CookieOptions { Path = EsquemaCookieAutenticacao.CaminhoTokenRenovacao, Secure = true }
        })
        {
            response.Cookies.Delete(EsquemaCookieAutenticacao.TokenRenovacao, opt);
        }
    }

    private CookieOptions CriarOpcoesTokenAcesso(bool manterConectado) => new()
    {
        HttpOnly = true,
        Secure = EhHttps(),
        SameSite = SameSiteMode.Strict,
        Path = EsquemaCookieAutenticacao.PathRaiz,
        Expires = manterConectado
            ? DateTimeOffset.UtcNow.Add(_auth.SessaoPersistenteDuracao)
            : DateTimeOffset.UtcNow.Add(_auth.DuracaoTokenAcesso)
    };

    private CookieOptions CriarOpcoesTokenRenovacao(bool manterConectado) => new()
    {
        HttpOnly = true,
        Secure = EhHttps(),
        SameSite = SameSiteMode.Strict,
        Path = EsquemaCookieAutenticacao.CaminhoTokenRenovacao,
        Expires = manterConectado
            ? DateTimeOffset.UtcNow.Add(_auth.DuracaoTokenRenovacao)
            : null
    };

    private HttpResponse ObterResponse() =>
        _httpContextAccessor.HttpContext?.Response
        ?? throw new InvalidOperationException("HttpContext indisponível para manipular cookies.");

    private bool EhHttps() =>
        _httpContextAccessor.HttpContext?.Request.IsHttps ?? false;
}
