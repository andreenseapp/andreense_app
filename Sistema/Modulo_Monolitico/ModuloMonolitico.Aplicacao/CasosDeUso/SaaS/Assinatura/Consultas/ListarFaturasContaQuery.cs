using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarFaturasContaQuery(Guid ContaId) : IRequest<IEnumerable<FaturaSaaS>>;

public class ListarFaturasContaQueryHandler : IRequestHandler<ListarFaturasContaQuery, IEnumerable<FaturaSaaS>>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ListarFaturasContaQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<FaturaSaaS>> Handle(ListarFaturasContaQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarFaturasDaContaAsync(comando.ContaId);
}
