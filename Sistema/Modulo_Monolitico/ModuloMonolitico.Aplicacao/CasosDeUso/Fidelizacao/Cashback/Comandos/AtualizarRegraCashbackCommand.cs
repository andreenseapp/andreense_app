using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarRegraCashbackCommand(CashbackRegra Regra) : IRequest;

public class AtualizarRegraCashbackCommandHandler : IRequestHandler<AtualizarRegraCashbackCommand>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public AtualizarRegraCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public Task Handle(AtualizarRegraCashbackCommand comando, CancellationToken cancellationToken)
        => _cashbackRepositorio.AtualizarRegraAsync(comando.Regra);
}
