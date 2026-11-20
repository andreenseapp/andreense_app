using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS;

public class PlanosIndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlanosIndexModel> _logger;

    public PlanosIndexModel(IMediator mediator, ILogger<PlanosIndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<PlanoSaaS> Planos { get; private set; } = new List<PlanoSaaS>();
    
    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à listagem de planos SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Planos = await _mediator.Send(new ListarPlanosSaaSQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar os planos SaaS para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Planos.ErroCarregarLista;
        }

        return Page();
    }
}
