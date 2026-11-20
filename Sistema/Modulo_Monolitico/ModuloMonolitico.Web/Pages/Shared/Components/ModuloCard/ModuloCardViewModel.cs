namespace ModuloMonolitico.Web.Pages.Shared.Components.ModuloCard;

public class ModuloCardViewModel
{
    public string Url { get; set; } = "#";
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string TextoLink { get; set; } = "Acessar";
    public string CorHexadecimal { get; set; } = "#3b82f6";
    public string CorRgb { get; set; } = "59, 130, 246";
    public string SvgIcone { get; set; } = string.Empty;
    public bool EmBreve { get; set; } = false;
}
