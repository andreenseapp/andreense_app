using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Fiscal;

public class FiscalRepositorio : RepositorioBase, IFiscalRepositorio
{
    public FiscalRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    public async Task<CertificadoDigital?> ObterCertificadoPorEmpresaAsync(Guid empresaId)
    {
        const string sql = """
            SELECT
                id                     AS Id,
                empresa_id             AS EmpresaId,
                nome_arquivo           AS NomeArquivo,
                pfx_base64             AS ConteudoPfxBase64,
                senha                  AS Senha,
                data_vencimento        AS DataVencimento,
                status                 AS Situacao,
                data_inclusao          AS DataInclusao,
                data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM public.certificados_digitais
            WHERE empresa_id = @EmpresaId
              AND data_exclusao IS NULL
              AND status = 'ativo'
            ORDER BY data_inclusao DESC
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<CertificadoDigital>(sql, new { EmpresaId = empresaId });
    }

    public async Task CadastrarCertificadoAsync(CertificadoDigital certificado)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            const string sqlInativar = """
                UPDATE public.certificados_digitais
                SET data_exclusao = NOW(), status = 'inativo', data_ultima_atualizacao = NOW()
                WHERE empresa_id = @EmpresaId
                  AND data_exclusao IS NULL
                  AND status = 'ativo'
                """;
            await conexao.ExecuteAsync(sqlInativar, new { EmpresaId = certificado.EmpresaId });
            const string sqlInserir = """
                INSERT INTO public.certificados_digitais
                    (empresa_id, nome_arquivo, pfx_base64, senha, data_vencimento, status, data_inclusao)
                VALUES
                    (@EmpresaId, @NomeArquivo, @ConteudoPfxBase64, @Senha, @DataVencimento, 'ativo', NOW())
                """;
            await conexao.ExecuteAsync(sqlInserir, new
            {
                EmpresaId      = certificado.EmpresaId,
                NomeArquivo    = certificado.NomeArquivo,
                ConteudoPfxBase64 = certificado.ConteudoPfxBase64,
                Senha          = certificado.Senha,
                DataVencimento = certificado.DataVencimento
            });
        });
    }

    public async Task DeletarSeguroCertificadoAsync(Guid empresaId)
    {
        const string sql = """
            UPDATE public.certificados_digitais
            SET data_exclusao = NOW(), status = 'inativo', data_ultima_atualizacao = NOW()
            WHERE empresa_id = @EmpresaId
              AND data_exclusao IS NULL
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { EmpresaId = empresaId });
    }

    public async Task<IEnumerable<ConfiguracaoEmissao>> ListarConfiguracoesEmissaoAsync(Guid empresaId)
    {
        const string sql = """
            SELECT
                id                      AS Id,
                empresa_id              AS EmpresaId,
                certificado_id          AS CertificadoId,
                ambiente                AS Ambiente,
                serie                   AS Serie,
                modelo                  AS Modelo,
                proximo_numero_nf       AS ProximoNumeroNf,
                csc_id                  AS CscId,
                csc_token               AS CscToken,
                tipo_emissao_padrao     AS TipoEmissaoPadrao,
                tipo_impressao_default  AS TipoImpressaoPadrao,
                data_ultima_emissao     AS DataUltimaEmissao,
                tratamento_desconto     AS TratamentoDesconto,
                status                  AS Situacao,
                data_inclusao           AS DataInclusao,
                data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM public.configuracoes_emissoes
            WHERE empresa_id = @EmpresaId
              AND data_exclusao IS NULL
            ORDER BY modelo, serie
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<ConfiguracaoEmissao>(sql, new { EmpresaId = empresaId });
    }

    public async Task<ConfiguracaoEmissao?> ObterConfiguracaoEmissaoPorIdAsync(Guid empresaId, Guid id)
    {
        const string sql = """
            SELECT
                id                      AS Id,
                empresa_id              AS EmpresaId,
                certificado_id          AS CertificadoId,
                ambiente                AS Ambiente,
                serie                   AS Serie,
                modelo                  AS Modelo,
                proximo_numero_nf       AS ProximoNumeroNf,
                csc_id                  AS CscId,
                csc_token               AS CscToken,
                tipo_emissao_padrao     AS TipoEmissaoPadrao,
                tipo_impressao_default  AS TipoImpressaoPadrao,
                data_ultima_emissao     AS DataUltimaEmissao,
                tratamento_desconto     AS TratamentoDesconto,
                status                  AS Situacao,
                data_inclusao           AS DataInclusao,
                data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM public.configuracoes_emissoes
            WHERE id = @Id AND empresa_id = @EmpresaId
              AND data_exclusao IS NULL
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<ConfiguracaoEmissao>(sql, new { Id = id, EmpresaId = empresaId });
    }

    public async Task CadastrarConfiguracaoEmissaoAsync(ConfiguracaoEmissao config)
    {
        await using var conexao = CriarConexao();

        if (config.Situacao == "ativo")
        {
            const string sqlInativar = """
                UPDATE public.configuracoes_emissoes 
                SET status = 'inativo', data_ultima_atualizacao = NOW()
                WHERE empresa_id = @EmpresaId AND modelo = @Modelo AND id != @Id AND status = 'ativo'
                """;
            await conexao.ExecuteAsync(sqlInativar, new { EmpresaId = config.EmpresaId, Modelo = config.Modelo, Id = config.Id });
        }

        config.Id = Guid.NewGuid();
        const string sqlInsert = """
            INSERT INTO public.configuracoes_emissoes
                (id, empresa_id, certificado_id, ambiente, serie, modelo, proximo_numero_nf,
                 csc_id, csc_token, tipo_emissao_padrao, tipo_impressao_default,
                 tratamento_desconto, status, data_inclusao)
            VALUES
                (@Id, @EmpresaId, @CertificadoId, @Ambiente, @Serie, @Modelo, @ProximoNumeroNf,
                 @CscId, @CscToken, @TipoEmissaoPadrao, @TipoImpressaoPadrao,
                 @TratamentoDesconto, @Situacao, NOW())
            """;

        await conexao.ExecuteAsync(sqlInsert, new
        {
            Id                   = config.Id,
            EmpresaId            = config.EmpresaId,
            CertificadoId        = config.CertificadoId,
            Ambiente             = config.Ambiente,
            Serie                = config.Serie,
            Modelo               = config.Modelo,
            ProximoNumeroNf      = config.ProximoNumeroNf,
            CscId                = config.CscId,
            CscToken             = config.CscToken,
            TipoEmissaoPadrao    = config.TipoEmissaoPadrao,
            TipoImpressaoPadrao = config.TipoImpressaoPadrao,
            TratamentoDesconto   = config.TratamentoDesconto,
            Situacao               = config.Situacao
        });
    }

    public async Task AtualizarConfiguracaoEmissaoAsync(ConfiguracaoEmissao config)
    {
        await using var conexao = CriarConexao();

        if (config.Situacao == "ativo")
        {
            const string sqlInativar = """
                UPDATE public.configuracoes_emissoes 
                SET status = 'inativo', data_ultima_atualizacao = NOW()
                WHERE empresa_id = @EmpresaId AND modelo = @Modelo AND id != @Id AND status = 'ativo'
                """;
            await conexao.ExecuteAsync(sqlInativar, new { EmpresaId = config.EmpresaId, Modelo = config.Modelo, Id = config.Id });
        }

        const string sqlUpdate = """
            UPDATE public.configuracoes_emissoes SET
                certificado_id          = @CertificadoId,
                ambiente                = @Ambiente,
                serie                   = @Serie,
                modelo                  = @Modelo,
                proximo_numero_nf       = @ProximoNumeroNf,
                csc_id                  = @CscId,
                csc_token               = @CscToken,
                tipo_emissao_padrao     = @TipoEmissaoPadrao,
                tipo_impressao_default  = @TipoImpressaoPadrao,
                tratamento_desconto     = @TratamentoDesconto,
                status                  = @Situacao,
                data_ultima_atualizacao = NOW()
            WHERE id = @Id AND empresa_id = @EmpresaId
            """;
        
        await conexao.ExecuteAsync(sqlUpdate, new
        {
            Id                   = config.Id,
            EmpresaId            = config.EmpresaId,
            CertificadoId        = config.CertificadoId,
            Ambiente             = config.Ambiente,
            Serie                = config.Serie,
            Modelo               = config.Modelo,
            ProximoNumeroNf      = config.ProximoNumeroNf,
            CscId                = config.CscId,
            CscToken             = config.CscToken,
            TipoEmissaoPadrao    = config.TipoEmissaoPadrao,
            TipoImpressaoPadrao = config.TipoImpressaoPadrao,
            TratamentoDesconto   = config.TratamentoDesconto,
            Situacao               = config.Situacao
        });
    }

    public async Task DeletarSeguroConfiguracaoEmissaoAsync(Guid empresaId, Guid id)
    {
        const string sql = """
            UPDATE public.configuracoes_emissoes
            SET data_exclusao = NOW(), status = 'inativo', data_ultima_atualizacao = NOW()
            WHERE id = @Id AND empresa_id = @EmpresaId
              AND data_exclusao IS NULL
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = id, EmpresaId = empresaId });
    }
}



