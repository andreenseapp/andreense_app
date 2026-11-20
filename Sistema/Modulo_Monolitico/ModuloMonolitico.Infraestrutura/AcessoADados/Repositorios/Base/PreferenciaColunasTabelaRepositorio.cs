using Dapper;
using ModuloMonolitico.Dominio.Entidades.Base;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Base;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

public class PreferenciaColunasTabelaRepositorio : RepositorioBase, IPreferenciaColunasTabelaRepositorio
{
    public PreferenciaColunasTabelaRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task AtualizarAsync(PreferenciaTelaUsuario preferencia)
    {
        const string sqlVerificar = """
            SELECT COUNT(*) FROM preferencias_tela_usuario 
            WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId AND tela_path = @CaminhoTela AND tabela_id = @TabelaId
            """;

        const string sqlUpdate = """
            UPDATE preferencias_tela_usuario 
            SET colunas_ocultas_json = @ColunasOcultasJson, data_atualizacao = @DataAtualizacao
            WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId AND tela_path = @CaminhoTela AND tabela_id = @TabelaId
            """;

        const string sqlInsert = """
            INSERT INTO preferencias_tela_usuario (id, usuario_id, empresa_id, tela_path, tabela_id, colunas_ocultas_json, data_atualizacao)
            VALUES (@Id, @UsuarioId, @EmpresaId, @CaminhoTela, @TabelaId, @ColunasOcultasJson, @DataAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var existe = await conexao.ExecuteScalarAsync<int>(sqlVerificar, preferencia) > 0;

        if (existe)
        {
            await conexao.ExecuteAsync(sqlUpdate, preferencia);
        }
        else
        {
            await conexao.ExecuteAsync(sqlInsert, preferencia);
        }
    }

    public async Task<PreferenciaTelaUsuario?> ObterPreferenciaAsync(Guid usuarioId, Guid empresaId, string caminhoTela, string tabelaId)
    {
        const string sql = """
            SELECT id AS Id, usuario_id AS UsuarioId, empresa_id AS EmpresaId, 
                   tela_path AS CaminhoTela, tabela_id AS TabelaId, colunas_ocultas_json AS ColunasOcultasJson,
                   data_atualizacao AS DataAtualizacao
            FROM preferencias_tela_usuario
            WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId AND tela_path = @CaminhoTela AND tabela_id = @TabelaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<PreferenciaTelaUsuario>(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId, CaminhoTela = caminhoTela, TabelaId = tabelaId });
    }
}
