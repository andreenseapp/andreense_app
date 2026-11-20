using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterResultadosEnqueteComunicadoQuery(Guid ComunicadoId) : IRequest<IEnumerable<ResultadoEnqueteDto>>;

public class ObterResultadosEnqueteComunicadoQueryHandler : IRequestHandler<ObterResultadosEnqueteComunicadoQuery, IEnumerable<ResultadoEnqueteDto>>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public ObterResultadosEnqueteComunicadoQueryHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task<IEnumerable<ResultadoEnqueteDto>> Handle(ObterResultadosEnqueteComunicadoQuery comando, CancellationToken cancellationToken)
        => _repositorio.ObterResultadosEnqueteAsync(comando.ComunicadoId);
}
