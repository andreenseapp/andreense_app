using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterPlanoSaaSQuery(Guid Id) : IRequest<PlanoSaaS?>;

public class ObterPlanoSaaSQueryHandler : IRequestHandler<ObterPlanoSaaSQuery, PlanoSaaS?>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ObterPlanoSaaSQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<PlanoSaaS?> Handle(ObterPlanoSaaSQuery comando, CancellationToken cancellationToken)
        => _repositorio.ObterPlanoPorIdAsync(comando.Id);
}
