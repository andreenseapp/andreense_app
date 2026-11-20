using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Fidelidade;

public class FiltroFidelidadeVazio { }

public class IndexModel : AppListBasePageModel<FidelidadeCartao, FiltroFidelidadeVazio>
{
    protected override string? ChavePermissao => "Servicos.Fidelidade.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
        TamanhoPagina = 30;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!EmpresaAtiva.HasValue)
        {
            Logger.LogWarning(
                "Listagem de cartões fidelidade bloqueada: usuário '{Email}' sem empresa ativa.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.SemEmpresaVinculada;
            return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);
        }

        if (PaginaAtual < 1) PaginaAtual = 1;

        var resultado = await _mediator.Send(new ListarCartoesFidelidadePorEmpresaQuery(
            EmpresaAtiva.Value,
            OrdenarPor,
            OrdenarDirecao,
            PaginaAtual,
            TamanhoPagina));

        TotalItens = resultado.TotalItens;
        Itens = resultado.Itens;

        Logger.LogInformation(
            "Listagem de cartões fidelidade carregada por '{Email}'. Empresa: {EmpresaId}, Total: {Total}, Página: {Pagina}.",
            UsuarioEmail, EmpresaAtiva.Value, TotalItens, PaginaAtual);

        return Page();
    }
}
