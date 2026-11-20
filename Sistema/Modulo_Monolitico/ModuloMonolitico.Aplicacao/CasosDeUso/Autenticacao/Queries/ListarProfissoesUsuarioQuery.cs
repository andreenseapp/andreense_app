using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarProfissoesUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<UsuarioProfissao>>;

public class ListarProfissoesUsuarioQueryHandler : IRequestHandler<ListarProfissoesUsuarioQuery, IEnumerable<UsuarioProfissao>>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ListarProfissoesUsuarioQueryHandler(IUsuarioRepositorio usuarioRepositorio)
        => _usuarioRepositorio = usuarioRepositorio;

    public Task<IEnumerable<UsuarioProfissao>> Handle(ListarProfissoesUsuarioQuery comando, CancellationToken cancellationToken)
        => _usuarioRepositorio.ListarProfissoesAsync(comando.UsuarioId);
}
