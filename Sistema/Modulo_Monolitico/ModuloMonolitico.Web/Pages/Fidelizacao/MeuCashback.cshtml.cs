using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao;

public class MeuCashbackModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<MeuCashbackModel> _logger;

    public MeuCashbackModel(IMediator mediator, ILogger<MeuCashbackModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    public IReadOnlyList<SaldoPorEmpresa> SaldosPorEmpresa { get; set; } = [];
    public IReadOnlyList<CashbackCredito> HistoricoCreditos { get; set; } = [];
    public decimal TotalDisponivel { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (UsuarioId == Guid.Empty)
        {
            _logger.LogWarning("Tentativa de acesso ao Meu Cashback sem usuário logado.");
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
        }

        _logger.LogInformation("Carregando histórico de cashback para o usuário {UsuarioId}, Página {Pagina}", UsuarioId, Pagina);

        var (saldos, creditos, totalDisponivel, totalPaginas) =
            await _mediator.Send(new ObterMeuCashbackQuery(UsuarioId, Pagina));

        HistoricoCreditos = creditos;
        TotalDisponivel = totalDisponivel;
        ViewData[ViewDataKeys.PaginaAtual] = Pagina;
        ViewData[ViewDataKeys.TotalPaginas] = totalPaginas;

        SaldosPorEmpresa = saldos.Select(s => new SaldoPorEmpresa
        {
            EmpresaId = s.EmpresaId,
            NomeEmpresa = s.NomeEmpresa,
            SaldoDisponivel = s.SaldoDisponivel,
            DataUltimaAtualizacao = s.DataUltimaAtualizacao
        }).ToList();

        return Page();
    }

    public class SaldoPorEmpresa
    {
        public Guid EmpresaId { get; set; }
        public string NomeEmpresa { get; set; } = string.Empty;
        public decimal SaldoDisponivel { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
