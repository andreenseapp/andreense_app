using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarContatosPorContaQuery(Guid ContaId, string? FiltroNome = null, string? FiltroTipo = null) : IRequest<IEnumerable<Contato>>;

public class ListarContatosPorContaQueryHandler : IRequestHandler<ListarContatosPorContaQuery, IEnumerable<Contato>>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public ListarContatosPorContaQueryHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public Task<IEnumerable<Contato>> Handle(ListarContatosPorContaQuery comando, CancellationToken cancellationToken)
        => _contatoRepositorio.ListarPorContaIdAsync(comando.ContaId, comando.FiltroNome, comando.FiltroTipo);
}
