using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarConvitesUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<ConviteEquipeDto>>;

public class ListarConvitesUsuarioQueryHandler : IRequestHandler<ListarConvitesUsuarioQuery, IEnumerable<ConviteEquipeDto>>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ListarConvitesUsuarioQueryHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<IEnumerable<ConviteEquipeDto>> Handle(ListarConvitesUsuarioQuery comando, CancellationToken cancellationToken)
    {
        return await _usuarioRepositorio.ListarConvitesPendentesDoUsuarioAsync(comando.UsuarioId);
    }
}


