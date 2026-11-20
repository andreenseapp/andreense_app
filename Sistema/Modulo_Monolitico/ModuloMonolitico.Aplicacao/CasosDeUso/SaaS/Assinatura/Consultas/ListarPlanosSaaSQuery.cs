using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarPlanosSaaSQuery() : IRequest<IEnumerable<PlanoSaaS>>;

public class ListarPlanosSaaSQueryHandler : IRequestHandler<ListarPlanosSaaSQuery, IEnumerable<PlanoSaaS>>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ListarPlanosSaaSQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<PlanoSaaS>> Handle(ListarPlanosSaaSQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarPlanosSaaSAsync();
}
