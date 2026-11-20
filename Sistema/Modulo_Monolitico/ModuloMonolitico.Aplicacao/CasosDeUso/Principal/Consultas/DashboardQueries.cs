using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterContadoresPrincipalQuery(Guid ContaId, Guid UsuarioId) : IRequest<(int MensagensNaoLidas, int NotificacoesNaoLidas)>;

public class ObterContadoresPrincipalQueryHandler : IRequestHandler<ObterContadoresPrincipalQuery, (int, int)>
{
    private readonly IMensagensRepositorio _mensagensRepositorio;
    private readonly INotificacaoRepositorio _notificacaoRepositorio;

    public ObterContadoresPrincipalQueryHandler(IMensagensRepositorio mensagensRepositorio, INotificacaoRepositorio notificacaoRepositorio)
    {
        _mensagensRepositorio = mensagensRepositorio;
        _notificacaoRepositorio = notificacaoRepositorio;
    }

    public async Task<(int, int)> Handle(ObterContadoresPrincipalQuery comando, CancellationToken cancellationToken)
    {
        var mensagens = await _mensagensRepositorio.ContarMensagensNaoLidasAsync(comando.UsuarioId);
        var notificacoes = await _notificacaoRepositorio.ContarNotificacoesNaoLidasAsync(comando.ContaId, comando.UsuarioId);
        return (mensagens, notificacoes);
    }
}
