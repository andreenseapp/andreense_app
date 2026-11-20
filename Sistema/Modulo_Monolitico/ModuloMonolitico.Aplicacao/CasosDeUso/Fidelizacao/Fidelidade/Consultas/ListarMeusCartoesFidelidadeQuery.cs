using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarMeusCartoesFidelidadeQuery(Guid UsuarioId) : IRequest<IEnumerable<FidelidadeCartaoComProgressoDto>>;

public class ListarMeusCartoesFidelidadeQueryHandler
    : IRequestHandler<ListarMeusCartoesFidelidadeQuery, IEnumerable<FidelidadeCartaoComProgressoDto>>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public ListarMeusCartoesFidelidadeQueryHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public async Task<IEnumerable<FidelidadeCartaoComProgressoDto>> Handle(ListarMeusCartoesFidelidadeQuery comando, CancellationToken cancellationToken)
    {
        return await _fidelidadeRepositorio.ListarCartoesComProgressoPorUsuarioAsync(comando.UsuarioId);
    }
}
