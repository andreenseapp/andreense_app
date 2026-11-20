using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao;

public class IndexModel : AppBasePageModel
{
  protected override bool ExigeEmpresaAtiva => false;

  public void OnGet()
  {
  }
}
