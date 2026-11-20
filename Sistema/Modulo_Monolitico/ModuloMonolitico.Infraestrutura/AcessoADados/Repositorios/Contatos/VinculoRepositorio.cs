using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Contatos;

public class VinculoRepositorio : RepositorioBase, IVinculoRepositorio, IVinculoQueries
{
    public VinculoRepositorio(string connectionString) : base(connectionString) { }


    public async Task CadastrarSolicitacaoAsync(SolicitacaoVinculo solicitacao)
    {
        const string sql = """
            INSERT INTO public.solicitacoes_vinculo
                (id, conta_solicitante_id, contato_id, contato_tipo, conta_alvo_id,
                 papel_proposto, mensagem, status, data_inclusao)
            VALUES
                (@Id, @ContaSolicitanteId, @ContaSolicitanteId, 'conta', @ContaAlvoId,
                 @TipoVinculo, @Mensagem, 'pendente', now())
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            solicitacao.Id,
            solicitacao.ContaSolicitanteId,
            solicitacao.ContaAlvoId,
            solicitacao.TipoVinculo,
            solicitacao.Mensagem
        });
    }

    public async Task<SolicitacaoVinculo?> ObterSolicitacaoPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                sv.id, sv.conta_solicitante_id AS ContaSolicitanteId,
                sv.conta_alvo_id AS ContaAlvoId,
                sv.papel_proposto AS TipoVinculo,
                sv.mensagem, sv.status AS Situacao,
                sv.data_inclusao AS DataInclusao,
                sv.data_resposta AS DataResposta,
                sv.data_ultima_atualizacao AS DataUltimaAtualizacao,
                c.nome AS NomeSolicitante,
                c.email AS EmailSolicitante
            FROM public.solicitacoes_vinculo sv
            LEFT JOIN public.contas c ON c.id = sv.conta_solicitante_id
            WHERE sv.id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<SolicitacaoVinculo>(sql, new { Id = id });
    }

    public async Task<IEnumerable<SolicitacaoVinculo>> ListarSolicitacoesEnviadasAsync(Guid contaId)
    {
        const string sql = """
            SELECT
                sv.id, sv.conta_solicitante_id AS ContaSolicitanteId,
                sv.conta_alvo_id AS ContaAlvoId,
                sv.papel_proposto AS TipoVinculo,
                sv.mensagem, sv.status AS Situacao,
                sv.data_inclusao AS DataInclusao,
                sv.data_resposta AS DataResposta,
                c.nome AS NomeSolicitante,
                c.email AS EmailSolicitante
            FROM public.solicitacoes_vinculo sv
            LEFT JOIN public.contas c ON c.id = sv.conta_alvo_id
            WHERE sv.conta_solicitante_id = @ContaId
            ORDER BY sv.data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<SolicitacaoVinculo>(sql, new { ContaId = contaId });
    }

    public async Task<IEnumerable<SolicitacaoVinculo>> ListarSolicitacoesRecebidasAsync(Guid contaId)
    {
        const string sql = """
            SELECT
                sv.id, sv.conta_solicitante_id AS ContaSolicitanteId,
                sv.conta_alvo_id AS ContaAlvoId,
                sv.papel_proposto AS TipoVinculo,
                sv.mensagem, sv.status AS Situacao,
                sv.data_inclusao AS DataInclusao,
                sv.data_resposta AS DataResposta,
                c.nome AS NomeSolicitante,
                c.email AS EmailSolicitante
            FROM public.solicitacoes_vinculo sv
            LEFT JOIN public.contas c ON c.id = sv.conta_solicitante_id
            WHERE sv.conta_alvo_id = @ContaId
            ORDER BY sv.data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<SolicitacaoVinculo>(sql, new { ContaId = contaId });
    }

    public async Task AtualizarSituacaoSolicitacaoAsync(Guid id, string novaSituacao, Guid? contaAlvoId)
    {
        const string sql = """
            UPDATE public.solicitacoes_vinculo
            SET status = @Situacao,
                conta_alvo_id = COALESCE(@ContaAlvoId, conta_alvo_id),
                data_resposta = now()
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { Id = id, Situacao = novaSituacao, ContaAlvoId = contaAlvoId });
    }


    public async Task CadastrarVinculoAsync(VinculoConta vinculo)
    {
        const string sql = """
            INSERT INTO public.vinculos_contas
                (id, conta_a_id, conta_b_id, tipo, status, solicitacao_id, data_aceite, data_inclusao)
            VALUES
                (@Id, @ContaAId, @ContaBId, @Tipo, 'ativo', @SolicitacaoId, now(), now())
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            vinculo.Id,
            vinculo.ContaAId,
            vinculo.ContaBId,
            vinculo.Tipo,
            vinculo.SolicitacaoId
        });
    }

    public async Task<IEnumerable<VinculoConta>> ListarVinculosAtivosPorContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT
                vc.id, vc.conta_a_id AS ContaAId, vc.conta_b_id AS ContaBId,
                vc.tipo, vc.status AS Situacao, vc.solicitacao_id AS SolicitacaoId,
                vc.data_aceite AS DataAceite, vc.data_inclusao AS DataInclusao,
                CASE
                    WHEN vc.conta_a_id = @ContaId THEN cb.nome
                    ELSE ca.nome
                END AS NomeContaParceira,
                CASE
                    WHEN vc.conta_a_id = @ContaId THEN cb.email
                    ELSE ca.email
                END AS EmailContaParceira
            FROM public.vinculos_contas vc
            LEFT JOIN public.contas ca ON ca.id = vc.conta_a_id
            LEFT JOIN public.contas cb ON cb.id = vc.conta_b_id
            WHERE (vc.conta_a_id = @ContaId OR vc.conta_b_id = @ContaId)
              AND vc.status = 'ativo'
            ORDER BY vc.data_aceite DESC
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<VinculoConta>(sql, new { ContaId = contaId });
    }

    public async Task RevogarVinculoAsync(Guid vinculoId, Guid contaId)
    {
        const string sql = """
            UPDATE public.vinculos_contas
            SET status = 'revogado'
            WHERE id = @VinculoId
              AND (conta_a_id = @ContaId OR conta_b_id = @ContaId)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { VinculoId = vinculoId, ContaId = contaId });
    }


    public async Task<IEnumerable<ResultadoSeletorUsuario>> BuscarContatosInternosAsync(
        Guid contaId, Guid empresaId, string papel, string? termo)
    {
        var termoBusca = string.IsNullOrWhiteSpace(termo) ? "%" : $"%{termo.Trim()}%";
        var tabelaPapel = papel switch
        {
            "fornecedor" => "empresas_fornecedores",
            "transportador" => "empresas_transportadores",
            _ => "empresas_clientes"
        };

        var sql = $"""
            SELECT
                pf.id AS Id,
                'contato' AS Fonte,
                'fisica' AS TipoPessoa,
                pf.nome AS NomeExibicao,
                pf.cpf AS Documento,
                pf.email AS Email,
                pf.telefone AS Telefone,
                pf.envia_email_agendamento AS EnviaEmailAgendamento,
                NULL::uuid AS ContaVinculadaId
            FROM public.contatos_pessoas_fisicas pf
            INNER JOIN public.{tabelaPapel} p ON p.contato_id = pf.id AND p.empresa_id = @EmpresaId
            WHERE pf.conta_id = @ContaId
              AND pf.status = true
              AND (pf.nome ILIKE @Termo OR pf.cpf ILIKE @Termo OR pf.email ILIKE @Termo)

            UNION ALL

            SELECT
                pj.id AS Id,
                'contato' AS Fonte,
                'juridica' AS TipoPessoa,
                COALESCE(pj.nome_fantasia, pj.razao_social) AS NomeExibicao,
                pj.cnpj AS Documento,
                pj.email AS Email,
                pj.telefone AS Telefone,
                pj.envia_email_agendamento AS EnviaEmailAgendamento,
                NULL::uuid AS ContaVinculadaId
            FROM public.contatos_pessoas_juridicas pj
            INNER JOIN public.{tabelaPapel} p ON p.contato_id = pj.id AND p.empresa_id = @EmpresaId
            WHERE pj.conta_id = @ContaId
              AND pj.status = true
              AND (pj.razao_social ILIKE @Termo OR pj.nome_fantasia ILIKE @Termo
                   OR pj.cnpj ILIKE @Termo OR pj.email ILIKE @Termo)

            ORDER BY NomeExibicao
            LIMIT 20
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ResultadoSeletorUsuario>(sql, new
        {
            ContaId = contaId,
            EmpresaId = empresaId,
            Termo = termoBusca
        });
    }

    public async Task<IEnumerable<ResultadoSeletorUsuario>> BuscarContatosVinculadosAsync(
        Guid contaId, string papel, string? termo)
    {
        var termoBusca = string.IsNullOrWhiteSpace(termo) ? "%" : $"%{termo.Trim()}%";
        var tabelaPapel = papel switch
        {
            "fornecedor" => "empresas_fornecedores",
            "transportador" => "empresas_transportadores",
            _ => "empresas_clientes"
        };


        var sql = $"""
            SELECT * FROM (
                SELECT DISTINCT ON (pj.id)
                    pj.id AS Id,
                    'vinculo' AS Fonte,
                    'juridica' AS TipoPessoa,
                    COALESCE(pj.nome_fantasia, pj.razao_social) AS NomeExibicao,
                    pj.cnpj AS Documento,
                    pj.email AS Email,
                    pj.telefone AS Telefone,
                    parceira.id AS ContaVinculadaId
                FROM public.vinculos_contas vc
                JOIN public.contas parceira ON parceira.id = CASE
                    WHEN vc.conta_a_id = @ContaId THEN vc.conta_b_id
                    ELSE vc.conta_a_id
                END
                JOIN public.contatos_pessoas_juridicas pj ON pj.conta_id = parceira.id AND pj.status = true
                JOIN public.{tabelaPapel} p ON p.contato_id = pj.id
                WHERE (vc.conta_a_id = @ContaId OR vc.conta_b_id = @ContaId)
                  AND vc.status = 'ativo'
                  AND (pj.razao_social ILIKE @Termo OR pj.nome_fantasia ILIKE @Termo
                       OR pj.cnpj ILIKE @Termo OR pj.email ILIKE @Termo)
            ) AS q1

            UNION ALL

            SELECT * FROM (
                SELECT DISTINCT ON (pf.id)
                    pf.id AS Id,
                    'vinculo' AS Fonte,
                    'fisica' AS TipoPessoa,
                    pf.nome AS NomeExibicao,
                    pf.cpf AS Documento,
                    pf.email AS Email,
                    pf.telefone AS Telefone,
                    parceira.id AS ContaVinculadaId
                FROM public.vinculos_contas vc
                JOIN public.contas parceira ON parceira.id = CASE
                    WHEN vc.conta_a_id = @ContaId THEN vc.conta_b_id
                    ELSE vc.conta_a_id
                END
                JOIN public.contatos_pessoas_fisicas pf ON pf.conta_id = parceira.id AND pf.status = true
                JOIN public.{tabelaPapel} p ON p.contato_id = pf.id
                WHERE (vc.conta_a_id = @ContaId OR vc.conta_b_id = @ContaId)
                  AND vc.status = 'ativo'
                  AND (pf.nome ILIKE @Termo OR pf.cpf ILIKE @Termo OR pf.email ILIKE @Termo)
            ) AS q2

            ORDER BY NomeExibicao
            LIMIT 20
            """;

        await using var conn = CriarConexao();
        await conn.OpenAsync();
        return await conn.QueryAsync<ResultadoSeletorUsuario>(sql, new
        {
            ContaId = contaId,
            Termo = termoBusca
        });
    }

    public async Task<Guid?> ObterContaIdPorEmailAsync(string email)
    {
        const string sql = """
            SELECT c.id
            FROM public.contas c
            JOIN public.usuarios u ON u.email = @Email
            JOIN public.usuarios_contas uc ON uc.conta_id = c.id AND uc.usuario_id = u.id AND uc.is_titular = true
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<Guid?>(sql, new { Email = email });
    }
}



