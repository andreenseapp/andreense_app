using Dapper;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Cidadania
{
    public class AchadosPerdidosRepositorio : RepositorioBase, IAchadosPerdidosRepositorio
    {
        public AchadosPerdidosRepositorio(string conexaoString) : base(conexaoString)
        {
        }

        public async Task<IEnumerable<AchadoPerdido>> ListarAtivosAsync()
        {
            const string sql = """
                SELECT id, usuario_id AS UsuarioId, tipo, titulo, descricao, local_visto AS LocalVisto,
                       data_evento AS DataEvento, contato_info AS ContatoInfo, esconder_contato AS EsconderContato,
                       url_imagem AS UrlImagem, status AS Situacao, data_inclusao AS DataInclusao, 
                       data_ultima_atualizacao AS DataUltimaAtualizacao
                FROM cidade_achados_perdidos
                WHERE status = 'ativo'
                ORDER BY data_inclusao DESC
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryAsync<AchadoPerdido>(sql);
        }

        public async Task<AchadoPerdido?> ObterPorIdAsync(Guid id)
        {
            const string sql = """
                SELECT id, usuario_id AS UsuarioId, tipo, titulo, descricao, local_visto AS LocalVisto,
                       data_evento AS DataEvento, contato_info AS ContatoInfo, esconder_contato AS EsconderContato,
                       url_imagem AS UrlImagem, status AS Situacao, data_inclusao AS DataInclusao, 
                       data_ultima_atualizacao AS DataUltimaAtualizacao
                FROM cidade_achados_perdidos
                WHERE id = @Id
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryFirstOrDefaultAsync<AchadoPerdido>(sql, new { Id = id });
        }

        public async Task CadastrarAsync(AchadoPerdido item)
        {
            const string sql = """
                INSERT INTO cidade_achados_perdidos (
                    usuario_id, tipo, titulo, descricao, local_visto, data_evento, 
                    contato_info, esconder_contato, url_imagem, status
                ) VALUES (
                    @UsuarioId, @Tipo, @Titulo, @Descricao, @LocalVisto, @DataEvento, 
                    @ContatoInfo, @EsconderContato, @UrlImagem, @Situacao
                )
                """;

            await using var conexao = CriarConexao();
            await conexao.ExecuteAsync(sql, item);
        }

        public async Task AtualizarAsync(AchadoPerdido item)
        {
            const string sql = """
                UPDATE cidade_achados_perdidos
                SET tipo = @Tipo,
                    titulo = @Titulo,
                    descricao = @Descricao,
                    local_visto = @LocalVisto,
                    data_evento = @DataEvento,
                    contato_info = @ContatoInfo,
                    esconder_contato = @EsconderContato,
                    url_imagem = @UrlImagem,
                    status = @Situacao,
                    data_ultima_atualizacao = now()
                WHERE id = @Id
                """;

            await using var conexao = CriarConexao();
            await conexao.ExecuteAsync(sql, item);
        }

        public async Task<IEnumerable<AchadoPerdido>> ListarPorUsuarioAsync(Guid usuarioId)
        {
            const string sql = """
                SELECT id, usuario_id AS UsuarioId, tipo, titulo, descricao, local_visto AS LocalVisto,
                       data_evento AS DataEvento, contato_info AS ContatoInfo, esconder_contato AS EsconderContato,
                       url_imagem AS UrlImagem, status AS Situacao, data_inclusao AS DataInclusao, 
                       data_ultima_atualizacao AS DataUltimaAtualizacao
                FROM cidade_achados_perdidos
                WHERE usuario_id = @UsuarioId
                ORDER BY data_inclusao DESC
                """;

            await using var conexao = CriarConexao();
            return await conexao.QueryAsync<AchadoPerdido>(sql, new { UsuarioId = usuarioId });
        }
    }
}





