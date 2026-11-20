using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarAtalhosDashboardQuery(Guid UsuarioId, Guid EmpresaId) : IRequest<IEnumerable<MenuItemAtalhoDto>>;

public class ListarAtalhosDashboardQueryHandler : IRequestHandler<ListarAtalhosDashboardQuery, IEnumerable<MenuItemAtalhoDto>>
{
    private readonly IGrupoUsuarioRepositorio _grupoUsuarioRepositorio;

    public ListarAtalhosDashboardQueryHandler(IGrupoUsuarioRepositorio grupoUsuarioRepositorio)
        => _grupoUsuarioRepositorio = grupoUsuarioRepositorio;

    public Task<IEnumerable<MenuItemAtalhoDto>> Handle(ListarAtalhosDashboardQuery comando, CancellationToken cancellationToken)
        => _grupoUsuarioRepositorio.ListarAtalhosDashboardAsync(comando.UsuarioId, comando.EmpresaId);
}
