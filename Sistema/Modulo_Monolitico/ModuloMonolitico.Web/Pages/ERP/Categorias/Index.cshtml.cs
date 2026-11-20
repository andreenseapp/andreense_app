using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.ERP.Categorias;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.Categorias.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<CategoriaDto> Categorias { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Categorias = await _mediator.Send(new ListarCategoriasQuery(ContaId));
        Logger.LogInformation(
            "Listagem de categorias carregada por '{Email}'. Conta: {ContaId}. Total: {Total}.",
            UsuarioEmail, ContaId, Categorias.Count());
        return Page();
    }
}
