using MediatR;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Principal;

public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public int MensagensNaoLidas { get; private set; }
    public int NotificacoesNaoLidas { get; private set; }

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var (mensagens, notificacoes) = await _mediator.Send(new ObterContadoresPrincipalQuery(ContaId, UsuarioId));
            MensagensNaoLidas = mensagens;
            NotificacoesNaoLidas = notificacoes;
            _logger.LogInformation(
                "Menu principal carregado para o usuário {UsuarioId}. Mensagens não lidas: {Mensagens}, notificações: {Notificacoes}.",
                UsuarioId,
                mensagens,
                notificacoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao carregar contadores da página principal para o usuário {UsuarioId}.", UsuarioId);
            MensagensNaoLidas = 0;
            NotificacoesNaoLidas = 0;
        }
    }
}
