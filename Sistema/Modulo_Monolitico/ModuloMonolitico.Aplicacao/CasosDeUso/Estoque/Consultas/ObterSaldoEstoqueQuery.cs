using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterSaldoEstoqueQuery(Guid SaldoId, Guid ContaId) : IRequest<EstoqueSaldo?>;

public class ObterSaldoEstoqueQueryHandler : IRequestHandler<ObterSaldoEstoqueQuery, EstoqueSaldo?>
{
    private readonly IEstoqueRepositorio _estoqueRepositorio;

    public ObterSaldoEstoqueQueryHandler(IEstoqueRepositorio estoqueRepositorio)
        => _estoqueRepositorio = estoqueRepositorio;

    public Task<EstoqueSaldo?> Handle(ObterSaldoEstoqueQuery comando, CancellationToken cancellationToken)
        => _estoqueRepositorio.ObterSaldoPorIdAsync(comando.SaldoId, comando.ContaId);
}
