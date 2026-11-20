using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.SaaS;

public class ComunicadoSaaSRepositorio : IComunicadoSaaSRepositorio
{
    private readonly string _stringDeConexao;

    public ComunicadoSaaSRepositorio(string stringDeConexao)
    {
        _stringDeConexao = stringDeConexao;
    }

    public async Task<ComunicadoSaaS?> ObterPrimeiroPendenteParaUsuarioAsync(Guid contaId, Guid usuarioId, string moduloAtual)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = @"
            SELECT c.* 
            FROM saas_comunicados c
            WHERE c.ativo = true
              AND (c.data_expiracao IS NULL OR c.data_expiracao > now())
              AND (c.modulo_alvo IS NULL OR c.modulo_alvo = '' OR c.modulo_alvo = 'Todos' OR c.modulo_alvo = @ModuloAtual)
              AND NOT EXISTS (
                  SELECT 1 
                  FROM saas_comunicados_leituras l
                  WHERE l.comunicado_id = c.id
                    AND l.usuario_id = @UsuarioId
                    AND l.conta_id = @ContaId
              )
            ORDER BY c.data_inclusao ASC
            LIMIT 1;
        ";

        var comunicado = await _conexao.QueryFirstOrDefaultAsync<ComunicadoSaaS>(consulta, new { ContaId = contaId, UsuarioId = usuarioId, ModuloAtual = moduloAtual });

        if (comunicado != null && comunicado.Tipo == "enquete")
        {
            var consultaOpcoes = @"
                SELECT *
                FROM saas_comunicados_opcoes
                WHERE comunicado_id = @ComunicadoId
                ORDER BY ordem ASC;
            ";
            
            var opcoes = await _conexao.QueryAsync<OpcaoComunicadoSaaS>(consultaOpcoes, new { ComunicadoId = comunicado.Id });
            

            var propriedade = typeof(ComunicadoSaaS).GetProperty(nameof(ComunicadoSaaS.Opcoes));
            propriedade?.SetValue(comunicado, opcoes.ToList());
        }

        return comunicado;
    }

    public async Task RegistrarLeituraAsync(LeituraComunicadoSaaS leitura)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = @"
            INSERT INTO saas_comunicados_leituras 
                (id, comunicado_id, conta_id, usuario_id, opcao_selecionada_id, data_leitura)
            VALUES 
                (@Id, @ComunicadoId, @ContaId, @UsuarioId, @OpcaoSelecionadaId, @DataLeitura);
        ";

        await _conexao.ExecuteAsync(consulta, leitura);
    }

    public async Task<System.Collections.Generic.IEnumerable<ComunicadoSaaS>> ListarTodosAsync()
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = "SELECT * FROM saas_comunicados ORDER BY data_inclusao DESC;";
        return await _conexao.QueryAsync<ComunicadoSaaS>(consulta);
    }

    public async Task<ComunicadoSaaS?> ObterPorIdAsync(Guid id)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = "SELECT * FROM saas_comunicados WHERE id = @Id;";
        var comunicado = await _conexao.QueryFirstOrDefaultAsync<ComunicadoSaaS>(consulta, new { Id = id });

        if (comunicado != null)
        {
            var consultaOpcoes = "SELECT * FROM saas_comunicados_opcoes WHERE comunicado_id = @Id ORDER BY ordem ASC;";
            var opcoes = await _conexao.QueryAsync<OpcaoComunicadoSaaS>(consultaOpcoes, new { Id = id });
            var propriedade = typeof(ComunicadoSaaS).GetProperty(nameof(ComunicadoSaaS.Opcoes));
            propriedade?.SetValue(comunicado, opcoes.ToList());
        }

        return comunicado;
    }

    public async Task CadastrarComunicadoAsync(ComunicadoSaaS comunicado, System.Collections.Generic.List<OpcaoComunicadoSaaS> opcoes)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        await _conexao.OpenAsync();
        using var transacao = await _conexao.BeginTransactionAsync();

        try
        {
            var consultaComunicado = @"
                INSERT INTO saas_comunicados 
                    (id, data_inclusao, titulo, mensagem, tipo, modulo_alvo, data_expiracao, ativo, operador_inclusao)
                VALUES 
                    (@Id, @DataInclusao, @Titulo, @Mensagem, @Tipo, @ModuloAlvo, @DataExpiracao, @Ativo, @OperadorInclusao);
            ";
            await _conexao.ExecuteAsync(consultaComunicado, comunicado, transacao);

            if (opcoes != null && opcoes.Count > 0)
            {
                var consultaOpcao = @"
                    INSERT INTO saas_comunicados_opcoes 
                        (id, comunicado_id, texto, ordem)
                    VALUES 
                        (@Id, @ComunicadoId, @Texto, @Ordem);
                ";
                await _conexao.ExecuteAsync(consultaOpcao, opcoes, transacao);
            }

            await transacao.CommitAsync();
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }

    public async Task DeletarFisicoComunicadoAsync(Guid id)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        await _conexao.OpenAsync();
        using var transacao = await _conexao.BeginTransactionAsync();

        try
        {
            await _conexao.ExecuteAsync("DELETE FROM saas_comunicados_leituras WHERE comunicado_id = @Id;", new { Id = id }, transacao);
            await _conexao.ExecuteAsync("DELETE FROM saas_comunicados_opcoes WHERE comunicado_id = @Id;", new { Id = id }, transacao);
            await _conexao.ExecuteAsync("DELETE FROM saas_comunicados WHERE id = @Id;", new { Id = id }, transacao);

            await transacao.CommitAsync();
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }

    public async Task<System.Collections.Generic.IEnumerable<ResultadoEnqueteDto>> ObterResultadosEnqueteAsync(Guid comunicadoId)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = @"
            SELECT 
                o.id AS OpcaoId,
                o.texto AS Texto,
                o.ordem AS Ordem,
                COUNT(l.id)::int AS Votos
            FROM saas_comunicados_opcoes o
            LEFT JOIN saas_comunicados_leituras l ON l.opcao_selecionada_id = o.id
            WHERE o.comunicado_id = @ComunicadoId
            GROUP BY o.id, o.texto, o.ordem
            ORDER BY o.ordem ASC;
        ";
        return await _conexao.QueryAsync<ResultadoEnqueteDto>(consulta, new { ComunicadoId = comunicadoId });
    }

    public async Task<System.Collections.Generic.IEnumerable<ComunicadoSaaSComMetricasDto>> ListarTodosComMetricasAsync()
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = @"
            SELECT 
                c.id as Id, 
                c.data_inclusao as DataInclusao, 
                c.titulo as Titulo, 
                c.mensagem as Mensagem, 
                c.tipo as Tipo, 
                c.modulo_alvo as ModuloAlvo, 
                c.data_expiracao as DataExpiracao,
                c.ativo as Ativo,
                COUNT(l.id) as TotalLidos,
                COUNT(l.opcao_selecionada_id) as TotalRespostas
            FROM saas_comunicados c
            LEFT JOIN saas_comunicados_leituras l ON l.comunicado_id = c.id
            GROUP BY c.id
            ORDER BY c.data_inclusao DESC;
        ";
        return await _conexao.QueryAsync<ComunicadoSaaSComMetricasDto>(consulta);
    }

    public async Task<System.Collections.Generic.IEnumerable<InteracaoComunicadoDto>> ObterInteracoesAsync(Guid comunicadoId)
    {
        using var _conexao = new Npgsql.NpgsqlConnection(_stringDeConexao);
        var consulta = @"
            SELECT 
                co.nome_fantasia as NomeConta,
                u.nome as NomeUsuario,
                l.data_leitura as DataLeitura,
                o.texto as OpcaoEscolhida
            FROM saas_comunicados_leituras l
            INNER JOIN contas co ON co.id = l.conta_id
            INNER JOIN usuarios u ON u.id = l.usuario_id
            LEFT JOIN saas_comunicados_opcoes o ON o.id = l.opcao_selecionada_id
            WHERE l.comunicado_id = @ComunicadoId
            ORDER BY l.data_leitura DESC;
        ";
        return await _conexao.QueryAsync<InteracaoComunicadoDto>(consulta, new { ComunicadoId = comunicadoId });
    }
}



