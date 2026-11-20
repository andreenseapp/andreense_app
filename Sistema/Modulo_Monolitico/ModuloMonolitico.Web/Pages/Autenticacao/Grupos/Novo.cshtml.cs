using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Autenticacao.Grupos;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<NovoModel> _logger;

    public NovoModel(IMediator mediator, ILogger<NovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override string? ChavePermissao => "Base.GrupoUsuario.Cadastrar";

    [BindProperty]
    public CadastrarGrupoCommand Comando { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public IActionResult OnGet()
    {
        _logger.LogInformation("Formulário de novo grupo acessado por '{Email}'. Empresa: {EmpresaId}.", UsuarioEmail, EmpresaAtiva);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            Comando.EmpresaId = EmpresaAtiva!.Value;

            var resultado = await _mediator.Send(Comando);

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Novo grupo '{GrupoNome}' cadastrado na empresa {EmpresaId} pelo usuário '{Email}'.",
                    Comando.Nome,
                    Comando.EmpresaId,
                    UsuarioEmail);
                return RedirectToPage("Index");
            }

            MensagemErro = resultado.Mensagem;
            _logger.LogWarning(
                "Falha ao cadastrar grupo '{GrupoNome}' pelo usuário '{Email}': {Mensagem}.",
                Comando.Nome,
                UsuarioEmail,
                resultado.Mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao cadastrar grupo pelo usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.ErroSalvar;
        }

        return Page();
    }
}
