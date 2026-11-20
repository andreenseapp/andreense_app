using MediatR;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.TiposVendedor;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.TiposVendedor.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<TipoVendedor> TiposVendedor { get; private set; } = [];

    public async Task OnGetAsync()
    {
        TiposVendedor = await _mediator.Send(new ListarTiposVendedorQuery(ContaId));

        Logger.LogInformation(
            "Listagem de tipos de vendedor carregada por '{Email}'. Total: {Total}.",
            UsuarioEmail, TiposVendedor.Count());
    }
}
