using Dapper;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class BuscaRecenteRepositorio : RepositorioBase, IBuscaRecenteRepositorio
{
    public BuscaRecenteRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task RegistrarAsync(BuscaRecente busca, int limite)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sqlUpsert = """
                INSERT INTO imobiliaria_buscas_recentes
                    (id, usuario_id, chave_filtro, filtro_json, rotulo, data_inclusao, data_ultima_busca)
                VALUES
                    (@Id, @UsuarioId, @ChaveFiltro, CAST(@FiltroJson AS jsonb), @Rotulo, @DataInclusao, @DataUltimaBusca)
                ON CONFLICT (usuario_id, chave_filtro) DO UPDATE SET
                    filtro_json = EXCLUDED.filtro_json,
                    rotulo = EXCLUDED.rotulo,
                    data_ultima_busca = EXCLUDED.data_ultima_busca
                """;

            await conexao.ExecuteAsync(sqlUpsert, new
            {
                busca.Id,
                busca.UsuarioId,
                busca.ChaveFiltro,
                busca.FiltroJson,
                busca.Rotulo,
                busca.DataInclusao,
                busca.DataUltimaBusca
            });

            const string sqlLimpar = """
                DELETE FROM imobiliaria_buscas_recentes
                WHERE id IN (
                    SELECT id
                    FROM imobiliaria_buscas_recentes
                    WHERE usuario_id = @UsuarioId
                    ORDER BY data_ultima_busca DESC
                    OFFSET @Limite
                )
                """;

            await conexao.ExecuteAsync(sqlLimpar, new { busca.UsuarioId, Limite = limite });
        });
    }

    public async Task<IReadOnlyList<BuscaRecente>> ListarPorUsuarioAsync(Guid usuarioId, int limite)
    {
        const string sql = """
            SELECT
                id AS Id,
                usuario_id AS UsuarioId,
                chave_filtro AS ChaveFiltro,
                filtro_json::text AS FiltroJson,
                rotulo AS Rotulo,
                data_inclusao AS DataInclusao,
                data_ultima_busca AS DataUltimaBusca
            FROM imobiliaria_buscas_recentes
            WHERE usuario_id = @UsuarioId
            ORDER BY data_ultima_busca DESC
            LIMIT @Limite
            """;

        await using var conexao = CriarConexao();
        var itens = await conexao.QueryAsync<BuscaRecente>(sql, new { UsuarioId = usuarioId, Limite = limite });
        return itens.ToList();
    }

    public async Task<bool> DeletarFisicoAsync(Guid id, Guid usuarioId)
    {
        const string sql = """
            DELETE FROM imobiliaria_buscas_recentes
            WHERE id = @Id AND usuario_id = @UsuarioId
            """;

        await using var conexao = CriarConexao();
        var afetados = await conexao.ExecuteAsync(sql, new { Id = id, UsuarioId = usuarioId });
        return afetados > 0;
    }

    public async Task DeletarFisicoTodasAsync(Guid usuarioId)
    {
        const string sql = """
            DELETE FROM imobiliaria_buscas_recentes
            WHERE usuario_id = @UsuarioId
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId });
    }
}
