using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record InativarRegraCashbackCommand(Guid Id) : IRequest;

public class InativarRegraCashbackCommandHandler : IRequestHandler<InativarRegraCashbackCommand>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public InativarRegraCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public async Task Handle(InativarRegraCashbackCommand comando, CancellationToken cancellationToken)
    {
        var regra = await _cashbackRepositorio.ObterRegraPorIdAsync(comando.Id)
            ?? throw new InvalidOperationException(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.RegraNaoEncontrada);

        regra.Situacao = "inativo";
        await _cashbackRepositorio.AtualizarRegraAsync(regra);
    }
}
