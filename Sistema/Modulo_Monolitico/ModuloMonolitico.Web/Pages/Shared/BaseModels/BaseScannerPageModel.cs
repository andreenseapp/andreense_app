using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Shared
{
    public abstract class BaseScannerPageModel : AppBasePageModel
    {
        public string IdElementoScanner => "qr-reader";
        public abstract string TituloPagina { get; }
        public abstract string SubtituloPagina { get; }

        public virtual void OnGet() { }

        public abstract Task<IActionResult> OnPostProcessarCodigoAsync(string codigo);
    }
}
