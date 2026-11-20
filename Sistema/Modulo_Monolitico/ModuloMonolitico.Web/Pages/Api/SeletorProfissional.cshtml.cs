using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Api;

[IgnoreAntiforgeryToken]
public class SeletorProfissionalModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    protected override bool ExigeEmpresaAtiva => true;

    public SeletorProfissionalModel(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> OnGetAsync(string? q = null, Guid? produtoVariacaoId = null)
    {
        var empresaId = EmpresaAtiva!.Value;

        if (produtoVariacaoId == Guid.Empty)
            produtoVariacaoId = null;

        var prestadores = await _mediator.Send(
            new ListarPrestadoresQuery(empresaId, ContaId, ProdutoVariacaoId: produtoVariacaoId));

        var resultado = prestadores
            .Where(p => string.IsNullOrWhiteSpace(q) ||
                        p.NomeExibicao.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Select(p => new
            {
                id    = p.UsuarioId.HasValue ? p.UsuarioId.ToString() : p.ContatoId.ToString(),
                nome  = p.NomeExibicao,
                email = p.Email ?? string.Empty,
                tipo  = p.UsuarioId.HasValue ? "usuario" : "contato"
            })
            .ToList();

        return new JsonResult(resultado);
    }
}

