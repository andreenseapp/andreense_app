using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.CampoInput;

public class CampoInputViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string label,
        string fieldName,
        string? valorAtual = null,
        string? placeholder = null,
        string tipo = "text",
        bool obrigatorio = false,
        string? iconSvg = null,
        string? mascara = null)
    {
        var model = new CampoInputViewModel
        {
            Label = label,
            FieldName = fieldName,
            ValorAtual = valorAtual,
            Placeholder = placeholder,
            Tipo = tipo,
            Obrigatorio = obrigatorio,
            IconSvg = iconSvg,
            Mascara = mascara
        };

        return View(model);
    }
}
