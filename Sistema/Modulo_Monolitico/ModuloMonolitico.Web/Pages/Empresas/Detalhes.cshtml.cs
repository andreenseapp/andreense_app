using MediatR;

using Microsoft.AspNetCore.Mvc;

using ModuloMonolitico.Aplicacao.CasosDeUso;

using ModuloMonolitico.Dominio.Entidades;

using ModuloMonolitico.Web.Pages.Shared;



namespace ModuloMonolitico.Web.Pages.Empresas;



public class DetalhesModel : AppBasePageModel

{

    protected override string? ChavePermissao => "Base.Empresas.Detalhes";

    protected override bool ExigeEmpresaAtiva => false;



    private readonly IMediator _mediator;



    public DetalhesModel(IMediator mediator) => _mediator = mediator;



    public Empresa? Empresa { get; private set; }



    [TempData]

    public string? MensagemSucesso { get; set; }



    [TempData]

    public string? MensagemErro { get; set; }



    public async Task<IActionResult> OnGetAsync(Guid id)

    {

        Empresa = await _mediator.Send(new ObterEmpresaQuery(id, ContaId));



        if (Empresa == null)

        {

            Logger.LogWarning(

                "Empresa {Id} não encontrada para visualização pelo usuário '{Email}'. ContaId: {ContaId}",

                id, UsuarioEmail, ContaId);

            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NaoEncontrada;

            return ModuloRotas.Redirecionar(ModuloRotas.Empresas.Index);

        }



        Logger.LogInformation(

            "Detalhes da empresa '{Nome}' (ID: {Id}) visualizados pelo usuário '{Email}'.",

            Empresa.Nome, Empresa.Id, UsuarioEmail);



        return Page();

    }

}

