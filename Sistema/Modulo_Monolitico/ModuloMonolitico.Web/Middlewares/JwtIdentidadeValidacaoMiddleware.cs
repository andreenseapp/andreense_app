using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Servicos;
using System.Text.Json;

namespace ModuloMonolitico.Web.Middlewares;

public class JwtIdentidadeValidacaoMiddleware
{
    private static readonly HashSet<string> _rotasIgnoradas = new(
        ModuloRotas.TodasRotasAutenticacao
            .Concat(ModuloRotas.RotasPortalPublicasImoveis)
            .Append(ModuloRotas.Raiz.Index),
        StringComparer.OrdinalIgnoreCase);

    private readonly RequestDelegate _next;

    public JwtIdentidadeValidacaoMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IServicoIdentidadeJwt identidade)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (context.User.Identity?.IsAuthenticated != true
            || _rotasIgnoradas.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase))
            || path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/img", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)
            || (path.StartsWith("/Contextos", StringComparison.OrdinalIgnoreCase) && 
                (path.Contains("/css/", StringComparison.OrdinalIgnoreCase) || 
                 path.Contains("/js/", StringComparison.OrdinalIgnoreCase) || 
                 path.Contains("/img/", StringComparison.OrdinalIgnoreCase) || 
                 System.IO.Path.HasExtension(path))))
        {
            await _next(context);
            return;
        }

        if (!identidade.TentarResolverUsuarioId(context.User, out var usuarioId))
        {
            await RejeitarAsync(context, identidade, path);
            return;
        }

        var emailJwt = identidade.ObterEmail(context.User);
        if (!await identidade.ValidarIdentidadeContraBancoAsync(usuarioId, emailJwt, context.RequestAborted))
        {
            await RejeitarAsync(context, identidade, path);
            return;
        }

        await _next(context);
    }

    private static async Task RejeitarAsync(HttpContext context, IServicoIdentidadeJwt identidade, string path)
    {
        identidade.EncerrarSessaoInvalida(context);

        var request = context.Request;
        bool isApi = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/Api/", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/api/", StringComparison.OrdinalIgnoreCase);
        bool isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest"
            || request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase);

        if (isApi || isAjax)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var mensagem = MensagensSistema.Autenticacao.SessaoExpirada;
            await context.Response.WriteAsync(
                $"{{\"sucesso\":false,\"codigo\":\"SESSAO_EXPIRADA\",\"mensagem\":{JsonSerializer.Serialize(mensagem)}}}");
            return;
        }

        context.Response.Redirect(ModuloRotas.Autenticacao.Login);
    }
}

