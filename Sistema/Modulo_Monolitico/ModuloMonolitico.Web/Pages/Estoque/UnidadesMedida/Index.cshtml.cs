using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Estoque.UnidadesMedida;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.UnidadesMedida.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<UnidadeMedidaDto> Unidades { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Unidades = await _mediator.Send(new ListarUnidadesMedidaQuery(ContaId));
        Logger.LogInformation(
            "Listagem de unidades de medida carregada por '{Email}'. Conta: {ContaId}. Total: {Total}.",
            UsuarioEmail, ContaId, Unidades.Count());
        return Page();
    }
}
