using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fiscal;

public class IndexModel : AppBasePageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        if (!EmpresaAtiva.HasValue)
        {
            _logger.LogWarning("Usuário {UsuarioId} acessou o hub Fiscal sem empresa ativa.", UsuarioId);
        }
    }
}
