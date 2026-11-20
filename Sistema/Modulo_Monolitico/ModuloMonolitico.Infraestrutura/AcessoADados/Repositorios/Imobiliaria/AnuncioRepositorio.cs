using Dapper;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class AnuncioRepositorio : RepositorioBase, IAnuncioRepositorio
{
    public AnuncioRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<ResultadoPaginado<AnuncioResumoDto>> ListarComFiltrosAsync(FiltroAnuncio filtro)
    {
        var where = new StringBuilder("WHERE a.status = 'ativo'");
        var parametros = new DynamicParameters();

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

            if (codigos.Count > 0)
            {
                where.Append("""
                     AND (
                        SELECT COUNT(DISTINCT am.codigo)
                        FROM imobiliaria_imovel_amenidades ia
                        INNER JOIN imobiliaria_amenidades am ON am.id = ia.amenidade_id
                        WHERE ia.imovel_id = i.id AND am.codigo = ANY(@CodigosAmenidade)
                    ) = @QtdAmenidades
                    """);
                parametros.Add("CodigosAmenidade", codigos.ToArray());
                parametros.Add("QtdAmenidades", codigos.Count);
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
                a.eh_destaque AS EhDestaque,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa,
                COALESCE(pa.nome_exibicao, emp.nome) AS NomeAnunciante,
                pa.foto_logo_url AS LogoAnunciante
            FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT e.nome
                FROM empresas e
                WHERE e.conta_id = a.conta_id
                LIMIT 1
            ) emp ON TRUE
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM imobiliaria_fotos f
                WHERE f.anuncio_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            {where}
            ORDER BY a.eh_destaque DESC, a.data_inclusao DESC
            LIMIT @Limite OFFSET @Offset
            """;

        var sqlBatch = $"{sqlCount}; {sqlItens};";

        parametros.Add("Limite", filtro.TamanhoPagina);
        parametros.Add("Offset", filtro.Offset);

        await using var conexao = CriarConexao();
        await using var multi = await conexao.QueryMultipleAsync(sqlBatch, parametros);
        
        var total = await multi.ReadFirstAsync<int>();
        var itens = await multi.ReadAsync<AnuncioResumoDto>();

        return new ResultadoPaginado<AnuncioResumoDto>(itens.ToList(), total, filtro.Pagina, filtro.TamanhoPagina);
    }

    public async Task<AnuncioDetalheDto?> ObterDetalheAsync(Guid anuncioId)
    {
        const string sqlAnuncio = """
            SELECT
                a.id AS AnuncioId,
                a.imovel_id AS ImovelId,
                a.usuario_id AS UsuarioId,
                a.titulo AS Titulo,
                a.descricao AS Descricao,
                a.tipo_negocio AS TipoNegocio,
                i.tipo_imovel AS TipoImovel,
                a.valor_principal AS ValorPrincipal,
                a.valor_condominio AS ValorCondominio,
                a.valor_iptu AS ValorIptu,
                (CASE WHEN a.tipo_negocio = 'aluguel' THEN (a.valor_principal + a.valor_condominio + a.valor_iptu) ELSE a.valor_principal END) AS PacoteTotal,
                a.contato_whatsapp AS ContatoWhatsapp,
                i.bairro AS Bairro,
                i.cep AS Cep,
                CASE WHEN a.mostrar_logradouro THEN i.logradouro ELSE NULL END AS Logradouro,
                a.mostrar_logradouro AS MostrarLogradouro,
                CASE WHEN a.mostrar_numero THEN i.numero ELSE NULL END AS Numero,
                a.mostrar_numero AS MostrarNumero,
                i.complemento AS Complemento,
                i.latitude AS Latitude,
                i.longitude AS Longitude,
                i.quartos AS Quartos,
                i.banheiros AS Banheiros,
                i.vagas_garagem AS VagasGaragem,
                i.area_total AS AreaTotal,
                i.andar AS Andar,
                i.tem_elevador AS TemElevador,
                i.tem_piscina AS TemPiscina,
                i.aceita_pet AS AceitaPet,
                i.tem_churrasqueira AS TemChurrasqueira,
                i.tem_salao_festas AS TemSalaoFestas,
                i.tem_quadra AS TemQuadra,
                i.tem_academia AS TemAcademia,
                i.tem_portaria_24h AS TemPortaria24h,
                i.tem_varanda AS TemVaranda,
                i.tem_playground AS TemPlayground,
                i.tem_agua_separada AS TemAguaSeparada,
                i.tem_luz_separada AS TemLuzSeparada,
                i.tem_entrada_separada AS TemEntradaSeparada,
                i.tem_cobertura AS TemCobertura,
                i.situacao_obra AS SituacaoObra,
                a.codigo_publico AS CodigoPublico,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.data_inclusao AS DataInclusao,
                a.conta_id AS ContaId,
                COALESCE(
                    pa.nome_exibicao,
                    (SELECT e.nome FROM empresas e WHERE e.conta_id = a.conta_id LIMIT 1)
                ) AS NomeAnunciante,
                pa.foto_logo_url AS LogoAnunciante,
                pa.creci AS CreciAnunciante,
                pa.site_url AS UrlSiteAnunciante,
                pa.instagram AS InstagramAnunciante
            FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = a.conta_id
            WHERE a.id = @AnuncioId
            """;

        const string sqlFotos = """
            SELECT url FROM imobiliaria_fotos
            WHERE anuncio_id = @AnuncioId
            ORDER BY ordem ASC
            """;

        const string sqlAmenidades = """
            SELECT
                a.id AS Id,
                a.codigo AS Codigo,
                a.nome AS Nome,
                a.categoria AS Categoria,
                a.ordem AS Ordem
            FROM imobiliaria_imovel_amenidades ia
            INNER JOIN imobiliaria_amenidades a ON a.id = ia.amenidade_id
            WHERE ia.imovel_id = @ImovelId
            ORDER BY
                CASE a.categoria
                    WHEN 'imovel' THEN 1
                    WHEN 'condominio' THEN 2
                    WHEN 'seguranca' THEN 3
                    ELSE 9
                END,
                a.ordem ASC,
                a.nome ASC
            """;

        await using var conexao = CriarConexao();
        var dto = await conexao.QueryFirstOrDefaultAsync<AnuncioDetalheDto>(sqlAnuncio, new { AnuncioId = anuncioId });

        if (dto is not null)
        {
            var fotos = await conexao.QueryAsync<string>(sqlFotos, new { AnuncioId = anuncioId });
            dto.Fotos = [.. fotos];
            var amenidades = await conexao.QueryAsync<AmenidadeDto>(sqlAmenidades, new { ImovelId = dto.ImovelId });
            dto.Amenidades = amenidades.ToList();
        }

        return dto;
    }

    public async Task<IEnumerable<AnuncioResumoDto>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        const string sql = """
            SELECT
                a.id AS AnuncioId,
                a.imovel_id AS ImovelId,
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
                a.eh_destaque AS EhDestaque,
                a.data_inclusao AS DataInclusao,
                (SELECT f.url FROM imobiliaria_fotos f WHERE f.anuncio_id = a.id ORDER BY f.ordem LIMIT 1) AS FotoCapa
            FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            WHERE a.usuario_id = @UsuarioId
            ORDER BY a.eh_destaque DESC, a.data_inclusao DESC
            """;

        await using var conexao = CriarConexao();   
        return await conexao.QueryAsync<AnuncioResumoDto>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<AnuncioResumoDto>> ListarDestaquesAsync(string? tipoNegocio, int limite = 6)
    {
        var where = new StringBuilder("WHERE a.status = 'ativo' AND a.eh_destaque = TRUE");
        var parametros = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(tipoNegocio))
        {
            where.Append(" AND a.tipo_negocio = @TipoNegocio");
            parametros.Add("TipoNegocio", tipoNegocio);
        }

        parametros.Add("Limite", limite);

        var sql = $"""
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
                a.eh_destaque AS EhDestaque,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa,
                COALESCE(pa.nome_exibicao, emp.nome) AS NomeAnunciante,
                pa.foto_logo_url AS LogoAnunciante
            FROM imobiliaria_anuncios a
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT e.nome
                FROM empresas e
                WHERE e.conta_id = a.conta_id
                LIMIT 1
            ) emp ON TRUE
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM imobiliaria_fotos f
                WHERE f.anuncio_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            {where}
            ORDER BY a.data_inclusao DESC
            LIMIT @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioResumoDto>(sql, parametros);
    }

    public async Task<ResultadoPaginado<AnuncioResumoDto>> ListarPorFavoritosDoUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina)
    {
        const string sqlCount = """
            SELECT COUNT(*)
            FROM imoveis_favoritos fav
            INNER JOIN LATERAL (
                SELECT a2.id
                FROM imobiliaria_anuncios a2
                WHERE a2.imovel_id = fav.imovel_id AND a2.status = 'ativo'
                ORDER BY a2.data_inclusao DESC
                LIMIT 1
            ) anuncio_ativo ON TRUE
            WHERE fav.usuario_id = @UsuarioId
            """;

        const string sqlItens = """
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
                a.eh_destaque AS EhDestaque,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa,
                COALESCE(pa.nome_exibicao, emp.nome) AS NomeAnunciante,
                pa.foto_logo_url AS LogoAnunciante
            FROM imoveis_favoritos fav
            INNER JOIN LATERAL (
                SELECT a2.*
                FROM imobiliaria_anuncios a2
                WHERE a2.imovel_id = fav.imovel_id AND a2.status = 'ativo'
                ORDER BY a2.data_inclusao DESC
                LIMIT 1
            ) a ON TRUE
            INNER JOIN imobiliaria_imoveis i ON i.id = a.imovel_id
            LEFT JOIN imobiliaria_perfis_anunciante pa ON pa.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT e.nome
                FROM empresas e
                WHERE e.conta_id = a.conta_id
                LIMIT 1
            ) emp ON TRUE
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM imobiliaria_fotos f
                WHERE f.anuncio_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            WHERE fav.usuario_id = @UsuarioId
            ORDER BY fav.data_criacao DESC
            LIMIT @Limite OFFSET @Offset
            """;

        var parametros = new DynamicParameters();
        parametros.Add("UsuarioId", usuarioId);
        parametros.Add("Limite", tamanhoPagina);
        parametros.Add("Offset", (pagina - 1) * tamanhoPagina);

        var sqlBatch = $"{sqlCount}; {sqlItens};";

        await using var conexao = CriarConexao();
        await using var multi = await conexao.QueryMultipleAsync(sqlBatch, parametros);

        var total = await multi.ReadFirstAsync<int>();
        var itens = await multi.ReadAsync<AnuncioResumoDto>();

        return new ResultadoPaginado<AnuncioResumoDto>(itens.ToList(), total, pagina, tamanhoPagina);
    }

    public async Task CadastrarAsync(Imovel imovel, Anuncio anuncio)
    {
        const string sqlImovel = """
            INSERT INTO imobiliaria_imoveis (
                id, conta_id, usuario_id, tipo_imovel, area_total, quartos, banheiros,
                vagas_garagem, andar, total_andares, tem_elevador, tem_piscina,
                aceita_pet, tem_churrasqueira, tem_salao_festas, tem_quadra,
                tem_academia, tem_portaria_24h, tem_varanda, tem_playground,
                tem_agua_separada, tem_luz_separada, tem_entrada_separada, tem_cobertura,
                situacao_obra, logradouro, numero, complemento, bairro, cidade, cep, latitude, longitude
            ) VALUES (
                @Id, @ContaId, @UsuarioId, @TipoImovel, @AreaTotal, @Quartos, @Banheiros,
                @VagasGaragem, @Andar, @TotalAndares, @TemElevador, @TemPiscina,
                @AceitaPet, @TemChurrasqueira, @TemSalaoFestas, @TemQuadra,
                @TemAcademia, @TemPortaria24h, @TemVaranda, @TemPlayground,
                @TemAguaSeparada, @TemLuzSeparada, @TemEntradaSeparada, @TemCobertura,
                @SituacaoObra, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Cep, @Latitude, @Longitude
            )
            """;

        const string sqlAnuncio = """
            INSERT INTO imobiliaria_anuncios (
                id, imovel_id, conta_id, usuario_id, tipo_negocio, valor_principal,
                valor_condominio, valor_iptu, titulo, descricao, contato_whatsapp,
                mostrar_logradouro, mostrar_numero, status, codigo_publico, data_expiracao
            ) VALUES (
                @Id, @ImovelId, @ContaId, @UsuarioId, @TipoNegocio, @ValorPrincipal,
                @ValorCondominio, @ValorIptu, @Titulo, @Descricao, @ContatoWhatsapp,
                @MostrarLogradouro, @MostrarNumero, @Situacao, @CodigoPublico, @DataExpiracao
            )
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlImovel, imovel);
            await conexao.ExecuteAsync(sqlAnuncio, anuncio);
        });
    }

    public async Task AtualizarAsync(Imovel imovel, Anuncio anuncio)
    {
        const string sqlImovel = """
            UPDATE imobiliaria_imoveis SET
                tipo_imovel = @TipoImovel, area_total = @AreaTotal, quartos = @Quartos, banheiros = @Banheiros,
                vagas_garagem = @VagasGaragem, andar = @Andar, total_andares = @TotalAndares, tem_elevador = @TemElevador,
                tem_piscina = @TemPiscina, aceita_pet = @AceitaPet, tem_churrasqueira = @TemChurrasqueira,
                tem_salao_festas = @TemSalaoFestas, tem_quadra = @TemQuadra, tem_academia = @TemAcademia,
                tem_portaria_24h = @TemPortaria24h, tem_varanda = @TemVaranda, tem_playground = @TemPlayground,
                tem_agua_separada = @TemAguaSeparada, tem_luz_separada = @TemLuzSeparada,
                tem_entrada_separada = @TemEntradaSeparada, tem_cobertura = @TemCobertura,
                situacao_obra = @SituacaoObra,
                logradouro = @Logradouro, numero = @Numero, complemento = @Complemento,
                bairro = @Bairro, cidade = @Cidade, cep = @Cep, latitude = @Latitude, longitude = @Longitude
            WHERE id = @Id AND conta_id = @ContaId
            """;

        const string sqlAnuncio = """
            UPDATE imobiliaria_anuncios SET
                tipo_negocio = @TipoNegocio, valor_principal = @ValorPrincipal, valor_condominio = @ValorCondominio,
                valor_iptu = @ValorIptu, titulo = @Titulo, descricao = @Descricao, contato_whatsapp = @ContatoWhatsapp,
                mostrar_logradouro = @MostrarLogradouro, mostrar_numero = @MostrarNumero, data_ultima_atualizacao = NOW()
            WHERE id = @Id AND conta_id = @ContaId
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlImovel, imovel);
            await conexao.ExecuteAsync(sqlAnuncio, anuncio);
        });
    }

    public async Task AtualizarAnuncioAsync(Anuncio anuncio)
    {
        const string sql = """
            UPDATE imobiliaria_anuncios SET
                tipo_negocio = @TipoNegocio,
                valor_principal = @ValorPrincipal,
                valor_condominio = @ValorCondominio,
                valor_iptu = @ValorIptu,
                titulo = @Titulo,
                descricao = @Descricao,
                contato_whatsapp = @ContatoWhatsapp,
                mostrar_logradouro = @MostrarLogradouro,
                mostrar_numero = @MostrarNumero,
                data_ultima_atualizacao = NOW()
            WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, anuncio);
    }

    public async Task AtualizarSituacaoAsync(Guid anuncioId, string novaSituacao)
    {
        const string sql = """
            UPDATE imobiliaria_anuncios SET
                status = @Situacao,
                data_ultima_atualizacao = NOW()
            WHERE id = @AnuncioId
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { AnuncioId = anuncioId, Situacao = novaSituacao });
    }

    public async Task AlternarDestaqueAsync(Guid anuncioId, bool ehDestaque)
    {
        const string sql = """
            UPDATE imobiliaria_anuncios SET
                eh_destaque = @EhDestaque,
                data_ultima_atualizacao = NOW()
            WHERE id = @AnuncioId
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { AnuncioId = anuncioId, EhDestaque = ehDestaque });
    }

    public async Task CadastrarFotoAsync(AnuncioFoto foto)
    {
        const string sql = """
            INSERT INTO imobiliaria_fotos (id, anuncio_id, url, ordem)
            VALUES (@Id, @AnuncioId, @UrlImagem, @Ordem)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, foto);
    }

    public async Task DeletarFisicoFotoAsync(Guid fotoId)
    {
        const string sql = "DELETE FROM imobiliaria_fotos WHERE id = @Id";

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { Id = fotoId });
    }

    public async Task<AnuncioFoto?> ObterFotoPorIdAsync(Guid fotoId)
    {
        const string sql = """
            SELECT id AS Id, anuncio_id AS AnuncioId, url AS UrlImagem, ordem AS Ordem
            FROM imobiliaria_fotos WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<AnuncioFoto>(sql, new { Id = fotoId });
    }

    public async Task<Anuncio?> ObterAnuncioPorIdAsync(Guid anuncioId)
    {
        const string sql = """
            SELECT id AS Id, imovel_id AS ImovelId, conta_id AS ContaId,
                   usuario_id AS UsuarioId, tipo_negocio AS TipoNegocio,
                   valor_principal AS ValorPrincipal, valor_condominio AS ValorCondominio,
                   valor_iptu AS ValorIptu, titulo AS Titulo, descricao AS Descricao,
                   contato_whatsapp AS ContatoWhatsapp, mostrar_logradouro AS MostrarLogradouro,
                   mostrar_numero AS MostrarNumero, status AS Situacao,
                   eh_destaque AS EhDestaque,
                   data_expiracao AS DataExpiracao
            FROM imobiliaria_anuncios WHERE id = @Id
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<Anuncio>(sql, new { Id = anuncioId });
    }

    public async Task<int> ContarFotosAsync(Guid anuncioId)
    {
        const string sql = "SELECT COUNT(*) FROM imobiliaria_fotos WHERE anuncio_id = @AnuncioId";
        await using var conexao = CriarConexao();
        return await conexao.ExecuteScalarAsync<int>(sql, new { AnuncioId = anuncioId });
    }

    public async Task<IEnumerable<AnuncioFoto>> ListarFotosPorAnuncioAsync(Guid anuncioId)
    {
        const string sql = """
            SELECT 
                id AS Id,
                anuncio_id AS AnuncioId,
                url AS UrlImagem,
                ordem AS Ordem,
                data_inclusao AS DataInclusao
            FROM imobiliaria_fotos
            WHERE anuncio_id = @AnuncioId
            ORDER BY ordem ASC
            """;
        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioFoto>(sql, new { AnuncioId = anuncioId });
    }
}
