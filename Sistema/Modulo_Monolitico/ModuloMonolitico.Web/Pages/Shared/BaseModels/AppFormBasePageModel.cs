using ModuloMonolitico.Web.Core.Constantes;
using Microsoft.AspNetCore.Mvc;

namespace ModuloMonolitico.Web.Pages.Shared;

public abstract class AppFormBasePageModel<TForm> : AppBasePageModel where TForm : class, new()
{
    [BindProperty]
    public TForm Form { get; set; } = new TForm();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    protected void LimparRascunho(string formularioId)
    {
        ViewData[ViewDataKeys.LimparInstantaneo] = formularioId;
    }
}
