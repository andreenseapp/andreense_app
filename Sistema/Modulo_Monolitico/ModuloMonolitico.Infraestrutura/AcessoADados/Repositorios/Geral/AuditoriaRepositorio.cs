using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using Dapper;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Geral
{
    public class AuditoriaRepositorio : RepositorioBase, IAuditoriaRepositorio
    {
        public AuditoriaRepositorio(string conexaoString) : base(conexaoString) { }

        public async Task RegistrarLogAsync(Auditoria log)
        {
            await using var conexao = CriarConexao();
            await conexao.OpenAsync();
            var sql = @"INSERT INTO auditoria 
                        (tabela, registro_id, usuario_id, usuario_email, acao, detalhes, data_hora) 
                        VALUES 
                        (@Tabela, @RegistroId, @UsuarioId, @UsuarioEmail, @Acao, @Detalhes, NOW())";
            await conexao.ExecuteAsync(sql, log);
        }

        public async Task<IEnumerable<Auditoria>> ListarLogsPorRegistroAsync(string tabela, string registroId)
        {
            await using var conexao = CriarConexao();
            await conexao.OpenAsync();
            var sql = @"SELECT 
                            id AS Id, 
                            tabela AS Tabela, 
                            registro_id AS RegistroId, 
                            usuario_id AS UsuarioId, 
                            usuario_email AS UsuarioEmail, 
                            acao AS Acao, 
                            detalhes AS Detalhes, 
                            data_hora AS DataHora
                        FROM auditoria 
                        WHERE tabela = @Tabela AND registro_id = @RegistroId 
                        ORDER BY data_hora DESC";
            return await conexao.QueryAsync<Auditoria>(sql, new { Tabela = tabela, RegistroId = registroId });
        }
    }
}



