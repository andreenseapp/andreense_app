using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarAssinaturaContaCommand(ContaAssinatura Assinatura) : IRequest;

public class AtualizarAssinaturaContaCommandHandler : IRequestHandler<AtualizarAssinaturaContaCommand>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public AtualizarAssinaturaContaCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task Handle(AtualizarAssinaturaContaCommand comando, CancellationToken cancellationToken)
        => _repositorio.AtualizarAssinaturaAsync(comando.Assinatura);
}
