using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class DefinirPrecoCommand : IRequest
{
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProdutoVariacaoId { get; set; }
    public Guid ContextoPrecoId { get; set; }
    public decimal Valor { get; set; }
    public decimal PercentualComissao { get; set; } = 0;
}

public class DefinirPrecoCommandHandler : IRequestHandler<DefinirPrecoCommand>
{
    private readonly IContextoPrecoRepositorio _repo;

    public DefinirPrecoCommandHandler(IContextoPrecoRepositorio repo) => _repo = repo;

    public async Task Handle(DefinirPrecoCommand comando, CancellationToken cancellationToken)
    {
        var preco = new Preco
        {
            ContaId = comando.ContaId,
            EmpresaId = comando.EmpresaId,
            ProdutoVariacaoId = comando.ProdutoVariacaoId,
            ContextoPrecoId = comando.ContextoPrecoId,
            Valor = comando.Valor,
            PercentualComissao = comando.PercentualComissao
        };
        await _repo.DefinirPrecoAsync(preco);
    }
}



