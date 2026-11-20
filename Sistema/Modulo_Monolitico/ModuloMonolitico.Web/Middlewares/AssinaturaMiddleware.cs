using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Middlewares;

public class AssinaturaMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly HashSet<string> _rotasLiberadas = new(
        ModuloRotas.TodasRotasAutenticacao
            .Append(ModuloRotas.SaaS.Suspensa)
            .Append("/Api/Auth/Refresh"),
        StringComparer.OrdinalIgnoreCase);

    public AssinaturaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (!context.User.Identity?.IsAuthenticated == true
            || _rotasLiberadas.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase))
            || path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var saasSituacao = context.User.FindFirst("saas_status")?.Value ?? SituacaoAssinatura.Trial;

        if (saasSituacao == SituacaoAssinatura.Bloqueado || saasSituacao == SituacaoAssinatura.Cancelado)
        {
            context.Response.Redirect(ModuloRotas.SaaS.Suspensa);
            return;
        }

        await _next(context);
    }
}

public static class AssinaturaMiddlewareExtensions
{
    public static IApplicationBuilder UseAssinaturaCheck(this IApplicationBuilder builder)
        => builder.UseMiddleware<AssinaturaMiddleware>();
}
