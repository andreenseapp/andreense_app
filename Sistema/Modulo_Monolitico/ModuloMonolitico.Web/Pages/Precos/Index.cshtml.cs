using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Precos;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.ContextosPreco.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public List<ContextoPreco> Contextos { get; set; } = [];

    public List<Empresa> Empresas { get; set; } = [];

    public Guid EmpresaIdSelecionada { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? empresaId)
    {
        Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();
        EmpresaIdSelecionada = empresaId ?? (Empresas.FirstOrDefault()?.Id ?? Guid.Empty);

        if (EmpresaIdSelecionada != Guid.Empty)
        {
            var lista = await _mediator.Send(new ListarContextosPrecoQuery(ContaId, EmpresaIdSelecionada));
            Contextos = lista.ToList();
        }

        Logger.LogInformation(
            "Listagem de contextos de preço carregada por '{Email}'. Empresa: {EmpresaId}, Total: {Total}.",
            UsuarioEmail, EmpresaIdSelecionada, Contextos.Count);

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarSeguroAsync(Guid id, Guid empresaId)
    {
        try
        {
            await _mediator.Send(new DeletarContextoPrecoCommand { Id = id, ContaId = ContaId });
            Logger.LogInformation(
                "Contexto de preço {Id} excluído pelo usuário '{Email}'.",
                id, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Erro ao excluir contexto de preço {Id} pelo usuário '{Email}'.",
                id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Precos.ErroDeletar;
        }

        return RedirectToPage(new { empresaId });
    }
}
