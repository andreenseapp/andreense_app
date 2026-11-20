using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterUsuarioPorIdQuery(Guid Id) : IRequest<Usuario?>;

public class ObterUsuarioPorIdQueryHandler : IRequestHandler<ObterUsuarioPorIdQuery, Usuario?>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ObterUsuarioPorIdQueryHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task<Usuario?> Handle(ObterUsuarioPorIdQuery comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.ObterPorIdAsync(comando.Id);
}
