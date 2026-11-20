using ModuloMonolitico.Web.Core.Constantes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Shared;

public abstract class ContextoStandalonePageModel : PageModel
{
    public abstract string ContextoPasta { get; }

    public abstract string PaginaSlug { get; }

    public string UrlCssComum => $"/Contextos/{ContextoPasta}/css/comum.css";

    public string UrlJsComum => $"/Contextos/{ContextoPasta}/js/comum.js";

    public string UrlCssPagina => $"/Contextos/{ContextoPasta}/css/{PaginaSlug}.css";

    public string UrlJsPagina => $"/Contextos/{ContextoPasta}/js/{PaginaSlug}.js";

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        ViewData[ViewDataKeys.ContextoPasta] = ContextoPasta;
        ViewData[ViewDataKeys.PaginaSlug] = PaginaSlug;
        base.OnPageHandlerExecuting(context);
    }

    protected bool RequisicaoAjax()
        => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);

    protected JsonResult ResponderErroAjax(string mensagem, int statusCode = StatusCodes.Status400BadRequest)
        => new JsonResult(ModuloRotas.JsonErro(mensagem).Value) { StatusCode = statusCode };

    protected void NotificarSucesso(string mensagem = MensagensSistema.Geral.Sucesso)
        => TempData[TempDataKeys.MensagemSucesso] = mensagem;

    protected void NotificarErro(string mensagem)
        => TempData[TempDataKeys.MensagemErro] = mensagem;
}
