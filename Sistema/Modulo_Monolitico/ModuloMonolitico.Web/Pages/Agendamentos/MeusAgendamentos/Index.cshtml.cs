using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.MeusAgendamentos;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    public IEnumerable<Agendamento> Proximos { get; private set; } = [];
    public IEnumerable<Agendamento> Historico { get; private set; } = [];

    public int TotalPaginas { get; private set; }
    public int TotalItens { get; private set; }

    [TempData] public string? MensagemSucesso { get; set; }

    public string UrlDetalhes(Guid id) =>
        $"{ModuloRotas.Agendamentos.MeusAgendamentos_Detalhes}?id={id}";

    public async Task OnGetAsync()
    {
        if (PaginaAtual < 1)
            PaginaAtual = 1;

        const int tamanhoPagina = 20;

        _logger.LogInformation(
            "Carregando agendamentos do cliente '{Email}'. Página: {PaginaAtual}.",
            UsuarioEmail,
            PaginaAtual);

        Proximos = await _mediator.Send(
            new ListarProximosAgendamentosClienteQuery(UsuarioId));

        var resultadoHistorico = await _mediator.Send(
            new ListarHistoricoAgendamentosClienteQuery(UsuarioId, PaginaAtual, tamanhoPagina));

        TotalItens = resultadoHistorico.TotalItens;
        TotalPaginas = resultadoHistorico.TotalPaginas;
        PaginaAtual = resultadoHistorico.PaginaAtual;
        Historico = resultadoHistorico.Itens;
    }
}
