using FluentValidation;
using MediatR;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class DeletarSeguroProdutoCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
}

public class DeletarSeguroProdutoCommandValidador : AbstractValidator<DeletarSeguroProdutoCommand>
{
    public DeletarSeguroProdutoCommandValidador()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do produto é obrigatório.");
        RuleFor(x => x.ContaId).NotEmpty().WithMessage("A conta é obrigatória.");
    }
}

public class DeletarSeguroProdutoCommandHandler : IRequestHandler<DeletarSeguroProdutoCommand>
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public DeletarSeguroProdutoCommandHandler(IProdutoRepositorio produtoRepositorio)
    {
        _produtoRepositorio = produtoRepositorio;
    }

    public async Task Handle(DeletarSeguroProdutoCommand comando, CancellationToken cancellationToken)
    {
        var produtoExistente = await _produtoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
        if (produtoExistente == null)
            throw new Exception(MensagensSistema.Produto.NaoEncontrado);

        await _produtoRepositorio.DeletarSeguroAsync(comando.Id, comando.ContaId);
    }
}
