using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;
using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Contatos.Vinculos;

public class SolicitarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty]
    public FormularioSolicitarVinculo Formulario { get; set; } = new();

    [TempData] public string? MensagemErro { get; set; }

    public SolicitarModel(IMediator mediator) => _mediator = mediator;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Logger.LogWarning("Tentativa de solicitar vínculo com dados inválidos pelo usuário '{Email}'.", UsuarioEmail);
            return Page();
        }

        if (Formulario.Email.Equals(UsuarioEmail, StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogWarning("Usuário '{Email}' tentou solicitar vínculo consigo mesmo.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Vinculos.ErroSolicitacaoPropria;
            return Page();
        }

        var resultado = await _mediator.Send(new SolicitarVinculoCommand(
            ContaId,
            Formulario.Email.ToLower(),
            Formulario.TipoVinculo,
            Formulario.Mensagem
        ));

        if (resultado.Sucesso)
        {
            Logger.LogInformation("Usuário '{Email}' enviou solicitação de vínculo para '{EmailAlvo}'.", UsuarioEmail, Formulario.Email);
            NotificarSucesso();
            return RedirectToPage("Index");
        }

        Logger.LogWarning("Falha ao solicitar vínculo para '{EmailAlvo}' pelo usuário '{Email}': {Erro}", Formulario.Email, UsuarioEmail, resultado.Erro);
        MensagemErro = resultado.Erro ?? ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Vinculos.ErroEnviarSolicitacao;
        return Page();
    }

    public class FormularioSolicitarVinculo
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tipo de vínculo")]
        [Required(ErrorMessage = "O tipo do vínculo é obrigatório.")]
        public string TipoVinculo { get; set; } = "confianca";

        [Display(Name = "Mensagem")]
        [MaxLength(500, ErrorMessage = "A mensagem pode ter no máximo 500 caracteres.")]
        public string? Mensagem { get; set; }
    }
}
