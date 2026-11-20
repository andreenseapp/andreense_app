using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Produto.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;
using ProdutoEntidade = ModuloMonolitico.Dominio.Entidades.Produto;

namespace ModuloMonolitico.Web.Pages.Produto;

public class IndexModel : AppListBasePageModel<ProdutoEntidade, ProdutoFiltro>
{
    protected override string? ChavePermissao => "Produto.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
        TamanhoPagina = 12;
    }

    public IEnumerable<ProdutoEntidade> Produtos => Itens;

    public async Task<IActionResult> OnGetAsync()
    {
        if (PaginaAtual < 1) PaginaAtual = 1;

        var resultado = await _mediator.Send(new ListarProdutosPaginadoQuery(
            ContaId,
            PaginaAtual,
            TamanhoPagina,
            Filtros.Busca,
            Filtros.Tipo,
            OrdenarPor ?? "descricao",
            OrdenarDirecao ?? "asc"));

        Itens = resultado.Itens;
        TotalItens = resultado.TotalItens;
        return Page();
    }

    public async Task<IActionResult> OnPostDeletarSeguroAsync(Guid id)
    {
        try
        {
            await _mediator.Send(new DeletarSeguroProdutoCommand { Id = id, ContaId = ContaId });
            Logger.LogInformation("Produto (ID: {Id}) deletado com sucesso na Conta {ContaId} pelo usuário '{Email}'.", id, ContaId, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao deletar Produto (ID: {Id}) na Conta {ContaId} pelo usuário '{Email}'.", id, ContaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Produto.ErroDeletar;
        }

        return RedirectToPage();
    }
}
