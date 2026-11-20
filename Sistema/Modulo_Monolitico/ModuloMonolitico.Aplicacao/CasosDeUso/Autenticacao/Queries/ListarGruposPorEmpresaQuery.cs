using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarGruposPorEmpresaQuery(Guid EmpresaId) : IRequest<IEnumerable<GrupoUsuario>>;

public class ListarGruposPorEmpresaQueryHandler : IRequestHandler<ListarGruposPorEmpresaQuery, IEnumerable<GrupoUsuario>>
{
    private readonly IGrupoUsuarioRepositorio _grupoUsuarioRepositorio;

    public ListarGruposPorEmpresaQueryHandler(IGrupoUsuarioRepositorio grupoUsuarioRepositorio)
        => _grupoUsuarioRepositorio = grupoUsuarioRepositorio;

    public async Task<IEnumerable<GrupoUsuario>> Handle(ListarGruposPorEmpresaQuery comando, CancellationToken cancellationToken)
    {
        var paginado = await _grupoUsuarioRepositorio.ListarPorEmpresaIdAsync(comando.EmpresaId, "nome", false, 500, 1);
        return paginado.Itens;
    }
}
