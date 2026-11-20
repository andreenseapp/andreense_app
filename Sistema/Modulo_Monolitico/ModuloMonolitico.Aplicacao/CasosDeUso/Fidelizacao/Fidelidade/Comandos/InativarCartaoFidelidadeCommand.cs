using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record InativarCartaoFidelidadeCommand(Guid Id) : IRequest;

public class InativarCartaoFidelidadeCommandHandler : IRequestHandler<InativarCartaoFidelidadeCommand>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public InativarCartaoFidelidadeCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public async Task Handle(InativarCartaoFidelidadeCommand comando, CancellationToken cancellationToken)
    {
        var cartao = await _fidelidadeRepositorio.ObterCartaoPorIdAsync(comando.Id)
            ?? throw new InvalidOperationException(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.CartaoNaoEncontrado);

        cartao.Situacao = "inativo";
        await _fidelidadeRepositorio.AtualizarCartaoAsync(cartao);
    }
}
