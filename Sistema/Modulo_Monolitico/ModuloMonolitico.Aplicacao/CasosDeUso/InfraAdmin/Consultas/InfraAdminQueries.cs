using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarUltimosConectadosQuery(int Quantidade = 50) : IRequest<IEnumerable<TentativaLoginListagemDto>>;

public class ListarUltimosConectadosQueryHandler : IRequestHandler<ListarUltimosConectadosQuery, IEnumerable<TentativaLoginListagemDto>>
{
    private readonly ITentativaLoginRepositorio _tentativaLoginRepositorio;

    public ListarUltimosConectadosQueryHandler(ITentativaLoginRepositorio tentativaLoginRepositorio)
        => _tentativaLoginRepositorio = tentativaLoginRepositorio;

    public Task<IEnumerable<TentativaLoginListagemDto>> Handle(ListarUltimosConectadosQuery comando, CancellationToken cancellationToken)
        => _tentativaLoginRepositorio.ListarUltimosConectadosAsync(comando.Quantidade);
}

