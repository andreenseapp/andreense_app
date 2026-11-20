using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Grupos;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<NovoModel> _logger;

    public NovoModel(IMediator mediator, ILogger<NovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [BindProperty]
    public CadastrarGrupoCommand Comando { get; set; } = new();

    public IActionResult OnGet()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à criação de grupos do InfraAdmin para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de criar grupo do InfraAdmin pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            Comando.EmpresaId = EmpresaAtiva!.Value;

            var resultado = await _mediator.Send(Comando);

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation("Grupo '{GrupoNome}' criado com sucesso pelo usuário Master '{Email}'.", Comando.Nome, UsuarioEmail);
                return RedirectToPage("Index");
            }

            MensagemErro = resultado.Mensagem;
            _logger.LogWarning("Falha ao criar grupo pelo usuário Master '{Email}': {Mensagem}", UsuarioEmail, resultado.Mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao criar grupo pelo usuário Master '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
        }

        return Page();
    }
}
