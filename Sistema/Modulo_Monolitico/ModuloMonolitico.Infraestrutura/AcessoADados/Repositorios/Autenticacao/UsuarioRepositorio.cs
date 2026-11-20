using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using Npgsql;
using ModuloMonolitico.Infraestrutura.AcessoADados.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Infraestrutura.Extensions;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Autenticacao;

public class UsuarioRepositorio : RepositorioBase, IUsuarioRepositorio
{
    private readonly Contexto.DbContext? _dbContext;
    private readonly ILogger<UsuarioRepositorio>? _logger;

    public UsuarioRepositorio(
        string connectionString, 
        Contexto.DbContext? dbContext = null,
        ILogger<UsuarioRepositorio>? logger = null) : base(connectionString)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        const string sql = """
            SELECT id, nome, email, email_validado AS EmailValidado, senha_hash AS SenhaHash, ativo, url_imagem AS UrlImagem
            FROM usuarios
            WHERE lower(email) = lower(@Email)
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, nome, email, email_validado AS EmailValidado, senha_hash AS SenhaHash, ativo, url_imagem AS UrlImagem
            FROM usuarios
            WHERE id = @Id
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Guid?> ObterContaIdPrincipalAsync(Guid usuarioId)
    {
        const string sql = "SELECT conta_id FROM usuarios_contas WHERE usuario_id = @UsuarioId LIMIT 1";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Guid?>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<bool> EmailJaExisteAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM usuarios WHERE email = @Email";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var quantidade = await conexao.ExecuteScalarAsync<int>(sql, new { Email = email });
        return quantidade > 0;
    }

    public async Task CadastrarComContaEEmpresaAsync(Usuario usuario, Conta conta, Empresa empresa)
    {
        var slugPortal = conta.Id.ToString("N")[..12];
        var idEmpresaMaster = Guid.Empty;

        const string sqlUsuario = """
            INSERT INTO usuarios (id, nome, email, senha_hash, ativo, url_imagem)
            VALUES (@Id, @Nome, @Email, @SenhaHash, @Ativo, @UrlImagem)
            """;

        const string sqlConta = """
            INSERT INTO contas (id, portal_slug, nome)
            VALUES (@Id, @IdentificadorPortal, @Nome)
            """;

        const string sqlEmpresa = """
            INSERT INTO empresas (id, conta_id, nome, cnpj)
            VALUES (@Id, @ContaId, @Nome, @Cnpj)
            """;

        const string sqlVinculoConta = """
            INSERT INTO usuarios_contas (id, usuario_id, conta_id, is_titular)
            VALUES (gen_random_uuid(), @UsuarioId, @ContaId, true)
            ON CONFLICT (usuario_id, conta_id) DO NOTHING
            """;

        const string sqlVinculoEmpresa = """
            INSERT INTO usuarios_empresas (id, usuario_id, empresa_id, grupo_id, ativo, data_inclusao)
            VALUES (gen_random_uuid(), @UsuarioId, @EmpresaId, @GrupoId, TRUE, NOW())
            ON CONFLICT (usuario_id, empresa_id) DO NOTHING
            """;

        await using var conexaoLeitura = CriarConexao();
        await conexaoLeitura.OpenAsync();
        var gruposMaster = await conexaoLeitura.QueryAsync<dynamic>(
            "SELECT id, nome, descricao, is_admin, ativo FROM grupos_usuarios WHERE empresa_id = @E",
            new { E = idEmpresaMaster }
        );
        await conexaoLeitura.CloseAsync();

        var idGrupoAdmin = Guid.Empty;
        var gruposNovos = new List<dynamic>();

        foreach (var grupoMaster in gruposMaster)
        {
            var novoId = Guid.NewGuid();
            if (grupoMaster.is_admin) idGrupoAdmin = novoId;
            gruposNovos.Add(new { Id = novoId, OldId = grupoMaster.id, Nome = (string)grupoMaster.nome, Descricao = (string)grupoMaster.descricao, EhAdministrador = (bool)grupoMaster.is_admin, Ativo = (bool)grupoMaster.ativo });
        }

        if (idGrupoAdmin == Guid.Empty)
        {
            idGrupoAdmin = Guid.NewGuid();
            gruposNovos.Add(new { Id = idGrupoAdmin, OldId = Guid.Empty, Nome = "Administrador", Descricao = "Grupo com acesso total ao sistema", EhAdministrador = true, Ativo = true });
        }

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlUsuario, new
            {
                usuario.Id, usuario.Nome, usuario.Email, usuario.SenhaHash, usuario.Ativo, usuario.UrlImagem
            });

            await conexao.ExecuteAsync(sqlConta, new
            {
                conta.Id, IdentificadorPortal = slugPortal, conta.Nome
            });

            await conexao.ExecuteAsync(sqlEmpresa, new
            {
                empresa.Id, empresa.ContaId, empresa.Nome, empresa.Cnpj
            });

            foreach (var g in gruposNovos)
            {
                await conexao.ExecuteAsync("INSERT INTO grupos_usuarios (id, empresa_id, nome, descricao, is_admin, ativo) VALUES (@Id, @E, @N, @D, @I, @A)",
                    new { Id = g.Id, E = empresa.Id, N = g.Nome, D = g.Descricao, I = g.EhAdministrador, A = g.Ativo });

                if ((Guid)g.OldId != Guid.Empty)
                {
                    await conexao.ExecuteAsync(@"
                        INSERT INTO grupos_permissoes (id, grupo_id, recurso_id, data_inclusao, atalho_dashboard)
                        SELECT gen_random_uuid(), @NovoGrupoId, recurso_id, NOW(), atalho_dashboard
                        FROM grupos_permissoes
                        WHERE grupo_id = @GrupoAntigoId
                    ", new { NovoGrupoId = g.Id, GrupoAntigoId = g.OldId });
                }
            }

            await conexao.ExecuteAsync(sqlVinculoConta, new
            {
                UsuarioId = usuario.Id, ContaId = conta.Id
            });

            await conexao.ExecuteAsync(sqlVinculoEmpresa, new
            {
                UsuarioId = usuario.Id, EmpresaId = empresa.Id, GrupoId = idGrupoAdmin
            });
        });
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        const string sql = """
            UPDATE usuarios
            SET nome = @Nome,
                email = @Email,
                senha_hash = @SenhaHash,
                ativo = @Ativo,
                url_imagem = @UrlImagem
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            usuario.Nome,
            usuario.Email,
            usuario.SenhaHash,
            usuario.Ativo,
            usuario.UrlImagem,
            usuario.Id
        });
    }

    public async Task InativarAsync(Guid id)
    {
        const string sql = "UPDATE usuarios SET ativo = false WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id });
    }

    public async Task RedefinirAdministradorAsync(Usuario usuario)
    {
        if (_dbContext == null) throw new InvalidOperationException("DbContext não foi injetado.");
        if (_logger == null) throw new InvalidOperationException("ILogger não foi injetado.");

        var idMaster = Guid.Empty;

        var contaExiste = await _dbContext.Contas.Where(c => c.Id == idMaster).AnyWithLoggingAsync(_logger, nameof(RedefinirAdministradorAsync));
        if (!contaExiste)
        {
            var contaMaster = new Conta
            {
                Id = idMaster,
                IdentificadorPortal = "andreense-master",
                Nome = "Andreense Master",
                Email = "andreense.app@gmail.com"
            };
            _dbContext.Contas.Add(contaMaster);

            var empresaMaster = new Empresa
            {
                Id = idMaster,
                ContaId = idMaster,
                Nome = "Andreense Master",
                RazaoSocial = "Andreense Hub LTDA",
                Ativo = true,
                DataInclusao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow
            };
            _dbContext.Empresas.Add(empresaMaster);
        }

        var vinculoContaExiste = await _dbContext.UsuariosContas
            .Where(uc => uc.UsuarioId == usuario.Id && uc.ContaId == idMaster)
            .AnyWithLoggingAsync(_logger, nameof(RedefinirAdministradorAsync));

        if (!vinculoContaExiste)
        {
            var vinculoConta = new UsuarioConta
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                ContaId = idMaster,
                EhTitular = true,
                Ativo = true
            };
            _dbContext.UsuariosContas.Add(vinculoConta);
        }

        var vinculoEmpresaExiste = await _dbContext.UsuariosEmpresas
            .Where(ue => ue.UsuarioId == usuario.Id && ue.EmpresaId == idMaster)
            .AnyWithLoggingAsync(_logger, nameof(RedefinirAdministradorAsync));

        if (!vinculoEmpresaExiste)
        {
            var vinculoEmpresa = new UsuarioEmpresa
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                EmpresaId = idMaster,
                Ativo = true
            };
            _dbContext.UsuariosEmpresas.Add(vinculoEmpresa);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Usuario>> ListarPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT u.id, u.nome, u.email, u.email_validado AS EmailValidado, u.senha_hash AS SenhaHash, u.ativo, u.url_imagem AS UrlImagem
            FROM usuarios u
            INNER JOIN usuarios_contas uc ON u.id = uc.usuario_id
            WHERE uc.conta_id = @ContaId
            ORDER BY u.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Usuario>(sql, new { ContaId = contaId });
    }

    public async Task<IEnumerable<UsuarioProfissao>> ListarProfissoesAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT id, usuario_id AS UsuarioId, nome
            FROM usuario_profissoes
            WHERE usuario_id = @UsuarioId
            ORDER BY nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<UsuarioProfissao>(sql, new { UsuarioId = usuarioId });
    }

    public async Task CadastrarProfissaoAsync(UsuarioProfissao profissao)
    {
        const string sql = """
            INSERT INTO usuario_profissoes (id, usuario_id, nome)
            VALUES (@Id, @UsuarioId, @Nome)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { profissao.Id, profissao.UsuarioId, profissao.Nome });
    }

    public async Task DeletarFisicoProfissaoAsync(Guid profissaoId, Guid usuarioId)
    {
        const string sql = """
            DELETE FROM usuario_profissoes
            WHERE id = @Id AND usuario_id = @UsuarioId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { Id = profissaoId, UsuarioId = usuarioId });
    }

    public async Task<(IEnumerable<MembroEquipeDto> Itens, int Total)> ListarEquipeDaEmpresaAsync(Guid empresaId, int pagina, int tamanhoPagina)
    {
        const string sqlContagem = "SELECT COUNT(1) FROM usuarios_empresas WHERE empresa_id = @EmpresaId";

        const string sql = """
            SELECT 
                u.id AS UsuarioId, 
                u.nome AS Nome, 
                u.email AS Email, 
                u.url_imagem AS UrlImagem, 
                gu.nome AS Perfil, 
                ue.ativo AS Ativo, 
                ue.data_inclusao AS DataInclusao,
                COALESCE(uc.is_titular, FALSE) AS EhTitular
            FROM usuarios_empresas ue
            INNER JOIN usuarios u ON u.id = ue.usuario_id
            INNER JOIN empresas e ON e.id = ue.empresa_id
            LEFT JOIN usuarios_contas uc ON uc.usuario_id = u.id AND uc.conta_id = e.conta_id
            LEFT JOIN grupos_usuarios gu ON gu.id = ue.grupo_id
            WHERE ue.empresa_id = @EmpresaId
            ORDER BY uc.is_titular DESC, ue.ativo DESC, u.nome
            OFFSET @Deslocamento LIMIT @Limite
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var total = await conexao.ExecuteScalarAsync<int>(sqlContagem, new { EmpresaId = empresaId });
        var itens = await conexao.QueryAsync<MembroEquipeDto>(sql, new {
            EmpresaId = empresaId,
            Deslocamento = (pagina - 1) * tamanhoPagina,
            Limite = tamanhoPagina
        });

        return (itens, total);
    }

    public async Task<IEnumerable<MinhaEmpresaDto>> ListarEmpresasDoUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT 
                e.id AS EmpresaId, 
                e.nome AS EmpresaNome, 
                gu.nome AS Perfil, 
                ue.data_inclusao AS DataInclusao
            FROM usuarios_empresas ue
            INNER JOIN empresas e ON e.id = ue.empresa_id
            LEFT JOIN grupos_usuarios gu ON gu.id = ue.grupo_id
            WHERE ue.usuario_id = @UsuarioId AND ue.ativo = TRUE
            ORDER BY e.nome
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<MinhaEmpresaDto>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<ConviteEquipeDto>> ListarConvitesPendentesDoUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT 
                e.id AS EmpresaId, 
                e.nome AS EmpresaNome, 
                gu.nome AS Perfil, 
                ue.data_inclusao AS DataInclusao
            FROM usuarios_empresas ue
            INNER JOIN empresas e ON e.id = ue.empresa_id
            LEFT JOIN grupos_usuarios gu ON gu.id = ue.grupo_id
            WHERE ue.usuario_id = @UsuarioId AND ue.ativo = FALSE
            ORDER BY ue.data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ConviteEquipeDto>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<bool> ExisteVinculoAsync(Guid usuarioId, Guid empresaId)
    {
        const string sql = "SELECT COUNT(1) FROM usuarios_empresas WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var quantidade = await conexao.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId });
        return quantidade > 0;
    }

    public async Task ConvidarMembroAsync(Guid usuarioId, Guid empresaId, Guid grupoId)
    {
        const string sql = """
            INSERT INTO usuarios_empresas (id, usuario_id, empresa_id, grupo_id, ativo, data_inclusao)
            VALUES (gen_random_uuid(), @UsuarioId, @EmpresaId, @GrupoId, FALSE, NOW())
            ON CONFLICT (usuario_id, empresa_id) DO NOTHING
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId, GrupoId = grupoId });
    }

    public async Task ResponderConviteAsync(Guid usuarioId, Guid empresaId, bool aceitar)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        if (aceitar)
        {
            const string sqlAtualizar = "UPDATE usuarios_empresas SET ativo = TRUE WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId";
            await conexao.ExecuteAsync(sqlAtualizar, new { UsuarioId = usuarioId, EmpresaId = empresaId });
        }
        else
        {
            const string sqlRemover = "DELETE FROM usuarios_empresas WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId AND ativo = FALSE";
            await conexao.ExecuteAsync(sqlRemover, new { UsuarioId = usuarioId, EmpresaId = empresaId });
        }
    }

    public async Task DeletarFisicoMembroAsync(Guid usuarioId, Guid empresaId)
    {
        const string sql = "DELETE FROM usuarios_empresas WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId });
    }

    public async Task AtualizarGrupoMembroAsync(Guid usuarioId, Guid empresaId, Guid novoGrupoId)
    {
        const string sql = "UPDATE usuarios_empresas SET grupo_id = @NovoGrupoId WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { UsuarioId = usuarioId, EmpresaId = empresaId, NovoGrupoId = novoGrupoId });
    }
}
