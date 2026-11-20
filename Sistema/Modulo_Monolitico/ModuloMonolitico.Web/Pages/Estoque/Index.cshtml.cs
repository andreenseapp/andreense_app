using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Estoque;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "ERP.Estoque.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public List<EstoqueSaldo> Saldos { get; set; } = [];
    public List<Empresa> Empresas { get; set; } = [];
    public Guid EmpresaIdSelecionada { get; set; }

    [BindProperty(SupportsGet = true, Name = "busca")]
    [Display(Name = "Busca")]
    public string? TermoBusca { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? empresaId)
    {
        Empresas = (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).ToList();
        EmpresaIdSelecionada = empresaId ?? (Empresas.FirstOrDefault()?.Id ?? Guid.Empty);

        if (EmpresaIdSelecionada != Guid.Empty)
        {
            var lista = await _mediator.Send(new ListarSaldosEstoqueQuery(ContaId, EmpresaIdSelecionada, TermoBusca));
            Saldos = lista.ToList();
        }

        Logger.LogInformation(
            "Posição de estoque carregada por '{Email}'. Empresa: {EmpresaId}, Itens: {Total}, Busca: {Busca}.",
            UsuarioEmail, EmpresaIdSelecionada, Saldos.Count, TermoBusca ?? "(vazio)");

        return Page();
    }
}
