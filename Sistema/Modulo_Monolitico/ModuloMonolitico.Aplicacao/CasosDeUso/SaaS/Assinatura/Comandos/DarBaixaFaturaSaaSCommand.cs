using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DarBaixaFaturaSaaSCommand(Guid FaturaId, Guid OperadorId) : IRequest;

public class DarBaixaFaturaSaaSCommandHandler : IRequestHandler<DarBaixaFaturaSaaSCommand>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public DarBaixaFaturaSaaSCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task Handle(DarBaixaFaturaSaaSCommand comando, CancellationToken cancellationToken)
        => _repositorio.DarBaixaFaturaAsync(comando.FaturaId, comando.OperadorId);
}
