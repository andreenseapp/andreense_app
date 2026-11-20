using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterProdutoQuery(Guid Id, Guid ContaId) : IRequest<Produto?>;

public class ObterProdutoQueryHandler : IRequestHandler<ObterProdutoQuery, Produto?>
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public ObterProdutoQueryHandler(IProdutoRepositorio produtoRepositorio)
        => _produtoRepositorio = produtoRepositorio;

    public Task<Produto?> Handle(ObterProdutoQuery comando, CancellationToken cancellationToken)
        => _produtoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}
