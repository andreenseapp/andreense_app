using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Core.Constantes;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public abstract class ContatoFormularioPageModel : AppFormBasePageModel<ContatoFormularioModelo>
{
    protected readonly IMediator Mediator;

    protected ContatoFormularioPageModel(IMediator mediator)
    {
        Mediator = mediator;
    }

    public SelectList ListaVendedores { get; set; } = null!;

    protected async Task CarregarListaVendedoresAsync()
    {
        var vendedores = await Mediator.Send(new ListarVendedoresQuery(ContaId));
        ListaVendedores = new SelectList(vendedores, "Id", "NomeFantasia");
        ViewData[ViewDataKeys.ListaVendedores] = ListaVendedores;
    }
}
