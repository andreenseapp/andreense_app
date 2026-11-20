using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarSeguroEmpresasEmLoteCommand(Guid ContaId, IEnumerable<Guid> Ids) : IRequest;

public class DeletarSeguroEmpresasEmLoteCommandHandler : IRequestHandler<DeletarSeguroEmpresasEmLoteCommand>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public DeletarSeguroEmpresasEmLoteCommandHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public Task Handle(DeletarSeguroEmpresasEmLoteCommand comando, CancellationToken cancellationToken)
        => _empresaRepositorio.DeletarSeguroEmLoteAsync(comando.Ids, comando.ContaId);
}
