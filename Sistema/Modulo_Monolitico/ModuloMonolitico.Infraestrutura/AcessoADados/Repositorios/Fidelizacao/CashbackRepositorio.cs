using Dapper;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Fidelizacao;

public class CashbackRepositorio : RepositorioBase, ICashbackRepositorio
{
    public CashbackRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    #region Gestão de Regras
    public async Task<IEnumerable<CashbackRegra>> ListarRegrasPorEmpresaAsync(Guid empresaId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, percentual, dias_validade AS DiasValidade,
                   data_inicio_vigencia AS DataInicioVigencia, data_fim_vigencia AS DataFimVigencia,
                   valor_maximo_unidade AS ValorMaximoUnidade, valor_maximo_total AS ValorMaximoTotal,
                   tem_referencia AS TemReferencia, url_imagem AS UrlImagem, status AS Situacao, ordem,
                   data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_regras
            WHERE empresa_id = @EmpresaId
            ORDER BY ordem, titulo
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<CashbackRegra>(sql, new { EmpresaId = empresaId });
    }

    public async Task<CashbackRegra?> ObterRegraPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, percentual, dias_validade AS DiasValidade,
                   data_inicio_vigencia AS DataInicioVigencia, data_fim_vigencia AS DataFimVigencia,
                   valor_maximo_unidade AS ValorMaximoUnidade, valor_maximo_total AS ValorMaximoTotal,
                   tem_referencia AS TemReferencia, url_imagem AS UrlImagem, status AS Situacao, ordem,
                   data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_regras
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<CashbackRegra>(sql, new { Id = id });
    }

    public async Task<IEnumerable<CashbackRegra>> ObterRegrasAtivasPorEmpresaAsync(Guid empresaId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, percentual, dias_validade AS DiasValidade,
                   data_inicio_vigencia AS DataInicioVigencia, data_fim_vigencia AS DataFimVigencia,
                   valor_maximo_unidade AS ValorMaximoUnidade, valor_maximo_total AS ValorMaximoTotal,
                   tem_referencia AS TemReferencia, url_imagem AS UrlImagem, status AS Situacao, ordem,
                   data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_regras
            WHERE empresa_id = @EmpresaId AND status = 'ativo'
            ORDER BY ordem DESC, data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<CashbackRegra>(sql, new { EmpresaId = empresaId });
    }

    public async Task CadastrarRegraAsync(CashbackRegra regra)
    {
        const string sql = """
            INSERT INTO cashback_regras (
                empresa_id, titulo, descricao, percentual, dias_validade, data_inicio_vigencia, 
                data_fim_vigencia, valor_maximo_unidade, valor_maximo_total, tem_referencia, 
                url_imagem, status, ordem, operador_inclusao
            ) VALUES (
                @EmpresaId, @Titulo, @Descricao, @Percentual, @DiasValidade, @DataInicioVigencia, 
                @DataFimVigencia, @ValorMaximoUnidade, @ValorMaximoTotal, @TemReferencia, 
                @UrlImagem, @Situacao, @Ordem, @OperadorInclusao
            )
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, regra);
    }

    public async Task AtualizarRegraAsync(CashbackRegra regra)
    {
        const string sql = """
            UPDATE cashback_regras
            SET titulo = @Titulo,
                descricao = @Descricao,
                percentual = @Percentual,
                dias_validade = @DiasValidade,
                data_inicio_vigencia = @DataInicioVigencia,
                data_fim_vigencia = @DataFimVigencia,
                valor_maximo_unidade = @ValorMaximoUnidade,
                valor_maximo_total = @ValorMaximoTotal,
                tem_referencia = @TemReferencia,
                status = @Situacao,
                ordem = @Ordem,
                data_ultima_atualizacao = now()
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, regra);
    }

    public async Task AtualizarUrlImagemRegraAsync(Guid id, string? urlImagem)
    {
        const string sql = "UPDATE cashback_regras SET url_imagem = @UrlImagem, data_ultima_atualizacao = now() WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, UrlImagem = urlImagem });
    }
    #endregion

    #region Gestão de Créditos e Saldos
    public async Task<CashbackSaldo?> ObterSaldoAsync(Guid empresaId, Guid usuarioId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, usuario_id AS UsuarioId, saldo_disponivel AS SaldoDisponivel,
                   data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_saldos
            WHERE empresa_id = @EmpresaId AND usuario_id = @UsuarioId
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<CashbackSaldo>(sql, new { EmpresaId = empresaId, UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<CashbackSaldo>> ListarSaldosPorUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, usuario_id AS UsuarioId, saldo_disponivel AS SaldoDisponivel,
                   data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_saldos
            WHERE usuario_id = @UsuarioId AND saldo_disponivel > 0
            ORDER BY data_ultima_atualizacao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<CashbackSaldo>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<SaldoCashbackPorEmpresaDto>> ListarSaldosComEmpresaPorUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT s.empresa_id AS EmpresaId, 
                   COALESCE(NULLIF(TRIM(e.razao_social), ''), NULLIF(TRIM(e.nome), ''), 'Loja Parceira') AS NomeEmpresa,
                   s.saldo_disponivel AS SaldoDisponivel, 
                   s.data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM cashback_saldos s
            INNER JOIN empresas e ON e.id = s.empresa_id
            WHERE s.usuario_id = @UsuarioId AND s.saldo_disponivel > 0
            ORDER BY s.saldo_disponivel DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<SaldoCashbackPorEmpresaDto>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<CashbackCredito>> ListarCreditosDisponiveisAsync(Guid empresaId, Guid usuarioId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, usuario_id AS UsuarioId, regra_id AS RegraId,
                   valor_inicial AS ValorInicial, valor_utilizado AS ValorUtilizado,
                   descricao, codigo_referencia AS CodigoReferencia, status AS Situacao,
                   data_liberacao AS DataLiberacao, data_expiracao AS DataExpiracao,
                   data_inclusao AS DataInclusao
            FROM cashback_creditos
            WHERE empresa_id = @EmpresaId AND usuario_id = @UsuarioId
              AND status = 'disponivel'
              AND (data_expiracao IS NULL OR data_expiracao > now())
            ORDER BY data_inclusao ASC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<CashbackCredito>(sql, new { EmpresaId = empresaId, UsuarioId = usuarioId });
    }

    public async Task<ResultadoPaginado<CashbackCredito>> ListarHistoricoCreditosPorUsuarioAsync(Guid usuarioId, int paginaAtual = 1, int tamanhoPagina = 50)
    {
        var deslocamento = (paginaAtual - 1) * tamanhoPagina;
        
        var sqlBase = "FROM cashback_creditos WHERE usuario_id = @UsuarioId";
        var sqlCount = $"SELECT COUNT(1) {sqlBase}";
        
        var sqlItens = $"""
            SELECT id, empresa_id AS EmpresaId, usuario_id AS UsuarioId, regra_id AS RegraId,
                   valor_inicial AS ValorInicial, valor_utilizado AS ValorUtilizado,
                   descricao, codigo_referencia AS CodigoReferencia, status AS Situacao,
                   data_liberacao AS DataLiberacao, data_expiracao AS DataExpiracao,
                   data_inclusao AS DataInclusao
            {sqlBase}
            ORDER BY data_inclusao DESC
            OFFSET @Offset ROWS
            FETCH NEXT @TamanhoPagina ROWS ONLY
            """;

        await using var conexao = CriarConexao();
        var parametros = new { UsuarioId = usuarioId, Offset = deslocamento, TamanhoPagina = tamanhoPagina };
        
        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var itens = await conexao.QueryAsync<CashbackCredito>(sqlItens, parametros);
        
        return new ResultadoPaginado<CashbackCredito>(itens, total, paginaAtual, tamanhoPagina);
    }

    public async Task ExecutarCreditoAsync(CashbackCredito credito, decimal novoSaldo)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await using var trans = await conexao.BeginTransactionAsync();

        try
        {
            const string sqlCredito = """
                INSERT INTO cashback_creditos (
                    empresa_id, usuario_id, regra_id, valor_inicial, descricao, 
                    codigo_referencia, status, data_liberacao, data_expiracao
                ) VALUES (
                    @EmpresaId, @UsuarioId, @RegraId, @ValorInicial, @Descricao, 
                    @CodigoReferencia, @Situacao, @DataLiberacao, @DataExpiracao
                )
                """;

            await conexao.ExecuteAsync(sqlCredito, credito, trans);

            const string sqlSaldo = """
                INSERT INTO cashback_saldos (empresa_id, usuario_id, saldo_disponivel)
                VALUES (@EmpresaId, @UsuarioId, @ValorInicial)
                ON CONFLICT (empresa_id, usuario_id) DO UPDATE
                SET saldo_disponivel = cashback_saldos.saldo_disponivel + @ValorInicial,
                    data_ultima_atualizacao = now()
                """;

            await conexao.ExecuteAsync(sqlSaldo, new { credito.EmpresaId, credito.UsuarioId, credito.ValorInicial }, trans);

            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task ExecutarDebitoAsync(Guid empresaId, Guid usuarioId, decimal valorGasto, List<(Guid CreditoId, decimal ValorDebitado)> utilizacoes, decimal novoSaldo)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await using var trans = await conexao.BeginTransactionAsync();

        try
        {
            foreach (var item in utilizacoes)
            {
                const string sqlUtil = "INSERT INTO cashback_utilizacoes (credito_id, valor_utilizado) VALUES (@CreditoId, @ValorUtilizado)";
                await conexao.ExecuteAsync(sqlUtil, new { CreditoId = item.CreditoId, ValorUtilizado = item.ValorDebitado }, trans);
                const string sqlUpdateCred = """
                    UPDATE cashback_creditos
                    SET valor_utilizado = valor_utilizado + @ValorUtilizado,
                        status = CASE WHEN valor_inicial <= (valor_utilizado + @ValorUtilizado) THEN 'esgotado' ELSE 'disponivel' END
                    WHERE id = @CreditoId
                    """;
                await conexao.ExecuteAsync(sqlUpdateCred, new { CreditoId = item.CreditoId, ValorUtilizado = item.ValorDebitado }, trans);
            }
            const string sqlSaldo = """
                UPDATE cashback_saldos
                SET saldo_disponivel = saldo_disponivel - @ValorGasto,
                    data_ultima_atualizacao = now()
                WHERE empresa_id = @EmpresaId AND usuario_id = @UsuarioId
                """;
            await conexao.ExecuteAsync(sqlSaldo, new { EmpresaId = empresaId, UsuarioId = usuarioId, ValorGasto = valorGasto }, trans);

            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region Tokens/QR Code
    public async Task CadastrarCodigoAsync(CashbackToken token)
    {
        const string sql = """
            INSERT INTO cashback_tokens (token, empresa_id, valor_compra, tipo, status, data_expiracao)
            VALUES (@Codigo, @EmpresaId, @ValorCompra, @Tipo, @Situacao, @DataExpiracao)
            """;
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, token);
    }

    public async Task<CashbackToken?> ObterCodigoPorValorAsync(string token)
    {
        const string sql = """
            SELECT id, token AS Codigo, empresa_id AS EmpresaId, valor_compra AS ValorCompra,
                   tipo, status AS Situacao, data_expiracao AS DataExpiracao, data_inclusao AS DataInclusao
            FROM cashback_tokens
            WHERE token = @Token
            """;
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<CashbackToken>(sql, new { Token = token });
    }

    public async Task AtualizarSituacaoCodigoAsync(Guid id, string situacao)
    {
        const string sql = "UPDATE cashback_tokens SET status = @Situacao WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, Situacao = situacao });
    }
    #endregion
}



