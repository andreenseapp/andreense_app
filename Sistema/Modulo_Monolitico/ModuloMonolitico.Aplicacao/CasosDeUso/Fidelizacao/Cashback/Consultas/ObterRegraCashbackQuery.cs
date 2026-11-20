using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterRegraCashbackQuery(Guid Id) : IRequest<CashbackRegra?>;

public class ObterRegraCashbackQueryHandler : IRequestHandler<ObterRegraCashbackQuery, CashbackRegra?>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public ObterRegraCashbackQueryHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public Task<CashbackRegra?> Handle(ObterRegraCashbackQuery comando, CancellationToken cancellationToken)
        => _cashbackRepositorio.ObterRegraPorIdAsync(comando.Id);
}
