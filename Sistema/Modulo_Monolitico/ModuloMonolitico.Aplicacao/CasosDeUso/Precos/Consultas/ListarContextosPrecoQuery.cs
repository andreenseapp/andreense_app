using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarContextosPrecoQuery(Guid ContaId, Guid EmpresaId) : IRequest<IEnumerable<ContextoPreco>>;

public class ListarContextosPrecoQueryHandler : IRequestHandler<ListarContextosPrecoQuery, IEnumerable<ContextoPreco>>
{
    private readonly IContextoPrecoRepositorio _contextoPrecoRepositorio;

    public ListarContextosPrecoQueryHandler(IContextoPrecoRepositorio contextoPrecoRepositorio)
        => _contextoPrecoRepositorio = contextoPrecoRepositorio;

    public Task<IEnumerable<ContextoPreco>> Handle(ListarContextosPrecoQuery comando, CancellationToken cancellationToken)
        => _contextoPrecoRepositorio.ListarPorContaEEmpresaAsync(comando.ContaId, comando.EmpresaId);
}
