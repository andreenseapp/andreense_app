using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Imobiliaria;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var destino = ModuloRotas.Imoveis.Index + Request.QueryString.Value;
        return RedirectPermanent(destino);
    }
}
