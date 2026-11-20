using Dapper;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Veiculos;

public class PerfilVendedorRepositorio : RepositorioBase, IPerfilVendedorRepositorio
{
    public PerfilVendedorRepositorio(string conexaoString) : base(conexaoString) { }

    public async Task<PerfilVendedorDto?> ObterPorContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT
                pv.conta_id AS ContaId,
                COALESCE(pv.nome_exibicao, e.nome) AS NomeExibicao,
                pv.descricao AS Descricao,
                pv.foto_logo_url AS UrlFotoLogo,
                pv.whatsapp AS Whatsapp,
                pv.url_site AS UrlSite,
                pv.instagram AS Instagram
            FROM empresas e
            LEFT JOIN veiculos_perfis_vendedor pv ON pv.conta_id = e.conta_id
            WHERE e.conta_id = @ContaId
            LIMIT 1
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<PerfilVendedorDto>(sql, new { ContaId = contaId });
    }

    public async Task CadastrarAsync(PerfilVendedor perfil)
    {
        const string sql = """
            INSERT INTO veiculos_perfis_vendedor
                (id, conta_id, nome_exibicao, descricao, foto_logo_url, whatsapp, url_site, instagram, data_inclusao, data_atualizacao)
            VALUES
                (@Id, @ContaId, @NomeExibicao, @Descricao, @UrlFotoLogo, @Whatsapp, @UrlSite, @Instagram, @DataInclusao, @DataAtualizacao)
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, perfil);
    }

    public async Task AtualizarAsync(PerfilVendedor perfil)
    {
        const string sql = """
            UPDATE veiculos_perfis_vendedor SET
                nome_exibicao = @NomeExibicao,
                descricao = @Descricao,
                foto_logo_url = @UrlFotoLogo,
                whatsapp = @Whatsapp,
                url_site = @UrlSite,
                instagram = @Instagram,
                data_atualizacao = NOW()
            WHERE conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.ExecuteAsync(sql, perfil);
    }

    public async Task<PerfilVendedor?> ObterEntidadePorContaAsync(Guid contaId)
    {
        const string sql = """
            SELECT id AS Id, conta_id AS ContaId, nome_exibicao AS NomeExibicao,
                descricao AS Descricao, foto_logo_url AS UrlFotoLogo, whatsapp AS Whatsapp,
                url_site AS UrlSite, instagram AS Instagram,
                data_inclusao AS DataInclusao, data_atualizacao AS DataAtualizacao
            FROM veiculos_perfis_vendedor
            WHERE conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        return await conexao.QueryFirstOrDefaultAsync<PerfilVendedor>(sql, new { ContaId = contaId });
    }
}
