using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarSaldosEstoqueQuery(Guid ContaId, Guid EmpresaId, string? TermoBusca = null)
    : IRequest<IEnumerable<EstoqueSaldo>>;

public class ListarSaldosEstoqueQueryHandler : IRequestHandler<ListarSaldosEstoqueQuery, IEnumerable<EstoqueSaldo>>
{
    private readonly IEstoqueRepositorio _estoqueRepositorio;

    public ListarSaldosEstoqueQueryHandler(IEstoqueRepositorio estoqueRepositorio)
        => _estoqueRepositorio = estoqueRepositorio;

    public Task<IEnumerable<EstoqueSaldo>> Handle(ListarSaldosEstoqueQuery comando, CancellationToken cancellationToken)
        => _estoqueRepositorio.ListarSaldosPorContaEEmpresaAsync(comando.ContaId, comando.EmpresaId, comando.TermoBusca);
}
