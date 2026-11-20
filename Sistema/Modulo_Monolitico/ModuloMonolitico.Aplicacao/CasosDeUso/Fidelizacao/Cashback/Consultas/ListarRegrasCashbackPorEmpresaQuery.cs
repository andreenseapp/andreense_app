using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarRegrasCashbackPorEmpresaQuery(Guid EmpresaId) : IRequest<IEnumerable<CashbackRegra>>;

public class ListarRegrasCashbackPorEmpresaQueryHandler
    : IRequestHandler<ListarRegrasCashbackPorEmpresaQuery, IEnumerable<CashbackRegra>>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public ListarRegrasCashbackPorEmpresaQueryHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public Task<IEnumerable<CashbackRegra>> Handle(ListarRegrasCashbackPorEmpresaQuery comando, CancellationToken cancellationToken)
        => _cashbackRepositorio.ListarRegrasPorEmpresaAsync(comando.EmpresaId);
}
