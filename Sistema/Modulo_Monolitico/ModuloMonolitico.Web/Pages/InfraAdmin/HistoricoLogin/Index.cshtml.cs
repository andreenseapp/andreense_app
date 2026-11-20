using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.HistoricoLogin;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? FiltroEmail { get; set; }

    public int TamanhoPagina { get; } = 30;
    public int TotalItens { get; private set; }
    public int TotalPaginas => (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

    public IEnumerable<TentativaLoginListagemDto> Itens { get; private set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado ao histórico de logins para o usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (PaginaAtual < 1) PaginaAtual = 1;

        try
        {
            var resultado = await _mediator.Send(new ListarTentativasLoginQuery(PaginaAtual, TamanhoPagina, FiltroEmail));

            Itens = resultado.Itens;
            TotalItens = resultado.TotalItens;

            _logger.LogInformation("Histórico de logins consultado por '{Email}'. Filtro: '{Filtro}', Página: {Pagina}, Total: {Total}.",
                UsuarioEmail, FiltroEmail ?? "(nenhum)", PaginaAtual, TotalItens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao listar histórico de logins para o usuário '{Email}'.", UsuarioEmail);
            Itens = [];
            TotalItens = 0;
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Autenticacao.ErroHistoricoLogin;
        }

        return Page();
    }
}
