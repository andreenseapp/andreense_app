using ModuloMonolitico.Web.Core.Constantes;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Geral.SeletorDemo;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => true;

    [BindProperty]
    public Guid? ClienteId { get; set; }

    [BindProperty]
    public Guid? FornecedorId { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        var msg = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.ERP.PostEfetuado, ClienteId, FornecedorId);
        TempData[TempDataKeys.MensagemDemo] = msg;
        return RedirectToPage();
    }
}
