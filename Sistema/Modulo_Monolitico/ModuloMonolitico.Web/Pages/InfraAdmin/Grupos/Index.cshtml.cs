using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Grupos;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public ResultadoPaginado<GrupoUsuario>? DadosPaginados { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? OrdenarPor { get; set; } = "nome";

    [BindProperty(SupportsGet = true)]
    public bool Descendente { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado aos grupos do InfraAdmin para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        if (Pagina < 1)
            Pagina = 1;

        try
        {
            DadosPaginados = await _mediator.Send(new ListarGruposQuery(EmpresaAtiva!.Value, OrdenarPor, Descendente, 50, Pagina));

            _logger.LogInformation(
                "Grupos de usuários consultados por '{Email}'. Empresa: {EmpresaId}, Página: {Pagina}, Total: {Total}.",
                UsuarioEmail,
                EmpresaAtiva.Value,
                Pagina,
                DadosPaginados.TotalItens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar grupos de usuários para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroCarregarLista;
            DadosPaginados = new ResultadoPaginado<GrupoUsuario>();
        }

        return Page();
    }
}
