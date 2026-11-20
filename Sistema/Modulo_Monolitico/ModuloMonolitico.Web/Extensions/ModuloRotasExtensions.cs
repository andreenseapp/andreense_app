using ModuloMonolitico.Web.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModuloMonolitico.Web.Extensions;

public static class ModuloRotasExtensions
{
    public static void ConfigurarRotasCanonicas(this RazorPagesOptions options)
    {
        foreach (var (paginaPadrao, rotaCanonica) in ModuloRotas.Paginas)
        {
            var rota = rotaCanonica.StartsWith('/') ? rotaCanonica[1..] : rotaCanonica;
            options.Conventions.AddPageRoute(paginaPadrao, rota);

            if (!paginaPadrao.EndsWith("/Index", System.StringComparison.OrdinalIgnoreCase))
            {
                options.Conventions.AddPageRoute(paginaPadrao + "/Index", rota);
            }
        }
    }

}

