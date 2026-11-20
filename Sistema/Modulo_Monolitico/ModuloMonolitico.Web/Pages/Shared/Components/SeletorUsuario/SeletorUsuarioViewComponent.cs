using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.SeletorUsuario;

public class SeletorUsuarioViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string papel = "cliente",
        string label = "Cliente",
        string fieldName = "ContatoId",
        Guid? valorAtual = null,
        string? nomeAtual = null,
        bool desabilitado = false,
        string? urlNovo = null)
    {
        var vm = new SeletorUsuarioViewModel
        {
            Papel = papel.ToLower(),
            Label = label,
            FieldName = fieldName,
            ValorAtual = valorAtual,
            NomeAtual = nomeAtual,
            Desabilitado = desabilitado,
            UrlNovo = urlNovo,
            ComponentId = $"seletor_{fieldName.Replace(".", "_").ToLower()}_{Guid.NewGuid():N}"
        };

        return View(vm);
    }
}

public class SeletorUsuarioViewModel
{
    public string Papel { get; set; } = "cliente";
    public string Label { get; set; } = "Cliente";
    public string FieldName { get; set; } = "ContatoId";
    public Guid? ValorAtual { get; set; }
    public string? NomeAtual { get; set; }
    public bool Desabilitado { get; set; }
    public string? UrlNovo { get; set; }
    public string ComponentId { get; set; } = string.Empty;
}
