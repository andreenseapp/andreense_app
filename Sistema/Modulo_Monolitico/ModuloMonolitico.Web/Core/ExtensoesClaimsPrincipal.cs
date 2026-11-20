using System.Security.Claims;
using ModuloMonolitico.Web.Options;

namespace ModuloMonolitico.Web.Core;

public static class ExtensoesClaimsPrincipal
{
    public static bool EhAdministrador(this ClaimsPrincipal? principal)
    {
        if (principal is null) return false;

        var grupo = principal.FindFirst(TiposClaimAutenticacao.GrupoCodigo)?.Value;

        return grupo == "1";
    }

    public static string ObterGrupoCodigo(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(TiposClaimAutenticacao.GrupoCodigo)?.Value ?? string.Empty;
    }

    public static string ObterIdSessao(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(TiposClaimAutenticacao.IdSessao)?.Value ?? string.Empty;
    }

    public static Guid ObterUsuarioId(this ClaimsPrincipal? principal)
    {
        if (principal is null) return Guid.Empty;
        
        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claimValue, out var id) ? id : Guid.Empty;
    }
}
