using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Estoque;

public class EstoqueRepositorio : RepositorioBase, IEstoqueRepositorio
{
    public EstoqueRepositorio(string connectionString) : base(connectionString) { }



    public async Task<Guid> CadastrarAreaAsync(AreaEstoque area)
    {
        return await ExecutarEmTransacaoAsync(async conexao =>
        {
            area.Id = area.Id == Guid.Empty ? Guid.NewGuid() : area.Id;
            await conexao.ExecuteAsync("""
                INSERT INTO erp_areas_estoque (id, conta_id, empresa_id, nome, descricao, ativo)
                VALUES (@Id, @ContaId, @EmpresaId, @Nome, @Descricao, @Ativo)
                """, new { area.Id, area.ContaId, area.EmpresaId, area.Nome, area.Descricao, area.Ativo });
            return area.Id;
        });
    }

    public async Task AtualizarAreaAsync(AreaEstoque area)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("""
                UPDATE erp_areas_estoque
                SET nome = @Nome, descricao = @Descricao, ativo = @Ativo, atualizado_em = NOW()
                WHERE id = @Id AND conta_id = @ContaId
                """, new { area.Id, area.ContaId, area.Nome, area.Descricao, area.Ativo });
        });
    }

    public async Task DeletarFisicoAreaAsync(Guid id, Guid contaId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "DELETE FROM erp_areas_estoque WHERE id = @Id AND conta_id = @ContaId",
                new { Id = id, ContaId = contaId });
        });
    }

    public async Task<IEnumerable<AreaEstoque>> ListarAreasPorContaEEmpresaAsync(Guid contaId, Guid empresaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<AreaEstoque>("""
            SELECT id AS Id, conta_id AS ContaId, empresa_id AS EmpresaId, nome AS Nome,
                   descricao AS Descricao, ativo AS Ativo,
                   criado_em AS DataInclusao, atualizado_em AS DataUltimaAtualizacao
            FROM erp_areas_estoque
            WHERE conta_id = @ContaId AND empresa_id = @EmpresaId
            ORDER BY nome
            """, new { ContaId = contaId, EmpresaId = empresaId });
    }



    public async Task<Guid> CadastrarLocalAsync(LocalEstoque local)
    {
        return await ExecutarEmTransacaoAsync(async conexao =>
        {
            local.Id = local.Id == Guid.Empty ? Guid.NewGuid() : local.Id;
            await conexao.ExecuteAsync("""
                INSERT INTO erp_locais_estoque
                    (id, conta_id, empresa_id, area_estoque_id, nome, codigo_visual, tipo, prioridade_saida, ordem, ativo)
                VALUES
                    (@Id, @ContaId, @EmpresaId, @AreaEstoqueId, @Nome, @CodigoVisual, @Tipo, @PrioridadeSaida, @Ordem, @Ativo)
                """, new
            {
                local.Id, local.ContaId, local.EmpresaId, local.AreaEstoqueId,
                local.Nome, local.CodigoVisual, local.Tipo, local.PrioridadeSaida, local.Ordem, local.Ativo
            });
            return local.Id;
        });
    }

    public async Task AtualizarLocalAsync(LocalEstoque local)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("""
                UPDATE erp_locais_estoque
                SET area_estoque_id = @AreaEstoqueId, nome = @Nome, codigo_visual = @CodigoVisual,
                    tipo = @Tipo, prioridade_saida = @PrioridadeSaida, ordem = @Ordem,
                    ativo = @Ativo, atualizado_em = NOW()
                WHERE id = @Id AND conta_id = @ContaId
                """, new
            {
                local.Id, local.ContaId, local.AreaEstoqueId, local.Nome,
                local.CodigoVisual, local.Tipo, local.PrioridadeSaida, local.Ordem, local.Ativo
            });
        });
    }

    public async Task DeletarFisicoLocalAsync(Guid id, Guid contaId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "DELETE FROM erp_locais_estoque WHERE id = @Id AND conta_id = @ContaId",
                new { Id = id, ContaId = contaId });
        });
    }

    public async Task<IEnumerable<LocalEstoque>> ListarLocaisPorAreaAsync(Guid areaEstoqueId, Guid contaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<LocalEstoque>("""
            SELECT l.id AS Id, l.conta_id AS ContaId, l.empresa_id AS EmpresaId,
                   l.area_estoque_id AS AreaEstoqueId, l.nome AS Nome,
                   l.codigo_visual AS CodigoVisual, l.tipo AS Tipo,
                   l.prioridade_saida AS PrioridadeSaida, l.ordem AS Ordem, l.ativo AS Ativo,
                   l.criado_em AS DataInclusao, l.atualizado_em AS DataUltimaAtualizacao,
                   a.nome AS AreaNome
            FROM erp_locais_estoque l
            INNER JOIN erp_areas_estoque a ON l.area_estoque_id = a.id
            WHERE l.area_estoque_id = @AreaEstoqueId AND l.conta_id = @ContaId
            ORDER BY l.ordem, l.nome
            """, new { AreaEstoqueId = areaEstoqueId, ContaId = contaId });
    }

    public async Task<IEnumerable<LocalEstoque>> ListarLocaisPorContaEEmpresaAsync(Guid contaId, Guid empresaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<LocalEstoque>("""
            SELECT l.id AS Id, l.conta_id AS ContaId, l.empresa_id AS EmpresaId,
                   l.area_estoque_id AS AreaEstoqueId, l.nome AS Nome,
                   l.codigo_visual AS CodigoVisual, l.tipo AS Tipo,
                   l.prioridade_saida AS PrioridadeSaida, l.ordem AS Ordem, l.ativo AS Ativo,
                   l.criado_em AS DataInclusao, l.atualizado_em AS DataUltimaAtualizacao,
                   a.nome AS AreaNome
            FROM erp_locais_estoque l
            INNER JOIN erp_areas_estoque a ON l.area_estoque_id = a.id
            WHERE l.conta_id = @ContaId AND l.empresa_id = @EmpresaId
            ORDER BY a.nome, l.ordem, l.nome
            """, new { ContaId = contaId, EmpresaId = empresaId });
    }



    public async Task<EstoqueSaldo> CadastrarSaldoAsync(EstoqueSaldo saldo, EstoqueMovimentacao movimentacao)
    {
        return await ExecutarEmTransacaoAsync(async conexao =>
        {
            var saldoExistente = await conexao.QueryFirstOrDefaultAsync<EstoqueSaldo>("""
                SELECT id AS Id, quantidade_atual AS QuantidadeAtual
                FROM erp_estoque_saldo
                WHERE produto_variacao_id = @ProdutoVariacaoId
                  AND local_estoque_id    = @LocalEstoqueId
                  AND (lote = @Lote OR (lote IS NULL AND @Lote IS NULL))
                  AND (data_validade = @DataValidade OR (data_validade IS NULL AND @DataValidade IS NULL))
                  AND (data_fabricacao = @DataFabricacao OR (data_fabricacao IS NULL AND @DataFabricacao IS NULL))
                LIMIT 1
                """, new
            {
                saldo.ProdutoVariacaoId,
                saldo.LocalEstoqueId,
                saldo.Lote,
                saldo.DataValidade,
                saldo.DataFabricacao
            });

            Guid saldoId;

            if (saldoExistente != null)
            {
                saldoId = saldoExistente.Id;
                var novaQtd = saldoExistente.QuantidadeAtual + saldo.QuantidadeAtual;
                await conexao.ExecuteAsync("""
                    UPDATE erp_estoque_saldo
                    SET quantidade_atual = @Qtd, atualizado_em = NOW()
                    WHERE id = @Id
                    """, new { Id = saldoId, Qtd = novaQtd });
            }
            else
            {
                saldoId = Guid.NewGuid();
                await conexao.ExecuteAsync("""
                    INSERT INTO erp_estoque_saldo
                        (id, conta_id, empresa_id, produto_variacao_id, local_estoque_id,
                         lote, data_validade, data_fabricacao, quantidade_atual, status, data_entrada)
                    VALUES
                        (@Id, @ContaId, @EmpresaId, @ProdutoVariacaoId, @LocalEstoqueId,
                         @Lote, @DataValidade, @DataFabricacao, @QuantidadeAtual, 'disponivel', NOW())
                    """, new
                {
                    Id = saldoId,
                    saldo.ContaId, saldo.EmpresaId, saldo.ProdutoVariacaoId, saldo.LocalEstoqueId,
                    saldo.Lote, saldo.DataValidade, saldo.DataFabricacao, saldo.QuantidadeAtual
                });
            }

            movimentacao.Id = Guid.NewGuid();
            movimentacao.EstoqueSaldoId = saldoId;
            movimentacao.Tipo = "ENTRADA";
            await conexao.ExecuteAsync("""
                INSERT INTO erp_estoque_movimentacoes
                    (id, conta_id, empresa_id, estoque_saldo_id, tipo, quantidade, motivo,
                     lote, data_validade, usuario_responsavel_id)
                VALUES
                    (@Id, @ContaId, @EmpresaId, @EstoqueSaldoId, @Tipo, @Quantidade, @Motivo,
                     @Lote, @DataValidade, @UsuarioResponsavelId)
                """, new
            {
                movimentacao.Id, movimentacao.ContaId, movimentacao.EmpresaId, movimentacao.EstoqueSaldoId,
                movimentacao.Tipo, movimentacao.Quantidade, movimentacao.Motivo,
                movimentacao.Lote, movimentacao.DataValidade, movimentacao.UsuarioResponsavelId
            });

            var resultado = await conexao.QueryFirstAsync<EstoqueSaldo>("""
                SELECT s.id AS Id, s.conta_id AS ContaId, s.empresa_id AS EmpresaId,
                       s.produto_variacao_id AS ProdutoVariacaoId, s.local_estoque_id AS LocalEstoqueId,
                       s.lote AS Lote, s.data_validade AS DataValidade, s.data_fabricacao AS DataFabricacao,
                       s.quantidade_atual AS QuantidadeAtual, s.status AS Situacao,
                       s.data_entrada AS DataEntrada, s.criado_em AS DataInclusao, s.atualizado_em AS DataUltimaAtualizacao,
                       l.nome AS LocalNome, a.nome AS AreaNome
                FROM erp_estoque_saldo s
                INNER JOIN erp_locais_estoque l ON s.local_estoque_id = l.id
                INNER JOIN erp_areas_estoque  a ON l.area_estoque_id  = a.id
                WHERE s.id = @Id
                """, new { Id = saldoId });

            return resultado;
        });
    }

    public async Task<EstoqueSaldo> RegistrarSaidaAsync(Guid saldoId, decimal quantidade, string? motivo, Guid contaId, Guid? usuarioId)
    {
        return await ExecutarEmTransacaoAsync(async conexao =>
        {
            var saldo = await conexao.QueryFirstOrDefaultAsync<EstoqueSaldo>("""
                SELECT id AS Id, conta_id AS ContaId, empresa_id AS EmpresaId,
                       produto_variacao_id AS ProdutoVariacaoId, local_estoque_id AS LocalEstoqueId,
                       lote AS Lote, data_validade AS DataValidade, quantidade_atual AS QuantidadeAtual
                FROM erp_estoque_saldo
                WHERE id = @Id AND conta_id = @ContaId
                LIMIT 1
                """, new { Id = saldoId, ContaId = contaId });

            if (saldo == null)
                throw new InvalidOperationException(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.SaldoNaoEncontrado);

            if (saldo.QuantidadeAtual < quantidade)
                throw new InvalidOperationException(
                    string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Estoque.SaldoInsuficiente, saldo.QuantidadeAtual, quantidade));

            var novaQtd = saldo.QuantidadeAtual - quantidade;

            await conexao.ExecuteAsync("""
                UPDATE erp_estoque_saldo
                SET quantidade_atual = @Qtd, atualizado_em = NOW()
                WHERE id = @Id
                """, new { Id = saldoId, Qtd = novaQtd });

            await conexao.ExecuteAsync("""
                INSERT INTO erp_estoque_movimentacoes
                    (id, conta_id, empresa_id, estoque_saldo_id, tipo, quantidade, motivo,
                     lote, data_validade, usuario_responsavel_id)
                VALUES
                    (@Id, @ContaId, @EmpresaId, @SaldoId, 'SAIDA', @Quantidade, @Motivo,
                     @Lote, @DataValidade, @UsuarioId)
                """, new
            {
                Id = Guid.NewGuid(), saldo.ContaId, saldo.EmpresaId,
                SaldoId = saldoId, Quantidade = quantidade, Motivo = motivo,
                saldo.Lote, saldo.DataValidade, UsuarioId = usuarioId
            });

            return await ObterSaldoComJoinAsync(conexao, saldoId);
        });
    }

    public async Task<IEnumerable<EstoqueSaldo>> ListarSaldosPorVariacaoAsync(Guid produtoVariacaoId, Guid contaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<EstoqueSaldo>("""
            SELECT s.id AS Id, s.conta_id AS ContaId, s.empresa_id AS EmpresaId,
                   s.produto_variacao_id AS ProdutoVariacaoId, s.local_estoque_id AS LocalEstoqueId,
                   s.lote AS Lote, s.data_validade AS DataValidade, s.data_fabricacao AS DataFabricacao,
                   s.quantidade_atual AS QuantidadeAtual, s.status AS Situacao,
                   s.data_entrada AS DataEntrada, s.criado_em AS DataInclusao, s.atualizado_em AS DataUltimaAtualizacao,
                   l.nome AS LocalNome, a.nome AS AreaNome
            FROM erp_estoque_saldo s
            INNER JOIN erp_locais_estoque l ON s.local_estoque_id = l.id
            INNER JOIN erp_areas_estoque  a ON l.area_estoque_id  = a.id
            WHERE s.produto_variacao_id = @ProdutoVariacaoId AND s.conta_id = @ContaId
            ORDER BY s.data_validade ASC NULLS LAST, s.lote ASC NULLS LAST, s.data_entrada DESC
            """, new { ProdutoVariacaoId = produtoVariacaoId, ContaId = contaId });
    }

    public async Task<IEnumerable<EstoqueSaldo>> ListarSaldosPorContaEEmpresaAsync(Guid contaId, Guid empresaId, string? termoBusca = null)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var filtro = string.IsNullOrWhiteSpace(termoBusca)
            ? string.Empty
            : "AND (p.descricao ILIKE '%' || @Termo || '%' OR pv.descricao ILIKE '%' || @Termo || '%' OR s.lote ILIKE '%' || @Termo || '%')";

        return await conexao.QueryAsync<EstoqueSaldo>($"""
            SELECT s.id AS Id, s.conta_id AS ContaId, s.empresa_id AS EmpresaId,
                   s.produto_variacao_id AS ProdutoVariacaoId, s.local_estoque_id AS LocalEstoqueId,
                   s.lote AS Lote, s.data_validade AS DataValidade, s.data_fabricacao AS DataFabricacao,
                   s.quantidade_atual AS QuantidadeAtual, s.status AS Situacao,
                   s.data_entrada AS DataEntrada, s.criado_em AS DataInclusao, s.atualizado_em AS DataUltimaAtualizacao,
                   l.nome AS LocalNome, a.nome AS AreaNome,
                   p.descricao AS ProdutoNome, pv.descricao AS VariacaoNome
            FROM erp_estoque_saldo s
            INNER JOIN erp_locais_estoque  l  ON s.local_estoque_id    = l.id
            INNER JOIN erp_areas_estoque   a  ON l.area_estoque_id     = a.id
            INNER JOIN erp_produtos_variacoes pv ON s.produto_variacao_id = pv.id
            INNER JOIN erp_produtos        p  ON pv.produto_id          = p.id
            WHERE s.conta_id = @ContaId AND s.empresa_id = @EmpresaId
            {filtro}
            ORDER BY p.descricao, pv.descricao, s.data_validade ASC NULLS LAST
            """, new { ContaId = contaId, EmpresaId = empresaId, Termo = termoBusca ?? string.Empty });
    }

    public async Task<EstoqueSaldo?> ObterSaldoPorIdAsync(Guid id, Guid contaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await ObterSaldoComJoinAsync(conexao, id);
    }

    public async Task<IEnumerable<EstoqueMovimentacao>> ListarMovimentacoesPorSaldoAsync(Guid saldoId, Guid contaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        return await conexao.QueryAsync<EstoqueMovimentacao>("""
            SELECT id AS Id, conta_id AS ContaId, empresa_id AS EmpresaId,
                   estoque_saldo_id AS EstoqueSaldoId, tipo AS Tipo,
                   quantidade AS Quantidade, motivo AS Motivo,
                   lote AS Lote, data_validade AS DataValidade,
                   usuario_responsavel_id AS UsuarioResponsavelId,
                   criado_em AS DataInclusao
            FROM erp_estoque_movimentacoes
            WHERE estoque_saldo_id = @SaldoId AND conta_id = @ContaId
            ORDER BY criado_em DESC
            """, new { SaldoId = saldoId, ContaId = contaId });
    }

    private static async Task<EstoqueSaldo> ObterSaldoComJoinAsync(Npgsql.NpgsqlConnection conexao, Guid saldoId)
    {
        return await conexao.QueryFirstAsync<EstoqueSaldo>("""
            SELECT s.id AS Id, s.conta_id AS ContaId, s.empresa_id AS EmpresaId,
                   s.produto_variacao_id AS ProdutoVariacaoId, s.local_estoque_id AS LocalEstoqueId,
                   s.lote AS Lote, s.data_validade AS DataValidade, s.data_fabricacao AS DataFabricacao,
                   s.quantidade_atual AS QuantidadeAtual, s.status AS Situacao,
                   s.data_entrada AS DataEntrada, s.criado_em AS DataInclusao, s.atualizado_em AS DataUltimaAtualizacao,
                   l.nome AS LocalNome, a.nome AS AreaNome
            FROM erp_estoque_saldo s
            INNER JOIN erp_locais_estoque l ON s.local_estoque_id = l.id
            INNER JOIN erp_areas_estoque  a ON l.area_estoque_id  = a.id
            WHERE s.id = @Id
            """, new { Id = saldoId });
    }
}



