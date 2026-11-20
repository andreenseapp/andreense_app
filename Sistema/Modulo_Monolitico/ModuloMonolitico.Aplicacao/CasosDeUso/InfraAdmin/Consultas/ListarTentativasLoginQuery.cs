using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarTentativasLoginQuery(int Pagina, int TamanhoPagina, string? FiltroEmail = null) 
    : IRequest<ResultadoPaginado<TentativaLoginListagemDto>>;

public class ListarTentativasLoginQueryHandler : IRequestHandler<ListarTentativasLoginQuery, ResultadoPaginado<TentativaLoginListagemDto>>
{
    private readonly ITentativaLoginRepositorio _repositorio;

    public ListarTentativasLoginQueryHandler(ITentativaLoginRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoPaginado<TentativaLoginListagemDto>> Handle(ListarTentativasLoginQuery comando, CancellationToken cancellationToken)
    {
        return await _repositorio.ListarPaginadoAsync(comando.Pagina, comando.TamanhoPagina, comando.FiltroEmail);
    }
}


