using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarSeguroContatoCommand(Guid Id, Guid ContaId) : IRequest;

public class DeletarSeguroContatoCommandHandler : IRequestHandler<DeletarSeguroContatoCommand>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public DeletarSeguroContatoCommandHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public Task Handle(DeletarSeguroContatoCommand comando, CancellationToken cancellationToken)
        => _contatoRepositorio.DeletarSeguroAsync(comando.Id, comando.ContaId);
}
