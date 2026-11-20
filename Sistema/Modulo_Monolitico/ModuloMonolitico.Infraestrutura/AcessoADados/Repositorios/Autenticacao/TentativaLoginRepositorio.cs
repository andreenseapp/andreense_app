using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao;

public class TentativaLoginRepositorio : RepositorioBase, ITentativaLoginRepositorio
{
    public TentativaLoginRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task RegistrarAsync(RegistroTentativaLogin registro)
    {
        const string sql = """
            INSERT INTO tentativas_login
                (email, ip_address, user_agent, provedor, sucesso, motivo_falha)
            VALUES
                (@Email, @EnderecoIp, @AgenteUsuario, @Provedor, @Sucesso, @MotivoFalha)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new
        {
            registro.Email,
            registro.EnderecoIp,
            registro.AgenteUsuario,
            registro.Provedor,
            registro.Sucesso,
            registro.MotivoFalha
        });
    }

    public async Task<int> ContarFalhasRecentesAsync(string email, string ip, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE email = @Email
              AND ip_address = @Ip
              AND sucesso = false
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Email = email, Ip = ip, Limite = limite });
    }

    public async Task<int> ContarFalhasRecentesPorIpAsync(string ip, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE ip_address = @Ip
              AND sucesso = false
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Ip = ip, Limite = limite });
    }

    public async Task<int> ContarSolicitacoesRecentesAsync(string email, string ip, string provedor, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE email = @Email
              AND ip_address = @Ip
              AND provedor = @Provedor
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Email = email, Ip = ip, Provedor = provedor, Limite = limite });
    }

    public async Task<int> ContarSolicitacoesRecentesPorIpAsync(string ip, string provedor, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE ip_address = @Ip
              AND provedor = @Provedor
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Ip = ip, Provedor = provedor, Limite = limite });
    }

    public async Task<int> ContarFalhasRecentesPorProvedorAsync(string email, string ip, string provedor, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE email = @Email
              AND ip_address = @Ip
              AND provedor = @Provedor
              AND sucesso = false
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Email = email, Ip = ip, Provedor = provedor, Limite = limite });
    }

    public async Task<int> ContarFalhasRecentesPorIpEProvedorAsync(string ip, string provedor, TimeSpan janela)
    {
        var limite = DateTimeOffset.UtcNow.Subtract(janela);

        const string sql = """
            SELECT COUNT(1)
            FROM tentativas_login
            WHERE ip_address = @Ip
              AND provedor = @Provedor
              AND sucesso = false
              AND data_inclusao >= @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { Ip = ip, Provedor = provedor, Limite = limite });
    }

    public async Task<ResultadoPaginado<TentativaLoginListagemDto>> ListarPaginadoAsync(int pagina, int tamanhoPagina, string? filtroEmail = null)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;

        var sqlBase = "FROM tentativas_login WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(filtroEmail))
        {
            sqlBase += " AND email ILIKE @FiltroEmail";
        }

        var sqlTotal = $"SELECT COUNT(1) {sqlBase}";
        var sqlItens = $"""
            SELECT t.id, u.id as UsuarioId, t.email, t.ip_address as EnderecoIp, t.user_agent as AgenteUsuario, 
                   t.provedor, t.sucesso, t.motivo_falha as MotivoFalha, t.data_inclusao as DataInclusao
            FROM tentativas_login t
            LEFT JOIN usuarios u ON u.email = t.email
            WHERE 1=1
            """;
        if (!string.IsNullOrWhiteSpace(filtroEmail))
        {
            sqlItens += " AND t.email ILIKE @FiltroEmail";
        }
        sqlItens += """
            
            ORDER BY t.data_inclusao DESC
            LIMIT @TamanhoPagina OFFSET @Deslocamento
            """;

        var parametros = new
        {
            FiltroEmail = $"%{filtroEmail}%",
            TamanhoPagina = tamanhoPagina,
            Deslocamento = deslocamento
        };

        await using var conexao = CriarConexao();
        var total = await conexao.ExecuteScalarAsync<int>(sqlTotal, parametros);
        var itens = await conexao.QueryAsync<TentativaLoginListagemDto>(sqlItens, parametros);

        return new ResultadoPaginado<TentativaLoginListagemDto>(itens, total, pagina, tamanhoPagina);
    }

    public async Task<IEnumerable<TentativaLoginListagemDto>> ListarUltimosConectadosAsync(int quantidade = 50)
    {
        const string sql = """
            SELECT * FROM (
                SELECT DISTINCT ON (t.email)
                       t.id, u.id as UsuarioId, t.email, t.ip_address as EnderecoIp, t.user_agent as AgenteUsuario, 
                       t.provedor, t.sucesso, t.motivo_falha as MotivoFalha, t.data_inclusao as DataInclusao
                FROM tentativas_login t
                LEFT JOIN usuarios u ON u.email = t.email
                WHERE t.sucesso = true
                ORDER BY t.email, t.data_inclusao DESC
            ) t
            ORDER BY DataInclusao DESC
            LIMIT @Quantidade
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<TentativaLoginListagemDto>(sql, new { Quantidade = quantidade });
    }
}
