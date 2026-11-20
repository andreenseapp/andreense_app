using MediatR;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Dominio.Constantes;

namespace ModuloMonolitico.Web.Pages.ERP;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public List<MenuItemDefinicao> Atalhos { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (!EmpresaAtiva.HasValue)
        {
            _logger.LogWarning("Usuário {UsuarioId} acessou hub ERP sem empresa ativa.", UsuarioId);
            return;
        }

        var atalhosDb = await _mediator.Send(new ListarAtalhosDashboardQuery(UsuarioId, EmpresaAtiva.Value));
        var todasTelas = MenuSistemaDefinicoes.ObterTodosItensPlanificados();

        Atalhos = todasTelas
            .Where(t => atalhosDb.Any(a => a.Chave == t.ChavePermissao))
            .ToList();

        if (UsuarioEhMaster && !Atalhos.Any())
        {
            Atalhos = todasTelas
                .Where(t => t.ChavePermissao == "ERP.Empresas.Visualizar" || t.ChavePermissao == "Produto.Visualizar")
                .ToList();
        }
    }
}
