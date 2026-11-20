using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarTodosRecursosQuery : IRequest<IEnumerable<RecursoSistema>>;

public class ListarTodosRecursosQueryHandler : IRequestHandler<ListarTodosRecursosQuery, IEnumerable<RecursoSistema>>
{
    private readonly IGrupoUsuarioRepositorio _grupoRepo;

    public ListarTodosRecursosQueryHandler(IGrupoUsuarioRepositorio grupoRepo)
    {
        _grupoRepo = grupoRepo;
    }

    public async Task<IEnumerable<RecursoSistema>> Handle(ListarTodosRecursosQuery comando, CancellationToken cancellationToken)
        => await _grupoRepo.ListarTodosRecursosAsync();
}
