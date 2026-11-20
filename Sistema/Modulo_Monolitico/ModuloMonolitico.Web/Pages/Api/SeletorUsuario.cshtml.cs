using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Api;

[IgnoreAntiforgeryToken]
public class SeletorUsuarioModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    protected override bool ExigeEmpresaAtiva => false;

    public SeletorUsuarioModel(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> OnGetAsync(
        string papel = "cliente",
        string modo = "contatos",
        string? q = null)
    {
        var papelNormalizado = papel.ToLower() switch
        {
            "fornecedor" => "fornecedor",
            "transportador" => "transportador",
            _ => "cliente"
        };

        var modoNormalizado = modo.ToLower() == "vinculos" ? "vinculos" : "contatos";

        var empresaId = EmpresaAtiva ?? Guid.Empty;

        try
        {
            var resultados = await _mediator.Send(new BuscarContatosOuVinculosQuery(
                ContaId: ContaId,
                EmpresaId: empresaId,
                Papel: papelNormalizado,
                Modo: modoNormalizado,
                Termo: q));

            return new JsonResult(resultados.Select(r => new
            {
                id = r.Id,
                fonte = r.Fonte,
                tipoPessoa = r.TipoPessoa,
                nome = r.NomeExibicao,
                documento = r.Documento,
                email = r.Email,
                telefone = r.Telefone,
                enviaEmailAgendamento = r.EnviaEmailAgendamento,
                contaVinculadaId = r.ContaVinculadaId
            }));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro interno ao buscar contatos.");
            return new JsonResult(ModuloRotas.JsonErro("Erro interno ao buscar contatos.").Value) { StatusCode = 500 };
        }
    }
}

