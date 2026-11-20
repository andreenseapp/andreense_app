using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarCartaoFidelidadeCommand(FidelidadeCartao Cartao, Guid EmpresaId, Guid OperadorId) : IRequest;

public class CadastrarCartaoFidelidadeCommandHandler : IRequestHandler<CadastrarCartaoFidelidadeCommand>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public CadastrarCartaoFidelidadeCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public async Task Handle(CadastrarCartaoFidelidadeCommand comando, CancellationToken cancellationToken)
    {
        var cartao = comando.Cartao;
        cartao.Id = Guid.NewGuid();
        cartao.EmpresaId = comando.EmpresaId;
        cartao.OperadorInclusao = comando.OperadorId;
        cartao.DataInclusao = DateTime.UtcNow;
        cartao.DataUltimaAtualizacao = DateTime.UtcNow;

        await _fidelidadeRepositorio.CadastrarCartaoAsync(cartao);
    }
}
