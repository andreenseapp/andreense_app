using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ResponderConviteEquipeCommand(Guid UsuarioId, Guid EmpresaId, bool Aceitar) : IRequest<bool>;

public class ResponderConviteEquipeCommandHandler : IRequestHandler<ResponderConviteEquipeCommand, bool>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ResponderConviteEquipeCommandHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<bool> Handle(ResponderConviteEquipeCommand comando, CancellationToken cancellationToken)
    {
        await _usuarioRepositorio.ResponderConviteAsync(comando.UsuarioId, comando.EmpresaId, comando.Aceitar);
        return true;
    }
}


