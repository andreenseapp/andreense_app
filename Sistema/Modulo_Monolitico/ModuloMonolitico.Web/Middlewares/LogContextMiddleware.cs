using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace ModuloMonolitico.Web.Middlewares;

public class LogContextMiddleware
{
    private readonly RequestDelegate _next;

    public LogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var empresaAtivaId = context.User.FindFirst("empresa_ativa_id")?.Value;
        var usuarioId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var contaInfo = !string.IsNullOrEmpty(empresaAtivaId) ? empresaAtivaId : "Anonimo";
        var tela = context.Request.Path.Value ?? "Desconhecido";
        var modulo = ExtrairModulo(tela);

        using (LogContext.PushProperty("ContaId", contaInfo))
        using (LogContext.PushProperty("UsuarioId", usuarioId ?? "Anonimo"))
        using (LogContext.PushProperty("Tela", tela))
        using (LogContext.PushProperty("Modulo", modulo))
        {
            await _next(context);
        }
    }

    private static string ExtrairModulo(string tela)
    {
        if (string.IsNullOrWhiteSpace(tela) || tela == "Desconhecido")
            return "Sistema";

        var partes = tela.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0)
            return "Sistema";

        if (partes[0].Equals("Contextos", StringComparison.OrdinalIgnoreCase) && partes.Length > 1)
            return partes[1];

        if (partes[0].Equals("Api", StringComparison.OrdinalIgnoreCase) && partes.Length > 1)
            return partes[1];

        return partes[0];
    }
}
