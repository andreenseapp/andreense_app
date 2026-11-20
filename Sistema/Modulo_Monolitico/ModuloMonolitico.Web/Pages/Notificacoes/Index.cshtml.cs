using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Notificacoes;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public List<NotificacaoDto> Notificacoes { get; private set; } = new();
    public string FiltroAtivo { get; private set; } = "todas";
    public int TotalNaoLidas => Notificacoes.Count(n => !n.Lida);

    [BindProperty] public Guid NotificacaoId { get; set; }

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string? filtro)
    {
        FiltroAtivo = filtro ?? "todas";
        Notificacoes = (await _mediator.Send(new ListarNotificacoesQuery(ContaId, UsuarioId, FiltroAtivo))).ToList();
    }

    public async Task<IActionResult> OnPostMarcarLidaAsync()
    {
        var sucesso = await _mediator.Send(new MarcarNotificacaoComoLidaCommand(ContaId, UsuarioId, NotificacaoId));

        if (sucesso)
            Logger.LogInformation("Usuario {UsuarioId} marcou a notificacao {NotificacaoId} como lida.", UsuarioId, NotificacaoId);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarcarTodasLidasAsync()
    {
        await _mediator.Send(new MarcarTodasNotificacoesComoLidasCommand(ContaId, UsuarioId));
        Logger.LogInformation("Usuario {UsuarioId} marcou todas as notificacoes como lidas.", UsuarioId);

        return RedirectToPage();
    }
}
