using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarTopicosMensagensQuery(Guid UsuarioId) : IRequest<IEnumerable<TopicoInboxDto>>;

public class ListarTopicosMensagensQueryHandler : IRequestHandler<ListarTopicosMensagensQuery, IEnumerable<TopicoInboxDto>>
{
    private readonly IMensagensRepositorio _mensagensRepositorio;

    public ListarTopicosMensagensQueryHandler(IMensagensRepositorio mensagensRepositorio)
        => _mensagensRepositorio = mensagensRepositorio;

    public Task<IEnumerable<TopicoInboxDto>> Handle(ListarTopicosMensagensQuery comando, CancellationToken cancellationToken)
        => _mensagensRepositorio.ListarTopicosAsync(comando.UsuarioId);
}

public record ObterMensagensTopicoQuery(Guid TopicoId, Guid UsuarioId) : IRequest<ObterMensagensTopicoResultado?>;

public record ObterMensagensTopicoResultado(
    IEnumerable<MensagemDto> Mensagens,
    IEnumerable<TopicoInboxDto> Topicos,
    TopicoInboxDto TopicoAtivo);

public class ObterMensagensTopicoQueryHandler : IRequestHandler<ObterMensagensTopicoQuery, ObterMensagensTopicoResultado?>
{
    private readonly IMensagensRepositorio _mensagensRepositorio;

    public ObterMensagensTopicoQueryHandler(IMensagensRepositorio mensagensRepositorio)
        => _mensagensRepositorio = mensagensRepositorio;

    public async Task<ObterMensagensTopicoResultado?> Handle(ObterMensagensTopicoQuery comando, CancellationToken cancellationToken)
    {
        var topicos = (await _mensagensRepositorio.ListarTopicosAsync(comando.UsuarioId)).ToList();
        var topico = topicos.FirstOrDefault(t => t.Id == comando.TopicoId);
        if (topico == null)
            return null;

        var mensagens = await _mensagensRepositorio.ListarMensagensAsync(comando.TopicoId);
        await _mensagensRepositorio.MarcarTopicoComoLidoAsync(comando.TopicoId, comando.UsuarioId);

        var index = topicos.FindIndex(t => t.Id == comando.TopicoId);
        if (index >= 0)
            topicos[index] = topicos[index] with { NaoLidas = 0 };

        return new ObterMensagensTopicoResultado(mensagens, topicos, topicos[index]);
    }
}

public record EnviarMensagemTopicoCommand(Guid TopicoId, Guid RemetenteId, string CorpoTexto) : IRequest<bool>;

public class EnviarMensagemTopicoCommandHandler : IRequestHandler<EnviarMensagemTopicoCommand, bool>
{
    private readonly IMensagensRepositorio _mensagensRepositorio;

    public EnviarMensagemTopicoCommandHandler(IMensagensRepositorio mensagensRepositorio)
        => _mensagensRepositorio = mensagensRepositorio;

    public async Task<bool> Handle(EnviarMensagemTopicoCommand comando, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(comando.CorpoTexto))
            return false;

        var ehParticipante = await _mensagensRepositorio.EhParticipanteAsync(comando.TopicoId, comando.RemetenteId);
        if (!ehParticipante)
            return false;

        await _mensagensRepositorio.CadastrarMensagemAsync(comando.TopicoId, comando.RemetenteId, comando.CorpoTexto);
        return true;
    }
}
