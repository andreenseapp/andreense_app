using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Middlewares;

public class JwtUsuarioContextMiddleware
{
    public const string UsuarioIdItemKey = "UsuarioId";

    private readonly RequestDelegate _next;

    public JwtUsuarioContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IServicoIdentidadeJwt identidade)
    {
        if (context.User.Identity?.IsAuthenticated == true
            && identidade.TentarResolverUsuarioId(context.User, out var id))
        {
            context.Items[UsuarioIdItemKey] = id;
        }

        await _next(context);
    }
}

