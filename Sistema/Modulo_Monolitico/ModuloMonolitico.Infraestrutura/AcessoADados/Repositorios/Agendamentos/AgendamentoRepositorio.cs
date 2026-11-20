using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Agendamentos;

public class AgendamentoRepositorio : RepositorioBase, IAgendamentoRepositorio, IAgendamentoQueries
{
    public AgendamentoRepositorio(string conexaoString) : base(conexaoString) { }
    public async Task<IEnumerable<ServicoAgendavel>> ListarServicosDaContaAsync(Guid contaId, bool apenasAtivos = true)
    {
        var sql = $"""
            SELECT pv.id AS Id, pv.produto_id AS ProdutoId, pv.conta_id AS ContaId,
                   CASE WHEN pv.descricao = 'Padrão' OR pv.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || pv.descricao END AS Descricao,
                   pv.preco_venda AS PrecoVenda, pv.duracao_minutos AS DuracaoMinutos, pv.cor_hex AS CorHexadecimal, pv.ativo AS Ativo
            FROM erp_produtos_variacoes pv
            INNER JOIN erp_produtos p ON p.id = pv.produto_id
            WHERE p.conta_id = @ContaId AND p.tipo_item = 'servico' AND p.deleted_at IS NULL
            {(apenasAtivos ? "AND pv.ativo = true AND p.ativo = true" : "")}
            ORDER BY p.descricao, pv.descricao
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ServicoAgendavel>(sql, new { ContaId = contaId });
    }
            public async Task<IEnumerable<Prestador>> ListarPrestadoresAsync(Guid empresaId, Guid contaId, bool apenasAtivos = true, Guid? produtoVariacaoId = null)
            {
                var filtroServico = produtoVariacaoId.HasValue
                    ? """
                      AND (
                          NOT EXISTS (
                              SELECT 1 FROM agendamento_prestador_servico ps0
                              WHERE ps0.empresa_id = @EmpresaId
                                AND (
                                    (p."UsuarioId" IS NOT NULL AND ps0.usuario_id = p."UsuarioId") OR
                                    (p."ContatoId" IS NOT NULL AND ps0.contato_id = p."ContatoId")
                                )
                          )
                          OR EXISTS (
                              SELECT 1 FROM agendamento_prestador_servico ps
                              WHERE ps.empresa_id = @EmpresaId
                                AND ps.produto_variacao_id = @ProdutoVariacaoId
                                AND (
                                    (p."UsuarioId" IS NOT NULL AND ps.usuario_id = p."UsuarioId") OR
                                    (p."ContatoId" IS NOT NULL AND ps.contato_id = p."ContatoId")
                                )
                          )
                      )
                      """
                    : string.Empty;
                var sql = $"""
                    WITH Prestadores AS (
                        -- 1. Usuários vinculados à empresa
                        SELECT u.id AS "UsuarioId", NULL::uuid AS "ContatoId", u.nome AS "NomeExibicao", u.email AS "Email"
                        FROM usuarios u
                        INNER JOIN usuarios_empresas ue ON ue.usuario_id = u.id
                        WHERE ue.empresa_id = @EmpresaId
                
                        UNION
                
                        -- 2. Contatos com papel de funcionário (Pessoa Física)
                        SELECT NULL::uuid AS "UsuarioId", c.id AS "ContatoId", c.nome AS "NomeExibicao", c.email AS "Email"
                        FROM contatos_pessoas_fisicas c
                        INNER JOIN empresas_funcionarios ef ON ef.contato_id = c.id
                        WHERE c.conta_id = @ContaId AND ef.empresa_id = @EmpresaId
                
                        UNION
                
                        -- 3. Contatos com papel de funcionário (Pessoa Jurídica)
                        SELECT NULL::uuid AS "UsuarioId", c.id AS "ContatoId", c.razao_social AS "NomeExibicao", c.email AS "Email"
                        FROM contatos_pessoas_juridicas c
                        INNER JOIN empresas_funcionarios ef ON ef.contato_id = c.id
                        WHERE c.conta_id = @ContaId AND ef.empresa_id = @EmpresaId
                    )
                    SELECT p."UsuarioId", p."ContatoId", p."NomeExibicao", p."Email"
                    FROM Prestadores p
                    WHERE 1 = 1
                    {filtroServico}
                    ORDER BY p."NomeExibicao"
                    """;
            
                await using var conexao = CriarConexao();
                await conexao.OpenAsync();
                var prestadores = (await conexao.QueryAsync<Prestador>(sql, new { EmpresaId = empresaId, ContaId = contaId, ProdutoVariacaoId = produtoVariacaoId })).ToList();

        if (prestadores.Any())
        {
            const string sqlServicos = """
                SELECT ps.usuario_id AS UsuarioId, ps.contato_id AS ContatoId,
                       pv.id AS Id, pv.produto_id AS ProdutoId, pv.conta_id AS ContaId,
                       CASE WHEN pv.descricao = 'Padrão' OR pv.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || pv.descricao END AS Descricao,
                       pv.preco_venda AS PrecoVenda, pv.duracao_minutos AS DuracaoMinutos, pv.cor_hex AS CorHexadecimal
                FROM agendamento_prestador_servico ps
                INNER JOIN erp_produtos_variacoes pv ON pv.id = ps.produto_variacao_id
                INNER JOIN erp_produtos p ON p.id = pv.produto_id
                WHERE ps.empresa_id = @EmpresaId
                """;
            var servicosVinculados = (await conexao.QueryAsync<PrestadorServicoDto>(sqlServicos, new { EmpresaId = empresaId })).ToList();

            foreach (var prestador in prestadores)
            {
                prestador.Servicos = servicosVinculados
                    .Where(s => (s.UsuarioId.HasValue && prestador.UsuarioId.HasValue && s.UsuarioId.Value == prestador.UsuarioId.Value) ||
                                (s.ContatoId.HasValue && prestador.ContatoId.HasValue && s.ContatoId.Value == prestador.ContatoId.Value))
                    .Select(s => new ServicoAgendavel
                    {
                        Id         = s.Id,
                        ProdutoId  = s.ProdutoId,
                        ContaId    = s.ContaId,
                        Descricao  = s.Descricao,
                        PrecoVenda = s.PrecoVenda,
                        DuracaoMinutos = s.DuracaoMinutos,
                        CorHexadecimal     = s.CorHexadecimal
                    }).ToList();
            }
        }
        return prestadores;
    }

    public async Task VincularServicosAoPrestadorAsync(Guid empresaId, Guid? usuarioId, Guid? contatoId, IEnumerable<Guid> produtoVariacaoIds)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            if (usuarioId.HasValue)
                await conexao.ExecuteAsync("DELETE FROM agendamento_prestador_servico WHERE empresa_id = @EmpresaId AND usuario_id = @UsuarioId", new { EmpresaId = empresaId, UsuarioId = usuarioId });
            else if (contatoId.HasValue)
                await conexao.ExecuteAsync("DELETE FROM agendamento_prestador_servico WHERE empresa_id = @EmpresaId AND contato_id = @ContatoId", new { EmpresaId = empresaId, ContatoId = contatoId });

            if (produtoVariacaoIds != null && produtoVariacaoIds.Any())
            {
                const string sql = """
                    INSERT INTO agendamento_prestador_servico (empresa_id, usuario_id, contato_id, produto_variacao_id)
                    VALUES (@EmpresaId, @UsuarioId, @ContatoId, @ProdutoVariacaoId)
                    ON CONFLICT DO NOTHING
                    """;
                foreach (var pvId in produtoVariacaoIds)
                {
                    await conexao.ExecuteAsync(sql, new { EmpresaId = empresaId, UsuarioId = usuarioId, ContatoId = contatoId, ProdutoVariacaoId = pvId });
                }
            }
        });
    }
    public async Task<IEnumerable<ConfiguracaoHorario>> ObterConfiguracoesHorariosAsync(
        Guid empresaId, Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null)
    {
        var sql = """
            SELECT id AS Id, empresa_id AS EmpresaId,
                   profissional_usuario_id AS ProfissionalUsuarioId,
                   profissional_contato_id AS ProfissionalContatoId,
                   dia_semana AS DiaSemana,
                   hora_inicio AS HoraInicio, hora_fim AS HoraFim,
                   intervalo_minutos AS IntervaloMinutos, ativo AS Ativo
            FROM agendamento_config_horarios
            WHERE empresa_id = @EmpresaId
              AND profissional_usuario_id IS NOT DISTINCT FROM @ProfissionalUsuarioId
              AND profissional_contato_id IS NOT DISTINCT FROM @ProfissionalContatoId
            ORDER BY dia_semana
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ConfiguracaoHorario>(sql,
            new { EmpresaId = empresaId, ProfissionalUsuarioId = profissionalUsuarioId, ProfissionalContatoId = profissionalContatoId });
    }

    public async Task AtualizarConfiguracoesHorariosAsync(
        Guid empresaId, IEnumerable<ConfiguracaoHorario> horarios,
        Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("""
                DELETE FROM agendamento_config_horarios
                WHERE empresa_id = @EmpresaId
                  AND profissional_usuario_id IS NOT DISTINCT FROM @ProfissionalUsuarioId
                  AND profissional_contato_id IS NOT DISTINCT FROM @ProfissionalContatoId
                """, new { EmpresaId = empresaId, ProfissionalUsuarioId = profissionalUsuarioId, ProfissionalContatoId = profissionalContatoId });

            const string sql = """
                INSERT INTO agendamento_config_horarios
                    (id, empresa_id, profissional_usuario_id, profissional_contato_id,
                     dia_semana, hora_inicio, hora_fim, intervalo_minutos, ativo)
                VALUES
                    (gen_random_uuid(), @EmpresaId, @ProfissionalUsuarioId, @ProfissionalContatoId,
                     @DiaSemana, @HoraInicio, @HoraFim, @IntervaloMinutos, @Ativo)
                """;
            foreach (var h in horarios)
                await conexao.ExecuteAsync(sql, new
                {
                    EmpresaId = empresaId,
                    ProfissionalUsuarioId = profissionalUsuarioId,
                    ProfissionalContatoId = profissionalContatoId,
                    h.DiaSemana, h.HoraInicio, h.HoraFim, h.IntervaloMinutos, h.Ativo
                });
        });
    }
    public async Task<IEnumerable<HorarioBloqueado>> ListarHorariosBloqueadosAsync(Guid empresaId, DateTime? de = null, DateTime? ate = null)
    {
        const string sql = """
            SELECT b.id AS Id, b.empresa_id AS EmpresaId, b.profissional_usuario_id AS ProfissionalUsuarioId, b.profissional_contato_id AS ProfissionalContatoId,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional,
                   b.data_inicio AS DataInicio, b.data_fim AS DataFim,
                   b.motivo AS Motivo, b.data_inclusao AS DataInclusao
            FROM agendamento_horarios_bloqueados b
            LEFT JOIN usuarios u ON u.id = b.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = b.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = b.profissional_contato_id
            WHERE b.empresa_id = @EmpresaId
              AND (@De IS NULL OR b.data_fim >= @De)
              AND (@Ate IS NULL OR b.data_inicio <= @Ate)
            ORDER BY b.data_inicio
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<HorarioBloqueado>(sql, new { EmpresaId = empresaId, De = de, Ate = ate });
    }

    public async Task CadastrarHorarioBloqueadoAsync(HorarioBloqueado bloqueio)
    {
        const string sql = """
            INSERT INTO agendamento_horarios_bloqueados
                (id, empresa_id, profissional_usuario_id, profissional_contato_id, data_inicio, data_fim, motivo, data_inclusao)
            VALUES
                (@Id, @EmpresaId, @ProfissionalUsuarioId, @ProfissionalContatoId, @DataInicio, @DataFim, @Motivo, @DataInclusao)
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, bloqueio);
    }

    public async Task DeletarFisicoHorarioBloqueadoAsync(Guid id, Guid empresaId)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(
            "DELETE FROM agendamento_horarios_bloqueados WHERE id = @Id AND empresa_id = @EmpresaId",
            new { Id = id, EmpresaId = empresaId });
    }
    public async Task<Agendamento?> ObterPorIdAsync(Guid id, Guid empresaId)
    {
        const string sql = """
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.cliente_email AS ClienteEmail,
                   a.cliente_telefone AS ClienteTelefone, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   a.observacoes AS Observacoes, a.motivo_cancelamento AS MotivoCancelamento,
                   a.cancelado_por AS CanceladoPor, a.preco_cobrado AS PrecoCobrado,
                   a.data_inclusao AS DataInclusao, a.data_ultima_atualizacao AS DataUltimaAtualizacao,
                   a.operador_inclusao AS OperadorInclusao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            WHERE a.id = @Id AND a.empresa_id = @EmpresaId
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Agendamento>(sql, new { Id = id, EmpresaId = empresaId });
    }

    public async Task<Agendamento?> ObterPorIdEClienteAsync(Guid id, Guid clienteUsuarioId)
    {
        var sqlBase = """
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            INNER JOIN empresas e ON e.id = a.empresa_id
            WHERE a.id = @Id AND a.cliente_usuario_id = @ClienteUsuarioId
            """;
        var sqlItens = $"""
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional,
                   e.nome AS EmpresaNome
            {sqlBase}
            """;
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<Agendamento>(sqlItens, new { Id = id, ClienteUsuarioId = clienteUsuarioId });
    }

    public async Task<IEnumerable<Agendamento>> ListarPorPeriodoAsync(Guid empresaId, DateTime de, DateTime ate, Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null)
    {
        const string sql = """
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            WHERE a.empresa_id = @EmpresaId
              AND a.data_hora_inicio >= @De
              AND a.data_hora_inicio <= @Ate
              AND (@ProfissionalUsuarioId IS NULL OR a.profissional_usuario_id = @ProfissionalUsuarioId)
              AND (@ProfissionalContatoId IS NULL OR a.profissional_contato_id = @ProfissionalContatoId)
            ORDER BY a.data_hora_inicio
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Agendamento>(sql, new { EmpresaId = empresaId, De = de, Ate = ate, ProfissionalUsuarioId = profissionalUsuarioId, ProfissionalContatoId = profissionalContatoId });
    }

    public async Task<bool> ExisteConflitoHorarioAsync(Guid empresaId, Guid? profissionalUsuarioId, Guid? profissionalContatoId, DateTime dataHoraInicio, DateTime dataHoraFim)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM agendamentos a
                WHERE a.empresa_id = @EmpresaId
                  AND a.status != 'cancelado'
                  AND a.data_hora_inicio < @DataHoraFim
                  AND a.data_hora_fim > @DataHoraInicio
                  AND (
                      (@ProfissionalUsuarioId IS NOT NULL AND a.profissional_usuario_id = @ProfissionalUsuarioId)
                      OR (@ProfissionalContatoId IS NOT NULL AND a.profissional_contato_id = @ProfissionalContatoId)
                      OR (@ProfissionalUsuarioId IS NULL AND @ProfissionalContatoId IS NULL)
                  )
            )
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.ExecuteScalarAsync<bool>(sql, new { EmpresaId = empresaId, ProfissionalUsuarioId = profissionalUsuarioId, ProfissionalContatoId = profissionalContatoId, DataHoraInicio = dataHoraInicio, DataHoraFim = dataHoraFim });
    }

    public async Task<ResultadoPaginado<Agendamento>> ListarPorClienteAsync(Guid clienteUsuarioId, int pagina = 1, int tamanhoPagina = 50)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;
        var sqlBase = """
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            INNER JOIN empresas e ON e.id = a.empresa_id
            WHERE a.cliente_usuario_id = @ClienteUsuarioId
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase}";
        var sqlItens = $"""
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional,
                   e.nome AS EmpresaNome
            {sqlBase}
            ORDER BY a.data_hora_inicio DESC
            OFFSET @Offset ROWS
            FETCH NEXT @TamanhoPagina ROWS ONLY
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        
        var parametros = new { ClienteUsuarioId = clienteUsuarioId, Offset = deslocamento, TamanhoPagina = tamanhoPagina };
        var total = await conexao.ExecuteScalarAsync<int>(sqlTotal, parametros);
        var itens = await conexao.QueryAsync<Agendamento>(sqlItens, parametros);
        
        return new ResultadoPaginado<Agendamento>(itens, total, pagina, tamanhoPagina);
    }

    public async Task<IEnumerable<Agendamento>> ListarProximosPorClienteAsync(Guid clienteUsuarioId)
    {
        var sqlItens = """
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional,
                   e.nome AS EmpresaNome
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            INNER JOIN empresas e ON e.id = a.empresa_id
            WHERE a.cliente_usuario_id = @ClienteUsuarioId 
              AND a.data_hora_inicio >= NOW() 
              AND a.status != 'cancelado'
            ORDER BY a.data_hora_inicio ASC
            LIMIT 50
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Agendamento>(sqlItens, new { ClienteUsuarioId = clienteUsuarioId });
    }

    public async Task<ResultadoPaginado<Agendamento>> ListarHistoricoPorClienteAsync(Guid clienteUsuarioId, int pagina = 1, int tamanhoPagina = 20)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;
        var sqlBase = """
            FROM agendamentos a
            INNER JOIN erp_produtos_variacoes ts ON ts.id = a.produto_variacao_id
            INNER JOIN erp_produtos p ON p.id = ts.produto_id
            LEFT JOIN usuarios u ON u.id = a.profissional_usuario_id
            LEFT JOIN contatos_pessoas_fisicas cpf ON cpf.id = a.profissional_contato_id
            LEFT JOIN contatos_pessoas_juridicas cpj ON cpj.id = a.profissional_contato_id
            INNER JOIN empresas e ON e.id = a.empresa_id
            WHERE a.cliente_usuario_id = @ClienteUsuarioId
              AND (a.data_hora_inicio < NOW() OR a.status = 'cancelado')
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase}";
        var sqlItens = $"""
            SELECT a.id AS Id, a.empresa_id AS EmpresaId, a.produto_variacao_id AS ProdutoVariacaoId,
                   a.profissional_usuario_id AS ProfissionalUsuarioId, a.profissional_contato_id AS ProfissionalContatoId,
                   a.cliente_usuario_id AS ClienteUsuarioId,
                   a.cliente_nome AS ClienteNome, a.data_hora_inicio AS DataHoraInicio,
                   a.data_hora_fim AS DataHoraFim, a.status AS Situacao,
                   CASE WHEN ts.descricao = 'Padrão' OR ts.descricao = '' THEN p.descricao ELSE p.descricao || ' - ' || ts.descricao END AS NomeTipoServico,
                   ts.cor_hex AS CorHexadecimalServico, ts.duracao_minutos AS DuracaoMinutos,
                   COALESCE(u.nome, cpf.nome, cpj.razao_social) AS NomeProfissional,
                   e.nome AS EmpresaNome
            {sqlBase}
            ORDER BY a.data_hora_inicio DESC
            OFFSET @Offset ROWS
            FETCH NEXT @TamanhoPagina ROWS ONLY
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        
        var parametros = new { ClienteUsuarioId = clienteUsuarioId, Offset = deslocamento, TamanhoPagina = tamanhoPagina };
        var total = await conexao.ExecuteScalarAsync<int>(sqlTotal, parametros);
        var itens = await conexao.QueryAsync<Agendamento>(sqlItens, parametros);
        
        return new ResultadoPaginado<Agendamento>(itens, total, pagina, tamanhoPagina);
    }

    public async Task CadastrarAsync(Agendamento agendamento)
    {
        const string sql = """
            INSERT INTO agendamentos
                (id, empresa_id, produto_variacao_id, profissional_usuario_id, profissional_contato_id, cliente_usuario_id,
                 cliente_nome, cliente_email, cliente_telefone, data_hora_inicio, data_hora_fim,
                 status, observacoes, preco_cobrado, data_inclusao, data_ultima_atualizacao, operador_inclusao)
            VALUES
                (@Id, @EmpresaId, @ProdutoVariacaoId, @ProfissionalUsuarioId, @ProfissionalContatoId, @ClienteUsuarioId,
                 @ClienteNome, @ClienteEmail, @ClienteTelefone, @DataHoraInicio, @DataHoraFim,
                 @Situacao, @Observacoes, @PrecoCobrado, @DataInclusao, @DataUltimaAtualizacao, @OperadorInclusao)
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, agendamento);
    }

    public async Task AtualizarSituacaoAsync(Guid id, Guid empresaId, string novaSituacao, string? motivo = null, string? canceladoPor = null)
    {
        const string sql = """
            UPDATE agendamentos
            SET status = @Situacao,
                motivo_cancelamento = COALESCE(@Motivo, motivo_cancelamento),
                cancelado_por = COALESCE(@CanceladoPor, cancelado_por),
                data_ultima_atualizacao = NOW()
            WHERE id = @Id AND empresa_id = @EmpresaId
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { Id = id, EmpresaId = empresaId, Situacao = novaSituacao, Motivo = motivo, CanceladoPor = canceladoPor });
    }
    public async Task<IEnumerable<HorarioDisponivel>> ObterHorariosDisponiveisAsync(
        Guid empresaId, Guid produtoVariacaoId, DateTime data,
        Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var duracao = await conexao.ExecuteScalarAsync<int>(
            "SELECT duracao_minutos FROM erp_produtos_variacoes WHERE id = @Id",
            new { Id = produtoVariacaoId });

        if (duracao == 0) return Enumerable.Empty<HorarioDisponivel>();
        var diaSemana = (int)data.DayOfWeek;
        var configEmpresa = await conexao.QueryFirstOrDefaultAsync<ConfiguracaoHorario>("""
            SELECT dia_semana AS DiaSemana, hora_inicio AS HoraInicio, hora_fim AS HoraFim, intervalo_minutos AS IntervaloMinutos
            FROM agendamento_config_horarios
            WHERE empresa_id = @EmpresaId
              AND profissional_usuario_id IS NULL
              AND profissional_contato_id IS NULL
              AND dia_semana = @DiaSemana
              AND ativo = true
            """, new { EmpresaId = empresaId, DiaSemana = diaSemana });

        if (configEmpresa == null) return Enumerable.Empty<HorarioDisponivel>();
        var sqlProfs = $"""
            WITH Prestadores AS (
                -- 1. Usuários vinculados à empresa
                SELECT u.id AS UsuarioId, NULL::uuid AS ContatoId, u.nome AS NomeExibicao
                FROM usuarios u
                INNER JOIN usuarios_empresas ue ON ue.usuario_id = u.id
                WHERE ue.empresa_id = @EmpresaId AND ue.ativo = true

                UNION

                -- 2. Contatos com papel de funcionário (Pessoa Física)
                SELECT NULL::uuid AS UsuarioId, c.id AS ContatoId, c.nome AS NomeExibicao
                FROM contatos_pessoas_fisicas c
                INNER JOIN empresas_funcionarios ef ON ef.contato_id = c.id
                WHERE c.conta_id = (SELECT conta_id FROM empresas WHERE id = @EmpresaId) AND ef.empresa_id = @EmpresaId

                UNION

                -- 3. Contatos com papel de funcionário (Pessoa Jurídica)
                SELECT NULL::uuid AS UsuarioId, c.id AS ContatoId, c.razao_social AS NomeExibicao
                FROM contatos_pessoas_juridicas c
                INNER JOIN empresas_funcionarios ef ON ef.contato_id = c.id
                WHERE c.conta_id = (SELECT conta_id FROM empresas WHERE id = @EmpresaId) AND ef.empresa_id = @EmpresaId
            )
            SELECT p.UsuarioId, p.ContatoId, p.NomeExibicao
            FROM Prestadores p
            WHERE (
                NOT EXISTS (
                    SELECT 1 FROM agendamento_prestador_servico ps0
                    WHERE ps0.empresa_id = @EmpresaId
                      AND (
                          (ps0.usuario_id IS NOT NULL AND ps0.usuario_id = p.UsuarioId) OR
                          (ps0.contato_id IS NOT NULL AND ps0.contato_id = p.ContatoId)
                      )
                )
                OR EXISTS (
                    SELECT 1 FROM agendamento_prestador_servico ps
                    WHERE ps.empresa_id = @EmpresaId
                      AND ps.produto_variacao_id = @ProdutoVariacaoId
                      AND (
                          (ps.usuario_id IS NOT NULL AND ps.usuario_id = p.UsuarioId) OR
                          (ps.contato_id IS NOT NULL AND ps.contato_id = p.ContatoId)
                      )
                )
            )
            {(profissionalUsuarioId.HasValue ? "AND p.UsuarioId = @ProfissionalUsuarioId" : "")}
            {(profissionalContatoId.HasValue ? "AND p.ContatoId = @ProfissionalContatoId" : "")}
            """;
        var profissionais = (await conexao.QueryAsync<(Guid? UsuarioId, Guid? ContatoId, string NomeExibicao)>(
            sqlProfs, new { EmpresaId = empresaId, ProdutoVariacaoId = produtoVariacaoId,
                            ProfissionalUsuarioId = profissionalUsuarioId, ProfissionalContatoId = profissionalContatoId })).ToList();

        if (!profissionais.Any()) return Enumerable.Empty<HorarioDisponivel>();
        var inicioDia = data.Date;
        var fimDia    = data.Date.AddDays(1);
        var agendados = (await conexao.QueryAsync<(Guid? UsuarioId, Guid? ContatoId, DateTime Inicio, DateTime Fim)>("""
            SELECT profissional_usuario_id AS UsuarioId, profissional_contato_id AS ContatoId,
                   data_hora_inicio AS Inicio, data_hora_fim AS Fim
            FROM agendamentos
            WHERE empresa_id = @EmpresaId
              AND data_hora_inicio >= @De AND data_hora_inicio < @Ate
              AND status != 'cancelado'
            """, new { EmpresaId = empresaId, De = inicioDia, Ate = fimDia })).ToList();
        var bloqueios = (await conexao.QueryAsync<(Guid? UsuarioId, Guid? ContatoId, DateTime Inicio, DateTime Fim)>("""
            SELECT profissional_usuario_id AS UsuarioId, profissional_contato_id AS ContatoId,
                   data_inicio AS Inicio, data_fim AS Fim
            FROM agendamento_horarios_bloqueados
            WHERE empresa_id = @EmpresaId AND data_inicio < @Ate AND data_fim > @De
            """, new { EmpresaId = empresaId, De = inicioDia, Ate = fimDia })).ToList();
        var slots = new List<HorarioDisponivel>();
        var agora = DateTime.UtcNow;

        foreach (var prof in profissionais)
        {
            ConfiguracaoHorario? configProf = null;
            if (prof.UsuarioId.HasValue || prof.ContatoId.HasValue)
            {
                configProf = await conexao.QueryFirstOrDefaultAsync<ConfiguracaoHorario>("""
                    SELECT dia_semana AS DiaSemana, hora_inicio AS HoraInicio, hora_fim AS HoraFim, intervalo_minutos AS IntervaloMinutos
                    FROM agendamento_config_horarios
                    WHERE empresa_id = @EmpresaId
                      AND profissional_usuario_id IS NOT DISTINCT FROM @UsuarioId
                      AND profissional_contato_id IS NOT DISTINCT FROM @ContatoId
                      AND dia_semana = @DiaSemana
                      AND ativo = true
                    """, new { EmpresaId = empresaId, UsuarioId = prof.UsuarioId, ContatoId = prof.ContatoId, DiaSemana = diaSemana });
            }
            TimeOnly aberturaEfetiva, fechamentoEfetivo;
            int intervaloEfetivo;

            if (configProf != null)
            {
                aberturaEfetiva  = configProf.HoraInicio > configEmpresa.HoraInicio ? configProf.HoraInicio  : configEmpresa.HoraInicio;
                fechamentoEfetivo = configProf.HoraFim   < configEmpresa.HoraFim    ? configProf.HoraFim     : configEmpresa.HoraFim;
                intervaloEfetivo = configProf.IntervaloMinutos;
                if (aberturaEfetiva >= fechamentoEfetivo) continue;
            }
            else
            {
                aberturaEfetiva   = configEmpresa.HoraInicio;
                fechamentoEfetivo = configEmpresa.HoraFim;
                intervaloEfetivo  = configEmpresa.IntervaloMinutos;
            }

            var cursor = data.Date.Add(aberturaEfetiva.ToTimeSpan());
            var limite = data.Date.Add(fechamentoEfetivo.ToTimeSpan()).AddMinutes(-duracao);

            while (cursor <= limite)
            {
                var fim = cursor.AddMinutes(duracao);
                if (cursor <= agora) { cursor = cursor.AddMinutes(intervaloEfetivo); continue; }
                bool ocupado = agendados.Any(a =>
                    a.UsuarioId == prof.UsuarioId && a.ContatoId == prof.ContatoId &&
                    a.Inicio < fim && a.Fim > cursor);
                bool bloqueado = bloqueios.Any(b =>
                    (b.UsuarioId == null && b.ContatoId == null ||
                     (b.UsuarioId == prof.UsuarioId && b.ContatoId == prof.ContatoId)) &&
                    b.Inicio < fim && b.Fim > cursor);

                if (!ocupado && !bloqueado)
                    slots.Add(new HorarioDisponivel
                    {
                        DataHoraInicio        = cursor,
                        DataHoraFim           = fim,
                        ProfissionalUsuarioId = prof.UsuarioId,
                        ProfissionalContatoId = prof.ContatoId,
                        NomeProfissional      = prof.NomeExibicao
                    });

                cursor = cursor.AddMinutes(intervaloEfetivo);
            }
        }

        return slots.OrderBy(s => s.DataHoraInicio).ThenBy(s => s.NomeProfissional);
    }
}
file sealed class PrestadorServicoDto
{
    public Guid? UsuarioId { get; set; }
    public Guid? ContatoId { get; set; }
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid ContaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }
    public int DuracaoMinutos { get; set; }
    public string? CorHexadecimal { get; set; }
}




