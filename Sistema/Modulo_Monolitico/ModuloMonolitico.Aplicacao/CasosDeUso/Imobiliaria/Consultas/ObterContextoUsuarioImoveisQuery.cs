using MediatR;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.Imobiliaria.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Consultas;

public record ObterContextoUsuarioImoveisQuery(Guid UsuarioId) : IRequest<ContextoUsuarioImoveisDto>;

public class ObterContextoUsuarioImoveisQueryHandler : IRequestHandler<ObterContextoUsuarioImoveisQuery, ContextoUsuarioImoveisDto>
{
    private readonly IImovelFavoritoRepositorio _favoritoRepositorio;
    private readonly IAnuncioVisualizadoRepositorio _visualizadoRepositorio;
    private readonly IBuscaRecenteRepositorio _buscaRecenteRepositorio;

    public ObterContextoUsuarioImoveisQueryHandler(
        IImovelFavoritoRepositorio favoritoRepositorio,
        IAnuncioVisualizadoRepositorio visualizadoRepositorio,
        IBuscaRecenteRepositorio buscaRecenteRepositorio)
    {
        _favoritoRepositorio = favoritoRepositorio;
        _visualizadoRepositorio = visualizadoRepositorio;
        _buscaRecenteRepositorio = buscaRecenteRepositorio;
    }

    public async Task<ContextoUsuarioImoveisDto> Handle(ObterContextoUsuarioImoveisQuery request, CancellationToken cancellationToken)
    {
        var buscasTask = _buscaRecenteRepositorio.ListarPorUsuarioAsync(request.UsuarioId, BuscaRecenteFiltroHelper.Limite);

        var favoritosIds = await _favoritoRepositorio.ObterIdsFavoritosDoUsuarioAsync(request.UsuarioId);
        var visualizadosIds = await _visualizadoRepositorio.ObterIdsVisualizadosDoUsuarioAsync(request.UsuarioId);

        var buscas = (await buscasTask)
            .Select(b => BuscaRecenteFiltroHelper.MontarDto(
                b.Id,
                b.ChaveFiltro,
                b.FiltroJson,
                b.Rotulo,
                b.DataUltimaBusca))
            .Select(b => new BuscaRecenteResumoDto
            {
                ChaveFiltro = b.ChaveFiltro,
                Rotulo = b.Rotulo,
                QueryString = b.QueryString
            })
            .ToList();

        return new ContextoUsuarioImoveisDto
        {
            FavoritosIds = favoritosIds,
            VisualizadosIds = visualizadosIds,
            BuscasRecentes = buscas
        };
    }
}
