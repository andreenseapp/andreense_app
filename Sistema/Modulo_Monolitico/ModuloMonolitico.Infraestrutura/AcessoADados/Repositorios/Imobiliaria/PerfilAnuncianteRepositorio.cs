using Dapper;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class PerfilAnuncianteRepositorio : RepositorioBase, IPerfilAnuncianteRepositorio
{
    public PerfilAnuncianteRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<PerfilAnuncianteDto?> ObterPorContaIdAsync(Guid contaId)
    {
        const string sql = """
            SELECT
                pa.conta_id AS ContaId,
                COALESCE(pa.nome_exibicao, e.nome) AS NomeExibicao,
                pa.descricao AS Descricao,
                pa.foto_logo_url AS UrlFotoLogo,
                pa.whatsapp AS Whatsapp,
                pa.site_url AS UrlSite,
                pa.instagram AS Instagram,
                pa.creci AS Creci,
                (SELECT COUNT(*) FROM imobiliaria_anuncios a WHERE a.conta_id = @ContaId AND a.status = 'ativo') AS TotalAnuncios
            FROM empresas e
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = e.conta_id
            WHERE e.conta_id = @ContaId
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<PerfilAnuncianteDto>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarAsync(PerfilAnunciante perfil)
    {
        const string sql = """
            INSERT INTO imobiliaria_perfis_anunciante
                (id, conta_id, nome_exibicao, descricao, foto_logo_url, whatsapp, site_url, instagram, creci, data_inclusao, data_atualizacao)
            VALUES
                (@Id, @ContaId, @NomeExibicao, @Descricao, @UrlFotoLogo, @Whatsapp, @UrlSite, @Instagram, @Creci, @DataInclusao, @DataAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, perfil);
    }

    public async Task AtualizarAsync(PerfilAnunciante perfil)
    {
        const string sql = """
            UPDATE imobiliaria_perfis_anunciante SET
                nome_exibicao = @NomeExibicao,
                descricao = @Descricao,
                foto_logo_url = @UrlFotoLogo,
                whatsapp = @Whatsapp,
                site_url = @UrlSite,
                instagram = @Instagram,
                creci = @Creci,
                data_atualizacao = @DataAtualizacao
            WHERE conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, perfil);
    }

    public async Task<ResultadoPaginado<AnuncioResumoDto>> ListarAnunciosPorContaAsync(Guid contaId, FiltroAnuncio filtro)
    {
        var where = new StringBuilder("WHERE a.status = 'ativo' AND a.conta_id = @ContaId");
        var parametros = new DynamicParameters();
        parametros.Add("ContaId", contaId);

        if (!string.IsNullOrWhiteSpace(filtro.TipoNegocio))
        {
            where.Append(" AND a.tipo_negocio = @TipoNegocio");
            parametros.Add("TipoNegocio", filtro.TipoNegocio);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoImovel))
        {
            where.Append(" AND i.tipo_imovel = @TipoImovel");
            parametros.Add("TipoImovel", filtro.TipoImovel);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Bairro))
        {
            where.Append(" AND LOWER(i.bairro) LIKE LOWER(@Bairro)");
            parametros.Add("Bairro", $"%{filtro.Bairro}%");
        }

        if (filtro.QuartosMinimo.HasValue)
        {
            where.Append(" AND i.quartos >= @QuartosMinimo");
            parametros.Add("QuartosMinimo", filtro.QuartosMinimo.Value);
        }

        if (filtro.BanheirosMinimo.HasValue)
        {
            where.Append(" AND i.banheiros >= @BanheirosMinimo");
            parametros.Add("BanheirosMinimo", filtro.BanheirosMinimo.Value);
        }

        if (filtro.VagasMinimo.HasValue)
        {
            where.Append(" AND i.vagas_garagem >= @VagasMinimo");
            parametros.Add("VagasMinimo", filtro.VagasMinimo.Value);
        }

        if (filtro.ValorMinimo.HasValue)
        {
            var campoValorMin = filtro.BuscarPorPacote
                ? "(a.valor_principal + a.valor_condominio + a.valor_iptu)"
                : "a.valor_principal";
            where.Append($" AND {campoValorMin} >= @ValorMinimo");
            parametros.Add("ValorMinimo", filtro.ValorMinimo.Value);
        }

        if (filtro.ValorMaximo.HasValue)
        {
            var campoValor = filtro.BuscarPorPacote
                ? "(a.valor_principal + a.valor_condominio + a.valor_iptu)"
                : "a.valor_principal";
            where.Append($" AND {campoValor} <= @ValorMaximo");
            parametros.Add("ValorMaximo", filtro.ValorMaximo.Value);
        }

        if (filtro.ValorCondominioMinimo.HasValue)
        {
            where.Append(" AND a.valor_condominio >= @ValorCondominioMinimo");
            parametros.Add("ValorCondominioMinimo", filtro.ValorCondominioMinimo.Value);
        }

        if (filtro.ValorCondominioMaximo.HasValue)
        {
            where.Append(" AND a.valor_condominio <= @ValorCondominioMaximo");
            parametros.Add("ValorCondominioMaximo", filtro.ValorCondominioMaximo.Value);
        }

        if (filtro.AreaMinima.HasValue)
        {
            where.Append(" AND i.area_total >= @AreaMinima");
            parametros.Add("AreaMinima", filtro.AreaMinima.Value);
        }

        if (filtro.AreaMaxima.HasValue)
        {
            where.Append(" AND i.area_total <= @AreaMaxima");
            parametros.Add("AreaMaxima", filtro.AreaMaxima.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.SituacaoObra))
        {
            where.Append(" AND i.situacao_obra = @SituacaoObra");
            parametros.Add("SituacaoObra", filtro.SituacaoObra);
        }

        if (!string.IsNullOrWhiteSpace(filtro.CodigoPublico))
        {
            where.Append(" AND LOWER(a.codigo_publico) = LOWER(@CodigoPublico)");
            parametros.Add("CodigoPublico", filtro.CodigoPublico.Trim());
        }

        if (filtro.CodigosAmenidade is { Count: > 0 })
        {
            var codigos = filtro.CodigosAmenidade
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (var i = 0; i < codigos.Count; i++)
            {
                var nomeParam = $"CodigoAmenidade{i}";
                where.Append($"""
                     AND EXISTS (
                        SELECT 1
                        FROM imobiliaria_imovel_amenidades ia
                        INNER JOIN imobiliaria_amenidades am ON am.id = ia.amenidade_id
                        WHERE ia.imovel_id = i.id AND am.codigo = @{nomeParam}
                    )
                    """);
                parametros.Add(nomeParam, codigos[i]);
            }
        }

        var sqlCount = $"""
            SELECT COUNT(*) FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            {where}
            """;

        var sqlItens = $"""
            SELECT
                a.id AS AnuncioId,
                a.imovel_id AS ImovelId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                a.tipo_negocio AS TipoNegocio,
                i.tipo_imovel AS TipoImovel,
                a.valor_principal AS ValorPrincipal,
                a.valor_condominio AS ValorCondominio,
                a.valor_iptu AS ValorIptu,
                (CASE WHEN a.tipo_negocio = 'aluguel' THEN (a.valor_principal + a.valor_condominio + a.valor_iptu) ELSE a.valor_principal END) AS PacoteTotal,
                i.bairro AS Bairro,
                i.quartos AS Quartos,
                i.banheiros AS Banheiros,
                i.vagas_garagem AS VagasGaragem,
                i.area_total AS AreaTotal,
                a.status AS Situacao,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa,
                COALESCE(
                    pa.nome_exibicao,
                    (SELECT e.nome FROM empresas e WHERE e.conta_id = a.conta_id LIMIT 1)
                ) AS NomeAnunciante,
                pa.foto_logo_url AS LogoAnunciante
            FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM imobiliaria_fotos f
                WHERE f.anuncio_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            {where}
            ORDER BY a.data_inclusao DESC
            LIMIT @Limite OFFSET @Offset
            """;

        parametros.Add("Limite", filtro.TamanhoPagina);
        parametros.Add("Offset", filtro.Offset);

        await using var conexao = CriarConexao();
        var total = await conexao.ExecuteScalarAsync<int>(sqlCount, parametros);
        var itens = await conexao.QueryAsync<AnuncioResumoDto>(sqlItens, parametros);

        return new ResultadoPaginado<AnuncioResumoDto>(itens, total, filtro.Pagina, filtro.TamanhoPagina);
    }
}
