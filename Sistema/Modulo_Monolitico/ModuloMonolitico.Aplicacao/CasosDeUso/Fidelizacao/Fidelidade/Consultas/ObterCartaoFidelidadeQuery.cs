using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterCartaoFidelidadeQuery(Guid Id) : IRequest<FidelidadeCartao?>;

public class ObterCartaoFidelidadeQueryHandler : IRequestHandler<ObterCartaoFidelidadeQuery, FidelidadeCartao?>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public ObterCartaoFidelidadeQueryHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public Task<FidelidadeCartao?> Handle(ObterCartaoFidelidadeQuery comando, CancellationToken cancellationToken)
        => _fidelidadeRepositorio.ObterCartaoPorIdAsync(comando.Id);
}
