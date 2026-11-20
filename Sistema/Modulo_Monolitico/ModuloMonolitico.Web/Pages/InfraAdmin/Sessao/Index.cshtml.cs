using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Sessao;

public class IndexModel : AppBasePageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<InformacaoClaim> InformacoesClaims { get; private set; } = [];
    public string TokenArmazenado { get; private set; } = string.Empty;

    protected override bool ExigeEmpresaAtiva => false;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado ao inspetor de sessão para o usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        InformacoesClaims = User.Claims.Select(c => new InformacaoClaim
        {
            Tipo = c.Type,
            Valor = c.Value,
            Emissor = c.Issuer
        }).ToList();

        TokenArmazenado = Request.Cookies[ModuloMonolitico.Web.Options.EsquemaCookieAutenticacao.TokenAcesso]
            ?? "Não encontrado no cookie (talvez Bearer header)";

        _logger.LogInformation(
            "Inspetor de sessão consultado por '{Email}'. {TotalClaims} claims, token {TokenPresente}.",
            UsuarioEmail,
            InformacoesClaims.Count(),
            TokenArmazenado.StartsWith("Não encontrado") ? "ausente" : "presente");

        return Page();
    }

    public class InformacaoClaim
    {
        public string Tipo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Emissor { get; set; } = string.Empty;
    }
}
