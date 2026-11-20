using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado ao painel InfraAdmin para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        _logger.LogInformation("Painel InfraAdmin acessado pelo usuário '{Email}'.", UsuarioEmail);
        return Page();
    }
}
