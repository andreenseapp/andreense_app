using Microsoft.AspNetCore.Authorization;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Cidadania.EtiquetaEnvio;

[AllowAnonymous]
public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    public void OnGet()
    {
    }
}
