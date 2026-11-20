using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Extensions;

public static class PageModelRotasExtensions
{
    public static RedirectResult RedirectToRota(this PageModel page, string rota) =>
        ModuloRotas.Redirecionar(rota);

    public static RedirectResult RedirectToRota(this PageModel page, string rota, object routeValues) =>
        ModuloRotas.Redirecionar(rota, routeValues);

    public static JsonResult JsonResultSucesso(this PageModel page, string rotaRedirect, object? dadosExtras = null) =>
        ModuloRotas.JsonSucesso(rotaRedirect, dadosExtras);

    public static JsonResult JsonResultErro(this PageModel page, string mensagem) =>
        ModuloRotas.JsonErro(mensagem);
}
