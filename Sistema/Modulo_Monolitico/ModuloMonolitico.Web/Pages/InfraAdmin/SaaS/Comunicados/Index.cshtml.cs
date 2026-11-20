using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS.Comunicados;

public class ComunicadosIndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComunicadosIndexModel> _logger;

    public ComunicadosIndexModel(IMediator mediator, ILogger<ComunicadosIndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public IEnumerable<ComunicadoSaaSComMetricasDto> Comunicados { get; private set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à listagem de comunicados SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Comunicados = await _mediator.Send(new ListarComunicadosComMetricasQuery());

            _logger.LogInformation(
                "Listagem de comunicados SaaS consultada por '{Email}'. Total: {Total}.",
                UsuarioEmail,
                Comunicados.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar listagem de comunicados SaaS para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.ErroCarregarLista;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarFisicoAsync(Guid id)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de excluir comunicado SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            await _mediator.Send(new DeletarFisicoComunicadoSaaSCommand(id));
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Comunicado SaaS ID '{ComunicadoId}' excluído com sucesso pelo usuário Master '{Email}'.",
                id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao excluir comunicado SaaS ID '{ComunicadoId}' pelo usuário '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Comunicados.ErroExcluir;
        }

        return RedirectToPage();
    }
}
