using MediatR;

using Microsoft.AspNetCore.Mvc;

using ModuloMonolitico.Aplicacao.CasosDeUso;

using ModuloMonolitico.Dominio.Entidades;

using ModuloMonolitico.Web.Pages.Shared;



namespace ModuloMonolitico.Web.Pages.Fidelizacao.Cashback;



public class EditarModel : AppBasePageModel

{

    protected override string? ChavePermissao => "Servicos.Cashback.Editar";



    private readonly IMediator _mediator;



    public EditarModel(IMediator mediator) => _mediator = mediator;



    [BindProperty]

    public CashbackRegra Regra { get; set; } = new();



    [TempData]

    public string? MensagemSucesso { get; set; }



    [TempData]

    public string? MensagemErro { get; set; }



    public async Task<IActionResult> OnGetAsync(Guid id)

    {

        var regra = await _mediator.Send(new ObterRegraCashbackQuery(id));

        if (regra == null)

        {

            Logger.LogWarning(

                "Regra cashback {RegraId} não encontrada para edição pelo usuário '{Email}'.",

                id, UsuarioEmail);

            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.RegraNaoEncontrada;

            return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Cashback);

        }



        Regra = regra;



        Logger.LogInformation(

            "Formulário de edição da regra cashback '{Titulo}' (ID: {Id}) aberto pelo usuário '{Email}'.",

            regra.Titulo, regra.Id, UsuarioEmail);



        return Page();

    }



    public async Task<IActionResult> OnPostAsync()

    {

        if (!ModelState.IsValid)

            return Page();



        await _mediator.Send(new AtualizarRegraCashbackCommand(Regra));



        Logger.LogInformation(

            "Regra cashback '{Titulo}' (ID: {Id}) atualizada pelo usuário '{Email}'.",

            Regra.Titulo, Regra.Id, UsuarioEmail);



        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;

        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Cashback);

    }



    public async Task<IActionResult> OnPostInativarAsync(Guid id)

    {

        await _mediator.Send(new InativarRegraCashbackCommand(id));



        Logger.LogInformation(

            "Regra cashback {Id} inativada pelo usuário '{Email}'.",

            id, UsuarioEmail);



        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;

        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Cashback);

    }

}

