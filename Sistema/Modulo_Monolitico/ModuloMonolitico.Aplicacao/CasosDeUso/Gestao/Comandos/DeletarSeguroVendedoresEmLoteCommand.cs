using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarSeguroVendedoresEmLoteCommand(Guid ContaId, IEnumerable<Guid> Ids) : IRequest;

public class DeletarSeguroVendedoresEmLoteCommandHandler : IRequestHandler<DeletarSeguroVendedoresEmLoteCommand>
{
    private readonly IVendedorRepositorio _vendedorRepositorio;

    public DeletarSeguroVendedoresEmLoteCommandHandler(IVendedorRepositorio vendedorRepositorio)
        => _vendedorRepositorio = vendedorRepositorio;

    public Task Handle(DeletarSeguroVendedoresEmLoteCommand comando, CancellationToken cancellationToken)
        => _vendedorRepositorio.DeletarSeguroEmLoteAsync(comando.Ids, comando.ContaId);
}
