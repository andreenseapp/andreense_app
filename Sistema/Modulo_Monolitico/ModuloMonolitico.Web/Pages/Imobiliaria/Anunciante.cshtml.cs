using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Imobiliaria;

public class AnuncianteModel : PageModel
{
    public IActionResult OnGet()
    {
        var destino = ModuloRotas.Imoveis.Anunciante + Request.QueryString.Value;
        return RedirectPermanent(destino);
    }
}
