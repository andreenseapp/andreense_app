using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarCartaoFidelidadeCommand(FidelidadeCartao Cartao) : IRequest;

public class AtualizarCartaoFidelidadeCommandHandler : IRequestHandler<AtualizarCartaoFidelidadeCommand>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public AtualizarCartaoFidelidadeCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public Task Handle(AtualizarCartaoFidelidadeCommand comando, CancellationToken cancellationToken)
        => _fidelidadeRepositorio.AtualizarCartaoAsync(comando.Cartao);
}
