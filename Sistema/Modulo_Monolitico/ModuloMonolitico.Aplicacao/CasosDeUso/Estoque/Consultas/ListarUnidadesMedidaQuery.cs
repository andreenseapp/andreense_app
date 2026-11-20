using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarUnidadesMedidaQuery(Guid ContaId) : IRequest<IEnumerable<UnidadeMedidaDto>>;

public class ListarUnidadesMedidaQueryHandler : IRequestHandler<ListarUnidadesMedidaQuery, IEnumerable<UnidadeMedidaDto>>
{
    private readonly IUnidadeMedidaRepositorio _unidadeMedidaRepositorio;

    public ListarUnidadesMedidaQueryHandler(IUnidadeMedidaRepositorio unidadeMedidaRepositorio)
        => _unidadeMedidaRepositorio = unidadeMedidaRepositorio;

    public Task<IEnumerable<UnidadeMedidaDto>> Handle(ListarUnidadesMedidaQuery comando, CancellationToken cancellationToken)
        => _unidadeMedidaRepositorio.ListarPorContaAsync(comando.ContaId);
}
