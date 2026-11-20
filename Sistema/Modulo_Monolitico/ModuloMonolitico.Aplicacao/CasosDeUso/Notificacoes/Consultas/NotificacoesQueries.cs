using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarNotificacoesQuery(Guid ContaId, Guid UsuarioId, string Filtro) : IRequest<IEnumerable<NotificacaoDto>>;

public class ListarNotificacoesQueryHandler : IRequestHandler<ListarNotificacoesQuery, IEnumerable<NotificacaoDto>>
{
    private readonly INotificacaoRepositorio _notificacaoRepositorio;

    public ListarNotificacoesQueryHandler(INotificacaoRepositorio notificacaoRepositorio)
        => _notificacaoRepositorio = notificacaoRepositorio;

    public Task<IEnumerable<NotificacaoDto>> Handle(ListarNotificacoesQuery comando, CancellationToken cancellationToken)
        => _notificacaoRepositorio.ListarNotificacoesAsync(comando.ContaId, comando.UsuarioId, comando.Filtro);
}

public record MarcarNotificacaoComoLidaCommand(Guid ContaId, Guid UsuarioId, Guid NotificacaoId) : IRequest<bool>;

public class MarcarNotificacaoComoLidaCommandHandler : IRequestHandler<MarcarNotificacaoComoLidaCommand, bool>
{
    private readonly INotificacaoRepositorio _notificacaoRepositorio;

    public MarcarNotificacaoComoLidaCommandHandler(INotificacaoRepositorio notificacaoRepositorio)
        => _notificacaoRepositorio = notificacaoRepositorio;

    public Task<bool> Handle(MarcarNotificacaoComoLidaCommand comando, CancellationToken cancellationToken)
        => _notificacaoRepositorio.MarcarComoLidaAsync(comando.ContaId, comando.UsuarioId, comando.NotificacaoId);
}

public record MarcarTodasNotificacoesComoLidasCommand(Guid ContaId, Guid UsuarioId) : IRequest<Unit>;

public class MarcarTodasNotificacoesComoLidasCommandHandler : IRequestHandler<MarcarTodasNotificacoesComoLidasCommand, Unit>
{
    private readonly INotificacaoRepositorio _notificacaoRepositorio;

    public MarcarTodasNotificacoesComoLidasCommandHandler(INotificacaoRepositorio notificacaoRepositorio)
        => _notificacaoRepositorio = notificacaoRepositorio;

    public async Task<Unit> Handle(MarcarTodasNotificacoesComoLidasCommand comando, CancellationToken cancellationToken)
    {
        await _notificacaoRepositorio.MarcarTodasComoLidasAsync(comando.ContaId, comando.UsuarioId);
        return Unit.Value;
    }
}
