using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Varejo;

public class IndexModel : AppBasePageModel
{
  protected override bool ExigeEmpresaAtiva => true;

  public void OnGet()
  {
  }
}
