using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Aplicacao.Interfaces.Consultas;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.SaaS;

public class AssinaturaSaaSRepositorio : RepositorioBase, IAssinaturaSaaSRepositorio, IAssinaturaSaaSQueries
{
    public AssinaturaSaaSRepositorio(string connectionString) : base(connectionString) { }



    public Task<IEnumerable<PlanoSaaS>> ListarPlanosSaaSAsync()
        => ListarPlanosInternoAsync(apenasAtivos: false);

    public Task<IEnumerable<PlanoSaaS>> ListarPlanosAtivosAsync()
        => ListarPlanosInternoAsync(apenasAtivos: true);

    async Task<IEnumerable<PlanoSaaS>> ListarPlanosInternoAsync(bool apenasAtivos)
    {
        var filtroAtivo = apenasAtivos ? "WHERE p.ativo = true" : "";
        var sql = $"""
            SELECT p.id AS "Id", p.nome AS "Nome", p.descricao AS "Descricao", p.valor_mensal AS "ValorMensal", p.ativo AS "Ativo", p.data_inclusao AS "DataInclusao",
                   pf.plano_id AS "PlanoId", pf.feature_id AS "RecursoId", pf.valor AS "Valor",
                   f.chave as "RecursoChave", f.tipo as "RecursoTipo"
            FROM planos_saas p
            LEFT JOIN planos_features_saas pf ON pf.plano_id = p.id
            LEFT JOIN features_saas f ON f.id = pf.feature_id
            {filtroAtivo}
            ORDER BY p.valor_mensal
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var planosDict = new Dictionary<Guid, PlanoSaaS>();
        await conexao.QueryAsync<PlanoSaaS, PlanoRecursoSaaS, PlanoSaaS>(
            sql,
            (plano, feature) =>
            {
                if (!planosDict.TryGetValue(plano.Id, out var p))
                {
                    p = plano;
                    planosDict[plano.Id] = p;
                }
                if (feature != null && feature.RecursoId != Guid.Empty)
                    p.Recursos.Add(feature);
                return p;
            },
            splitOn: "PlanoId"
        );
        return planosDict.Values;
    }

    public async Task<PlanoSaaS?> ObterPlanoPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT p.id AS "Id", p.nome AS "Nome", p.descricao AS "Descricao", p.valor_mensal AS "ValorMensal", p.ativo AS "Ativo", p.data_inclusao AS "DataInclusao",
                   pf.plano_id AS "PlanoId", pf.feature_id AS "RecursoId", pf.valor AS "Valor",
                   f.chave as "RecursoChave", f.tipo as "RecursoTipo"
            FROM planos_saas p
            LEFT JOIN planos_features_saas pf ON pf.plano_id = p.id
            LEFT JOIN features_saas f ON f.id = pf.feature_id
            WHERE p.id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        PlanoSaaS? plano = null;
        await conexao.QueryAsync<PlanoSaaS, PlanoRecursoSaaS, PlanoSaaS>(sql, (p, f) =>
        {
            plano ??= p;
            if (f != null && f.RecursoId != Guid.Empty) plano.Recursos.Add(f);
            return p;
        }, new { Id = id }, splitOn: "PlanoId");

        return plano;
    }

    public async Task CadastrarPlanoAsync(PlanoSaaS plano, List<string> idsRecurso, Dictionary<string, string> valoresRecurso)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "INSERT INTO planos_saas (id, nome, descricao, valor_mensal, ativo, data_inclusao) VALUES (@Id, @Nome, @Descricao, @ValorMensal, @Ativo, @DataInclusao)",
                plano);

            foreach (var featureIdStr in idsRecurso)
            {
                if (!Guid.TryParse(featureIdStr, out var featureId)) continue;
                var valor = valoresRecurso.TryGetValue(featureIdStr, out var v) ? v : "1";
                await conexao.ExecuteAsync(
                    "INSERT INTO planos_features_saas (plano_id, feature_id, valor) VALUES (@PlanoId, @RecursoId, @Valor) ON CONFLICT DO NOTHING",
                    new { PlanoId = plano.Id, RecursoId = featureId, Valor = valor });
            }
        });
    }

    public async Task AtualizarPlanoAsync(PlanoSaaS plano, List<string> idsRecurso, Dictionary<string, string> valoresRecurso)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "UPDATE planos_saas SET nome = @Nome, descricao = @Descricao, valor_mensal = @ValorMensal, ativo = @Ativo WHERE id = @Id",
                plano);

            await conexao.ExecuteAsync("DELETE FROM planos_features_saas WHERE plano_id = @PlanoId", new { PlanoId = plano.Id });

            foreach (var featureIdStr in idsRecurso ?? new())
            {
                if (!Guid.TryParse(featureIdStr, out var featureId)) continue;
                var valor = valoresRecurso.TryGetValue(featureIdStr, out var v) ? v : "1";
                await conexao.ExecuteAsync(
                    "INSERT INTO planos_features_saas (plano_id, feature_id, valor) VALUES (@PlanoId, @RecursoId, @Valor)",
                    new { PlanoId = plano.Id, RecursoId = featureId, Valor = valor });
            }
        });
    }



    public async Task<IEnumerable<RecursoPlanoSaaS>> ListarRecursosAsync()
    {
        const string sql = "SELECT id, chave, tipo, nome, descricao, ativo, data_inclusao FROM features_saas WHERE ativo = true ORDER BY nome";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<RecursoPlanoSaaS>(sql);
    }



    public async Task<ContaAssinatura?> ObterAssinaturaDaContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT ca.id AS "Id", ca.conta_id AS "ContaId", ca.plano_id AS "PlanoId", ca.status AS "Situacao", ca.data_inicio AS "DataInicio",
                   ca.data_vencimento AS "DataVencimento", ca.valor_contratado AS "ValorContratado", ca.data_inclusao AS "DataInclusao", ca.data_ultima_atualizacao AS "DataUltimaAtualizacao",
                   p.nome AS "PlanoNome"
            FROM contas_assinaturas_saas ca
            INNER JOIN planos_saas p ON p.id = ca.plano_id
            WHERE ca.conta_id = @ContaId
            ORDER BY ca.data_inclusao DESC
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<ContaAssinatura>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarAssinaturaAsync(ContaAssinatura assinatura)
    {
        const string sql = """
            INSERT INTO contas_assinaturas_saas 
                (id, conta_id, plano_id, status, data_inicio, data_vencimento, valor_contratado, data_inclusao, data_ultima_atualizacao, operador_inclusao)
            VALUES 
                (@Id, @ContaId, @PlanoId, @Situacao, @DataInicio, @DataVencimento, @ValorContratado, @DataInclusao, @DataUltimaAtualizacao, @OperadorInclusao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, assinatura);
    }

    public async Task AtualizarAssinaturaAsync(ContaAssinatura assinatura)
    {
        const string sql = """
            UPDATE contas_assinaturas_saas
            SET status = @Situacao,
                data_vencimento = @DataVencimento,
                plano_id = @PlanoId,
                valor_contratado = @ValorContratado,
                data_ultima_atualizacao = @DataUltimaAtualizacao
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, assinatura);
    }



    public async Task<IEnumerable<ContaComplementoSaaS>> ListarComplementosDaContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT ca.id AS "Id", ca.conta_id AS "ContaId", ca.feature_id AS "RecursoId", ca.quantidade_adicional AS "QuantidadeAdicional", ca.preco_negociado AS "PrecoNegociado",
                   ca.ativo AS "Ativo", ca.data_inicio AS "DataInicio", ca.data_fim AS "DataFim",
                   f.chave AS "RecursoChave", f.nome AS "RecursoNome"
            FROM contas_addons_saas ca
            INNER JOIN features_saas f ON f.id = ca.feature_id
            WHERE ca.conta_id = @ContaId AND ca.ativo = true
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ContaComplementoSaaS>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarComplementoAsync(ContaComplementoSaaS addon)
    {
        const string sql = """
            INSERT INTO contas_addons_saas (id, conta_id, feature_id, quantidade_adicional, preco_negociado, ativo, data_inicio)
            VALUES (@Id, @ContaId, @RecursoId, @QuantidadeAdicional, @PrecoNegociado, @Ativo, @DataInicio)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, addon);
    }

    public async Task DesativarComplementoAsync(Guid addonId)
    {
        const string sql = "UPDATE contas_addons_saas SET ativo = false, data_fim = NOW() WHERE id = @Id";
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new { Id = addonId });
    }



    public async Task<IEnumerable<FaturaSaaS>> ListarFaturasDaContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT id AS "Id", conta_id AS "ContaId", mes_referencia AS "MesReferencia", valor_total AS "ValorTotal", status AS "Situacao", 
                   data_vencimento AS "DataVencimento", data_pagamento AS "DataPagamento", observacoes AS "Observacoes", operador_baixa AS "OperadorBaixa", data_inclusao AS "DataInclusao"
            FROM faturas_saas
            WHERE conta_id = @ContaId
            ORDER BY data_vencimento DESC
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<FaturaSaaS>(sql, new { ContaId = contaId });
    }

    public async Task<FaturaSaaS?> ObterFaturaPorIdAsync(Guid id)
    {
        const string sql = """
            SELECT id AS "Id", conta_id AS "ContaId", mes_referencia AS "MesReferencia", valor_total AS "ValorTotal", status AS "Situacao", 
                   data_vencimento AS "DataVencimento", data_pagamento AS "DataPagamento", observacoes AS "Observacoes", operador_baixa AS "OperadorBaixa", data_inclusao AS "DataInclusao"
            FROM faturas_saas
            WHERE id = @Id
            """;
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<FaturaSaaS>(sql, new { Id = id });
    }

    public async Task CadastrarFaturaAsync(FaturaSaaS fatura)
    {
        const string sql = """
            INSERT INTO faturas_saas (id, conta_id, mes_referencia, valor_total, status, data_vencimento, observacoes, data_inclusao)
            VALUES (@Id, @ContaId, @MesReferencia, @ValorTotal, @Situacao, @DataVencimento, @Observacoes, @DataInclusao)
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, fatura);
    }

    public async Task DarBaixaFaturaAsync(Guid faturaId, Guid operadorId)
    {
        const string sql = """
            UPDATE faturas_saas
            SET status = @Situacao, data_pagamento = @DataPagamento, operador_baixa = @OperadorId
            WHERE id = @FaturaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            FaturaId = faturaId,
            OperadorId = operadorId,
            Situacao = SituacaoFatura.Pago,
            DataPagamento = DateTime.UtcNow
        });
    }



    public async Task<ContaDireitosPlanoDto> ObterDireitosPlanoDaContaAsync(Guid contaId)
    {

        const string sqlAssinatura = """
            SELECT ca.status, ca.data_vencimento,
                   pf.feature_id, pf.valor,
                   f.chave as feature_chave, f.tipo as feature_tipo
            FROM contas_assinaturas_saas ca
            INNER JOIN planos_features_saas pf ON pf.plano_id = ca.plano_id
            INNER JOIN features_saas f ON f.id = pf.feature_id
            WHERE ca.conta_id = @ContaId
            ORDER BY ca.data_inclusao DESC
            LIMIT 50
            """;


        const string sqlAddons = """
            SELECT f.chave, ca.quantidade_adicional, f.tipo
            FROM contas_addons_saas ca
            INNER JOIN features_saas f ON f.id = ca.feature_id
            WHERE ca.conta_id = @ContaId AND ca.ativo = true
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var dto = new ContaDireitosPlanoDto();
        var param = new { ContaId = contaId };

        var rows = (await conexao.QueryAsync(sqlAssinatura, param)).ToList();

        if (!rows.Any())
        {

            dto.SituacaoAssinatura = SituacaoAssinatura.Trial;
            return dto;
        }

        dto.SituacaoAssinatura = rows.First().status ?? SituacaoAssinatura.Trial;
        dto.Vencimento = rows.First().data_vencimento;

        foreach (var row in rows)
        {
            string chave = row.feature_chave;
            string valor = row.valor;
            if (!dto.Recursos.ContainsKey(chave))
                dto.Recursos[chave] = valor;
        }


        var addons = await conexao.QueryAsync(sqlAddons, param);
        foreach (var addon in addons)
        {
            string chave = addon.chave;
            string tipo = addon.tipo;

            if (tipo == "quantidade")
            {
                var baseVal = dto.Recursos.TryGetValue(chave, out var v) && int.TryParse(v, out var n) ? n : 0;
                dto.Recursos[chave] = (baseVal + (int)addon.quantidade_adicional).ToString();
            }
            else
            {
                dto.Recursos[chave] = "1";
            }
        }

        return dto;
    }

    public async Task<IEnumerable<ContaResumoDto>> ListarContasComAssinaturaAsync()
    {
        const string sql = """
            SELECT DISTINCT ON (c.id)
                c.id            AS "ContaId",
                c.nome          AS "ContaNome",
                u.email         AS "EmailTitular",
                ca.id           AS "AssinaturaId",
                ca.status       AS "SituacaoAssinatura",
                ca.data_vencimento  AS "DataVencimento",
                ca.valor_contratado AS "ValorContratado",
                p.nome          AS "PlanoNome"
            FROM contas c
            LEFT JOIN usuarios_contas uc ON uc.conta_id = c.id AND uc.is_titular = true
            LEFT JOIN usuarios u ON u.id = uc.usuario_id
            LEFT JOIN contas_assinaturas_saas ca ON ca.conta_id = c.id
            LEFT JOIN planos_saas p ON p.id = ca.plano_id
            ORDER BY c.id, ca.data_inclusao DESC NULLS LAST
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<ContaResumoDto>(sql);
    }
}



