using Dapper;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Veiculos;

public class VeiculoFavoritoRepositorio : RepositorioBase, IVeiculoFavoritoRepositorio
{
    public VeiculoFavoritoRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<bool> EhFavoritadoAsync(Guid usuarioId, Guid anuncioVeiculoId)
    {
        const string sql = "SELECT COUNT(1) FROM veiculos_favoritos WHERE usuario_id = @UsuarioId AND anuncio_veiculo_id = @AnuncioVeiculoId";
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstAsync<int>(sql, new { UsuarioId = usuarioId, AnuncioVeiculoId = anuncioVeiculoId }) > 0;
    }

    public async Task FavoritarAsync(VeiculoFavorito favorito)
    {
        const string sql = """
            INSERT INTO veiculos_favoritos (usuario_id, anuncio_veiculo_id, data_criacao)
            VALUES (@UsuarioId, @AnuncioVeiculoId, @DataCriacao)
            ON CONFLICT DO NOTHING
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, favorito);
    }

    public async Task DesfavoritarAsync(Guid usuarioId, Guid anuncioVeiculoId)
    {
        const string sql = "DELETE FROM veiculos_favoritos WHERE usuario_id = @UsuarioId AND anuncio_veiculo_id = @AnuncioVeiculoId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId, AnuncioVeiculoId = anuncioVeiculoId });
    }
}
