using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.SaaS.Suspensa;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    public void OnGet()
    {
        Logger.LogWarning(
            "Usuário '{Email}' visualizou a tela de conta suspensa (status SaaS: {Status}).",
            UsuarioEmail, SaaSSituacao);
    }
}
