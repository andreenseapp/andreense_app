using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using Npgsql;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Empresas;

public class EmpresaRepositorio : RepositorioBase, IEmpresaRepositorio
{
    public EmpresaRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<Empresa>> ListarPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT id, conta_id AS ContaId, nome, razao_social AS RazaoSocial, cnpj, 
                   inscricao_estadual AS InscricaoEstadual, segmento AS Segmento, cnae_principal AS CnaePrincipal, cep, logradouro, numero, bairro, cidade, uf AS Uf
            FROM empresas
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            ORDER BY nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Empresa>(sql, new { ContaId = contaId });
    }

    public async Task<IEnumerable<Empresa>> ListarTodasAcessiveisAoUsuarioAsync(Guid usuarioId, Guid contaPrincipalId)
    {
        const string sql = """
            SELECT e.id, e.conta_id AS ContaId, e.nome, e.razao_social AS RazaoSocial, e.cnpj,
                   e.inscricao_estadual AS InscricaoEstadual, e.segmento AS Segmento,
                   e.cnae_principal AS CnaePrincipal, e.cep, e.logradouro, e.numero,
                   e.bairro, e.cidade, e.uf AS Uf
            FROM empresas e
            WHERE e.conta_id = @ContaPrincipalId
              AND e.deleted_at IS NULL

            UNION

            SELECT e.id, e.conta_id AS ContaId, e.nome, e.razao_social AS RazaoSocial, e.cnpj,
                   e.inscricao_estadual AS InscricaoEstadual, e.segmento AS Segmento,
                   e.cnae_principal AS CnaePrincipal, e.cep, e.logradouro, e.numero,
                   e.bairro, e.cidade, e.uf AS Uf
            FROM empresas e
            INNER JOIN usuarios_empresas ue ON ue.empresa_id = e.id
            WHERE ue.usuario_id = @UsuarioId
              AND ue.ativo = TRUE
              AND e.conta_id != @ContaPrincipalId
              AND e.deleted_at IS NULL

            ORDER BY nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Empresa>(sql, new { UsuarioId = usuarioId, ContaPrincipalId = contaPrincipalId });
    }

    public async Task<Empresa?> ObterPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, conta_id AS ContaId, nome, razao_social AS RazaoSocial, cnpj
            FROM empresas
            WHERE id = @Id AND deleted_at IS NULL
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Empresa>(sql, new { Id = id });
    }

    public async Task<Empresa?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        const string sql = """
            SELECT id, conta_id AS ContaId, nome, razao_social AS RazaoSocial, cnpj, 
                   inscricao_estadual AS InscricaoEstadual, segmento AS Segmento, cnae_principal AS CnaePrincipal, cep, logradouro, numero, bairro, cidade, uf AS Uf
            FROM empresas
            WHERE id = @Id AND conta_id = @ContaId AND deleted_at IS NULL
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var empresa = await conexao.QueryFirstOrDefaultAsync<Empresa>(sql, new { Id = id, ContaId = contaId });
        if (empresa is not null)
        {
            var sqlCnaes = "SELECT id, empresa_id AS EmpresaId, cnae FROM empresa_cnaes_secundarios WHERE empresa_id = @EmpresaId";
            var cnaes = await conexao.QueryAsync<EmpresaCnaeSecundario>(sqlCnaes, new { EmpresaId = empresa.Id });
            empresa.CnaesSecundarios = cnaes.ToList();
        }
        return empresa;
    }

    public async Task<Empresa?> ObterPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT id, conta_id AS ContaId, nome, razao_social AS RazaoSocial, cnpj, 
                   inscricao_estadual AS InscricaoEstadual, segmento AS Segmento, cnae_principal AS CnaePrincipal, cep, logradouro, numero, bairro, cidade, uf AS Uf
            FROM empresas
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Empresa>(sql, new { ContaId = contaId });
    }

    public async Task AtualizarAsync(Empresa empresa)
    {
        const string sql = """
            UPDATE empresas
            SET nome = @Nome,
                razao_social = @RazaoSocial,
                cnpj = @Cnpj,
                inscricao_estadual = @InscricaoEstadual,
                segmento = @Segmento,
                cnae_principal = @CnaePrincipal,
                cep = @Cep,
                logradouro = @Logradouro,
                numero = @Numero,
                bairro = @Bairro,
                cidade = @Cidade,
                uf = @Uf
            WHERE id = @Id
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sql, new
            {
                empresa.Nome,
                empresa.RazaoSocial,
                empresa.Cnpj,
                empresa.InscricaoEstadual,
                empresa.Segmento,
                empresa.CnaePrincipal,
                empresa.Cep,
                empresa.Logradouro,
                empresa.Numero,
                empresa.Bairro,
                empresa.Cidade,
                empresa.Uf,
                empresa.Id
            });

            await conexao.ExecuteAsync("DELETE FROM empresa_cnaes_secundarios WHERE empresa_id = @EmpresaId", new { EmpresaId = empresa.Id });

            if (empresa.CnaesSecundarios.Any())
            {
                var sqlInserirCnaes = "INSERT INTO empresa_cnaes_secundarios (id, empresa_id, cnae) VALUES (@Id, @EmpresaId, @Cnae)";
                await conexao.ExecuteAsync(sqlInserirCnaes, empresa.CnaesSecundarios);
            }
        });
    }

    public async Task CadastrarAsync(Empresa empresa)
    {
        const string sql = """
            INSERT INTO empresas
                (id, conta_id, nome, razao_social, cnpj, inscricao_estadual,
                 segmento, cnae_principal, cep, logradouro, numero, bairro, cidade, uf)
            VALUES
                (@Id, @ContaId, @Nome, @RazaoSocial, @Cnpj, @InscricaoEstadual,
                 @Segmento, @CnaePrincipal, @Cep, @Logradouro, @Numero, @Bairro, @Cidade, @Uf)
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sql, new
            {
                empresa.Id,
                empresa.ContaId,
                empresa.Nome,
                empresa.RazaoSocial,
                empresa.Cnpj,
                empresa.InscricaoEstadual,
                empresa.Segmento,
                empresa.CnaePrincipal,
                empresa.Cep,
                empresa.Logradouro,
                empresa.Numero,
                empresa.Bairro,
                empresa.Cidade,
                empresa.Uf
            });

            if (empresa.CnaesSecundarios.Any())
            {
                var sqlInserirCnaes = "INSERT INTO empresa_cnaes_secundarios (id, empresa_id, cnae) VALUES (@Id, @EmpresaId, @Cnae)";
                await conexao.ExecuteAsync(sqlInserirCnaes, empresa.CnaesSecundarios);
            }
        });
    }

    public async Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId)
    {
        const string sqlEmpresas = "UPDATE empresas SET deleted_at = NOW() WHERE id = ANY(@Ids) AND conta_id = @ContaId";

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            var idsArray = ids.ToArray();
            await conexao.ExecuteAsync(sqlEmpresas, new { Ids = idsArray, ContaId = contaId });
        });
    }
}
