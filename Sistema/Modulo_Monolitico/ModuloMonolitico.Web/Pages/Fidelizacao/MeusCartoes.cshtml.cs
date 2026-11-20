using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao;

public class MeusCartoesModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    public MeusCartoesModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<CartaoComProgresso> CartoesComProgresso { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        if (UsuarioId == Guid.Empty)
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

        var itens = await _mediator.Send(new ListarMeusCartoesFidelidadeQuery(UsuarioId));

        CartoesComProgresso = itens.Select(x => new CartaoComProgresso
        {
            Saldo = x.Saldo,
            Cartao = x.Cartao,
            PorcentagemProgresso = x.PorcentagemProgresso
        }).ToList();

        return Page();
    }

    public class CartaoComProgresso
    {
        public FidelidadeSaldo Saldo { get; set; } = null!;
        public FidelidadeCartao Cartao { get; set; } = null!;
        public int PorcentagemProgresso { get; set; }
    }
}
