using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModuloMonolitico.Web.Core;

public abstract class PageModelBase : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Limite { get; set; } = 500;
}
