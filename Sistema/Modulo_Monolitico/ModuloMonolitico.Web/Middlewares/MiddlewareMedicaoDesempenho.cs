using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Web.Options;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ModuloMonolitico.Web.Middlewares;

public class MiddlewareMedicaoDesempenho
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _ambiente;
    private readonly OpcoesDiagnosticoDesempenho _opcoes;
    private readonly ILogger<MiddlewareMedicaoDesempenho> _logger;

    public MiddlewareMedicaoDesempenho(
        RequestDelegate next,
        IWebHostEnvironment ambiente,
        IOptions<OpcoesDiagnosticoDesempenho> opcoes,
        ILogger<MiddlewareMedicaoDesempenho> logger)
    {
        _next = next;
        _ambiente = ambiente;
        _opcoes = opcoes.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRegistradorDesempenhoRequisicao registrador)
    {
        if (DeveHabilitar(context))
        {
            if (context.Request.Query.ContainsKey("diagnostico"))
            {
                context.Response.Cookies.Append(
                    ChavesSistema.CookieDiagnosticoDesempenho(),
                    "1",
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        MaxAge = TimeSpan.FromHours(8)
                    });
            }

            registrador.HabilitarParaRequisicao();
        }

        var cronometro = Stopwatch.StartNew();
        await _next(context);
        cronometro.Stop();

        if (!registrador.EstaHabilitado)
            return;

        registrador.RegistrarEtapa("requisicao_total", cronometro.ElapsedMilliseconds);

        var path = context.Request.Path.Value ?? "/";
        if (EhRecursoEstatico(path))
            return;

        if (cronometro.ElapsedMilliseconds >= _opcoes.LimiteLogLentidaoMs)
        {
            _logger.LogWarning(
                "Requisição lenta: {Metodo} {Caminho} em {DuracaoMs}ms",
                context.Request.Method,
                path,
                cronometro.ElapsedMilliseconds);
        }
    }

    private bool DeveHabilitar(HttpContext context)
    {
        if (_ambiente.IsDevelopment())
            return true;

        if (!_opcoes.HabilitadoEmProducao)
            return false;

        return context.Request.Query.ContainsKey("diagnostico")
            || context.Request.Cookies.ContainsKey(ChavesSistema.CookieDiagnosticoDesempenho());
    }

    private static bool EhRecursoEstatico(string path)
    {
        if (path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/img", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase))
            return true;

        return path.Contains("/css/", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/js/", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase);
    }
}

public static class MiddlewareMedicaoDesempenhoExtensions
{
    public static IApplicationBuilder UseMedicaoDesempenho(this IApplicationBuilder builder)
        => builder.UseMiddleware<MiddlewareMedicaoDesempenho>();
}
