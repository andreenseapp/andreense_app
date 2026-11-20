using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModuloMonolitico.Web.Core;

public static class ExtensoesResultadoPagina
{
    public static JsonResult JsonResultSucesso(this PageModel _, string urlRetorno)
        => new(new { sucesso = true, urlRedirecionamento = urlRetorno });

    public static JsonResult JsonResultErro(string mensagem)
        => new(new { success = false, message = mensagem });

    public static JsonResult JsonResultErro(this PageModel _, string mensagem)
        => JsonResultErro(mensagem);
}
