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

public class FidelidadeRepositorio : RepositorioBase, IFidelidadeRepositorio
{
    public FidelidadeRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    #region Gestão de Cartões
    public async Task<ResultadoPaginado<FidelidadeCartao>> ListarCartoesPorEmpresaAsync(Guid empresaId, string? ordenarPor = "ordem", string ordenarDirecao = "asc", int paginaAtual = 1, int tamanhoPagina = 30)
    {
        var deslocamento = (paginaAtual - 1) * tamanhoPagina;
        var direcao = string.Equals(ordenarDirecao, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        
        var clausulaOrdem = ordenarPor?.ToLower() switch
        {
            "titulo" => $"titulo {direcao}",
            "status" => $"status {direcao}",
            "pontos" => $"pontos_necessarios {direcao}",
            "data"   => $"data_inclusao {direcao}",
            _        => $"ordem {direcao}, titulo {direcao}"
        };

        var sqlBase = """
            FROM fidelidade_cartoes
            WHERE empresa_id = @EmpresaId
            """;

        var sqlCount = $"SELECT COUNT(1) {sqlBase}";
        var sqlItens = $"""
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, pontos_necessarios AS PontosNecessarios,
                   recompensa, regra_texto AS RegraTexto, dias_validade_carimbo AS DiasValidadeCarimbo,
                   dias_inatividade_cartao AS DiasInatividadeCartao, data_inicio_vigencia AS DataInicioVigencia,
                   data_fim_vigencia AS DataFimVigencia, tem_referencia AS TemReferencia, url_imagem AS UrlImagem,
                   status AS Situacao, ordem, data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            {sqlBase}
            ORDER BY {clausulaOrdem}
            LIMIT @TamanhoPagina OFFSET @Offset
            """;

        await using var conexao = CriarConexao();
        
        var parametros = new { EmpresaId = empresaId, TamanhoPagina = tamanhoPagina, Offset = deslocamento };
        
        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var itens = await conexao.QueryAsync<FidelidadeCartao>(sqlItens, parametros);

        return new ResultadoPaginado<FidelidadeCartao>(itens, total, paginaAtual, tamanhoPagina);
    }

    public async Task<FidelidadeCartao?> ObterCartaoPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, pontos_necessarios AS PontosNecessarios,
                   recompensa, regra_texto AS RegraTexto, dias_validade_carimbo AS DiasValidadeCarimbo,
                   dias_inatividade_cartao AS DiasInatividadeCartao, data_inicio_vigencia AS DataInicioVigencia,
                   data_fim_vigencia AS DataFimVigencia, tem_referencia AS TemReferencia, url_imagem AS UrlImagem,
                   status AS Situacao, ordem, data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_cartoes
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<FidelidadeCartao>(sql, new { Id = id });
    }

    public async Task<IEnumerable<FidelidadeCartao>> ObterCartoesAtivosPorEmpresaAsync(Guid empresaId)
    {
        const string sql = """
            SELECT id, empresa_id AS EmpresaId, titulo, descricao, pontos_necessarios AS PontosNecessarios,
                   recompensa, regra_texto AS RegraTexto, dias_validade_carimbo AS DiasValidadeCarimbo,
                   dias_inatividade_cartao AS DiasInatividadeCartao, data_inicio_vigencia AS DataInicioVigencia,
                   data_fim_vigencia AS DataFimVigencia, tem_referencia AS TemReferencia, url_imagem AS UrlImagem,
                   status AS Situacao, ordem, data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_cartoes
            WHERE empresa_id = @EmpresaId AND status = 'ativo'
            ORDER BY ordem DESC, data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<FidelidadeCartao>(sql, new { EmpresaId = empresaId });
    }

    public async Task CadastrarCartaoAsync(FidelidadeCartao cartao)
    {
        const string sql = """
            INSERT INTO fidelidade_cartoes (
                empresa_id, titulo, descricao, pontos_necessarios, recompensa, regra_texto, 
                dias_validade_carimbo, dias_inatividade_cartao, data_inicio_vigencia, 
                data_fim_vigencia, tem_referencia, url_imagem, status, ordem, operador_inclusao
            ) VALUES (
                @EmpresaId, @Titulo, @Descricao, @PontosNecessarios, @Recompensa, @RegraTexto, 
                @DiasValidadeCarimbo, @DiasInatividadeCartao, @DataInicioVigencia, 
                @DataFimVigencia, @TemReferencia, @UrlImagem, @Situacao, @Ordem, @OperadorInclusao
            )
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, cartao);
    }

    public async Task AtualizarCartaoAsync(FidelidadeCartao cartao)
    {
        const string sql = """
            UPDATE fidelidade_cartoes
            SET titulo = @Titulo,
                descricao = @Descricao,
                pontos_necessarios = @PontosNecessarios,
                recompensa = @Recompensa,
                regra_texto = @RegraTexto,
                dias_validade_carimbo = @DiasValidadeCarimbo,
                dias_inatividade_cartao = @DiasInatividadeCartao,
                data_inicio_vigencia = @DataInicioVigencia,
                data_fim_vigencia = @DataFimVigencia,
                tem_referencia = @TemReferencia,
                status = @Situacao,
                ordem = @Ordem,
                data_ultima_atualizacao = now()
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, cartao);
    }

    public async Task AtualizarUrlImagemCartaoAsync(Guid id, string? urlImagem)
    {
        const string sql = "UPDATE fidelidade_cartoes SET url_imagem = @UrlImagem, data_ultima_atualizacao = now() WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, UrlImagem = urlImagem });
    }
    #endregion

    #region Gestão de Carimbos e Saldos
    public async Task<FidelidadeSaldo?> ObterSaldoAsync(Guid cartaoId, Guid usuarioId)
    {
        const string sql = """
            SELECT id, cartao_id AS CartaoId, usuario_id AS UsuarioId, pontos_acumulados AS PontosAcumulados,
                   recompensas_acumuladas AS RecompensasAcumuladas, status AS Situacao, data_resgate_ultima AS DataResgateUltima,
                   data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_saldos
            WHERE cartao_id = @CartaoId AND usuario_id = @UsuarioId
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<FidelidadeSaldo>(sql, new { CartaoId = cartaoId, UsuarioId = usuarioId });
    }

    public async Task<FidelidadeSaldo?> ObterSaldoPorIdAsync(Guid saldoId)
    {
        const string sql = """
            SELECT id, cartao_id AS CartaoId, usuario_id AS UsuarioId, pontos_acumulados AS PontosAcumulados,
                   recompensas_acumuladas AS RecompensasAcumuladas, status AS Situacao, data_resgate_ultima AS DataResgateUltima,
                   data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_saldos
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<FidelidadeSaldo>(sql, new { Id = saldoId });
    }

    public async Task<IEnumerable<FidelidadeSaldo>> ListarSaldosPorUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT id, cartao_id AS CartaoId, usuario_id AS UsuarioId, pontos_acumulados AS PontosAcumulados,
                   recompensas_acumuladas AS RecompensasAcumuladas, status AS Situacao, data_resgate_ultima AS DataResgateUltima,
                   data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_saldos
            WHERE usuario_id = @UsuarioId AND status = 'ativo'
            ORDER BY data_ultima_atualizacao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<FidelidadeSaldo>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<FidelidadeCartaoComProgressoDto>> ListarCartoesComProgressoPorUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT 
                   s.id, s.cartao_id AS CartaoId, s.usuario_id AS UsuarioId, s.pontos_acumulados AS PontosAcumulados,
                   s.recompensas_acumuladas AS RecompensasAcumuladas, s.status AS Situacao, s.data_resgate_ultima AS DataResgateUltima,
                   s.data_inclusao AS DataInclusao, s.data_ultima_atualizacao AS DataUltimaAtualizacao,
                   c.id, c.empresa_id AS EmpresaId, c.titulo, c.descricao, c.pontos_necessarios AS PontosNecessarios,
                   c.recompensa, c.regra_texto AS RegraTexto, c.dias_validade_carimbo AS DiasValidadeCarimbo,
                   c.dias_inatividade_cartao AS DiasInatividadeCartao, c.data_inicio_vigencia AS DataInicioVigencia,
                   c.data_fim_vigencia AS DataFimVigencia, c.tem_referencia AS TemReferencia, c.url_imagem AS UrlImagem,
                   c.status AS Situacao, c.ordem, c.data_inclusao AS DataInclusao, c.operador_inclusao AS OperadorInclusao,
                   c.data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM fidelidade_saldos s
            INNER JOIN fidelidade_cartoes c ON c.id = s.cartao_id
            WHERE s.usuario_id = @UsuarioId AND s.status = 'ativo'
            ORDER BY s.data_ultima_atualizacao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<FidelidadeSaldo, FidelidadeCartao, FidelidadeCartaoComProgressoDto>(
            sql,
            (saldo, cartao) => 
            {
                var porcentagem = cartao.PontosNecessarios > 0
                    ? Math.Min(100, (int)Math.Round((double)saldo.PontosAcumulados / cartao.PontosNecessarios * 100))
                    : 0;
                return new FidelidadeCartaoComProgressoDto(saldo, cartao, porcentagem);
            },
            new { UsuarioId = usuarioId },
            splitOn: "id"
        );
    }

    public async Task ExecutarCarimboAsync(Guid cartaoId, Guid usuarioId, FidelidadeCarimbo carimbo, bool completouCartao)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await using var trans = await conexao.BeginTransactionAsync();

        try
        {
            const string sqlSaldo = """
                INSERT INTO fidelidade_saldos (cartao_id, usuario_id, pontos_acumulados, recompensas_acumuladas)
                VALUES (@CartaoId, @UsuarioId, 1, 0)
                ON CONFLICT (cartao_id, usuario_id) DO UPDATE
                SET pontos_acumulados = CASE WHEN @CompletouCartao THEN 0 ELSE fidelidade_saldos.pontos_acumulados + 1 END,
                    recompensas_acumuladas = CASE WHEN @CompletouCartao THEN fidelidade_saldos.recompensas_acumuladas + 1 ELSE fidelidade_saldos.recompensas_acumuladas END,
                    data_ultima_atualizacao = now()
                RETURNING id
                """;

            var saldoId = await conexao.ExecuteScalarAsync<Guid>(sqlSaldo, new { CartaoId = cartaoId, UsuarioId = usuarioId, CompletouCartao = completouCartao }, trans);
            carimbo.SaldoId = saldoId;
            const string sqlCarimbo = """
                INSERT INTO fidelidade_carimbos (saldo_id, status, codigo_referencia, data_expiracao)
                VALUES (@SaldoId, @Situacao, @CodigoReferencia, @DataExpiracao)
                """;
            if (completouCartao)
            {
                const string sqlUpdateOld = "UPDATE fidelidade_carimbos SET status = 'utilizado' WHERE saldo_id = @SaldoId AND status = 'valido'";
                await conexao.ExecuteAsync(sqlUpdateOld, new { SaldoId = saldoId }, trans);
                carimbo.Situacao = "utilizado";
            }

            await conexao.ExecuteAsync(sqlCarimbo, carimbo, trans);
            const string sqlLog = "INSERT INTO fidelidade_logs (saldo_id, tipo_evento, descricao, codigo_referencia) VALUES (@SaldoId, @Tipo, @Desc, @Ref)";
            await conexao.ExecuteAsync(sqlLog, new { 
                SaldoId = saldoId, 
                Tipo = "GANHOU_CARIMBO", 
                Desc = completouCartao ? "Ganhou carimbo e completou o cartão!" : "Ganhou 1 carimbo.",
                Ref = carimbo.CodigoReferencia
            }, trans);

            await trans.CommitAsync();
        }
        catch
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task ExecutarResgateAsync(Guid saldoId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await using var trans = await conexao.BeginTransactionAsync();

        try
        {
            const string sqlSaldo = """
                UPDATE fidelidade_saldos
                SET recompensas_acumuladas = recompensas_acumuladas - 1,
                    data_resgate_ultima = now(),
                    data_ultima_atualizacao = now()
                WHERE id = @Id AND recompensas_acumuladas > 0
                """;

            var rows = await conexao.ExecuteAsync(sqlSaldo, new { Id = saldoId }, trans);
            if (rows == 0) throw new InvalidOperationException("Cliente não possui recompensas para resgatar.");

            const string sqlLog = "INSERT INTO fidelidade_logs (saldo_id, tipo_evento, descricao) VALUES (@SaldoId, @Tipo, @Desc)";
            await conexao.ExecuteAsync(sqlLog, new { 
                SaldoId = saldoId, 
                Tipo = "RESGATOU_RECOMPENSA", 
                Desc = "Cliente resgatou uma recompensa."
            }, trans);

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
    public async Task CadastrarCodigoAsync(FidelidadeToken token)
    {
        const string sql = """
            INSERT INTO fidelidade_tokens (token, cartao_id, empresa_id, status, data_expiracao)
            VALUES (@Codigo, @CartaoId, @EmpresaId, @Situacao, @DataExpiracao)
            """;
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, token);
    }

    public async Task<FidelidadeToken?> ObterCodigoPorValorAsync(string token)
    {
        const string sql = """
            SELECT id, token AS Codigo, cartao_id AS CartaoId, empresa_id AS EmpresaId,
                   status AS Situacao, data_expiracao AS DataExpiracao, data_inclusao AS DataInclusao
            FROM fidelidade_tokens
            WHERE token = @Token
            """;
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<FidelidadeToken>(sql, new { Token = token });
    }

    public async Task AtualizarSituacaoCodigoAsync(Guid id, string situacao)
    {
        const string sql = "UPDATE fidelidade_tokens SET status = @Situacao WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, Situacao = situacao });
    }
    #endregion

    #region Logs
    public async Task RegistrarLogAsync(FidelidadeLog log)
    {
        const string sql = "INSERT INTO fidelidade_logs (saldo_id, tipo_evento, descricao, codigo_referencia) VALUES (@SaldoId, @TipoEvento, @Descricao, @CodigoReferencia)";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, log);
    }

    public async Task<IEnumerable<FidelidadeLog>> ListarLogsPorSaldoAsync(Guid saldoId)
    {
        const string sql = "SELECT id, saldo_id AS SaldoId, tipo_evento AS TipoEvento, descricao, codigo_referencia AS CodigoReferencia, data_inclusao AS DataInclusao FROM fidelidade_logs WHERE saldo_id = @SaldoId ORDER BY data_inclusao DESC";
        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<FidelidadeLog>(sql, new { SaldoId = saldoId });
    }
    #endregion
}



