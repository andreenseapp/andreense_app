using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterContextoPrecoQuery(Guid Id, Guid ContaId) : IRequest<ContextoPreco?>;

public class ObterContextoPrecoQueryHandler : IRequestHandler<ObterContextoPrecoQuery, ContextoPreco?>
{
    private readonly IContextoPrecoRepositorio _contextoPrecoRepositorio;

    public ObterContextoPrecoQueryHandler(IContextoPrecoRepositorio contextoPrecoRepositorio)
        => _contextoPrecoRepositorio = contextoPrecoRepositorio;

    public Task<ContextoPreco?> Handle(ObterContextoPrecoQuery comando, CancellationToken cancellationToken)
        => _contextoPrecoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}
