using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarRecursosPlanoSaaSQuery() : IRequest<IEnumerable<RecursoPlanoSaaS>>;

public class ListarRecursosPlanoSaaSQueryHandler : IRequestHandler<ListarRecursosPlanoSaaSQuery, IEnumerable<RecursoPlanoSaaS>>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ListarRecursosPlanoSaaSQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<RecursoPlanoSaaS>> Handle(ListarRecursosPlanoSaaSQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarRecursosAsync();
}
