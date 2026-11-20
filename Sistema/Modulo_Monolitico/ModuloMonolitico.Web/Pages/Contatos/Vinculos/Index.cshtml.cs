using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Vinculos;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Vinculos.Visualizar";

    private readonly IMediator _mediator;
    protected override bool ExigeEmpresaAtiva => false;

    public IEnumerable<VinculoConta> VinculosAtivos { get; private set; } = [];
    public IEnumerable<SolicitacaoVinculo> SolicitacoesEnviadas { get; private set; } = [];
    public IEnumerable<SolicitacaoVinculo> SolicitacoesRecebidas { get; private set; } = [];

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public async Task OnGetAsync()
    {
        var resultado = await _mediator.Send(new ListarVinculosQuery(ContaId));
        VinculosAtivos = resultado.Ativos;
        SolicitacoesEnviadas = resultado.Enviadas;
        SolicitacoesRecebidas = resultado.Recebidas;
    }

    public async Task<IActionResult> OnPostResponderAsync(Guid solicitacaoId, bool aceito)
    {
        var resultado = await _mediator.Send(new ResponderVinculoCommand(solicitacaoId, ContaId, aceito));

        if (resultado.Sucesso)
        {
            Logger.LogInformation(
                "Usuário '{Email}' {Acao} solicitação de vínculo {SolicitacaoId}.",
                UsuarioEmail, aceito ? "aceitou" : "recusou", solicitacaoId);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return RedirectToPage();
        }

        Logger.LogWarning(
            "Falha ao responder solicitação de vínculo {SolicitacaoId} pelo usuário '{Email}': {Erro}",
            solicitacaoId, UsuarioEmail, resultado.Erro);
        MensagemErro = resultado.Erro;
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRevogarAsync(Guid vinculoId)
    {
        await _mediator.Send(new RevogarVinculoCommand(vinculoId, ContaId));
        Logger.LogInformation("Usuário '{Email}' revogou vínculo {VinculoId}.", UsuarioEmail, vinculoId);
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage();
    }
}
