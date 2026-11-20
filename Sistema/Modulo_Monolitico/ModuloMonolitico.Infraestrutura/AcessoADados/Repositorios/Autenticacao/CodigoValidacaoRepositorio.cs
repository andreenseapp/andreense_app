using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao
{
    public class CodigoValidacaoRepositorio : RepositorioBase, ICodigoValidacaoRepositorio
    {
        public CodigoValidacaoRepositorio(string connectionString) : base(connectionString)
        {
        }

        public async Task CadastrarAsync(string email, string codigo, string tipo, DateTime expiraEm)
        {
            const string sql = """
                INSERT INTO codigos_validacao (email, codigo, tipo, expira_em)
                VALUES (@Email, @Codigo, @Tipo, @ExpiraEm)
                """;

            await using var conexao = CriarConexao();
            await conexao.ExecuteAsync(sql, new { Email = email, Codigo = codigo, Tipo = tipo, ExpiraEm = expiraEm });
        }

        public async Task<bool> ValidarAsync(string email, string codigo, string tipo)
        {
            const string sql = """
                UPDATE codigos_validacao
                SET usado = true
                WHERE id = (
                    SELECT id FROM codigos_validacao
                    WHERE email = @Email 
                      AND codigo = @Codigo 
                      AND tipo = @Tipo 
                      AND usado = false 
                      AND expira_em > now()
                    ORDER BY data_inclusao DESC
                    LIMIT 1
                )
                RETURNING true
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email, Codigo = codigo, Tipo = tipo });
        }

        public async Task<bool> ExisteCodigoValidoAsync(string email, string codigo, string tipo)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1 FROM codigos_validacao
                    WHERE email = @Email
                      AND codigo = @Codigo
                      AND tipo = @Tipo
                      AND usado = false
                      AND expira_em > now()
                )
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email, Codigo = codigo, Tipo = tipo });
        }

        public async Task<bool> PossuiCodigoPendenteAsync(string email, string tipo)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1 FROM codigos_validacao
                    WHERE email = @Email
                      AND tipo = @Tipo
                      AND usado = false
                      AND expira_em > now()
                )
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email, Tipo = tipo });
        }

        public async Task<string?> ObterCodigoPendenteMaisRecenteAsync(string email, string tipo)
        {
            const string sql = """
                SELECT codigo FROM codigos_validacao
                WHERE email = @Email
                  AND tipo = @Tipo
                  AND usado = false
                  AND expira_em > now()
                ORDER BY data_inclusao DESC
                LIMIT 1
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryFirstOrDefaultAsync<string?>(sql, new { Email = email, Tipo = tipo });
        }

        public async Task MarcarComoValidadoAsync(string email)
        {
            const string sql = "UPDATE usuarios SET email_validado = true WHERE email = @Email";
            await using var conexao = CriarConexao();
            await conexao.ExecuteAsync(sql, new { Email = email });
        }
    }
}
