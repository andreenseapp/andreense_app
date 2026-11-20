using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarMarcasProdutoQuery(Guid ContaId) : IRequest<IEnumerable<string>>;

public class ListarMarcasProdutoQueryHandler : IRequestHandler<ListarMarcasProdutoQuery, IEnumerable<string>>
{
    private readonly IProdutoRepositorio _repositorio;

    public ListarMarcasProdutoQueryHandler(IProdutoRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<IEnumerable<string>> Handle(ListarMarcasProdutoQuery comando, CancellationToken cancellationToken)
    {
        return await _repositorio.ListarMarcasAtivasAsync(comando.ContaId);
    }
}
