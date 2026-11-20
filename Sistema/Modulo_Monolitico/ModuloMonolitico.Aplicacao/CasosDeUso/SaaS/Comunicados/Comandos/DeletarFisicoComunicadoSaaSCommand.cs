using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarFisicoComunicadoSaaSCommand(Guid Id) : IRequest;

public class DeletarFisicoComunicadoSaaSCommandHandler : IRequestHandler<DeletarFisicoComunicadoSaaSCommand>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public DeletarFisicoComunicadoSaaSCommandHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task Handle(DeletarFisicoComunicadoSaaSCommand comando, CancellationToken cancellationToken)
        => _repositorio.DeletarFisicoComunicadoAsync(comando.Id);
}
