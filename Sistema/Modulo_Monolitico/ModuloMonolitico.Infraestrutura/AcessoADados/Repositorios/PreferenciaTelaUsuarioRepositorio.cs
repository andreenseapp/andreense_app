using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios;

public class PreferenciaTelaUsuarioRepositorio : RepositorioBase, IPreferenciaTelaUsuarioRepositorio
{
    public PreferenciaTelaUsuarioRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task<PreferenciaTelaUsuario?> ObterPreferenciaAsync(Guid contaId, Guid usuarioId, string telaIdentificador)
    {
        const string sql = """
            SELECT 
                id AS Id, 
                conta_id AS ContaId, 
                usuario_id AS UsuarioId, 
                tela_identificador AS TelaIdentificador, 
                colunas_visiveis_json AS ColunasVisiveisJson, 
                atualizado_em AS DataUltimaAtualizacao
            FROM erp_preferencias_tela
            WHERE conta_id = @ContaId 
              AND usuario_id = @UsuarioId 
              AND tela_identificador = @TelaIdentificador
            LIMIT 1
            """;

        await using var conn = CriarConexao();
        return await conn.QueryFirstOrDefaultAsync<PreferenciaTelaUsuario>(sql, new { ContaId = contaId, UsuarioId = usuarioId, TelaIdentificador = telaIdentificador });
    }

    public async Task AtualizarPreferenciaAsync(PreferenciaTelaUsuario preferencia)
    {
        const string sql = """
            INSERT INTO erp_preferencias_tela 
                (id, conta_id, usuario_id, tela_identificador, colunas_visiveis_json, atualizado_em)
            VALUES 
                (@Id, @ContaId, @UsuarioId, @TelaIdentificador, @ColunasVisiveisJson, @DataUltimaAtualizacao)
            ON CONFLICT (conta_id, usuario_id, tela_identificador) 
            DO UPDATE SET 
                colunas_visiveis_json = EXCLUDED.colunas_visiveis_json,
                atualizado_em = EXCLUDED.atualizado_em;
            """;

        await using var conn = CriarConexao();
        await conn.ExecuteAsync(sql, preferencia);
    }
}
