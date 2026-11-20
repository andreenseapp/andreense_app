using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class IndexModel : AppListBasePageModel<Contato, ContatosFiltro>
{
    protected override string? ChavePermissao => "CRM.Contatos.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
        TamanhoPagina = 12;
    }

    [BindProperty(SupportsGet = true)]
    public string? Papel { get; set; }

    public async Task OnGetAsync()
    {
        RestaurarOuSalvarFiltros("crm_contatos");

        var resultado = await _mediator.Send(new ListarContatosPaginadoQuery(
            ContaId,
            PaginaAtual,
            TamanhoPagina,
            Filtros.Busca,
            Filtros.Tipo,
            Filtros.Desde,
            OrdenarPor,
            OrdenarDirecao,
            Papel));

        Itens = resultado.Itens;
        TotalItens = resultado.TotalItens;

        Logger.LogInformation(
            "Listagem de contatos carregada por '{Email}'. Conta: {ContaId}, Papel: {Papel}, Total: {Total}.",
            UsuarioEmail, ContaId, Papel ?? "(todos)", TotalItens);
    }

    public async Task<IActionResult> OnPostDeletarSeguroAsync(Guid id)
    {
        try
        {
            await _mediator.Send(new DeletarSeguroContatoCommand(id, ContaId));
            Logger.LogInformation(
                "Contato {ContatoId} excluído com sucesso na Conta {ContaId} pelo usuário '{Email}'.",
                id, ContaId, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Erro ao excluir contato {ContatoId} na Conta {ContaId} pelo usuário '{Email}'.",
                id, ContaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Cadastro.ErroDeletar;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeletarSeguroEmLoteAsync()
    {
        if (IdsSelecionados == null || !IdsSelecionados.Any())
        {
            Logger.LogWarning(
                "Tentativa de exclusão em lote vazia na Conta {ContaId} pelo usuário '{Email}'.",
                ContaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Cadastro.NenhumSelecionadoExclusao;
            return RedirectToPage();
        }

        try
        {
            await _mediator.Send(new DeletarSeguroContatosEmLoteCommand(IdsSelecionados, ContaId));
            Logger.LogInformation(
                "Lote de {Quantidade} contatos excluído na Conta {ContaId} pelo usuário '{Email}'.",
                IdsSelecionados.Count, ContaId, UsuarioEmail);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Erro ao deletar contatos em lote na Conta {ContaId} pelo usuário '{Email}'.",
                ContaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Cadastro.ErroDeletarSelecionados;
        }

        return RedirectToPage();
    }
}

public class ContatosFiltro
{
    [Display(Name = "Busca")]
    public string? Busca { get; set; }

    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

    [Display(Name = "Desde")]
    public DateTime? Desde { get; set; }
}
