using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos;

public class IndexModel : AppBasePageModel
{
    public IActionResult OnGet()
    {
        return Redirect(ModuloRotas.AreaServicos.Index);
    }
}
