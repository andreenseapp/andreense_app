namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IMensagensRepositorio
{
    Task CadastrarMensagemSistemaAsync(Guid destinatarioId, Guid remetenteId, string titulo, string corpoTexto);
    Task<IEnumerable<TopicoInboxDto>> ListarTopicosAsync(Guid usuarioId);
    Task<IEnumerable<MensagemDto>> ListarMensagensAsync(Guid topicoId);
    Task MarcarTopicoComoLidoAsync(Guid topicoId, Guid usuarioId);
    Task<bool> EhParticipanteAsync(Guid topicoId, Guid usuarioId);
    Task CadastrarMensagemAsync(Guid topicoId, Guid remetenteId, string corpoTexto);
    Task<int> ContarMensagensNaoLidasAsync(Guid usuarioId);
}

public record TopicoInboxDto(
  Guid Id,
  string? Titulo,
  string Tipo,
  string? UltimaMensagemPreview,
  DateTime DataUltimaInteracao,
  int NaoLidas
);

public record MensagemDto(
  Guid Id,
  Guid RemetenteId,
  string CorpoTexto,
  DateTime DataEnvio,
  string Situacao
);


