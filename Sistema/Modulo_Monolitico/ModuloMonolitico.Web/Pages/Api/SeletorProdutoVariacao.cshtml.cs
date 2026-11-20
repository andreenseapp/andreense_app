using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Api;

[IgnoreAntiforgeryToken]
public class SeletorProdutoVariacaoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    protected override bool ExigeEmpresaAtiva => false;

    public SeletorProdutoVariacaoModel(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> OnGetAsync(
        string? q = null,
        bool apenasServicos = false)
    {
        var resultados = await _mediator.Send(new ListarProdutosVariacoesQuery(
            ContaId: ContaId,
            Termo: q,
            ApenasServicos: apenasServicos));

        return new JsonResult(resultados.Select(r => new
        {
            id = r.Id,
            nomeExibicao = r.NomeExibicao,
            tipoProduto = r.TipoProduto,
            codigoSku = r.CodigoExibicao,
            duracao = r.DuracaoMinutos,
            corHexadecimal = r.CorHexadecimal ?? "#6366f1",
            precoVenda = r.PrecoVenda
        }));
    }
}

