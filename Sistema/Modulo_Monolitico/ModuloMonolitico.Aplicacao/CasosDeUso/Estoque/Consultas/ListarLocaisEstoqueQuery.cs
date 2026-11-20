using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarLocaisEstoqueQuery(Guid ContaId, Guid EmpresaId, bool ApenasAtivos = true)
    : IRequest<IEnumerable<LocalEstoque>>;

public class ListarLocaisEstoqueQueryHandler : IRequestHandler<ListarLocaisEstoqueQuery, IEnumerable<LocalEstoque>>
{
    private readonly IEstoqueRepositorio _estoqueRepositorio;

    public ListarLocaisEstoqueQueryHandler(IEstoqueRepositorio estoqueRepositorio)
        => _estoqueRepositorio = estoqueRepositorio;

    public async Task<IEnumerable<LocalEstoque>> Handle(ListarLocaisEstoqueQuery comando, CancellationToken cancellationToken)
    {
        var locais = await _estoqueRepositorio.ListarLocaisPorContaEEmpresaAsync(comando.ContaId, comando.EmpresaId);
        return comando.ApenasAtivos ? locais.Where(l => l.Ativo) : locais;
    }
}
