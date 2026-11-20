using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Imobiliaria;

public class DetalhesModel : PageModel
{
    public IActionResult OnGet()
    {
        var destino = ModuloRotas.Imoveis.Detalhes + Request.QueryString.Value;
        return RedirectPermanent(destino);
    }
}
