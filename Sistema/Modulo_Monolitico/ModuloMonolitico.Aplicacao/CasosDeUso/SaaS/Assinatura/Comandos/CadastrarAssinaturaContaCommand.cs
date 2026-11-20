using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarAssinaturaContaCommand(ContaAssinatura Assinatura) : IRequest;

public class CadastrarAssinaturaContaCommandHandler : IRequestHandler<CadastrarAssinaturaContaCommand>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public CadastrarAssinaturaContaCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task Handle(CadastrarAssinaturaContaCommand comando, CancellationToken cancellationToken)
        => _repositorio.CadastrarAssinaturaAsync(comando.Assinatura);
}
