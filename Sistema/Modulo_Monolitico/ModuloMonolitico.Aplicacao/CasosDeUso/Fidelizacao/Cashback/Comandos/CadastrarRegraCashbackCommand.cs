using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarRegraCashbackCommand(CashbackRegra Regra, Guid EmpresaId, Guid OperadorId) : IRequest;

public class CadastrarRegraCashbackCommandHandler : IRequestHandler<CadastrarRegraCashbackCommand>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;

    public CadastrarRegraCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        => _cashbackRepositorio = cashbackRepositorio;

    public async Task Handle(CadastrarRegraCashbackCommand comando, CancellationToken cancellationToken)
    {
        var regra = comando.Regra;
        regra.Id = Guid.NewGuid();
        regra.EmpresaId = comando.EmpresaId;
        regra.OperadorInclusao = comando.OperadorId;
        regra.DataInclusao = DateTime.UtcNow;
        regra.DataUltimaAtualizacao = DateTime.UtcNow;

        await _cashbackRepositorio.CadastrarRegraAsync(regra);
    }
}
