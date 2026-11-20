using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Perfil.MeusVinculos;

[Authorize]
public class IndexModel : AppBasePageModel
{
    [TempData] public string? MensagemErro { get; set; }
    [TempData] public string? MensagemSucesso { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<ConviteEquipeDto> ConvitesPendentes { get; private set; } = new List<ConviteEquipeDto>();
    public IEnumerable<MinhaEmpresaDto> EmpresasAceitas { get; private set; } = new List<MinhaEmpresaDto>();

    public async Task OnGetAsync()
    {
        ConvitesPendentes = await _mediator.Send(new ListarConvitesUsuarioQuery(UsuarioId));
        EmpresasAceitas = await _mediator.Send(new ListarEmpresasUsuarioQuery(UsuarioId));
    }

    public async Task<IActionResult> OnPostResponderAsync(Guid empresaId, bool aceitar)
    {
        await _mediator.Send(new ResponderConviteEquipeCommand(UsuarioId, empresaId, aceitar));

        if (aceitar)
        {
            _logger.LogInformation(
                "Usuário '{UsuarioId}' aceitou convite para empresa {EmpresaId}.",
                UsuarioId, empresaId);
        }
        else
        {
            _logger.LogInformation(
                "Usuário '{UsuarioId}' recusou convite para empresa {EmpresaId}.",
                UsuarioId, empresaId);
        }

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage();
    }
}
