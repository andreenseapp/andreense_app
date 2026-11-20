using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Aplicacao.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarProdutosVariacoesQuery(
    Guid ContaId,
    string? Termo,
    bool ApenasServicos = false
) : IRequest<IEnumerable<ResultadoSeletorProduto>>;

public class ListarProdutosVariacoesQueryHandler : IRequestHandler<ListarProdutosVariacoesQuery, IEnumerable<ResultadoSeletorProduto>>
{
    private readonly IProdutoConsultas _produtoQueries;

    public ListarProdutosVariacoesQueryHandler(IProdutoConsultas produtoQueries)
    {
        _produtoQueries = produtoQueries;
    }

    public async Task<IEnumerable<ResultadoSeletorProduto>> Handle(ListarProdutosVariacoesQuery comando, CancellationToken cancellationToken)
    {
        return await _produtoQueries.BuscarParaSeletorAsync(comando.ContaId, comando.Termo, comando.ApenasServicos);
    }
}


