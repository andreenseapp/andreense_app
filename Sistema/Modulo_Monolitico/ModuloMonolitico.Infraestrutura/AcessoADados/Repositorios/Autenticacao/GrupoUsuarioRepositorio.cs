using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao;

public class GrupoUsuarioRepositorio : RepositorioBase, IGrupoUsuarioRepositorio
{
    public GrupoUsuarioRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task CadastrarAsync(GrupoUsuario grupo)
    {
        const string sql = """
            INSERT INTO grupos_usuarios (id, empresa_id, nome, descricao, is_admin, ativo, data_inclusao, operador_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @EmpresaId, @Nome, @Descricao, @EhAdministrador, @Ativo, @DataInclusao, @OperadorInclusao, @DataUltimaAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            grupo.Id,
            grupo.EmpresaId,
            grupo.Nome,
            grupo.Descricao,
            grupo.EhAdministrador,
            grupo.Ativo,
            grupo.DataInclusao,
            grupo.OperadorInclusao,
            grupo.DataUltimaAtualizacao
        });
    }

    public async Task AtualizarAsync(GrupoUsuario grupo)
    {
        const string sql = """
            UPDATE grupos_usuarios
            SET nome = @Nome,
                descricao = @Descricao,
                ativo = @Ativo,
                data_ultima_atualizacao = @DataUltimaAtualizacao
            WHERE id = @Id AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            grupo.Id,
            grupo.EmpresaId,
            grupo.Nome,
            grupo.Descricao,
            grupo.Ativo,
            grupo.DataUltimaAtualizacao
        });
    }

    public async Task<ResultadoPaginado<GrupoUsuario>> ListarPorEmpresaIdAsync(Guid empresaId, string? ordenarPor = "nome", bool descendente = false, int limite = 50, int pagina = 1)
    {
        var deslocamento = (pagina - 1) * limite;
        const string sqlContagem = "SELECT COUNT(*) FROM grupos_usuarios WHERE empresa_id = @EmpresaId";

        const string sqlBase = """
            SELECT id AS Id, empresa_id AS EmpresaId, nome AS Nome, descricao AS Descricao, 
                   is_admin AS EhAdministrador, ativo AS Ativo, data_inclusao AS DataInclusao, 
                   operador_inclusao AS OperadorInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM grupos_usuarios
            WHERE empresa_id = @EmpresaId
            """;

        var consulta = ModuloMonolitico.Infraestrutura.Extensions.BuilderPaginacaoDapper.ConstruirQuery(sqlBase, ordenarPor, descendente, limite, deslocamento);

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var total = await conexao.ExecuteScalarAsync<int>(sqlContagem, new { EmpresaId = empresaId });
        var itens = await conexao.QueryAsync<GrupoUsuario>(consulta, new { EmpresaId = empresaId });

        return new ResultadoPaginado<GrupoUsuario>(itens, total, pagina, limite);
    }

    public async Task<GrupoUsuario?> ObterPorIdEEmpresaAsync(Guid id, Guid empresaId)
    {
        const string sqlGrupo = """
            SELECT id AS Id, empresa_id AS EmpresaId, nome AS Nome, descricao AS Descricao, 
                   is_admin AS EhAdministrador, ativo AS Ativo, data_inclusao AS DataInclusao, 
                   operador_inclusao AS OperadorInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM grupos_usuarios
            WHERE id = @Id AND empresa_id = @EmpresaId
            """;

        const string sqlRecursos = """
            SELECT r.id AS Id, r.chave AS Chave, r.tipo AS Tipo, r.nome AS Nome, 
                   r.descricao AS Descricao, r.modulo AS Modulo, r.status AS Ativo,
                   gp.atalho_dashboard AS AtalhoDashboard
            FROM recursos_sistema r
            INNER JOIN grupos_permissoes gp ON r.id = gp.recurso_id
            WHERE gp.grupo_id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var grupo = await conexao.QueryFirstOrDefaultAsync<GrupoUsuario>(sqlGrupo, new { Id = id, EmpresaId = empresaId });

        if (grupo != null)
        {
            var recursos = await conexao.QueryAsync<RecursoSistema>(sqlRecursos, new { Id = id });
            grupo.Recursos = recursos.ToList();
        }

        return grupo;
    }

    public async Task<IEnumerable<RecursoSistema>> ListarTodosRecursosAsync()
    {
        const string sql = """
            SELECT id AS Id, chave AS Chave, tipo AS Tipo, nome AS Nome, 
                   descricao AS Descricao, modulo AS Modulo, status AS Ativo
            FROM recursos_sistema
            WHERE status = true
            ORDER BY modulo, nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<RecursoSistema>(sql);
    }

    public async Task VincularPermissoesAsync(Guid grupoId, IEnumerable<Guid> recursoIds, IEnumerable<Guid> atalhoIds)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("DELETE FROM grupos_permissoes WHERE grupo_id = @GrupoId", new { GrupoId = grupoId });

            var listaRecursos = (recursoIds ?? new List<Guid>()).ToList();
            var listaAtalhos = (atalhoIds ?? new List<Guid>()).ToList();
            var todosIds = listaRecursos.Union(listaAtalhos).Distinct().ToList();

            if (todosIds.Any())
            {
                const string sql = """
                    INSERT INTO grupos_permissoes (id, grupo_id, recurso_id, data_inclusao, atalho_dashboard)
                    VALUES (@Id, @GrupoId, @RecursoId, @DataInclusao, @AtalhoDashboard)
                    """;

                foreach (var idRecurso in todosIds)
                {
                    await conexao.ExecuteAsync(sql, new
                    {
                        Id = Guid.NewGuid(),
                        GrupoId = grupoId,
                        RecursoId = idRecurso,
                        DataInclusao = DateTime.UtcNow,
                        AtalhoDashboard = listaAtalhos.Contains(idRecurso)
                    });
                }
            }
        });
    }

    public async Task<bool> UsuarioPossuiPermissaoAsync(Guid usuarioId, Guid empresaId, string chavePermissao)
    {
        const string sql = """
            SELECT 1 
            FROM usuarios_empresas ue
            INNER JOIN grupos_usuarios gu ON ue.grupo_id = gu.id
            LEFT JOIN grupos_permissoes gp ON gu.id = gp.grupo_id
            LEFT JOIN recursos_sistema rs ON gp.recurso_id = rs.id
            WHERE ue.usuario_id = @UsuarioId 
              AND gu.empresa_id = @EmpresaId 
              AND gu.ativo = true
              AND (gu.is_admin = true OR (rs.chave = @Chave AND rs.status = true))
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var resultado = await conexao.QueryFirstOrDefaultAsync<int?>(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId, Chave = chavePermissao });
        return resultado.HasValue;
    }

    public async Task<IEnumerable<MenuItemAtalhoDto>> ListarAtalhosDashboardAsync(Guid usuarioId, Guid empresaId)
    {
        const string sql = """
            SELECT DISTINCT rs.chave AS Chave
            FROM usuarios_empresas ue
            INNER JOIN grupos_usuarios gu ON ue.grupo_id = gu.id
            INNER JOIN grupos_permissoes gp ON gu.id = gp.grupo_id
            INNER JOIN recursos_sistema rs ON gp.recurso_id = rs.id
            WHERE ue.usuario_id = @UsuarioId 
              AND gu.empresa_id = @EmpresaId 
              AND gu.ativo = true
              AND gp.atalho_dashboard = true
              AND rs.status = true
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<MenuItemAtalhoDto>(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId });
    }

    public async Task<int> ContarUsuariosVinculadosAsync(Guid grupoId)
    {
        const string sql = """
            SELECT COUNT(*) 
            FROM usuarios_empresas ue
            INNER JOIN usuarios u ON ue.usuario_id = u.id
            WHERE ue.grupo_id = @GrupoId AND u.ativo = true
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.ExecuteScalarAsync<int>(sql, new { GrupoId = grupoId });
    }

    public async Task DeletarFisicoAsync(Guid id, Guid empresaId)
    {
        const string sqlPermissoes = "DELETE FROM grupos_permissoes WHERE grupo_id = @Id";
        const string sqlGrupo = "DELETE FROM grupos_usuarios WHERE id = @Id AND empresa_id = @EmpresaId";

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlPermissoes, new { Id = id });
            await conexao.ExecuteAsync(sqlGrupo, new { Id = id, EmpresaId = empresaId });
        });
    }
}
