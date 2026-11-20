using FluentValidation;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class AtualizarProdutoCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string? Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? DescricaoComplementar { get; set; }
    public string TipoProduto { get; set; } = "produto";
    public string? Grupo { get; set; }
    public string? Marca { get; set; }
    public string UnidadeMedida { get; set; } = "UN";
    public string? Ncm { get; set; }
    public string? Cest { get; set; }
    public string? Observacao { get; set; }
    public bool Ativo { get; set; }
    
    public List<VariacaoDto> Variacoes { get; set; } = new();
}

public class AtualizarProdutoCommandValidador : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoCommandValidador()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do produto é obrigatório.");
        RuleFor(x => x.ContaId).NotEmpty().WithMessage("A conta é obrigatória.");
        RuleFor(x => x.Descricao).NotEmpty().WithMessage("A descrição do produto é obrigatória.");
        
        RuleForEach(x => x.Variacoes).ChildRules(variacao =>
        {
            variacao.RuleFor(v => v.Descricao).NotEmpty().WithMessage("A descrição da variação é obrigatória.");
        });
    }
}

public class AtualizarProdutoCommandHandler : IRequestHandler<AtualizarProdutoCommand>
{
    private readonly IProdutoRepositorio _produtoRepositorio;

    public AtualizarProdutoCommandHandler(IProdutoRepositorio produtoRepositorio)
    {
        _produtoRepositorio = produtoRepositorio;
    }

    public async Task Handle(AtualizarProdutoCommand comando, CancellationToken cancellationToken)
    {
        var produtoExistente = await _produtoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
        if (produtoExistente == null)
            throw new Exception(MensagensSistema.Produto.NaoEncontrado);

        var produto = new Produto
        {
            Id = comando.Id,
            ContaId = comando.ContaId,
            Codigo = comando.Codigo,
            Descricao = comando.Descricao,
            DescricaoComplementar = comando.DescricaoComplementar,
            TipoProduto = comando.TipoProduto,
            Grupo = comando.Grupo,
            Marca = comando.Marca,
            UnidadeMedida = comando.UnidadeMedida,
            Ncm = comando.Ncm,
            Cest = comando.Cest,
            Observacao = comando.Observacao,
            Ativo = comando.Ativo
        };

        var variacoes = comando.Variacoes.Select(v => new ProdutoVariacao
        {
            Id = v.Id == Guid.Empty ? Guid.NewGuid() : v.Id,
            ProdutoId = comando.Id,
            ContaId = comando.ContaId,
            Descricao = v.Descricao,
            CodigoBarras = v.CodigoBarras,
            CodigoSku = v.CodigoSku,
            PrecoVenda = v.PrecoVenda,
            Custo = v.Custo,
            EstoqueAtual = v.EstoqueAtual,
            EstoqueMinimo = v.EstoqueMinimo,
            Ativo = true,
            DataInclusao = DateTime.UtcNow 
        }).ToList();

        if (!variacoes.Any())
        {
            variacoes.Add(new ProdutoVariacao
            {
                Id = Guid.NewGuid(),
                ProdutoId = comando.Id,
                ContaId = comando.ContaId,
                Descricao = "Padrão",
                PrecoVenda = 0,
                Custo = 0,
                Ativo = true,
                DataInclusao = DateTime.UtcNow
            });
        }

        await _produtoRepositorio.AtualizarProdutoComVariacoesAsync(produto, variacoes);
    }
}
