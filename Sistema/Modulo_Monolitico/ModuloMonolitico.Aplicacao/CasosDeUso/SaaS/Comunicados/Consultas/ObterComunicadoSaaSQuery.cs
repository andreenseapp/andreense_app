using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterComunicadoSaaSQuery(Guid Id) : IRequest<ComunicadoSaaS?>;

public class ObterComunicadoSaaSQueryHandler : IRequestHandler<ObterComunicadoSaaSQuery, ComunicadoSaaS?>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public ObterComunicadoSaaSQueryHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task<ComunicadoSaaS?> Handle(ObterComunicadoSaaSQuery comando, CancellationToken cancellationToken)
        => _repositorio.ObterPorIdAsync(comando.Id);
}
