using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterAssinaturaContaQuery(Guid ContaId) : IRequest<ContaAssinatura?>;

public class ObterAssinaturaContaQueryHandler : IRequestHandler<ObterAssinaturaContaQuery, ContaAssinatura?>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public ObterAssinaturaContaQueryHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task<ContaAssinatura?> Handle(ObterAssinaturaContaQuery comando, CancellationToken cancellationToken)
        => _repositorio.ObterAssinaturaDaContaAsync(comando.ContaId);
}
