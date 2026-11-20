namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface INotificacaoRepositorio
{
    Task<IEnumerable<NotificacaoDto>> ListarNotificacoesAsync(Guid contaId, Guid usuarioId, string filtro);
    Task<bool> MarcarComoLidaAsync(Guid contaId, Guid usuarioId, Guid notificacaoId);
    Task MarcarTodasComoLidasAsync(Guid contaId, Guid usuarioId);
    Task<int> ContarNotificacoesNaoLidasAsync(Guid contaId, Guid usuarioId);
}

public record NotificacaoDto(
  Guid Id,
  string Titulo,
  string Mensagem,
  string Tipo,
  string? LinkAcao,
  bool Lida,
  bool EhGlobal,
  DateTime DataInclusao
);


