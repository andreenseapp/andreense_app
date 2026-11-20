using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record EnviarMensagemSistemaCommand(Guid DestinatarioId, Guid RemetenteId, string Titulo, string CorpoTexto) : IRequest<bool>;

public class EnviarMensagemSistemaCommandHandler : IRequestHandler<EnviarMensagemSistemaCommand, bool>
{
    private readonly IMensagensRepositorio _mensagensRepositorio;

    public EnviarMensagemSistemaCommandHandler(IMensagensRepositorio mensagensRepositorio)
    {
        _mensagensRepositorio = mensagensRepositorio;
    }

    public async Task<bool> Handle(EnviarMensagemSistemaCommand comando, CancellationToken cancellationToken)
    {
        await _mensagensRepositorio.CadastrarMensagemSistemaAsync(comando.DestinatarioId, comando.RemetenteId, comando.Titulo, comando.CorpoTexto);
        return true;
    }
}


