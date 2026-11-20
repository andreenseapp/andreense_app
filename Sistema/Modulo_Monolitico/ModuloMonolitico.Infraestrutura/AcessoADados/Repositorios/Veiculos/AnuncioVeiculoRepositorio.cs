using Dapper;
using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Veiculos;

public class AnuncioVeiculoRepositorio : RepositorioBase, IAnuncioVeiculoRepositorio
{
    public AnuncioVeiculoRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<ResultadoPaginado<AnuncioVeiculoResumoDto>> ListarComFiltrosAsync(FiltroAnuncioVeiculo filtro)
    {
        var where = new StringBuilder("WHERE a.status = 'ativo'");
        var parametros = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filtro.Marca))
        {
            where.Append(" AND LOWER(v.marca) LIKE LOWER(@Marca)");
            parametros.Add("Marca", $"%{filtro.Marca}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Modelo))
        {
            where.Append(" AND LOWER(v.modelo) LIKE LOWER(@Modelo)");
            parametros.Add("Modelo", $"%{filtro.Modelo}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoVeiculo))
        {
            where.Append(" AND v.tipo_veiculo = @TipoVeiculo");
            parametros.Add("TipoVeiculo", filtro.TipoVeiculo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Combustivel))
        {
            where.Append(" AND v.combustivel = @Combustivel");
            parametros.Add("Combustivel", filtro.Combustivel);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Cambio))
        {
            where.Append(" AND v.cambio = @Cambio");
            parametros.Add("Cambio", filtro.Cambio);
        }

        if (filtro.AnoMinimo.HasValue)
        {
            where.Append(" AND v.ano_modelo >= @AnoMinimo");
            parametros.Add("AnoMinimo", filtro.AnoMinimo.Value);
        }

        if (filtro.AnoMaximo.HasValue)
        {
            where.Append(" AND v.ano_modelo <= @AnoMaximo");
            parametros.Add("AnoMaximo", filtro.AnoMaximo.Value);
        }

        if (filtro.KmMaximo.HasValue)
        {
            where.Append(" AND v.quilometragem <= @KmMaximo");
            parametros.Add("KmMaximo", filtro.KmMaximo.Value);
        }

        if (filtro.PrecoMinimo.HasValue)
        {
            where.Append(" AND a.preco >= @PrecoMinimo");
            parametros.Add("PrecoMinimo", filtro.PrecoMinimo.Value);
        }

        if (filtro.PrecoMaximo.HasValue)
        {
            where.Append(" AND a.preco <= @PrecoMaximo");
            parametros.Add("PrecoMaximo", filtro.PrecoMaximo.Value);
        }

        if (filtro.AceitaTroca.HasValue)
        {
            where.Append(" AND a.aceita_troca = @AceitaTroca");
            parametros.Add("AceitaTroca", filtro.AceitaTroca.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.CodigoPublico))
        {
            where.Append(" AND LOWER(a.codigo_publico) = LOWER(@CodigoPublico)");
            parametros.Add("CodigoPublico", filtro.CodigoPublico.Trim());
        }

        var sqlCount = $"""
            SELECT COUNT(*) FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            {where}
            """;

        var sqlItens = $"""
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.versao AS Versao,
                v.ano_fabricacao AS AnoFabricacao,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa,
                COALESCE(pv.nome_exibicao, emp.nome) AS NomeVendedor,
                pv.foto_logo_url AS LogoVendedor
            FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN veiculos_perfis_vendedor pv ON pv.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT e.nome
                FROM empresas e
                WHERE e.conta_id = a.conta_id
                LIMIT 1
            ) emp ON TRUE
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM veiculos_fotos f
                WHERE f.anuncio_veiculo_id = a.id
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
        var itens = await multi.ReadAsync<AnuncioVeiculoResumoDto>();

        return new ResultadoPaginado<AnuncioVeiculoResumoDto>(itens.ToList(), total, filtro.Pagina, filtro.TamanhoPagina);
    }

    public async Task<AnuncioVeiculoDetalheDto?> ObterDetalheAsync(Guid anuncioVeiculoId)
    {
        var sql = """
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.usuario_id AS UsuarioId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                a.descricao AS Descricao,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.versao AS Versao,
                v.ano_fabricacao AS AnoFabricacao,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                v.placa AS Placa,
                v.tipo_veiculo AS TipoVeiculo,
                v.numero_portas AS NumeroPortas,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.contato_whatsapp AS ContatoWhatsapp,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                COALESCE(pv.nome_exibicao, emp.nome) AS NomeVendedor,
                pv.foto_logo_url AS LogoVendedor,
                pv.url_site AS UrlSiteVendedor,
                pv.instagram AS InstagramVendedor
            FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN veiculos_perfis_vendedor pv ON pv.conta_id = a.conta_id
            LEFT JOIN LATERAL (
                SELECT e.nome
                FROM empresas e
                WHERE e.conta_id = a.conta_id
                LIMIT 1
            ) emp ON TRUE
            WHERE a.id = @AnuncioVeiculoId;

            SELECT url FROM veiculos_fotos
            WHERE anuncio_veiculo_id = @AnuncioVeiculoId
            ORDER BY ordem;
            """;

        await using var conexao = CriarConexao();
        await using var multi = await conexao.QueryMultipleAsync(sql, new { AnuncioVeiculoId = anuncioVeiculoId });

        var dto = await multi.ReadFirstOrDefaultAsync<AnuncioVeiculoDetalheDto>();
        if (dto is null) return null;

        dto.Fotos = (await multi.ReadAsync<string>()).ToList();
        return dto;
    }

    public async Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        var sql = """
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.versao AS Versao,
                v.ano_fabricacao AS AnoFabricacao,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa
            FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM veiculos_fotos f
                WHERE f.anuncio_veiculo_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            WHERE a.usuario_id = @UsuarioId
            ORDER BY a.data_inclusao DESC
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioVeiculoResumoDto>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarDestaquesAsync(int limite = 6)
    {
        var sql = """
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa
            FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM veiculos_fotos f
                WHERE f.anuncio_veiculo_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            WHERE a.status = 'ativo' AND a.eh_destaque = true
            ORDER BY a.data_inclusao DESC
            LIMIT @Limite
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioVeiculoResumoDto>(sql, new { Limite = limite });
    }

    public async Task<ResultadoPaginado<AnuncioVeiculoResumoDto>> ListarPorFavoritosDoUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina)
    {
        var offset = (pagina - 1) * tamanhoPagina;

        var sqlCount = """
            SELECT COUNT(*) FROM veiculos_favoritos fav
            INNER JOIN veiculos_anuncios a ON a.id = fav.anuncio_veiculo_id
            WHERE fav.usuario_id = @UsuarioId AND a.status = 'ativo'
            """;

        var sqlItens = """
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa
            FROM veiculos_favoritos fav
            INNER JOIN veiculos_anuncios a ON a.id = fav.anuncio_veiculo_id
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM veiculos_fotos f
                WHERE f.anuncio_veiculo_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            WHERE fav.usuario_id = @UsuarioId AND a.status = 'ativo'
            ORDER BY fav.data_criacao DESC
            LIMIT @Limite OFFSET @Offset
            """;

        var sqlBatch = $"{sqlCount}; {sqlItens};";
        var parametros = new { UsuarioId = usuarioId, Limite = tamanhoPagina, Offset = offset };

        await using var conexao = CriarConexao();
        await using var multi = await conexao.QueryMultipleAsync(sqlBatch, parametros);

        var total = await multi.ReadFirstAsync<int>();
        var itens = await multi.ReadAsync<AnuncioVeiculoResumoDto>();

        return new ResultadoPaginado<AnuncioVeiculoResumoDto>(itens.ToList(), total, pagina, tamanhoPagina);
    }

    public async Task CadastrarAsync(Veiculo veiculo, AnuncioVeiculo anuncio)
    {
        var sqlVeiculo = """
            INSERT INTO veiculos (id, conta_id, usuario_id, marca, modelo, versao, ano_fabricacao, ano_modelo,
                quilometragem, combustivel, cambio, cor, placa, tipo_veiculo, numero_portas, data_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @ContaId, @UsuarioId, @Marca, @Modelo, @Versao, @AnoFabricacao, @AnoModelo,
                @Quilometragem, @Combustivel, @Cambio, @Cor, @Placa, @TipoVeiculo, @NumeroPortas, @DataInclusao, @DataUltimaAtualizacao)
            """;

        var sqlAnuncio = """
            INSERT INTO veiculos_anuncios (id, veiculo_id, conta_id, usuario_id, titulo, descricao, preco, aceita_troca,
                contato_whatsapp, status, eh_destaque, codigo_publico, data_expiracao, data_inclusao, data_ultima_atualizacao)
            VALUES (@Id, @VeiculoId, @ContaId, @UsuarioId, @Titulo, @Descricao, @Preco, @AceitaTroca,
                @ContatoWhatsapp, @Situacao, @EhDestaque, @CodigoPublico, @DataExpiracao, @DataInclusao, @DataUltimaAtualizacao)
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlVeiculo, veiculo);
            await conexao.ExecuteAsync(sqlAnuncio, anuncio);
        });
    }

    public async Task AtualizarAsync(Veiculo veiculo, AnuncioVeiculo anuncio)
    {
        var sqlVeiculo = """
            UPDATE veiculos SET
                marca = @Marca, modelo = @Modelo, versao = @Versao,
                ano_fabricacao = @AnoFabricacao, ano_modelo = @AnoModelo,
                quilometragem = @Quilometragem, combustivel = @Combustivel,
                cambio = @Cambio, cor = @Cor, placa = @Placa,
                tipo_veiculo = @TipoVeiculo, numero_portas = @NumeroPortas,
                data_ultima_atualizacao = NOW()
            WHERE id = @Id
            """;

        var sqlAnuncio = """
            UPDATE veiculos_anuncios SET
                titulo = @Titulo, descricao = @Descricao, preco = @Preco,
                aceita_troca = @AceitaTroca, contato_whatsapp = @ContatoWhatsapp,
                data_ultima_atualizacao = NOW()
            WHERE id = @Id
            """;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(sqlVeiculo, veiculo);
            await conexao.ExecuteAsync(sqlAnuncio, anuncio);
        });
    }

    public async Task AtualizarSituacaoAsync(Guid anuncioVeiculoId, string novaSituacao)
    {
        var sql = "UPDATE veiculos_anuncios SET status = @NovaSituacao, data_ultima_atualizacao = NOW() WHERE id = @AnuncioVeiculoId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { AnuncioVeiculoId = anuncioVeiculoId, NovaSituacao = novaSituacao });
    }

    public async Task AlternarDestaqueAsync(Guid anuncioVeiculoId, bool ehDestaque)
    {
        var sql = "UPDATE veiculos_anuncios SET eh_destaque = @EhDestaque, data_ultima_atualizacao = NOW() WHERE id = @AnuncioVeiculoId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { AnuncioVeiculoId = anuncioVeiculoId, EhDestaque = ehDestaque });
    }

    public async Task<AnuncioVeiculo?> ObterAnuncioPorIdAsync(Guid anuncioVeiculoId)
    {
        var sql = """
            SELECT id AS Id, veiculo_id AS VeiculoId, conta_id AS ContaId, usuario_id AS UsuarioId,
                titulo AS Titulo, descricao AS Descricao, preco AS Preco, aceita_troca AS AceitaTroca,
                contato_whatsapp AS ContatoWhatsapp, status AS Situacao, eh_destaque AS EhDestaque,
                codigo_publico AS CodigoPublico, data_expiracao AS DataExpiracao,
                data_inclusao AS DataInclusao, data_ultima_atualizacao AS DataUltimaAtualizacao
            FROM veiculos_anuncios WHERE id = @AnuncioVeiculoId
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<AnuncioVeiculo>(sql, new { AnuncioVeiculoId = anuncioVeiculoId });
    }

    public async Task CadastrarFotoAsync(AnuncioVeiculoFoto foto)
    {
        var sql = """
            INSERT INTO veiculos_fotos (id, anuncio_veiculo_id, url, ordem, data_inclusao)
            VALUES (@Id, @AnuncioVeiculoId, @UrlImagem, @Ordem, @DataInclusao)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, foto);
    }

    public async Task DeletarFisicoFotoAsync(Guid fotoId)
    {
        var sql = "DELETE FROM veiculos_fotos WHERE id = @FotoId";
        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, new { FotoId = fotoId });
    }

    public async Task<AnuncioVeiculoFoto?> ObterFotoPorIdAsync(Guid fotoId)
    {
        var sql = """
            SELECT id AS Id, anuncio_veiculo_id AS AnuncioVeiculoId, url AS UrlImagem,
                ordem AS Ordem, data_inclusao AS DataInclusao
            FROM veiculos_fotos WHERE id = @FotoId
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<AnuncioVeiculoFoto>(sql, new { FotoId = fotoId });
    }

    public async Task<int> ContarFotosAsync(Guid anuncioVeiculoId)
    {
        var sql = "SELECT COUNT(*) FROM veiculos_fotos WHERE anuncio_veiculo_id = @AnuncioVeiculoId";
        await using var conexao = CriarConexao();
        return await conexao.QueryFirstAsync<int>(sql, new { AnuncioVeiculoId = anuncioVeiculoId });
    }

    public async Task<IEnumerable<AnuncioVeiculoFoto>> ListarFotosPorAnuncioAsync(Guid anuncioVeiculoId)
    {
        var sql = """
            SELECT id AS Id, anuncio_veiculo_id AS AnuncioVeiculoId, url AS UrlImagem,
                ordem AS Ordem, data_inclusao AS DataInclusao
            FROM veiculos_fotos
            WHERE anuncio_veiculo_id = @AnuncioVeiculoId
            ORDER BY ordem
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioVeiculoFoto>(sql, new { AnuncioVeiculoId = anuncioVeiculoId });
    }

    public async Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarPorVendedorAsync(Guid contaId, int pagina, int tamanhoPagina)
    {
        var offset = (pagina - 1) * tamanhoPagina;

        var sql = """
            SELECT
                a.id AS AnuncioVeiculoId,
                a.veiculo_id AS VeiculoId,
                a.conta_id AS ContaId,
                a.titulo AS Titulo,
                v.marca AS Marca,
                v.modelo AS Modelo,
                v.ano_modelo AS AnoModelo,
                v.quilometragem AS Quilometragem,
                v.combustivel AS Combustivel,
                v.cambio AS Cambio,
                v.cor AS Cor,
                a.preco AS Preco,
                a.aceita_troca AS AceitaTroca,
                a.status AS Situacao,
                a.eh_destaque AS EhDestaque,
                a.codigo_publico AS CodigoPublico,
                a.data_inclusao AS DataInclusao,
                capa.url AS FotoCapa
            FROM veiculos_anuncios a
            INNER JOIN veiculos v ON v.id = a.veiculo_id
            LEFT JOIN LATERAL (
                SELECT f.url
                FROM veiculos_fotos f
                WHERE f.anuncio_veiculo_id = a.id
                ORDER BY f.ordem
                LIMIT 1
            ) capa ON TRUE
            WHERE a.conta_id = @ContaId AND a.status = 'ativo'
            ORDER BY a.eh_destaque DESC, a.data_inclusao DESC
            LIMIT @Limite OFFSET @Offset
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryAsync<AnuncioVeiculoResumoDto>(sql, new { ContaId = contaId, Limite = tamanhoPagina, Offset = offset });
    }
}
