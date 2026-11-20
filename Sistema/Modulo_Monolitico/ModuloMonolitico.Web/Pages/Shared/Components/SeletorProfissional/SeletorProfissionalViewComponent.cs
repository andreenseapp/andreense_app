using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.SeletorProfissional;

public class SeletorProfissionalViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string label = "Profissional",
        string fieldName = "PrestadorIdComposto",
        string? valorAtual = null,
        string? nomeAtual = null,
        bool obrigatorio = false,
        string? urlNovo = "/Contatos/Novo?Papel=funcionario")
    {
        var vm = new SeletorProfissionalViewModel
        {
            Label        = label,
            FieldName    = fieldName,
            ValorAtual   = valorAtual,
            NomeAtual    = nomeAtual,
            Obrigatorio  = obrigatorio,
            UrlNovo      = urlNovo,
            ComponentId  = $"spr_{Guid.NewGuid():N}"
        };

        return View(vm);
    }
}

public class SeletorProfissionalViewModel
{
    public string Label       { get; set; } = "Profissional";
    public string FieldName   { get; set; } = "PrestadorIdComposto";
    public string? ValorAtual { get; set; }
    public string? NomeAtual  { get; set; }
    public bool Obrigatorio   { get; set; }
    public string? UrlNovo    { get; set; } = "/Contatos/Novo?Papel=funcionario";
    public string ComponentId { get; set; } = string.Empty;
}
