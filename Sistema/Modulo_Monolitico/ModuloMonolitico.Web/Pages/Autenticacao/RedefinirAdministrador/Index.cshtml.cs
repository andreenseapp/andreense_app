using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

public class RedefinirAdministradorModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<RedefinirAdministradorModel> _logger;

    public RedefinirAdministradorModel(IMediator mediator, ILogger<RedefinirAdministradorModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty]
    [Display(Name = "E-mail do administrador")]
    [Required]
    [EmailAddress]
    public string? EmailAdministrador { get; set; }

    public string? MensagemResultado { get; private set; }
    public bool Sucesso { get; private set; }

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado a redefinir administrador para '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("POST redefinir administrador bloqueado para '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Redefinir administrador com dados inválidos pelo Master '{Email}'.", UsuarioEmail);
            MensagemResultado = "Informe um e-mail válido do administrador.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(EmailAdministrador))
        {
            _logger.LogWarning("Redefinir administrador solicitado sem e-mail pelo Master '{Email}'.", UsuarioEmail);
            MensagemResultado = "Informe o e-mail do administrador.";
            return Page();
        }

        var resultado = await _mediator.Send(new RedefinirAdministradorCommand { EmailAdministrador = EmailAdministrador.Trim() });
        Sucesso = resultado.Sucesso;
        MensagemResultado = resultado.Mensagem;

        if (Sucesso)
        {
            _logger.LogInformation("Redefinir administrador com sucesso para '{EmailAlvo}' executado pelo Master '{Email}'.", EmailAdministrador, UsuarioEmail);
        }
        else
        {
            _logger.LogWarning("Redefinir administrador falhou para '{EmailAlvo}' pelo Master '{Email}': {Mensagem}", EmailAdministrador, UsuarioEmail, resultado.Mensagem);
        }

        return Page();
    }
}
