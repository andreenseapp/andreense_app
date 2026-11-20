namespace ModuloMonolitico.Web.Pages.Shared.Toolbars;

public class ToolbarViewModel
{
    public string Tipo { get; set; } = "Hub";
    public string Titulo { get; set; }
    public string Subtitulo { get; set; }
    public string UrlVoltar { get; set; }
    public string TextoVoltar { get; set; } = "Voltar";
    public string TextoBotaoAcao { get; set; }
    public string UrlBotaoAcao { get; set; }
    public string IconeBotaoAcao { get; set; }
    public string? Tabela { get; set; }
    public string? RegistroId { get; set; }
    public bool MostrarBotaoVoltar => !string.IsNullOrEmpty(UrlVoltar);
    public bool MostrarBotaoAcao => !string.IsNullOrEmpty(TextoBotaoAcao) && !string.IsNullOrEmpty(UrlBotaoAcao);
    public bool MostrarBotaoLog => !string.IsNullOrEmpty(Tabela) && !string.IsNullOrEmpty(RegistroId);
}
