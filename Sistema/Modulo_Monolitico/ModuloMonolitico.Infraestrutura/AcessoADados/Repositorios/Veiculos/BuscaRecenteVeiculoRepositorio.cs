using Dapper;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Veiculos;

public class BuscaRecenteVeiculoRepositorio : RepositorioBase, IBuscaRecenteVeiculoRepositorio
{
    public BuscaRecenteVeiculoRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<IEnumerable<BuscaRecenteVeiculoDto>> ListarPorUsuarioAsync(Guid usuarioId, int limite = 10)
    {
        const string sql = """
            SELECT
                id AS Id,
                chave_filtro AS ChaveFiltro,
                rotulo AS Rotulo,
                filtro_json AS FiltroJson,
                data_ultima_busca AS DataUltimaBusca
            FROM veiculos_buscas_recentes
            WHERE usuario_id = @UsuarioId
            ORDER BY data_ultima_busca DESC
            LIMIT @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<BuscaRecenteVeiculoDto>(sql, new { UsuarioId = usuarioId, Limite = limite });
    }

    public async Task CadastrarOuAtualizarAsync(BuscaRecenteVeiculo busca)
    {
        const string sql = """
            INSERT INTO veiculos_buscas_recentes (id, usuario_id, chave_filtro, filtro_json, rotulo, data_inclusao, data_ultima_busca)
            VALUES (@Id, @UsuarioId, @ChaveFiltro, @FiltroJson, @Rotulo, @DataInclusao, @DataUltimaBusca)
            ON CONFLICT (usuario_id, chave_filtro)
            DO UPDATE SET
                filtro_json = EXCLUDED.filtro_json,
                rotulo = EXCLUDED.rotulo,
                data_ultima_busca = NOW()
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, busca);
    }

    public async Task DeletarFisicoAsync(Guid buscaId, Guid usuarioId)
    {
        const string sql = "DELETE FROM veiculos_buscas_recentes WHERE id = @BuscaId AND usuario_id = @UsuarioId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { BuscaId = buscaId, UsuarioId = usuarioId });
    }

    public async Task DeletarFisicoTodasAsync(Guid usuarioId)
    {
        const string sql = "DELETE FROM veiculos_buscas_recentes WHERE usuario_id = @UsuarioId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId });
    }
}
