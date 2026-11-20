using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Gestao;

public class TipoVendedorRepositorio : RepositorioBase, ITipoVendedorRepositorio
{
    public TipoVendedorRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    public async Task CadastrarAsync(TipoVendedor tipoVendedor)
    {
        const string sql = """
            INSERT INTO tipos_vendedor (id, conta_id, nome, descricao, status, data_inclusao, operador_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @ContaId, @Nome, @Descricao, @Ativo, @DataInclusao, @OperadorInclusao, @DataUltimaAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            tipoVendedor.Id,
            tipoVendedor.ContaId,
            tipoVendedor.Nome,
            tipoVendedor.Descricao,
            tipoVendedor.Ativo,
            tipoVendedor.DataInclusao,
            tipoVendedor.OperadorInclusao,
            tipoVendedor.DataUltimaAtualizacao
        });
    }

    public async Task AtualizarAsync(TipoVendedor tipoVendedor)
    {
        const string sql = """
            UPDATE tipos_vendedor
            SET nome = @Nome,
                descricao = @Descricao,
                status = @Ativo,
                data_ultima_atualizacao = @DataUltimaAtualizacao
            WHERE id = @Id AND conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            tipoVendedor.Id,
            tipoVendedor.ContaId,
            tipoVendedor.Nome,
            tipoVendedor.Descricao,
            tipoVendedor.Ativo,
            tipoVendedor.DataUltimaAtualizacao
        });
    }

    public async Task<IEnumerable<TipoVendedor>> ListarPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT id AS Id, conta_id AS ContaId, nome AS Nome, descricao AS Descricao, 
                   status AS Ativo, data_inclusao AS DataInclusao, 
                   operador_inclusao AS OperadorInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM tipos_vendedor
            WHERE conta_id = @ContaId
            ORDER BY nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<TipoVendedor>(sql, new { ContaId = contaId });
    }

    public async Task<TipoVendedor?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        const string sql = """
            SELECT id AS Id, conta_id AS ContaId, nome AS Nome, descricao AS Descricao, 
                   status AS Ativo, data_inclusao AS DataInclusao, 
                   operador_inclusao AS OperadorInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM tipos_vendedor
            WHERE id = @Id AND conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<TipoVendedor>(sql, new { Id = id, ContaId = contaId });
    }
}




