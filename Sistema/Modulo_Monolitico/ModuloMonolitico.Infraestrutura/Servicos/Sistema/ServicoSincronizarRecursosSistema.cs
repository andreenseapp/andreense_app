using Dapper;
using ModuloMonolitico.Dominio.Constantes;
using Npgsql;

namespace ModuloMonolitico.Infraestrutura.Servicos.Sistema;
public static class ServicoSincronizarRecursosSistema
{
    public static async Task ExecutarAsync(string connectionString)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            var recursos = RecursosSistemaDefinicoes.Todos;
            const string sqlUpsert = """
                INSERT INTO recursos_sistema (id, chave, nome, modulo, tipo, status, data_inclusao)
                VALUES (gen_random_uuid(), @Chave, @Nome, @Modulo, @Tipo, true, NOW())
                ON CONFLICT (chave) DO UPDATE SET
                    nome   = EXCLUDED.nome,
                    modulo = EXCLUDED.modulo,
                    tipo   = EXCLUDED.tipo,
                    status = true
                """;

            foreach (var r in recursos)
            {
                await conn.ExecuteAsync(sqlUpsert, new { r.Chave, r.Nome, r.Modulo, r.Tipo });
            }
            const string sqlGruposAdmin = """
                SELECT id FROM grupos_usuarios WHERE is_admin = true AND ativo = true
                """;

            var gruposAdminIds = await conn.QueryAsync<Guid>(sqlGruposAdmin);

            const string sqlInserirPermissoesAdmin = """
                INSERT INTO grupos_permissoes (id, grupo_id, recurso_id, data_inclusao, atalho_dashboard)
                SELECT gen_random_uuid(), @GrupoId, rs.id, NOW(), false
                FROM recursos_sistema rs
                WHERE rs.status = true
                  AND NOT EXISTS (
                      SELECT 1 FROM grupos_permissoes gp
                      WHERE gp.grupo_id = @GrupoId AND gp.recurso_id = rs.id
                  )
                """;

            foreach (var grupoId in gruposAdminIds)
            {
                await conn.ExecuteAsync(sqlInserirPermissoesAdmin, new { GrupoId = grupoId });
            }

            Console.WriteLine($"[RecursosSistema] Sincronização concluída. {recursos.Length} recursos verificados.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RecursosSistema] AVISO: Falha na sincronização de recursos: {ex.Message}");
        }
    }
}


