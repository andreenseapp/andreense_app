using ModuloMonolitico.Web.Pages.Shared;
using MediatR;
using ModuloMonolitico.Dominio.Constantes;
using ModuloMonolitico.Aplicacao.CasosDeUso;

namespace ModuloMonolitico.Web.Pages.Contatos;

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
            _logger.LogWarning("Usuário {UsuarioId} acessou o hub de Contatos sem empresa ativa.", UsuarioId);
            return;
        }

        var atalhosDb = await _mediator.Send(new ListarAtalhosDashboardQuery(UsuarioId, EmpresaAtiva.Value));

        var itensContatos = MenuSistemaDefinicoes.Modulos
            .Where(m => m.ModuloRaiz == "CRM")
            .SelectMany(m => m.Categorias)
            .SelectMany(c => c.Itens)
            .ToList();

        Atalhos = itensContatos
            .Where(t => atalhosDb.Any(a => a.Chave == t.ChavePermissao))
            .ToList();

        if (UsuarioEhMaster && !Atalhos.Any())
        {
            _logger.LogInformation("Master {UsuarioId} sem atalhos configurados em Contatos. Carregando padrões.", UsuarioId);
            Atalhos = itensContatos
                .Where(t => t.ChavePermissao == "ERP.Vendedores.Visualizar" ||
                            (t.ChavePermissao == "ERP.Contatos.Visualizar" && t.Titulo == "Leads"))
                .ToList();
        }
    }
}
