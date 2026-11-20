using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Cashback;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Cashback.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<CashbackRegra> Regras { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public int TotalPaginas { get; set; }

    public int TotalItens { get; set; }

    private const int TamanhoPagina = 20;

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!EmpresaAtiva.HasValue)
        {
            Logger.LogWarning(
                "Listagem de cashback bloqueada: usuário '{Email}' sem empresa ativa.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.SemEmpresaVinculada;
            return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);
        }

        if (Pagina < 1) Pagina = 1;

        var todasRegras = (await _mediator.Send(new ListarRegrasCashbackPorEmpresaQuery(EmpresaAtiva.Value))).ToList();

        TotalItens = todasRegras.Count;
        TotalPaginas = (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

        if (Pagina > TotalPaginas && TotalPaginas > 0)
            Pagina = TotalPaginas;

        var offset = (Pagina - 1) * TamanhoPagina;
        Regras = todasRegras.Skip(offset).Take(TamanhoPagina).ToList();

        Logger.LogInformation(
            "Listagem de regras de cashback carregada por '{Email}'. Empresa: {EmpresaId}, Total: {Total}, Página: {Pagina}.",
            UsuarioEmail, EmpresaAtiva.Value, TotalItens, Pagina);

        return Page();
    }
}
