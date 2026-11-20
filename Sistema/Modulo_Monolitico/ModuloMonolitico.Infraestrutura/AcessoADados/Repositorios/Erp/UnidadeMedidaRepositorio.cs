using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.ERP;

public class UnidadeMedidaRepositorio : RepositorioBase, IUnidadeMedidaRepositorio
{
    public UnidadeMedidaRepositorio(string stringDeConexao) : base(stringDeConexao) { }

    public async Task<IEnumerable<UnidadeMedidaDto>> ListarPorContaAsync(Guid contaId)
    {
        using var conexao = CriarConexao();
        return await conexao.QueryAsync<UnidadeMedidaDto>(
            """
            SELECT sigla AS Sigla, nome AS Nome, conta_id AS ContaId, ativo AS Ativo
            FROM erp_unidades_medida
            WHERE (conta_id = @ContaId OR conta_id IS NULL)
              AND deleted_at IS NULL
            ORDER BY sigla
            """,
            new { ContaId = contaId });
    }
}
