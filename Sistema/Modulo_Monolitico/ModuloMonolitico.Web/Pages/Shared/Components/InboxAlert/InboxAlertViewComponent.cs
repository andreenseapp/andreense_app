using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared.Components.InboxAlert;

public class InboxAlertViewModel
{
    public int MensagensNaoLidas { get; set; }
    public int NotificacoesNaoLidas { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class InboxAlertViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int mensagensNaoLidas, int notificacoesNaoLidas, string url)
    {
        var model = new InboxAlertViewModel
        {
            MensagensNaoLidas = mensagensNaoLidas,
            NotificacoesNaoLidas = notificacoesNaoLidas,
            Url = url
        };
        return View(model);
    }
}
