using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

public class SairModel : PageModel
{
    private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
    private readonly ILogger<SairModel> _logger;

    public SairModel(IServicoCookieAutenticacao cookiesAutenticacao, ILogger<SairModel> logger)
    {
        _cookiesAutenticacao = cookiesAutenticacao;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        _logger.LogInformation("Usuário deslogou do sistema.");
        _cookiesAutenticacao.LimparSessao();
        return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
    }
}

