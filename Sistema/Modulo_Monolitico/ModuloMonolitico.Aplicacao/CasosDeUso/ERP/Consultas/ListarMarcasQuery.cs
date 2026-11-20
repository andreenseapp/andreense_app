using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarMarcasQuery(Guid ContaId) : IRequest<IEnumerable<MarcaDto>>;

public class ListarMarcasQueryHandler : IRequestHandler<ListarMarcasQuery, IEnumerable<MarcaDto>>
{
    private readonly IMarcaRepositorio _marcaRepositorio;

    public ListarMarcasQueryHandler(IMarcaRepositorio marcaRepositorio)
        => _marcaRepositorio = marcaRepositorio;

    public Task<IEnumerable<MarcaDto>> Handle(ListarMarcasQuery comando, CancellationToken cancellationToken)
        => _marcaRepositorio.ListarPorContaAsync(comando.ContaId);
}
