using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.ERP;

public class CategoriaRepositorio : RepositorioBase, ICategoriaRepositorio
{
    public CategoriaRepositorio(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<CategoriaDto>> ListarPorContaAsync(Guid contaId)
    {
        using var conexao = CriarConexao();
        return await conexao.QueryAsync<CategoriaDto>(
            """
            SELECT id AS Id, nome AS Nome, descricao AS Descricao, ativo AS Ativo
            FROM erp_produto_categorias
            WHERE conta_id = @ContaId
              AND ativo = true
              AND deleted_at IS NULL
            ORDER BY nome
            """,
            new { ContaId = contaId });
    }
}
