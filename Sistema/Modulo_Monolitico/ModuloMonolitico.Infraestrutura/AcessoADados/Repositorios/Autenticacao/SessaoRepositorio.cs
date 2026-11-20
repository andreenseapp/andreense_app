using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using Npgsql;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao;

public class SessaoRepositorio : RepositorioBase, ISessaoRepositorio
{
    public SessaoRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task CadastrarAsync(SessaoUsuario sessao)
    {
        const string sql = """
            INSERT INTO sessoes_usuario
                (id, usuario_id, conta_id, refresh_token_hash, fingerprint_hash,
                 user_agent, ip_address, plataforma, ativa, data_inclusao, data_expiracao)
            VALUES
                (@Id, @UsuarioId, @ContaId, @HashTokenRenovacao, @HashImpressaoDigital,
                 @AgenteUsuario, @EnderecoIp, @Plataforma, @Ativa, @DataInclusao, @DataExpiracao)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, sessao);
    }

    public async Task<SessaoUsuario?> ObterPorHashRenovacaoAsync(string hashTokenRenovacao)
    {
        const string sql = """
            SELECT id, usuario_id AS UsuarioId, conta_id AS ContaId, 
                   refresh_token_hash AS HashTokenRenovacao, fingerprint_hash AS HashImpressaoDigital,
                   user_agent AS AgenteUsuario, ip_address AS EnderecoIp, plataforma AS Plataforma,
                   ativa AS Ativa, data_inclusao AS DataInclusao, data_expiracao AS DataExpiracao,
                   data_ultimo_uso AS DataUltimoUso, data_exclusao AS DataExclusao
            FROM sessoes_usuario
            WHERE refresh_token_hash = @Hash AND ativa = true
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<SessaoUsuario>(sql, new { Hash = hashTokenRenovacao });
    }

    public async Task RevogarAsync(Guid sessaoId)
    {
        const string sql = "UPDATE sessoes_usuario SET ativa = false, data_exclusao = now() WHERE id = @Id AND ativa = true";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = sessaoId });
    }

    public async Task RevogarTodasDoUsuarioAsync(Guid usuarioId)
    {
        const string sql = "UPDATE sessoes_usuario SET ativa = false, data_exclusao = now() WHERE usuario_id = @UsuarioId AND ativa = true";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId });
    }

    public async Task AtualizarUltimoUsoAsync(Guid sessaoId)
    {
        const string sql = "UPDATE sessoes_usuario SET data_ultimo_uso = now() WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = sessaoId });
    }
}
