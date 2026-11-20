using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.ModuloCard;

public class ModuloCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string url, string nome, string descricao, string textoLink,
        string corHexadecimal, string corRgb, string svgIcone, bool emBreve = false)
    {
        var model = new ModuloCardViewModel
        {
            Url = url,
            Nome = nome,
            Descricao = descricao,
            TextoLink = textoLink ?? "Acessar",
            CorHexadecimal = corHexadecimal,
            CorRgb = corRgb,
            SvgIcone = svgIcone,
            EmBreve = emBreve
        };
        return View(model);
    }
}
