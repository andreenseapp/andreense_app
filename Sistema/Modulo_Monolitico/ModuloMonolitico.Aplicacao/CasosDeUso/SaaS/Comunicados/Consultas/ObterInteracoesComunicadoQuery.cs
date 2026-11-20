using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterInteracoesComunicadoQuery(Guid ComunicadoId) : IRequest<IEnumerable<InteracaoComunicadoDto>>;

public class ObterInteracoesComunicadoQueryHandler : IRequestHandler<ObterInteracoesComunicadoQuery, IEnumerable<InteracaoComunicadoDto>>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public ObterInteracoesComunicadoQueryHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task<IEnumerable<InteracaoComunicadoDto>> Handle(ObterInteracoesComunicadoQuery comando, CancellationToken cancellationToken)
        => _repositorio.ObterInteracoesAsync(comando.ComunicadoId);
}
