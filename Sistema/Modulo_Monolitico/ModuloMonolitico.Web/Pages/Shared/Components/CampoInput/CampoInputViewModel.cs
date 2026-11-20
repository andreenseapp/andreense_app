namespace ModuloMonolitico.Web.Pages.Shared.Components.CampoInput;

public class CampoInputViewModel
{
    public string ComponentId { get; set; } = Guid.NewGuid().ToString("N");
    public string Label { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string? ValorAtual { get; set; }
    public string? Placeholder { get; set; }
    public string Tipo { get; set; } = "text";
    public bool Obrigatorio { get; set; }
    public string? IconSvg { get; set; }
    public string? Mascara { get; set; }
}
