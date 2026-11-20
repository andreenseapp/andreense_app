using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;
using ProdutoEntidade = ModuloMonolitico.Dominio.Entidades.Produto;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Produto;

public class ProdutoRepositorio : RepositorioBase, IProdutoRepositorio, IProdutoConsultas
{
    public ProdutoRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task CadastrarProdutoComVariacoesAsync(ProdutoEntidade produto, IEnumerable<ProdutoVariacao> variacoes)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sqlProduto = """
                INSERT INTO erp_produtos
                    (id, conta_id, codigo, descricao, descricao_compl, tipo_item, grupo, marca, unidade_medida, ncm, cest, observacao, ativo)
                VALUES
                    (@Id, @ContaId, @Codigo, @Descricao, @DescricaoComplementar, @TipoProduto, @Grupo, @Marca, @UnidadeMedida, @Ncm, @Cest, @Observacao, @Ativo)
                """;

            await conexao.ExecuteAsync(sqlProduto, new
            {
                produto.Id,
                produto.ContaId,
                produto.Codigo,
                produto.Descricao,
                produto.DescricaoComplementar,
                produto.TipoProduto,
                produto.Grupo,
                produto.Marca,
                produto.UnidadeMedida,
                produto.Ncm,
                produto.Cest,
                produto.Observacao,
                produto.Ativo
            });

            if (variacoes != null && variacoes.Any())
            {
                const string sqlVariacao = """
                    INSERT INTO erp_produtos_variacoes
                        (id, produto_id, conta_id, descricao, codigo_barras, sku, preco_venda, custo, estoque_atual, estoque_minimo, ativo, duracao_minutos, cor_hex)
                    VALUES
                        (@Id, @ProdutoId, @ContaId, @Descricao, @CodigoBarras, @CodigoSku, @PrecoVenda, @Custo, @EstoqueAtual, @EstoqueMinimo, @Ativo, @DuracaoMinutos, @CorHexadecimal)
                    """;

                foreach (var v in variacoes)
                {
                    await conexao.ExecuteAsync(sqlVariacao, new
                    {
                        v.Id,
                        v.ProdutoId,
                        v.ContaId,
                        v.Descricao,
                        v.CodigoBarras,
                        v.CodigoSku,
                        v.PrecoVenda,
                        v.Custo,
                        v.EstoqueAtual,
                        v.EstoqueMinimo,
                        v.Ativo,
                        v.DuracaoMinutos,
                        v.CorHexadecimal
                    });
                }
            }
        });
    }

    public async Task AtualizarProdutoComVariacoesAsync(ProdutoEntidade produto, IEnumerable<ProdutoVariacao> variacoes)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sqlProduto = """
                UPDATE erp_produtos
                SET codigo = @Codigo,
                    descricao = @Descricao,
                    descricao_compl = @DescricaoComplementar,
                    tipo_item = @TipoProduto,
                    grupo = @Grupo,
                    marca = @Marca,
                    unidade_medida = @UnidadeMedida,
                    ncm = @Ncm,
                    cest = @Cest,
                    observacao = @Observacao,
                    ativo = @Ativo,
                    atualizado_em = NOW()
                WHERE id = @Id AND conta_id = @ContaId
                """;

            await conexao.ExecuteAsync(sqlProduto, new
            {
                produto.Id,
                produto.ContaId,
                produto.Codigo,
                produto.Descricao,
                produto.DescricaoComplementar,
                produto.TipoProduto,
                produto.Grupo,
                produto.Marca,
                produto.UnidadeMedida,
                produto.Ncm,
                produto.Cest,
                produto.Observacao,
                produto.Ativo
            });

            await conexao.ExecuteAsync("DELETE FROM erp_produtos_variacoes WHERE produto_id = @Id AND conta_id = @ContaId",
                new { Id = produto.Id, ContaId = produto.ContaId });

            if (variacoes != null && variacoes.Any())
            {
                const string sqlVariacao = """
                    INSERT INTO erp_produtos_variacoes
                        (id, produto_id, conta_id, descricao, codigo_barras, sku, preco_venda, custo, estoque_atual, estoque_minimo, ativo, duracao_minutos, cor_hex)
                    VALUES
                        (@Id, @ProdutoId, @ContaId, @Descricao, @CodigoBarras, @CodigoSku, @PrecoVenda, @Custo, @EstoqueAtual, @EstoqueMinimo, @Ativo, @DuracaoMinutos, @CorHexadecimal)
                    """;

                foreach (var v in variacoes)
                {
                    await conexao.ExecuteAsync(sqlVariacao, new
                    {
                        v.Id,
                        v.ProdutoId,
                        v.ContaId,
                        v.Descricao,
                        v.CodigoBarras,
                        v.CodigoSku,
                        v.PrecoVenda,
                        v.Custo,
                        v.EstoqueAtual,
                        v.EstoqueMinimo,
                        v.Ativo,
                        v.DuracaoMinutos,
                        v.CorHexadecimal
                    });
                }
            }
        });
    }

    public async Task<IEnumerable<ProdutoEntidade>> ListarPorContaIdAsync(Guid contaId)
    {
        const string sqlProduto = """
            SELECT 
                id AS Id, conta_id AS ContaId, codigo AS Codigo, descricao AS Descricao, 
                descricao_compl AS DescricaoComplementar, tipo_item AS TipoProduto, grupo AS Grupo, 
                marca AS Marca, unidade_medida AS UnidadeMedida, ncm AS Ncm, cest AS Cest, 
                observacao AS Observacao, ativo AS Ativo, 
                criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_produtos
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            ORDER BY descricao
            """;

        const string sqlVariacoes = """
            SELECT 
                id AS Id, produto_id AS ProdutoId, conta_id AS ContaId, descricao AS Descricao, 
                codigo_barras AS CodigoBarras, sku AS CodigoSku, preco_venda AS PrecoVenda, custo AS Custo, 
                estoque_atual AS EstoqueAtual, estoque_minimo AS EstoqueMinimo, ativo AS Ativo, 
                criado_em AS DataInclusao, duracao_minutos AS DuracaoMinutos, cor_hex AS CorHexadecimal
            FROM erp_produtos_variacoes
            WHERE conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var produtos = (await conexao.QueryAsync<ProdutoEntidade>(sqlProduto, new { ContaId = contaId })).ToList();
        var variacoesBd = (await conexao.QueryAsync<ProdutoVariacao>(sqlVariacoes, new { ContaId = contaId })).ToList();

        foreach (var p in produtos)
        {
            p.Variacoes = variacoesBd.Where(v => v.ProdutoId == p.Id).ToList();
        }

        return produtos;
    }

    public async Task<ResultadoPaginado<ProdutoEntidade>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? tipo = null, string? ordenarPor = null, string? ordenarDirecao = null)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;
        var direcaoOrdenacao = string.Equals(ordenarDirecao, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var clausulaOrdem = ordenarPor?.ToLower() switch
        {
            "codigo" => $"p.codigo {direcaoOrdenacao}",
            "tipo" => $"p.tipo_item {direcaoOrdenacao}",
            "preco" => $"PrecoMin {direcaoOrdenacao}",
            "status" => $"p.ativo {direcaoOrdenacao}",
            _ => $"p.descricao {direcaoOrdenacao}"
        };

        var where = "p.conta_id = @ContaId AND p.deleted_at IS NULL";
        if (!string.IsNullOrWhiteSpace(busca))
            where += " AND (p.descricao ILIKE @Busca OR p.codigo ILIKE @Busca)";
        if (!string.IsNullOrWhiteSpace(tipo))
            where += " AND p.tipo_item = @Tipo";

        var sqlBase = $"""
            FROM erp_produtos p
            LEFT JOIN LATERAL (
                SELECT MIN(v.preco_venda) AS PrecoMin
                FROM erp_produtos_variacoes v
                WHERE v.produto_id = p.id AND v.conta_id = p.conta_id
            ) precos ON true
            WHERE {where}
            """;

        var sqlCount = $"SELECT COUNT(1) {sqlBase}";
        var sqlIds = $"""
            SELECT p.id
            {sqlBase}
            ORDER BY {clausulaOrdem}
            LIMIT @TamanhoPagina OFFSET @Deslocamento
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var parametros = new
        {
            ContaId = contaId,
            Busca = $"%{busca}%",
            Tipo = tipo,
            TamanhoPagina = tamanhoPagina,
            Deslocamento = deslocamento
        };

        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var ids = (await conexao.QueryAsync<Guid>(sqlIds, parametros)).ToList();

        if (ids.Count == 0)
            return new ResultadoPaginado<ProdutoEntidade>(Enumerable.Empty<ProdutoEntidade>(), total, pagina, tamanhoPagina);

        const string sqlProdutos = """
            SELECT 
                id AS Id, conta_id AS ContaId, codigo AS Codigo, descricao AS Descricao, 
                descricao_compl AS DescricaoComplementar, tipo_item AS TipoProduto, grupo AS Grupo, 
                marca AS Marca, unidade_medida AS UnidadeMedida, ncm AS Ncm, cest AS Cest, 
                observacao AS Observacao, ativo AS Ativo, 
                criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_produtos
            WHERE id = ANY(@Ids) AND conta_id = @ContaId
            """;

        const string sqlVariacoes = """
            SELECT 
                id AS Id, produto_id AS ProdutoId, conta_id AS ContaId, descricao AS Descricao, 
                codigo_barras AS CodigoBarras, sku AS CodigoSku, preco_venda AS PrecoVenda, custo AS Custo, 
                estoque_atual AS EstoqueAtual, estoque_minimo AS EstoqueMinimo, ativo AS Ativo, 
                criado_em AS DataInclusao, duracao_minutos AS DuracaoMinutos, cor_hex AS CorHexadecimal
            FROM erp_produtos_variacoes
            WHERE produto_id = ANY(@Ids) AND conta_id = @ContaId
            """;

        var produtosDict = (await conexao.QueryAsync<ProdutoEntidade>(sqlProdutos, new { Ids = ids.ToArray(), ContaId = contaId }))
            .ToDictionary(p => p.Id);

        var variacoesBd = (await conexao.QueryAsync<ProdutoVariacao>(sqlVariacoes, new { Ids = ids.ToArray(), ContaId = contaId })).ToList();
        foreach (var v in variacoesBd)
        {
            if (produtosDict.TryGetValue(v.ProdutoId, out var produto))
                produto.Variacoes.Add(v);
        }

        var ordenados = ids.Select(id => produtosDict[id]).ToList();
        return new ResultadoPaginado<ProdutoEntidade>(ordenados, total, pagina, tamanhoPagina);
    }

    public async Task<IEnumerable<ResultadoSeletorProduto>> BuscarParaSeletorAsync(Guid contaId, string? termo, bool apenasServicos)
    {
        var sql = """
            SELECT 
                v.id AS Id,
                p.tipo_item AS TipoProduto,
                p.descricao AS NomeProduto,
                v.descricao AS NomeVariacao,
                v.sku AS CodigoSku,
                p.codigo AS CodigoProduto,
                v.preco_venda AS PrecoVenda,
                v.duracao_minutos AS DuracaoMinutos,
                v.cor_hex AS CorHexadecimal
            FROM erp_produtos_variacoes v
            INNER JOIN erp_produtos p ON p.id = v.produto_id
            WHERE p.conta_id = @ContaId
              AND p.ativo = true 
              AND v.ativo = true
            """;

        var p = new DynamicParameters();
        p.Add("@ContaId", contaId);

        if (apenasServicos)
        {
            sql += " AND p.tipo_item = 'servico'";
        }

        if (!string.IsNullOrWhiteSpace(termo))
        {
            sql += " AND (p.descricao ILIKE @Termo OR v.descricao ILIKE @Termo OR v.sku ILIKE @Termo OR p.codigo ILIKE @Termo)";
            p.Add("@Termo", $"%{termo}%");
        }

        sql += " ORDER BY p.descricao, v.descricao LIMIT 50";

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ResultadoSeletorProduto>(sql, p);
    }

    public async Task<ProdutoEntidade?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        const string sqlProduto = """
            SELECT 
                id AS Id, conta_id AS ContaId, codigo AS Codigo, descricao AS Descricao, 
                descricao_compl AS DescricaoComplementar, tipo_item AS TipoProduto, grupo AS Grupo, 
                marca AS Marca, unidade_medida AS UnidadeMedida, ncm AS Ncm, cest AS Cest, 
                observacao AS Observacao, ativo AS Ativo, 
                criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_produtos
            WHERE id = @Id AND conta_id = @ContaId AND deleted_at IS NULL
            """;

        const string sqlVariacoes = """
            SELECT 
                id AS Id, produto_id AS ProdutoId, conta_id AS ContaId, descricao AS Descricao, 
                codigo_barras AS CodigoBarras, sku AS CodigoSku, preco_venda AS PrecoVenda, custo AS Custo, 
                estoque_atual AS EstoqueAtual, estoque_minimo AS EstoqueMinimo, ativo AS Ativo, 
                criado_em AS DataInclusao, duracao_minutos AS DuracaoMinutos, cor_hex AS CorHexadecimal
            FROM erp_produtos_variacoes
            WHERE produto_id = @Id AND conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var produto = await conexao.QueryFirstOrDefaultAsync<ProdutoEntidade>(sqlProduto, new { Id = id, ContaId = contaId });
        if (produto != null)
        {
            var variacoesBd = await conexao.QueryAsync<ProdutoVariacao>(sqlVariacoes, new { Id = id, ContaId = contaId });
            produto.Variacoes = variacoesBd.ToList();
        }

        return produto;
    }

    public async Task DeletarSeguroAsync(Guid id, Guid contaId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("UPDATE erp_produtos SET deleted_at = NOW() WHERE id = @Id AND conta_id = @ContaId",
                new { Id = id, ContaId = contaId });
        });
    }

    public async Task<IEnumerable<string>> ListarCategoriasAtivasAsync(Guid contaId)
    {
        const string sql = "SELECT nome FROM erp_produto_categorias WHERE conta_id = @ContaId AND ativo = true ORDER BY nome";
        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<string>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarCategoriaAsync(Guid contaId, string nome)
    {
        const string sql = "INSERT INTO erp_produto_categorias (conta_id, nome) VALUES (@ContaId, @Nome)";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { ContaId = contaId, Nome = nome });
    }

    public async Task<IEnumerable<string>> ListarMarcasAtivasAsync(Guid contaId)
    {
        const string sql = "SELECT nome FROM erp_marcas WHERE conta_id = @ContaId AND ativo = true ORDER BY nome";
        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<string>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarMarcaAsync(Guid contaId, string nome)
    {
        const string sql = "INSERT INTO erp_marcas (conta_id, nome) VALUES (@ContaId, @Nome)";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { ContaId = contaId, Nome = nome });
    }

    public async Task<IEnumerable<(string Sigla, string Nome)>> ListarUnidadesMedidaAtivasAsync(Guid contaId)
    {
        const string sql = "SELECT sigla AS Sigla, nome AS Nome FROM erp_unidades_medida WHERE (conta_id = @ContaId OR conta_id IS NULL) AND ativo = true ORDER BY sigla";
        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<(string, string)>(sql, new { ContaId = contaId });
    }

    public async Task<bool> UnidadeMedidaExisteAsync(Guid contaId, string sigla)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM erp_unidades_medida WHERE UPPER(sigla) = @Sigla AND (conta_id = @ContaId OR conta_id IS NULL))";
        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<bool>(sql, new { Sigla = sigla, ContaId = contaId });
    }

    public async Task CadastrarUnidadeMedidaAsync(Guid contaId, string sigla, string nome)
    {
        const string sql = "INSERT INTO erp_unidades_medida (sigla, conta_id, nome, quantidade_pecas, libera_qtd_conferencia, ativo) VALUES (@Sigla, @ContaId, @Nome, 1, true, true)";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Sigla = sigla, ContaId = contaId, Nome = nome });
    }
}
