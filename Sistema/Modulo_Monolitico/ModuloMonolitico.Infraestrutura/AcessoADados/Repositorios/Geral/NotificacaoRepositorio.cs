using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Geral;

public class NotificacaoRepositorio : RepositorioBase, INotificacaoRepositorio
{
    public NotificacaoRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<IEnumerable<NotificacaoDto>> ListarNotificacoesAsync(Guid contaId, Guid usuarioId, string filtro)
    {
        using var conexao = CriarConexao();

        var sql = """
          SELECT
            n.id      AS Id,
            n.titulo    AS Titulo,
            n.mensagem   AS Mensagem,
            n.tipo     AS Tipo,
            n.link_acao   AS LinkAcao,
            CASE
              WHEN n.usuario_id IS NOT NULL THEN n.lida
              ELSE (nlg.usuario_id IS NOT NULL)
            END       AS Lida,
            (n.usuario_id IS NULL) AS EhGlobal,
            n.data_inclusao AS DataInclusao
          FROM notificacoes n
          LEFT JOIN notificacoes_lidas_globais nlg
            ON nlg.notificacao_id = n.id AND nlg.usuario_id = @UsuarioId
          WHERE n.conta_id = @ContaId
           AND (
             n.usuario_id = @UsuarioId
             OR n.usuario_id IS NULL
           )
          ORDER BY n.data_inclusao DESC
          LIMIT 100
          """;

        var todos = (await conexao.QueryAsync<NotificacaoDto>(sql, new { ContaId = contaId, UsuarioId = usuarioId })).ToList();

        return filtro switch
        {
            "nao_lidas" => todos.Where(n => !n.Lida).ToList(),
            "info" => todos.Where(n => n.Tipo == "info").ToList(),
            "alerta" => todos.Where(n => n.Tipo == "alerta").ToList(),
            "sucesso" => todos.Where(n => n.Tipo == "sucesso").ToList(),
            "erro" => todos.Where(n => n.Tipo == "erro").ToList(),
            _ => todos
        };
    }

    public async Task<bool> MarcarComoLidaAsync(Guid contaId, Guid usuarioId, Guid notificacaoId)
    {
        using var conexao = CriarConexao();
        
        var not = await conexao.QueryFirstOrDefaultAsync<(Guid Id, Guid? UsuarioId)>(
            "SELECT id AS Id, usuario_id AS UsuarioId FROM notificacoes WHERE id = @Id AND conta_id = @ContaId",
            new { Id = notificacaoId, ContaId = contaId });

        if (not.Id == Guid.Empty) return false;

        if (not.UsuarioId == null)
        {
            await conexao.ExecuteAsync("""
                INSERT INTO notificacoes_lidas_globais (notificacao_id, usuario_id)
                VALUES (@NotificacaoId, @UsuarioId)
                ON CONFLICT DO NOTHING
                """, new { NotificacaoId = notificacaoId, UsuarioId = usuarioId });
        }
        else
        {
            await conexao.ExecuteAsync(
                "UPDATE notificacoes SET lida = true WHERE id = @Id AND usuario_id = @UsuarioId",
                new { Id = notificacaoId, UsuarioId = usuarioId });
        }
        return true;
    }

    public async Task MarcarTodasComoLidasAsync(Guid contaId, Guid usuarioId)
    {
        using var conexao = CriarConexao();

        await conexao.ExecuteAsync("""
            UPDATE notificacoes
            SET lida = true
            WHERE conta_id = @ContaId AND usuario_id = @UsuarioId AND lida = false
            """, new { ContaId = contaId, UsuarioId = usuarioId });

        await conexao.ExecuteAsync("""
            INSERT INTO notificacoes_lidas_globais (notificacao_id, usuario_id)
            SELECT n.id, @UsuarioId
            FROM notificacoes n
            WHERE n.conta_id = @ContaId
             AND n.usuario_id IS NULL
             AND NOT EXISTS (
               SELECT 1 FROM notificacoes_lidas_globais nlg
               WHERE nlg.notificacao_id = n.id AND nlg.usuario_id = @UsuarioId
             )
            ON CONFLICT DO NOTHING
            """, new { ContaId = contaId, UsuarioId = usuarioId });
    }
    public async Task<int> ContarNotificacoesNaoLidasAsync(Guid contaId, Guid usuarioId)
    {
        using var conexao = CriarConexao();
        const string sql = "SELECT COUNT(*) FROM notificacoes n WHERE n.conta_id = @ContaId AND ((n.usuario_id = @UsuarioId AND n.lida = false) OR (n.usuario_id IS NULL AND NOT EXISTS (SELECT 1 FROM notificacoes_lidas_globais nlg WHERE nlg.notificacao_id = n.id AND nlg.usuario_id = @UsuarioId)))";
        return await conexao.ExecuteScalarAsync<int>(sql, new { ContaId = contaId, UsuarioId = usuarioId });
    }
}

