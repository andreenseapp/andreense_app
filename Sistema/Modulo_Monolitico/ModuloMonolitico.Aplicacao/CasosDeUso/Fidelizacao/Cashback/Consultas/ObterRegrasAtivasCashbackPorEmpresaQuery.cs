using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterRegrasAtivasCashbackPorEmpresaQuery(Guid EmpresaId) : IRequest<IEnumerable<CashbackRegra>>;

public class ObterRegrasAtivasCashbackPorEmpresaQueryHandler
    : IRequestHandler<ObterRegrasAtivasCashbackPorEmpresaQuery, IEnumerable<CashbackRegra>>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public ObterRegrasAtivasCashbackPorEmpresaQueryHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public Task<IEnumerable<CashbackRegra>> Handle(ObterRegrasAtivasCashbackPorEmpresaQuery comando, CancellationToken cancellationToken)
        => _cashbackRepositorio.ObterRegrasAtivasPorEmpresaAsync(comando.EmpresaId);
}
