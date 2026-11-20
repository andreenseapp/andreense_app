using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.Vendedores;

public class VendedorFiltro
{
    public string? Busca { get; set; }
}

public class IndexModel : AppListBasePageModel<Vendedor, VendedorFiltro>
{
    protected override string? ChavePermissao => "CRM.Vendedores.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
        TamanhoPagina = 15;
    }

    public IEnumerable<Vendedor> Vendedores => Itens;

    public async Task<IActionResult> OnGetAsync()
    {
        if (PaginaAtual < 1) PaginaAtual = 1;

        var resultado = await _mediator.Send(new ListarVendedoresPaginadoQuery(
            ContaId,
            PaginaAtual,
            TamanhoPagina,
            Filtros.Busca,
            OrdenarPor ?? "nome",
            OrdenarDirecao ?? "asc"));

        Itens = resultado.Itens;
        TotalItens = resultado.TotalItens;

        Logger.LogInformation(
            "Listagem de vendedores carregada por '{Email}'. Total: {Total}, Busca: {Busca}.",
            UsuarioEmail, TotalItens, Filtros.Busca ?? "(vazio)");

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarSeguroEmLoteAsync()
    {
        if (IdsSelecionados == null || !IdsSelecionados.Any())
        {
            Logger.LogWarning(
                "Tentativa de exclusão em lote vazia de vendedores pelo usuário '{Email}'.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Gestao.Vendedores.NenhumSelecionado;
            return RedirectToPage();
        }

        await _mediator.Send(new DeletarSeguroVendedoresEmLoteCommand(ContaId, IdsSelecionados));
        Logger.LogInformation(
            "Lote de {Quantidade} vendedores excluído pelo usuário '{Email}'.",
            IdsSelecionados.Count, UsuarioEmail);
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage();
    }
}
