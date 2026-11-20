using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Precos;

public class ContextoPrecoRepositorio : RepositorioBase, IContextoPrecoRepositorio
{
    public ContextoPrecoRepositorio(string connectionString) : base(connectionString) { }

    public async Task<Guid> CadastrarAsync(ContextoPreco contexto)
    {
        return await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sql = """
                INSERT INTO erp_contextos_preco
                    (id, conta_id, empresa_id, nome, descricao, is_padrao, ativo, ordem)
                VALUES
                    (@Id, @ContaId, @EmpresaId, @Nome, @Descricao, @EhPadrao, @Ativo, @Ordem)
                RETURNING id
                """;

            contexto.Id = contexto.Id == Guid.Empty ? Guid.NewGuid() : contexto.Id;
            await conexao.ExecuteAsync(sql, new
            {
                contexto.Id,
                contexto.ContaId,
                contexto.EmpresaId,
                contexto.Nome,
                contexto.Descricao,
                contexto.EhPadrao,
                contexto.Ativo,
                contexto.Ordem
            });
            return contexto.Id;
        });
    }

    public async Task AtualizarAsync(ContextoPreco contexto)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sql = """
                UPDATE erp_contextos_preco
                SET nome          = @Nome,
                    descricao     = @Descricao,
                    is_padrao     = @EhPadrao,
                    ativo         = @Ativo,
                    ordem         = @Ordem,
                    atualizado_em = NOW()
                WHERE id = @Id AND conta_id = @ContaId
                """;

            await conexao.ExecuteAsync(sql, new
            {
                contexto.Id,
                contexto.ContaId,
                contexto.Nome,
                contexto.Descricao,
                contexto.EhPadrao,
                contexto.Ativo,
                contexto.Ordem
            });
        });
    }

    public async Task DeletarSeguroAsync(Guid id, Guid contaId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "UPDATE erp_contextos_preco SET deleted_at = NOW() WHERE id = @Id AND conta_id = @ContaId",
                new { Id = id, ContaId = contaId });
        });
    }

    public async Task<ContextoPreco?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryFirstOrDefaultAsync<ContextoPreco>("""
            SELECT id AS Id, conta_id AS ContaId, empresa_id AS EmpresaId, nome AS Nome,
                   descricao AS Descricao, is_padrao AS EhPadrao, ativo AS Ativo, ordem AS Ordem,
                   criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_contextos_preco
            WHERE id = @Id AND conta_id = @ContaId AND deleted_at IS NULL
            """, new { Id = id, ContaId = contaId });
    }

    public async Task<IEnumerable<ContextoPreco>> ListarPorContaEEmpresaAsync(Guid contaId, Guid empresaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<ContextoPreco>("""
            SELECT id AS Id, conta_id AS ContaId, empresa_id AS EmpresaId, nome AS Nome,
                   descricao AS Descricao, is_padrao AS EhPadrao, ativo AS Ativo, ordem AS Ordem,
                   criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_contextos_preco
            WHERE conta_id = @ContaId AND empresa_id = @EmpresaId AND deleted_at IS NULL
            ORDER BY is_padrao DESC, ordem, nome
            """, new { ContaId = contaId, EmpresaId = empresaId });
    }

    public async Task DefinirPrecoAsync(Preco preco)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sql = """
                INSERT INTO erp_precos
                    (id, conta_id, empresa_id, produto_variacao_id, contexto_preco_id, valor, percentual_comissao, ativo)
                VALUES
                    (@Id, @ContaId, @EmpresaId, @ProdutoVariacaoId, @ContextoPrecoId, @Valor, @PercentualComissao, TRUE)
                ON CONFLICT (produto_variacao_id, contexto_preco_id)
                DO UPDATE SET
                    valor               = EXCLUDED.valor,
                    percentual_comissao = EXCLUDED.percentual_comissao,
                    atualizado_em       = NOW()
                """;

            preco.Id = preco.Id == Guid.Empty ? Guid.NewGuid() : preco.Id;
            await conexao.ExecuteAsync(sql, new
            {
                preco.Id,
                preco.ContaId,
                preco.EmpresaId,
                preco.ProdutoVariacaoId,
                preco.ContextoPrecoId,
                preco.Valor,
                preco.PercentualComissao
            });
        });
    }

    public async Task<IEnumerable<Preco>> ListarPrecosPorVariacaoAsync(Guid produtoVariacaoId, Guid contaId)
    {
        await using var conn = CriarConexao();
        await conn.OpenAsync();

        return await conn.QueryAsync<Preco>("""
            SELECT p.id AS Id, p.conta_id AS ContaId, p.empresa_id AS EmpresaId,
                   p.produto_variacao_id AS ProdutoVariacaoId, p.contexto_preco_id AS ContextoPrecoId,
                   p.valor AS Valor, p.percentual_comissao AS PercentualComissao, p.ativo AS Ativo,
                   p.criado_em AS DataInclusao, p.atualizado_em AS DataUltimaAtualizacao,
                   cp.nome AS ContextoNome
            FROM erp_precos p
            INNER JOIN erp_contextos_preco cp ON p.contexto_preco_id = cp.id
            WHERE p.produto_variacao_id = @ProdutoVariacaoId AND p.conta_id = @ContaId AND cp.deleted_at IS NULL
            ORDER BY cp.is_padrao DESC, cp.ordem, cp.nome
            """, new { ProdutoVariacaoId = produtoVariacaoId, ContaId = contaId });
    }

    public async Task<IEnumerable<Preco>> ListarPrecosPorContextoAsync(Guid contextoPrecoId, Guid contaId)
    {
        await using var conn = CriarConexao();
        await conn.OpenAsync();

        return await conn.QueryAsync<Preco>("""
            SELECT p.id AS Id, p.conta_id AS ContaId, p.empresa_id AS EmpresaId,
                   p.produto_variacao_id AS ProdutoVariacaoId, p.contexto_preco_id AS ContextoPrecoId,
                   p.valor AS Valor, p.percentual_comissao AS PercentualComissao, p.ativo AS Ativo,
                   p.criado_em AS DataInclusao, p.atualizado_em AS DataUltimaAtualizacao,
                   cp.nome AS ContextoNome
            FROM erp_precos p
            INNER JOIN erp_contextos_preco cp ON p.contexto_preco_id = cp.id
            WHERE p.contexto_preco_id = @ContextoPrecoId AND p.conta_id = @ContaId AND cp.deleted_at IS NULL
            ORDER BY p.valor
            """, new { ContextoPrecoId = contextoPrecoId, ContaId = contaId });
    }
}




