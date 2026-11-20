using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Empresas;

public class EmpresaFiltro
{
    [Display(Name = "Busca")]
    public string? Busca { get; set; }
}

public class IndexModel : AppListBasePageModel<Empresa, EmpresaFiltro>
{
    protected override string? ChavePermissao => "Base.Empresas.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<Empresa> Empresas => Itens;

    public async Task<IActionResult> OnGetAsync()
    {
        Itens = await _mediator.Send(new ListarEmpresasContaQuery(ContaId, Filtros.Busca));
        TotalItens = Itens.Count();

        Logger.LogInformation(
            "Listagem de empresas carregada por '{Email}'. Total: {Total}, Busca: {Busca}.",
            UsuarioEmail, TotalItens, Filtros.Busca ?? "(vazio)");

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarSeguroEmLoteAsync()
    {
        if (IdsSelecionados == null || !IdsSelecionados.Any())
        {
            Logger.LogWarning(
                "Tentativa de exclusão em lote vazia de empresas pelo usuário '{Email}'.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NenhumaSelecionada;
            return RedirectToPage();
        }

        await _mediator.Send(new DeletarSeguroEmpresasEmLoteCommand(ContaId, IdsSelecionados));
        Logger.LogInformation(
            "Lote de {Quantidade} empresa(s) excluído pelo usuário '{Email}'.",
            IdsSelecionados.Count, UsuarioEmail);
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage();
    }
}
