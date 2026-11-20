using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.GruposSaaS;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<GrupoUsuarioSaaS> Grupos { get; private set; } = new List<GrupoUsuarioSaaS>();

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à listagem de grupos SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Grupos = await _mediator.Send(new ListarGruposSaaSQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar grupos SaaS para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroCarregarLista;
        }

        return Page();
    }
}
