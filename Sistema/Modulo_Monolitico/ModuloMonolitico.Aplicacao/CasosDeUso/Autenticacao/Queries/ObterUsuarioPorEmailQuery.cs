using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterUsuarioPorEmailQuery(string Email) : IRequest<Usuario?>;

public class ObterUsuarioPorEmailQueryHandler : IRequestHandler<ObterUsuarioPorEmailQuery, Usuario?>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ObterUsuarioPorEmailQueryHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task<Usuario?> Handle(ObterUsuarioPorEmailQuery comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.ObterPorEmailAsync(comando.Email);
}
