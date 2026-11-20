using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.InfraAdmin;

public class DocumentoRepositorio : RepositorioBase, IDocumentoRepositorio
{
    public DocumentoRepositorio(string conexaoString) : base(conexaoString)
    {
    }

    public async Task<IEnumerable<DocumentoLegal>> ListarDocumentosAsync()
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"SELECT 
                            id AS Id, 
                            tipo AS Tipo, 
                            versao AS Versao, 
                            titulo AS Titulo, 
                            texto AS Texto, 
                            url_imagem AS UrlImagem, 
                            status AS Situacao, 
                            ordem AS Ordem, 
                            data_inclusao AS DataInclusao, 
                            data_ultima_atualizacao AS DataUltimaAtualizacao, 
                            data_exclusao AS DataExclusao
                        FROM documentos_legais 
                        WHERE data_exclusao IS NULL
                        ORDER BY data_inclusao DESC";
        
        return await conexao.QueryAsync<DocumentoLegal>(sql);
    }

    public async Task<DocumentoLegal?> ObterPorIdAsync(Guid id)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"SELECT 
                            id AS Id, 
                            tipo AS Tipo, 
                            versao AS Versao, 
                            titulo AS Titulo, 
                            texto AS Texto, 
                            url_imagem AS UrlImagem, 
                            status AS Situacao, 
                            ordem AS Ordem, 
                            data_inclusao AS DataInclusao, 
                            data_ultima_atualizacao AS DataUltimaAtualizacao, 
                            data_exclusao AS DataExclusao
                        FROM documentos_legais 
                        WHERE id = @Id AND data_exclusao IS NULL";
                        
        return await conexao.QueryFirstOrDefaultAsync<DocumentoLegal>(sql, new { Id = id });
    }

    public async Task<DocumentoLegal?> ObterUltimaVersaoAtivaAsync(string tipo)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"SELECT 
                            id AS Id, 
                            tipo AS Tipo, 
                            versao AS Versao, 
                            titulo AS Titulo, 
                            texto AS Texto, 
                            url_imagem AS UrlImagem, 
                            status AS Situacao, 
                            ordem AS Ordem, 
                            data_inclusao AS DataInclusao, 
                            data_ultima_atualizacao AS DataUltimaAtualizacao, 
                            data_exclusao AS DataExclusao
                        FROM documentos_legais 
                        WHERE tipo = @Tipo AND status = 'ativo' AND data_exclusao IS NULL
                        ORDER BY data_inclusao DESC
                        LIMIT 1";
                        
        return await conexao.QueryFirstOrDefaultAsync<DocumentoLegal>(sql, new { Tipo = tipo });
    }

    public async Task<Guid> CadastrarAsync(DocumentoLegal documento)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"INSERT INTO documentos_legais 
                        (tipo, versao, titulo, texto, url_imagem, status, ordem, data_inclusao) 
                        VALUES 
                        (@Tipo, @Versao, @Titulo, @Texto, @UrlImagem, @Situacao, @Ordem, NOW()) 
                        RETURNING id";
                        
        return await conexao.ExecuteScalarAsync<Guid>(sql, documento);
    }

    public async Task<bool> AtualizarAsync(DocumentoLegal documento)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"UPDATE documentos_legais 
                        SET tipo = @Tipo, 
                            versao = @Versao, 
                            titulo = @Titulo, 
                            texto = @Texto, 
                            url_imagem = @UrlImagem, 
                            status = @Situacao, 
                            ordem = @Ordem 
                        WHERE id = @Id AND data_exclusao IS NULL";
                        
        return await conexao.ExecuteAsync(sql, documento) > 0;
    }

    public async Task<bool> RegistrarAceiteAsync(DocumentoAceite aceite)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"INSERT INTO documentos_aceites 
                        (usuario_id, documento_legal_id, data_aceite, status, ordem, data_inclusao) 
                        VALUES 
                        (@UsuarioId, @DocumentoLegalId, NOW(), 'ativo', @Ordem, NOW())";
                        
        return await conexao.ExecuteAsync(sql, aceite) > 0;
    }

    public async Task<bool> UsuarioJaAceitouVersaoAtualAsync(Guid usuarioId, string tipo)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        var sql = @"SELECT 1 
                        FROM documentos_aceites a
                        INNER JOIN documentos_legais l ON a.documento_legal_id = l.id
                        WHERE a.usuario_id = @UsuarioId 
                          AND l.tipo = @Tipo
                          AND l.status = 'ativo'
                          AND l.data_exclusao IS NULL
                        LIMIT 1";
                        
        var resultado = await conexao.ExecuteScalarAsync<int?>(sql, new { UsuarioId = usuarioId, Tipo = tipo });
        return resultado.HasValue;
    }
}
