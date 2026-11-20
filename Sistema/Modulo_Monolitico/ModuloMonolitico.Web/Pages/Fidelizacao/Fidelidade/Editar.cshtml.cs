using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Fidelidade;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Fidelidade.Editar";

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public FidelidadeCartao Cartao { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var cartao = await _mediator.Send(new ObterCartaoFidelidadeQuery(id));
        if (cartao == null)
        {
            Logger.LogWarning(
                "Cartão fidelidade {CartaoId} não encontrado para edição pelo usuário '{Email}'.",
                id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.CartaoNaoEncontrado;
            return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Fidelidade);
        }

        Cartao = cartao;

        Logger.LogInformation(
            "Formulário de edição do cartão fidelidade '{Titulo}' (ID: {Id}) aberto pelo usuário '{Email}'.",
            cartao.Titulo, cartao.Id, UsuarioEmail);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _mediator.Send(new AtualizarCartaoFidelidadeCommand(Cartao));

        Logger.LogInformation(
            "Cartão fidelidade '{Titulo}' (ID: {Id}) atualizado pelo usuário '{Email}'.",
            Cartao.Titulo, Cartao.Id, UsuarioEmail);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Fidelidade);
    }

    public async Task<IActionResult> OnPostInativarAsync(Guid id)
    {
        await _mediator.Send(new InativarCartaoFidelidadeCommand(id));

        Logger.LogInformation(
            "Cartão fidelidade {Id} inativado pelo usuário '{Email}'.",
            id, UsuarioEmail);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Fidelidade);
    }
}
