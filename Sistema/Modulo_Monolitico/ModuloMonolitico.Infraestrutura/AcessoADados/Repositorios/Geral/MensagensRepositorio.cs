using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Geral;

public class MensagensRepositorio : RepositorioBase, IMensagensRepositorio
{
    public MensagensRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    public async Task CadastrarMensagemSistemaAsync(Guid destinatarioId, Guid remetenteId, string titulo, string corpoTexto)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var contaId = await conexao.ExecuteScalarAsync<Guid?>(
            "SELECT conta_id FROM usuarios_contas WHERE usuario_id = @DestinatarioId LIMIT 1", 
            new { DestinatarioId = destinatarioId });

        if (contaId == null)
            throw new InvalidOperationException("Não foi possível enviar a mensagem pois o destinatário não possui uma conta vinculada.");
        const string sqlBuscaTopico = """
            SELECT t.id 
            FROM mensagens_topicos t
            JOIN mensagens_participantes p1 ON t.id = p1.topico_id AND p1.usuario_id = @RemetenteId
            JOIN mensagens_participantes p2 ON t.id = p2.topico_id AND p2.usuario_id = @DestinatarioId
            WHERE t.tipo = 'direta'
            LIMIT 1
            """;
            
        var topicoIdExistente = await conexao.ExecuteScalarAsync<Guid?>(sqlBuscaTopico, new { RemetenteId = remetenteId, DestinatarioId = destinatarioId });

        using var transaction = await conexao.BeginTransactionAsync();
        try
        {
            Guid topicoId;
            if (topicoIdExistente.HasValue)
            {
                topicoId = topicoIdExistente.Value;
                const string sqlUpdateTopico = """
                    UPDATE mensagens_topicos 
                    SET data_ultima_interacao = NOW(), ultima_mensagem_preview = @Corpo
                    WHERE id = @TopicoId
                    """;
                await conexao.ExecuteAsync(sqlUpdateTopico, new { TopicoId = topicoId, Corpo = corpoTexto });
                const string sqlUpdateParticipante = """
                    UPDATE mensagens_participantes 
                    SET mensagens_nao_lidas_count = 0 
                    WHERE topico_id = @TopicoId AND usuario_id = @DestinatarioId
                    """;
                await conexao.ExecuteAsync(sqlUpdateParticipante, new { TopicoId = topicoId, DestinatarioId = destinatarioId });
                const string sqlRemoveDuplicata = """
                    DELETE FROM mensagens 
                    WHERE topico_id = @TopicoId 
                      AND remetente_id = @RemetenteId 
                      AND corpo_texto = @Corpo 
                      AND data_envio > NOW() - INTERVAL '1 day'
                    """;
                await conexao.ExecuteAsync(sqlRemoveDuplicata, new { TopicoId = topicoId, RemetenteId = remetenteId, Corpo = corpoTexto });
            }
            else
            {
                topicoId = Guid.NewGuid();
                const string sqlTopico = """
                    INSERT INTO mensagens_topicos (id, conta_id, tipo, titulo, ultima_mensagem_preview, data_ultima_interacao)
                    VALUES (@TopicoId, @ContaId, 'direta', @Titulo, @Corpo, NOW())
                    """;
                await conexao.ExecuteAsync(sqlTopico, new { TopicoId = topicoId, ContaId = contaId, Titulo = titulo, Corpo = corpoTexto });
                const string sqlParticipanteRemetente = """
                    INSERT INTO mensagens_participantes (id, topico_id, usuario_id, mensagens_nao_lidas_count)
                    VALUES (gen_random_uuid(), @TopicoId, @RemetenteId, 0)
                    """;
                await conexao.ExecuteAsync(sqlParticipanteRemetente, new { TopicoId = topicoId, RemetenteId = remetenteId });
                if (remetenteId != destinatarioId)
                {
                    const string sqlParticipanteDestinatario = """
                        INSERT INTO mensagens_participantes (id, topico_id, usuario_id, mensagens_nao_lidas_count)
                        VALUES (gen_random_uuid(), @TopicoId, @DestinatarioId, 0)
                        """;
                    await conexao.ExecuteAsync(sqlParticipanteDestinatario, new { TopicoId = topicoId, DestinatarioId = destinatarioId });
                }
            }
            const string sqlMensagem = """
                INSERT INTO mensagens (id, topico_id, remetente_id, corpo_texto, status, data_envio)
                VALUES (gen_random_uuid(), @TopicoId, @RemetenteId, @Corpo, 'enviado', NOW())
                """;
            await conexao.ExecuteAsync(sqlMensagem, new { TopicoId = topicoId, RemetenteId = remetenteId, Corpo = corpoTexto });
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<TopicoInboxDto>> ListarTopicosAsync(Guid usuarioId)
    {
        using var conexao = CriarConexao();
        const string sqlTopicos = """
          SELECT
            t.id       AS Id,
            t.titulo     AS Titulo,
            t.tipo      AS Tipo,
            t.ultima_mensagem_preview AS UltimaMensagemPreview,
            t.data_ultima_interacao  AS DataUltimaInteracao,
            p.mensagens_nao_lidas_count AS NaoLidas
          FROM mensagens_topicos t
          INNER JOIN mensagens_participantes p
            ON p.topico_id = t.id AND p.usuario_id = @UsuarioId AND p.deletada_em IS NULL
          ORDER BY t.data_ultima_interacao DESC
          LIMIT 50
          """;

        return await conexao.QueryAsync<TopicoInboxDto>(sqlTopicos, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<MensagemDto>> ListarMensagensAsync(Guid topicoId)
    {
        using var conexao = CriarConexao();
        const string sqlMensagens = """
          SELECT
            m.id      AS Id,
            m.remetente_id AS RemetenteId,
            m.corpo_texto  AS CorpoTexto,
            m.data_envio  AS DataEnvio,
            m.status    AS Situacao
          FROM mensagens m
          WHERE m.topico_id = @TopicoId AND m.status = 'enviado'
          ORDER BY m.data_envio ASC
          LIMIT 200
          """;

        return await conexao.QueryAsync<MensagemDto>(sqlMensagens, new { TopicoId = topicoId });
    }

    public async Task MarcarTopicoComoLidoAsync(Guid topicoId, Guid usuarioId)
    {
        using var conexao = CriarConexao();
        
        const string sqlZerarContador = """
          UPDATE mensagens_participantes
          SET mensagens_nao_lidas_count = 0
          WHERE topico_id = @TopicoId AND usuario_id = @UsuarioId
          """;
        await conexao.ExecuteAsync(sqlZerarContador, new { TopicoId = topicoId, UsuarioId = usuarioId });

        const string sqlLeituras = """
          INSERT INTO mensagens_leituras (mensagem_id, usuario_id)
          SELECT m.id, @UsuarioId
          FROM mensagens m
          WHERE m.topico_id = @TopicoId
           AND m.status = 'enviado'
           AND m.remetente_id <> @UsuarioId
           AND NOT EXISTS (
             SELECT 1 FROM mensagens_leituras ml
             WHERE ml.mensagem_id = m.id AND ml.usuario_id = @UsuarioId
           )
          """;
        await conexao.ExecuteAsync(sqlLeituras, new { TopicoId = topicoId, UsuarioId = usuarioId });
    }

    public async Task<bool> EhParticipanteAsync(Guid topicoId, Guid usuarioId)
    {
        using var conexao = CriarConexao();
        const string sqlVerifica = """
          SELECT COUNT(1) FROM mensagens_participantes
          WHERE topico_id = @TopicoId AND usuario_id = @UsuarioId AND deletada_em IS NULL
          """;
        var quantidade = await conexao.ExecuteScalarAsync<int>(sqlVerifica, new { TopicoId = topicoId, UsuarioId = usuarioId });
        return quantidade > 0;
    }

    public async Task CadastrarMensagemAsync(Guid topicoId, Guid remetenteId, string corpoTexto)
    {
        using var conexao = CriarConexao();
        const string sqlInsert = """
          INSERT INTO mensagens (topico_id, remetente_id, corpo_texto, status, data_envio)
          VALUES (@TopicoId, @RemetenteId, @CorpoTexto, 'enviado', NOW())
          """;

        await conexao.ExecuteAsync(sqlInsert, new
        {
            TopicoId = topicoId,
            RemetenteId = remetenteId,
            CorpoTexto = corpoTexto.Trim()
        });
    }
    public async Task<int> ContarMensagensNaoLidasAsync(Guid usuarioId)
    {
        using var conexao = CriarConexao();
        const string sql = "SELECT COALESCE(SUM(mensagens_nao_lidas_count), 0) FROM mensagens_participantes WHERE usuario_id = @UsuarioId AND deletada_em IS NULL";
        return await conexao.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId });
    }
}

