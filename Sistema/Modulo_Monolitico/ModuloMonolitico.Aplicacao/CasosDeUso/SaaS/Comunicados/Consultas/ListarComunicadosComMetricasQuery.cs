using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarComunicadosComMetricasQuery() : IRequest<IEnumerable<ComunicadoSaaSComMetricasDto>>;

public class ListarComunicadosComMetricasQueryHandler : IRequestHandler<ListarComunicadosComMetricasQuery, IEnumerable<ComunicadoSaaSComMetricasDto>>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public ListarComunicadosComMetricasQueryHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task<IEnumerable<ComunicadoSaaSComMetricasDto>> Handle(ListarComunicadosComMetricasQuery comando, CancellationToken cancellationToken)
        => _repositorio.ListarTodosComMetricasAsync();
}
