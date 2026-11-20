using MediatR;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class CadastrarSaldoEstoqueCommand : IRequest<EstoqueSaldo>
{
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProdutoVariacaoId { get; set; }
    public Guid LocalEstoqueId { get; set; }
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
    public DateTime? DataFabricacao { get; set; }
    public decimal Quantidade { get; set; }
    public string? Motivo { get; set; } = "Entrada de estoque";
    public Guid? UsuarioResponsavelId { get; set; }
}

public class CadastrarSaldoEstoqueCommandHandler : IRequestHandler<CadastrarSaldoEstoqueCommand, EstoqueSaldo>
{
    private readonly IEstoqueRepositorio _repo;

    public CadastrarSaldoEstoqueCommandHandler(IEstoqueRepositorio repo) => _repo = repo;

    public async Task<EstoqueSaldo> Handle(CadastrarSaldoEstoqueCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Quantidade <= 0)
            throw new InvalidOperationException(MensagensSistema.Estoque.QuantidadeEntradaInvalida);

        var saldo = new EstoqueSaldo
        {
            ContaId = comando.ContaId,
            EmpresaId = comando.EmpresaId,
            ProdutoVariacaoId = comando.ProdutoVariacaoId,
            LocalEstoqueId = comando.LocalEstoqueId,
            Lote = comando.Lote,
            DataValidade = comando.DataValidade,
            DataFabricacao = comando.DataFabricacao,
            QuantidadeAtual = comando.Quantidade,
            Situacao = "disponivel"
        };

        var movimentacao = new EstoqueMovimentacao
        {
            ContaId = comando.ContaId,
            EmpresaId = comando.EmpresaId,
            Tipo = "ENTRADA",
            Quantidade = comando.Quantidade,
            Motivo = comando.Motivo,
            Lote = comando.Lote,
            DataValidade = comando.DataValidade,
            UsuarioResponsavelId = comando.UsuarioResponsavelId
        };

        return await _repo.CadastrarSaldoAsync(saldo, movimentacao);
    }
}
