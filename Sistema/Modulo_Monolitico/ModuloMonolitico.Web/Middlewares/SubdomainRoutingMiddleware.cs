using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Middlewares;

public class SubdomainRoutingMiddleware
{
    private readonly RequestDelegate _next;

    public SubdomainRoutingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var host = context.Request.Host.Host.ToLower();
        var path = context.Request.Path.Value ?? "";

        if (path == "/" || path == "")
        {
            var domain = host;
            if (domain.StartsWith("www."))
                domain = domain.Substring(4);

            var subdomain = domain.Split('.')[0];

            switch (subdomain)
            {
                case "erp": 
                    context.Request.Path = "/ERP/Index"; 
                    break;
                case "servico":
                case "servicos": 
                    context.Request.Path = "/AreaServicos/Index"; 
                    break;
                case "cliente":
                case "clientes": 
                    context.Request.Path = "/AreaCliente/Index"; 
                    break;
                case "varejo": 
                    context.Request.Path = "/Varejo/Index"; 
                    break;
                case "financeiro": 
                    context.Request.Path = "/Financeiro/Index"; 
                    break;
                case "logistica": 
                    context.Request.Path = "/Logistica/Index"; 
                    break;
                case "cidade": 
                    context.Request.Path = "/Cidadania/Index"; 
                    break;
                case "crm": 
                    context.Request.Path = "/CRM/Index"; 
                    break;
                case "helpdesk": 
                    context.Request.Path = "/HelpDesk/Index"; 
                    break;
                case "fiscal": 
                    context.Request.Path = "/Fiscal/Index"; 
                    break;
                case "imoveis":
                case "imóveis":
                    context.Request.Path = "/Imoveis";
                    break;
                case "curriculos":
                case "currículos":
                case "empregos":
                case "vagas":
                    context.Request.Path = "/Curriculos";
                    break;
            }
        }

        await _next(context);
    }
}

public static class SubdomainRoutingMiddlewareExtensions
{
    public static IApplicationBuilder UseSubdomainRouting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SubdomainRoutingMiddleware>();
    }
}

