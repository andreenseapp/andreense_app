using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.AreaServicos;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => true;

    public void OnGet()
    {
    }
}
