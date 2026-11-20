using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages;

[AllowAnonymous]
public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    public IActionResult OnGet()
    {
        if (User?.Identity?.IsAuthenticated == true)
            return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);

        return Page();
    }
}
