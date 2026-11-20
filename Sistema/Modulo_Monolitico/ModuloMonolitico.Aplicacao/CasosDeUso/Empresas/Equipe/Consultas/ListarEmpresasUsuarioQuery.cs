using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarEmpresasUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<MinhaEmpresaDto>>;

public class ListarEmpresasUsuarioQueryHandler : IRequestHandler<ListarEmpresasUsuarioQuery, IEnumerable<MinhaEmpresaDto>>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ListarEmpresasUsuarioQueryHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<IEnumerable<MinhaEmpresaDto>> Handle(ListarEmpresasUsuarioQuery comando, CancellationToken cancellationToken)
    {
        return await _usuarioRepositorio.ListarEmpresasDoUsuarioAsync(comando.UsuarioId);
    }
}


