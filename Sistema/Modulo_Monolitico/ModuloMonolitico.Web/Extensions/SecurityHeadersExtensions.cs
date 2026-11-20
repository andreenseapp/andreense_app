namespace ModuloMonolitico.Web.Extensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, IConfiguration configuration)
    {
        var r2PublicBase = configuration["CloudflareR2:PublicBucketUrl"]?.TrimEnd('/') ?? string.Empty;

        return app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;

            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(self), microphone=(), geolocation=()";
            headers["X-XSS-Protection"] = "0";

            if (context.Request.IsHttps)
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }

            headers["Content-Security-Policy"] = MontarPoliticaSegurancaConteudo(context, r2PublicBase);

            await next();
        });
    }

    internal static string MontarPoliticaSegurancaConteudo(HttpContext context, string r2PublicBase)
    {
        var imgSources = new List<string> { "'self'", "data:", "blob:" };
        if (!string.IsNullOrWhiteSpace(r2PublicBase))
            imgSources.Add(r2PublicBase);
        imgSources.Add("https:");

        var directives = new Dictionary<string, string>
        {
            ["default-src"] = "'self'",
            ["script-src"] = Join(
                "'self'",
                "'unsafe-inline'",
                "https://accounts.google.com",
                "https://www.gstatic.com",
                "https://unpkg.com"),
            ["style-src"] = Join(
                "'self'",
                "'unsafe-inline'",
                "https://fonts.googleapis.com",
                "https://accounts.google.com"),
            ["img-src"] = string.Join(' ', imgSources),
            ["font-src"] = Join("'self'", "data:", "https://fonts.gstatic.com"),
            ["connect-src"] = Join(
                "'self'",
                "https://accounts.google.com",
                "https://oauth2.googleapis.com"),
            ["frame-src"] = "https://accounts.google.com",
            ["media-src"] = Join("'self'", "blob:"),
            ["worker-src"] = Join("'self'", "blob:"),
            ["object-src"] = "'none'",
            ["base-uri"] = "'self'",
            ["form-action"] = "'self'",
            ["frame-ancestors"] = "'none'"
        };

        if (context.Request.IsHttps)
            directives["upgrade-insecure-requests"] = string.Empty;

        return string.Join("; ", directives.Select(d =>
            string.IsNullOrEmpty(d.Value) ? d.Key : $"{d.Key} {d.Value}"));
    }

    private static string Join(params string[] values) =>
        string.Join(' ', values);
}

