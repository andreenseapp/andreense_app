using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.SaaS;

public class GrupoUsuarioSaaSRepositorio : RepositorioBase, IGrupoUsuarioSaaSRepositorio
{
    public GrupoUsuarioSaaSRepositorio(string connectionString) : base(connectionString)
    {
    }



    public async Task CadastrarAsync(GrupoUsuarioSaaS grupo)
    {
        const string sql = """
            INSERT INTO grupos_usuarios_saas (id, nome, descricao, ativo, data_inclusao, operador_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @Nome, @Descricao, @Ativo, @DataInclusao, @OperadorInclusao, @DataUltimaAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            grupo.Id,
            grupo.Nome,
            grupo.Descricao,
            grupo.Ativo,
            grupo.DataInclusao,
            grupo.OperadorInclusao,
            grupo.DataUltimaAtualizacao
        });
    }

    public async Task AtualizarAsync(GrupoUsuarioSaaS grupo)
    {
        const string sql = """
            UPDATE grupos_usuarios_saas
            SET nome                    = @Nome,
                descricao               = @Descricao,
                ativo                   = @Ativo,
                data_ultima_atualizacao = @DataUltimaAtualizacao
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            grupo.Nome,
            grupo.Descricao,
            grupo.Ativo,
            grupo.DataUltimaAtualizacao,
            grupo.Id
        });
    }

    public async Task<IEnumerable<GrupoUsuarioSaaS>> ListarTodosAsync()
    {
        const string sql = """
            SELECT g.id               AS Id,
                   g.nome             AS Nome,
                   g.descricao        AS Descricao,
                   g.ativo            AS Ativo,
                   g.data_inclusao    AS DataInclusao,
                   g.operador_inclusao AS OperadorInclusao,
                   g.data_ultima_atualizacao AS DataUltimaAtualizacao,
                   COUNT(ug.usuario_id) AS TotalUsuarios
            FROM grupos_usuarios_saas g
            LEFT JOIN usuarios_grupos_saas ug ON ug.grupo_saas_id = g.id
            GROUP BY g.id, g.nome, g.descricao, g.ativo, g.data_inclusao, g.operador_inclusao, g.data_ultima_atualizacao
            ORDER BY g.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<GrupoUsuarioSaaS>(sql);
    }

    public async Task<GrupoUsuarioSaaS?> ObterPorIdAsync(Guid id)
    {
        const string sqlGrupo = """
            SELECT id AS Id, nome AS Nome, descricao AS Descricao, ativo AS Ativo,
                   data_inclusao AS DataInclusao, operador_inclusao AS OperadorInclusao,
                   data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM grupos_usuarios_saas
            WHERE id = @Id
            """;

        const string sqlRecursos = """
            SELECT r.id AS Id, r.chave AS Chave, r.tipo AS Tipo, r.nome AS Nome,
                   r.descricao AS Descricao, r.modulo AS Modulo, r.status AS Ativo
            FROM recursos_sistema_saas r
            INNER JOIN grupos_permissoes_saas gp ON gp.recurso_saas_id = r.id
            WHERE gp.grupo_saas_id = @GrupoId
            ORDER BY r.modulo, r.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var grupo = await conexao.QueryFirstOrDefaultAsync<GrupoUsuarioSaaS>(sqlGrupo, new { Id = id });

        if (grupo is not null)
        {
            var recursos = await conexao.QueryAsync<RecursoSistemaSaaS>(sqlRecursos, new { GrupoId = id });
            grupo.Recursos = recursos.ToList();
        }

        return grupo;
    }



    public async Task<IEnumerable<RecursoSistemaSaaS>> ListarTodosRecursosAsync()
    {
        const string sql = """
            SELECT id AS Id, chave AS Chave, tipo AS Tipo, nome AS Nome,
                   descricao AS Descricao, modulo AS Modulo, status AS Ativo,
                   data_inclusao AS DataInclusao
            FROM recursos_sistema_saas
            WHERE status = true
            ORDER BY modulo, nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<RecursoSistemaSaaS>(sql);
    }



    public async Task VincularPermissoesAsync(Guid grupoSaaSId, IEnumerable<Guid> recursoSaaSIds, Guid operadorId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "DELETE FROM grupos_permissoes_saas WHERE grupo_saas_id = @GrupoId",
                new { GrupoId = grupoSaaSId });

            var ids = recursoSaaSIds?.Distinct().ToList() ?? new List<Guid>();
            if (ids.Count == 0) return;

            const string sql = """
                INSERT INTO grupos_permissoes_saas (id, grupo_saas_id, recurso_saas_id, data_inclusao, operador_inclusao)
                VALUES (@Id, @GrupoId, @RecursoId, @DataInclusao, @OperadorId)
                """;

            foreach (var recId in ids)
            {
                await conexao.ExecuteAsync(sql, new
                {
                    Id = Guid.NewGuid(),
                    GrupoId = grupoSaaSId,
                    RecursoId = recId,
                    DataInclusao = DateTime.UtcNow,
                    OperadorId = operadorId
                });
            }
        });
    }



    public async Task VincularUsuarioAoGrupoAsync(Guid usuarioId, Guid grupoSaaSId, Guid operadorId)
    {
        const string sql = """
            INSERT INTO usuarios_grupos_saas (id, usuario_id, grupo_saas_id, data_inclusao, operador_inclusao)
            VALUES (@Id, @UsuarioId, @GrupoId, @DataInclusao, @OperadorId)
            ON CONFLICT (usuario_id, grupo_saas_id) DO NOTHING
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            GrupoId = grupoSaaSId,
            DataInclusao = DateTime.UtcNow,
            OperadorId = operadorId
        });
    }

    public async Task DesvincularUsuarioDoGrupoAsync(Guid usuarioId, Guid grupoSaaSId)
    {
        const string sql = """
            DELETE FROM usuarios_grupos_saas
            WHERE usuario_id = @UsuarioId AND grupo_saas_id = @GrupoId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId, GrupoId = grupoSaaSId });
    }

    public async Task<IEnumerable<UsuarioGrupoSaaS>> ListarUsuariosDoGrupoAsync(Guid grupoSaaSId)
    {
        const string sql = """
            SELECT ug.id               AS Id,
                   ug.usuario_id       AS UsuarioId,
                   ug.grupo_saas_id    AS GrupoSaaSId,
                   ug.data_inclusao    AS DataInclusao,
                   ug.operador_inclusao AS OperadorInclusao,
                   u.nome              AS NomeUsuario,
                   u.email             AS EmailUsuario,
                   g.nome              AS NomeGrupo
            FROM usuarios_grupos_saas ug
            INNER JOIN usuarios u ON u.id = ug.usuario_id
            INNER JOIN grupos_usuarios_saas g ON g.id = ug.grupo_saas_id
            WHERE ug.grupo_saas_id = @GrupoId
            ORDER BY u.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<UsuarioGrupoSaaS>(sql, new { GrupoId = grupoSaaSId });
    }

    public async Task<IEnumerable<GrupoUsuarioSaaS>> ListarGruposDoUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT g.id AS Id, g.nome AS Nome, g.descricao AS Descricao, g.ativo AS Ativo,
                   g.data_inclusao AS DataInclusao, g.data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM grupos_usuarios_saas g
            INNER JOIN usuarios_grupos_saas ug ON ug.grupo_saas_id = g.id
            WHERE ug.usuario_id = @UsuarioId AND g.ativo = true
            ORDER BY g.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<GrupoUsuarioSaaS>(sql, new { UsuarioId = usuarioId });
    }



    public async Task<bool> UsuarioPossuiPermissaoAsync(Guid usuarioId, string chavePermissao)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM usuarios_grupos_saas ugs
            INNER JOIN grupos_usuarios_saas   gus ON gus.id = ugs.grupo_saas_id
            INNER JOIN grupos_permissoes_saas gps ON gps.grupo_saas_id = gus.id
            INNER JOIN recursos_sistema_saas  rss ON rss.id = gps.recurso_saas_id
            WHERE ugs.usuario_id    = @UsuarioId
              AND rss.chave         = @Chave
              AND rss.status        = true
              AND gus.ativo         = true
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var count = await conexao.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId, Chave = chavePermissao });
        return count > 0;
    }

    public async Task<IEnumerable<string>> ListarPermissoesDoUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT DISTINCT rss.chave
            FROM usuarios_grupos_saas ugs
            INNER JOIN grupos_usuarios_saas   gus ON gus.id = ugs.grupo_saas_id
            INNER JOIN grupos_permissoes_saas gps ON gps.grupo_saas_id = gus.id
            INNER JOIN recursos_sistema_saas  rss ON rss.id = gps.recurso_saas_id
            WHERE ugs.usuario_id = @UsuarioId
              AND rss.status     = true
              AND gus.ativo      = true
            ORDER BY rss.chave
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<string>(sql, new { UsuarioId = usuarioId });
    }
}




