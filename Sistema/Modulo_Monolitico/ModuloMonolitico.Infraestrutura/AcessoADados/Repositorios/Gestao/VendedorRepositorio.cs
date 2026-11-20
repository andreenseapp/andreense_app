using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Gestao;

public class VendedorRepositorio : RepositorioBase, IVendedorRepositorio
{
    public VendedorRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    public async Task CadastrarAsync(Vendedor vendedor)
    {
        const string sql = """
            INSERT INTO vendedores (id, conta_id, usuario_id, tipo_vendedor_id, nome_fantasia, observacao, status, data_inclusao, operador_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @ContaId, @UsuarioId, @TipoVendedorId, @NomeFantasia, @Observacao, @Ativo, @DataInclusao, @OperadorInclusao, @DataUltimaAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            vendedor.Id,
            vendedor.ContaId,
            vendedor.UsuarioId,
            vendedor.TipoVendedorId,
            vendedor.NomeFantasia,
            vendedor.Observacao,
            vendedor.Ativo,
            vendedor.DataInclusao,
            vendedor.OperadorInclusao,
            vendedor.DataUltimaAtualizacao
        });
    }

    public async Task AtualizarAsync(Vendedor vendedor)
    {
        const string sql = """
            UPDATE vendedores
            SET usuario_id = @UsuarioId,
                tipo_vendedor_id = @TipoVendedorId,
                nome_fantasia = @NomeFantasia,
                observacao = @Observacao,
                status = @Ativo,
                data_ultima_atualizacao = @DataUltimaAtualizacao
            WHERE id = @Id AND conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            vendedor.Id,
            vendedor.ContaId,
            vendedor.UsuarioId,
            vendedor.TipoVendedorId,
            vendedor.NomeFantasia,
            vendedor.Observacao,
            vendedor.Ativo,
            vendedor.DataUltimaAtualizacao
        });
    }

    public async Task<IEnumerable<Vendedor>> ListarPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT 
                v.id AS Id, v.conta_id AS ContaId, v.usuario_id AS UsuarioId, v.tipo_vendedor_id AS TipoVendedorId, 
                v.nome_fantasia AS NomeFantasia, v.observacao AS Observacao, v.status AS Ativo, 
                v.data_inclusao AS DataInclusao, v.operador_inclusao AS OperadorInclusao, v.data_ultima_atualizacao AS DataUltimaAtualizacao,
                t.id AS Id, t.nome AS Nome, t.status AS Ativo
            FROM vendedores v
            INNER JOIN tipos_vendedor t ON v.tipo_vendedor_id = t.id
            WHERE v.conta_id = @ContaId AND v.deleted_at IS NULL
            ORDER BY v.nome_fantasia
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        
        var resultado = await conexao.QueryAsync<Vendedor, TipoVendedor, Vendedor>(sql, (v, t) =>
        {
            v.TipoVendedor = t;
            return v;
        }, new { ContaId = contaId }, splitOn: "Id");
        
        return resultado;
    }

    public async Task<Vendedor?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        const string sql = """
            SELECT 
                v.id AS Id, v.conta_id AS ContaId, v.usuario_id AS UsuarioId, v.tipo_vendedor_id AS TipoVendedorId, 
                v.nome_fantasia AS NomeFantasia, v.observacao AS Observacao, v.status AS Ativo, 
                v.data_inclusao AS DataInclusao, v.operador_inclusao AS OperadorInclusao, v.data_ultima_atualizacao AS DataUltimaAtualizacao,
                t.id AS Id, t.nome AS Nome, t.status AS Ativo
            FROM vendedores v
            INNER JOIN tipos_vendedor t ON v.tipo_vendedor_id = t.id
            WHERE v.id = @Id AND v.conta_id = @ContaId AND v.deleted_at IS NULL
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        
        var resultado = await conexao.QueryAsync<Vendedor, TipoVendedor, Vendedor>(sql, (v, t) =>
        {
            v.TipoVendedor = t;
            return v;
        }, new { Id = id, ContaId = contaId }, splitOn: "Id");
        
        return resultado.FirstOrDefault();
    }

    public async Task<ResultadoPaginado<Vendedor>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? ordenarPor = null, string? ordenarDirecao = null)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;
        var direcao = string.Equals(ordenarDirecao, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var clausulaOrdem = ordenarPor?.ToLower() switch
        {
            "tipo" => $"t.nome {direcao}",
            "ativo" => $"v.status {direcao}",
            _ => $"v.nome_fantasia {direcao}"
        };

        var where = "v.conta_id = @ContaId AND v.deleted_at IS NULL";
        if (!string.IsNullOrWhiteSpace(busca))
            where += " AND (v.nome_fantasia ILIKE @Busca OR t.nome ILIKE @Busca)";

        var sqlBase = $"""
            FROM vendedores v
            INNER JOIN tipos_vendedor t ON v.tipo_vendedor_id = t.id
            WHERE {where}
            """;

        var sqlCount = $"SELECT COUNT(1) {sqlBase}";
        var sqlItens = $"""
            SELECT 
                v.id AS Id, v.conta_id AS ContaId, v.usuario_id AS UsuarioId, v.tipo_vendedor_id AS TipoVendedorId, 
                v.nome_fantasia AS NomeFantasia, v.observacao AS Observacao, v.status AS Ativo, 
                v.data_inclusao AS DataInclusao, v.operador_inclusao AS OperadorInclusao, v.data_ultima_atualizacao AS DataUltimaAtualizacao,
                t.id AS Id, t.nome AS Nome, t.status AS Ativo
            {sqlBase}
            ORDER BY {clausulaOrdem}
            LIMIT @TamanhoPagina OFFSET @Offset
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var parametros = new
        {
            ContaId = contaId,
            Busca = $"%{busca}%",
            TamanhoPagina = tamanhoPagina,
            Offset = deslocamento
        };

        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var itens = await conexao.QueryAsync<Vendedor, TipoVendedor, Vendedor>(sqlItens, (v, t) =>
        {
            v.TipoVendedor = t;
            return v;
        }, parametros, splitOn: "Id");

        return new ResultadoPaginado<Vendedor>(itens, total, pagina, tamanhoPagina);
    }

    public async Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId)
    {
        const string sql = "UPDATE vendedores SET deleted_at = NOW() WHERE id = ANY(@Ids) AND conta_id = @ContaId";
        
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { Ids = ids.ToArray(), ContaId = contaId });
    }
}




