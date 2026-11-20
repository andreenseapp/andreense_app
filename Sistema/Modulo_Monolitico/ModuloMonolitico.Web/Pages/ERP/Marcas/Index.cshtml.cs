using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.ERP.Marcas;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.Marcas.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<MarcaDto> Marcas { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Marcas = await _mediator.Send(new ListarMarcasQuery(ContaId));
        Logger.LogInformation(
            "Listagem de marcas carregada por '{Email}'. Conta: {ContaId}. Total: {Total}.",
            UsuarioEmail, ContaId, Marcas.Count());
        return Page();
    }
}
