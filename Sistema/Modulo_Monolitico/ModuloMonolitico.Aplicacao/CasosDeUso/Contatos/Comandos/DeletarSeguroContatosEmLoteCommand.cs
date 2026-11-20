using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarSeguroContatosEmLoteCommand(IEnumerable<Guid> Ids, Guid ContaId) : IRequest;

public class DeletarSeguroContatosEmLoteCommandHandler : IRequestHandler<DeletarSeguroContatosEmLoteCommand>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public DeletarSeguroContatosEmLoteCommandHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public Task Handle(DeletarSeguroContatosEmLoteCommand comando, CancellationToken cancellationToken)
        => _contatoRepositorio.DeletarSeguroEmLoteAsync(comando.Ids, comando.ContaId);
}
