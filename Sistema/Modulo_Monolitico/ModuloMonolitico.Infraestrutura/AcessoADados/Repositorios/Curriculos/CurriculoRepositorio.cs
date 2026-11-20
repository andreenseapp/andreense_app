using Dapper;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Curriculos;

public class CurriculoRepositorio : RepositorioBase, ICurriculoRepositorio
{
    public CurriculoRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<ResultadoPaginado<CurriculoResumoDto>> ListarComFiltrosAsync(Guid contaId, FiltroCurriculo filtro)
    {
        var where = new StringBuilder("WHERE c.conta_id = @ContaId AND c.visivel = TRUE AND c.status = 'ativo'");
        var parametros = new DynamicParameters();
        parametros.Add("ContaId", contaId);

        if (!string.IsNullOrWhiteSpace(filtro.AreaAtuacao))
        {
            where.Append(" AND LOWER(c.area_atuacao) = LOWER(@AreaAtuacao)");
            parametros.Add("AreaAtuacao", filtro.AreaAtuacao);
        }

        if (!string.IsNullOrWhiteSpace(filtro.CargoDesejado))
        {
            where.Append(" AND LOWER(c.cargo_desejado) LIKE LOWER(@CargoDesejado)");
            parametros.Add("CargoDesejado", $"%{filtro.CargoDesejado}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nivel))
        {
            where.Append(" AND c.nivel = @Nivel");
            parametros.Add("Nivel", filtro.Nivel);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Modalidade))
        {
            where.Append(" AND c.modalidade = @Modalidade");
            parametros.Add("Modalidade", filtro.Modalidade);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Disponibilidade))
        {
            where.Append(" AND c.disponibilidade = @Disponibilidade");
            parametros.Add("Disponibilidade", filtro.Disponibilidade);
        }

        if (filtro.Pcd.HasValue)
        {
            where.Append(" AND c.pcd = @Pcd");
            parametros.Add("Pcd", filtro.Pcd.Value);
        }

        if (filtro.AceitaViagem.HasValue)
        {
            where.Append(" AND c.aceita_viagem = @AceitaViagem");
            parametros.Add("AceitaViagem", filtro.AceitaViagem.Value);
        }

        if (filtro.AceitaMudanca.HasValue)
        {
            where.Append(" AND c.aceita_mudanca = @AceitaMudanca");
            parametros.Add("AceitaMudanca", filtro.AceitaMudanca.Value);
        }

        if (filtro.PretensaoMinima.HasValue)
        {
            where.Append(" AND c.pretensao_salarial >= @PretensaoMinima");
            parametros.Add("PretensaoMinima", filtro.PretensaoMinima.Value);
        }

        if (filtro.PretensaoMaxima.HasValue)
        {
            where.Append(" AND c.pretensao_salarial <= @PretensaoMaxima");
            parametros.Add("PretensaoMaxima", filtro.PretensaoMaxima.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Idioma))
        {
            where.Append(" AND EXISTS (SELECT 1 FROM curriculo_idiomas ci WHERE ci.curriculo_id = c.id AND LOWER(ci.idioma) = LOWER(@Idioma))");
            parametros.Add("Idioma", filtro.Idioma);
        }

        var sqlCount = $"SELECT COUNT(*) FROM curriculos c {where}";

        var sqlItems = $@"
            SELECT
                c.id               AS Id,
                c.usuario_id       AS UsuarioId,
                u.nome             AS NomeCompleto,
                c.cargo_desejado   AS CargoDesejado,
                c.area_atuacao     AS AreaAtuacao,
                c.nivel            AS Nivel,
                c.modalidade       AS Modalidade,
                c.pretensao_salarial AS PretensaoSalarial,
                c.disponibilidade  AS Disponibilidade,
                c.pcd              AS Pcd,
                u.url_imagem       AS UrlFotoPerfil,
                exp.empresa        AS UltimaEmpresa,
                exp.cargo          AS UltimoCargoOcupado,
                fmr.curso          AS UltimaFormacao,
                c.data_atualizacao AS DataAtualizacao
            FROM curriculos c
            INNER JOIN usuarios u ON u.id = c.usuario_id
            LEFT JOIN LATERAL (
                SELECT empresa, cargo FROM curriculo_experiencias
                WHERE curriculo_id = c.id
                ORDER BY data_inicio DESC LIMIT 1
            ) exp ON TRUE
            LEFT JOIN LATERAL (
                SELECT curso FROM curriculo_formacoes
                WHERE curriculo_id = c.id
                ORDER BY data_inicio DESC LIMIT 1
            ) fmr ON TRUE
            {where}
            ORDER BY c.data_atualizacao DESC
            LIMIT @Limite OFFSET @Offset";

        parametros.Add("Limite", filtro.TamanhoPagina);
        parametros.Add("Offset", filtro.Offset);

        await using var conexao = CriarConexao();
        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var itens = await conexao.QueryAsync<CurriculoResumoDto>(sqlItems, parametros);

        return new ResultadoPaginado<CurriculoResumoDto>(itens, total, filtro.Pagina, filtro.TamanhoPagina);
    }

    public async Task<CurriculoDetalheDto?> ObterDetalhePublicoAsync(Guid contaId, Guid curriculoId)
    {
        const string sqlCurriculo = @"
            SELECT
                c.id               AS Id,
                c.usuario_id       AS UsuarioId,
                u.nome             AS NomeCompleto,
                c.cargo_desejado   AS CargoDesejado,
                c.area_atuacao     AS AreaAtuacao,
                c.nivel            AS Nivel,
                c.modalidade       AS Modalidade,
                c.pretensao_salarial AS PretensaoSalarial,
                c.disponibilidade  AS Disponibilidade,
                c.aceita_viagem    AS AceitaViagem,
                c.aceita_mudanca   AS AceitaMudanca,
                c.resumo_profissional AS ResumoProfissional,
                c.cnh_categorias   AS CnhCategorias,
                c.pcd              AS Pcd,
                c.tipo_deficiencia AS TipoDeficiencia,
                u.url_imagem       AS UrlFotoPerfil,
                ''                 AS Telefone,
                c.data_atualizacao AS DataAtualizacao
            FROM curriculos c
            INNER JOIN usuarios u ON u.id = c.usuario_id
            WHERE c.conta_id = @ContaId AND c.id = @Id AND c.visivel = TRUE AND c.status = 'ativo'";

        await using var conexao = CriarConexao();
        var curriculo = await conexao.QueryFirstOrDefaultAsync<CurriculoDetalheDto>(sqlCurriculo, new { ContaId = contaId, Id = curriculoId });
        if (curriculo is null) return null;

        await CarregarSubEntidesPublicoAsync(conexao, curriculo, curriculoId);
        return curriculo;
    }

    public async Task<CurriculoCompletoDto?> ObterCompletoDoUsuarioAsync(Guid contaId, Guid usuarioId)
    {
        const string sqlCurriculo = @"
            SELECT
                c.id                  AS Id,
                c.cargo_desejado      AS CargoDesejado,
                c.area_atuacao        AS AreaAtuacao,
                c.nivel               AS Nivel,
                c.modalidade          AS Modalidade,
                c.pretensao_salarial  AS PretensaoSalarial,
                c.disponibilidade     AS Disponibilidade,
                c.aceita_viagem       AS AceitaViagem,
                c.aceita_mudanca      AS AceitaMudanca,
                c.resumo_profissional AS ResumoProfissional,
                c.cnh_categorias      AS CnhCategorias,
                c.pcd                 AS Pcd,
                c.tipo_deficiencia    AS TipoDeficiencia,
                c.visivel             AS Visivel,
                c.status              AS Status,
                c.data_atualizacao    AS DataAtualizacao
            FROM curriculos c
            WHERE c.conta_id = @ContaId AND c.usuario_id = @UsuarioId";

        await using var conexao = CriarConexao();
        var curriculo = await conexao.QueryFirstOrDefaultAsync<CurriculoCompletoDto>(sqlCurriculo, new { ContaId = contaId, UsuarioId = usuarioId });
        if (curriculo is null) return null;

        await CarregarSubEntidadesCompletoAsync(conexao, curriculo, curriculo.Id);
        curriculo.PercentualPreenchimento = CalcularPercentual(curriculo);
        return curriculo;
    }

    public async Task<Guid> CadastrarOuObterIdAsync(Guid contaId, Guid usuarioId)
    {
        const string sql = @"
            INSERT INTO curriculos (conta_id, usuario_id)
            VALUES (@ContaId, @UsuarioId)
            ON CONFLICT (conta_id, usuario_id) DO UPDATE SET data_atualizacao = data_atualizacao
            RETURNING id";

        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<Guid>(sql, new { ContaId = contaId, UsuarioId = usuarioId });
    }

    public async Task AtualizarDadosPrincipaisAsync(Curriculo curriculo)
    {
        const string sql = @"
            UPDATE curriculos SET
                cargo_desejado      = @CargoDesejado,
                area_atuacao        = @AreaAtuacao,
                nivel               = @Nivel,
                modalidade          = @Modalidade,
                pretensao_salarial  = @PretensaoSalarial,
                disponibilidade     = @Disponibilidade,
                aceita_viagem       = @AceitaViagem,
                aceita_mudanca      = @AceitaMudanca,
                resumo_profissional = @ResumoProfissional,
                cnh_categorias      = @CnhCategorias,
                pcd                 = @Pcd,
                tipo_deficiencia    = @TipoDeficiencia,
                data_atualizacao    = NOW()
            WHERE conta_id = @ContaId AND usuario_id = @UsuarioId";

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, curriculo);
    }

    public async Task AlterarVisibilidadeAsync(Guid contaId, Guid usuarioId, bool visivel)
    {
        const string sql = "UPDATE curriculos SET visivel = @Visivel, data_atualizacao = NOW() WHERE conta_id = @ContaId AND usuario_id = @UsuarioId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { ContaId = contaId, UsuarioId = usuarioId, Visivel = visivel });
    }

    public async Task<Guid> CadastrarExperienciaAsync(CurriculoExperiencia experiencia)
    {
        const string sql = @"
            INSERT INTO curriculo_experiencias (curriculo_id, conta_id, empresa, cargo, data_inicio, data_fim, em_andamento, descricao, cidade)
            VALUES (@CurriculoId, @ContaId, @Empresa, @Cargo, @DataInicio, @DataFim, @EmAndamento, @Descricao, @Cidade)
            RETURNING id";
        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<Guid>(sql, experiencia);
    }

    public async Task AtualizarExperienciaAsync(CurriculoExperiencia experiencia)
    {
        const string sql = @"
            UPDATE curriculo_experiencias SET
                empresa = @Empresa, cargo = @Cargo, data_inicio = @DataInicio,
                data_fim = @DataFim, em_andamento = @EmAndamento, descricao = @Descricao, cidade = @Cidade
            WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, experiencia);
    }

    public async Task DeletarFisicoExperienciaAsync(Guid contaId, Guid curriculoId, Guid id)
    {
        const string sql = "DELETE FROM curriculo_experiencias WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, CurriculoId = curriculoId, ContaId = contaId });
    }

    public async Task<Guid> CadastrarFormacaoAsync(CurriculoFormacao formacao)
    {
        const string sql = @"
            INSERT INTO curriculo_formacoes (curriculo_id, conta_id, instituicao, curso, grau, data_inicio, data_fim, em_andamento)
            VALUES (@CurriculoId, @ContaId, @Instituicao, @Curso, @Grau, @DataInicio, @DataFim, @EmAndamento)
            RETURNING id";
        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<Guid>(sql, formacao);
    }

    public async Task AtualizarFormacaoAsync(CurriculoFormacao formacao)
    {
        const string sql = @"
            UPDATE curriculo_formacoes SET
                instituicao = @Instituicao, curso = @Curso, grau = @Grau,
                data_inicio = @DataInicio, data_fim = @DataFim, em_andamento = @EmAndamento
            WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, formacao);
    }

    public async Task DeletarFisicoFormacaoAsync(Guid contaId, Guid curriculoId, Guid id)
    {
        const string sql = "DELETE FROM curriculo_formacoes WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, CurriculoId = curriculoId, ContaId = contaId });
    }

    public async Task<Guid> CadastrarCursoAsync(CurriculoCurso curso)
    {
        const string sql = @"
            INSERT INTO curriculo_cursos (curriculo_id, conta_id, nome, instituicao, carga_horaria, ano_conclusao)
            VALUES (@CurriculoId, @ContaId, @Nome, @Instituicao, @CargaHoraria, @AnoConclusao)
            RETURNING id";
        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<Guid>(sql, curso);
    }

    public async Task AtualizarCursoAsync(CurriculoCurso curso)
    {
        const string sql = @"
            UPDATE curriculo_cursos SET nome = @Nome, instituicao = @Instituicao, carga_horaria = @CargaHoraria, ano_conclusao = @AnoConclusao
            WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, curso);
    }

    public async Task DeletarFisicoCursoAsync(Guid contaId, Guid curriculoId, Guid id)
    {
        const string sql = "DELETE FROM curriculo_cursos WHERE id = @Id AND curriculo_id = @CurriculoId AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, CurriculoId = curriculoId, ContaId = contaId });
    }

    public async Task SubstituirIdiomasAsync(Guid curriculoId, Guid contaId, IEnumerable<CurriculoIdioma> idiomas)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("DELETE FROM curriculo_idiomas WHERE curriculo_id = @CurriculoId AND conta_id = @ContaId", new { CurriculoId = curriculoId, ContaId = contaId });
            foreach (var idioma in idiomas)
            {
                await conexao.ExecuteAsync(
                    "INSERT INTO curriculo_idiomas (curriculo_id, conta_id, idioma, nivel) VALUES (@CurriculoId, @ContaId, @Idioma, @Nivel)",
                    new { CurriculoId = curriculoId, ContaId = contaId, idioma.Idioma, idioma.Nivel });
            }
        });
    }

    public async Task SubstituirHabilidadesAsync(Guid curriculoId, Guid contaId, IEnumerable<string> habilidades)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("DELETE FROM curriculo_habilidades WHERE curriculo_id = @CurriculoId AND conta_id = @ContaId", new { CurriculoId = curriculoId, ContaId = contaId });
            foreach (var habilidade in habilidades)
            {
                await conexao.ExecuteAsync(
                    "INSERT INTO curriculo_habilidades (curriculo_id, conta_id, habilidade) VALUES (@CurriculoId, @ContaId, @Habilidade)",
                    new { CurriculoId = curriculoId, ContaId = contaId, Habilidade = habilidade });
            }
        });
    }

    public async Task<ExperienciaDto?> ObterExperienciaPorIdAsync(Guid contaId, Guid id)
    {
        const string sql = @"
            SELECT id AS Id, empresa AS Empresa, cargo AS Cargo, data_inicio AS DataInicio,
                   data_fim AS DataFim, em_andamento AS EmAndamento, descricao AS Descricao, cidade AS Cidade
            FROM curriculo_experiencias WHERE id = @Id AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<ExperienciaDto>(sql, new { Id = id, ContaId = contaId });
    }

    public async Task<FormacaoDto?> ObterFormacaoPorIdAsync(Guid contaId, Guid id)
    {
        const string sql = @"
            SELECT id AS Id, instituicao AS Instituicao, curso AS Curso, grau AS Grau,
                   data_inicio AS DataInicio, data_fim AS DataFim, em_andamento AS EmAndamento
            FROM curriculo_formacoes WHERE id = @Id AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<FormacaoDto>(sql, new { Id = id, ContaId = contaId });
    }

    public async Task<CursoDto?> ObterCursoPorIdAsync(Guid contaId, Guid id)
    {
        const string sql = @"
            SELECT id AS Id, nome AS Nome, instituicao AS Instituicao, carga_horaria AS CargaHoraria, ano_conclusao AS AnoConclusao
            FROM curriculo_cursos WHERE id = @Id AND conta_id = @ContaId";
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<CursoDto>(sql, new { Id = id, ContaId = contaId });
    }

    private static async Task CarregarSubEntidesPublicoAsync(Npgsql.NpgsqlConnection conexao, CurriculoDetalheDto curriculo, Guid curriculoId)
    {
        var experiencias = await conexao.QueryAsync<ExperienciaDto>(@"
            SELECT id AS Id, empresa AS Empresa, cargo AS Cargo, data_inicio AS DataInicio,
                   data_fim AS DataFim, em_andamento AS EmAndamento, descricao AS Descricao, cidade AS Cidade
            FROM curriculo_experiencias WHERE curriculo_id = @CurriculoId ORDER BY data_inicio DESC", new { CurriculoId = curriculoId });
        curriculo.Experiencias = experiencias.ToList();

        var formacoes = await conexao.QueryAsync<FormacaoDto>(@"
            SELECT id AS Id, instituicao AS Instituicao, curso AS Curso, grau AS Grau,
                   data_inicio AS DataInicio, data_fim AS DataFim, em_andamento AS EmAndamento
            FROM curriculo_formacoes WHERE curriculo_id = @CurriculoId ORDER BY data_inicio DESC", new { CurriculoId = curriculoId });
        curriculo.Formacoes = formacoes.ToList();

        var cursos = await conexao.QueryAsync<CursoDto>(@"
            SELECT id AS Id, nome AS Nome, instituicao AS Instituicao, carga_horaria AS CargaHoraria, ano_conclusao AS AnoConclusao
            FROM curriculo_cursos WHERE curriculo_id = @CurriculoId ORDER BY ano_conclusao DESC", new { CurriculoId = curriculoId });
        curriculo.Cursos = cursos.ToList();

        var idiomas = await conexao.QueryAsync<IdiomaDto>(@"
            SELECT id AS Id, idioma AS Idioma, nivel AS Nivel
            FROM curriculo_idiomas WHERE curriculo_id = @CurriculoId", new { CurriculoId = curriculoId });
        curriculo.Idiomas = idiomas.ToList();

        var habilidades = await conexao.QueryAsync<string>(@"
            SELECT habilidade FROM curriculo_habilidades WHERE curriculo_id = @CurriculoId", new { CurriculoId = curriculoId });
        curriculo.Habilidades = habilidades.ToList();
    }

    private static async Task CarregarSubEntidadesCompletoAsync(Npgsql.NpgsqlConnection conexao, CurriculoCompletoDto curriculo, Guid curriculoId)
    {
        var experiencias = await conexao.QueryAsync<ExperienciaDto>(@"
            SELECT id AS Id, empresa AS Empresa, cargo AS Cargo, data_inicio AS DataInicio,
                   data_fim AS DataFim, em_andamento AS EmAndamento, descricao AS Descricao, cidade AS Cidade
            FROM curriculo_experiencias WHERE curriculo_id = @CurriculoId ORDER BY data_inicio DESC", new { CurriculoId = curriculoId });
        curriculo.Experiencias = experiencias.ToList();

        var formacoes = await conexao.QueryAsync<FormacaoDto>(@"
            SELECT id AS Id, instituicao AS Instituicao, curso AS Curso, grau AS Grau,
                   data_inicio AS DataInicio, data_fim AS DataFim, em_andamento AS EmAndamento
            FROM curriculo_formacoes WHERE curriculo_id = @CurriculoId ORDER BY data_inicio DESC", new { CurriculoId = curriculoId });
        curriculo.Formacoes = formacoes.ToList();

        var cursos = await conexao.QueryAsync<CursoDto>(@"
            SELECT id AS Id, nome AS Nome, instituicao AS Instituicao, carga_horaria AS CargaHoraria, ano_conclusao AS AnoConclusao
            FROM curriculo_cursos WHERE curriculo_id = @CurriculoId ORDER BY ano_conclusao DESC", new { CurriculoId = curriculoId });
        curriculo.Cursos = cursos.ToList();

        var idiomas = await conexao.QueryAsync<IdiomaDto>(@"
            SELECT id AS Id, idioma AS Idioma, nivel AS Nivel
            FROM curriculo_idiomas WHERE curriculo_id = @CurriculoId", new { CurriculoId = curriculoId });
        curriculo.Idiomas = idiomas.ToList();

        var habilidades = await conexao.QueryAsync<string>(@"
            SELECT habilidade FROM curriculo_habilidades WHERE curriculo_id = @CurriculoId", new { CurriculoId = curriculoId });
        curriculo.Habilidades = habilidades.ToList();
    }

    private static int CalcularPercentual(CurriculoCompletoDto c)
    {
        var pontos = 0;
        if (!string.IsNullOrWhiteSpace(c.CargoDesejado)) pontos += 15;
        if (!string.IsNullOrWhiteSpace(c.AreaAtuacao)) pontos += 10;
        if (!string.IsNullOrWhiteSpace(c.ResumoProfissional)) pontos += 15;
        if (c.PretensaoSalarial.HasValue) pontos += 10;
        if (c.Experiencias.Count > 0) pontos += 20;
        if (c.Formacoes.Count > 0) pontos += 15;
        if (c.Idiomas.Count > 0) pontos += 10;
        if (c.Habilidades.Count > 0) pontos += 5;
        return Math.Min(pontos, 100);
    }
}
