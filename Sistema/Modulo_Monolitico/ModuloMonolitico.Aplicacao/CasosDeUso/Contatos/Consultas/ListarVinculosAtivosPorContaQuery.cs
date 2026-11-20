using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarVinculosAtivosPorContaQuery(Guid ContaId) : IRequest<IEnumerable<VinculoConta>>;

public class ListarVinculosAtivosPorContaQueryHandler : IRequestHandler<ListarVinculosAtivosPorContaQuery, IEnumerable<VinculoConta>>
{
    private readonly IVinculoRepositorio _vinculoRepositorio;

    public ListarVinculosAtivosPorContaQueryHandler(IVinculoRepositorio vinculoRepositorio)
        => _vinculoRepositorio = vinculoRepositorio;

    public Task<IEnumerable<VinculoConta>> Handle(ListarVinculosAtivosPorContaQuery comando, CancellationToken cancellationToken)
        => _vinculoRepositorio.ListarVinculosAtivosPorContaAsync(comando.ContaId);
}
