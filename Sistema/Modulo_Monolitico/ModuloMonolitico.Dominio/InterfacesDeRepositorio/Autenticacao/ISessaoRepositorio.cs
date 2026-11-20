using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface ISessaoRepositorio
{
    Task CadastrarAsync(SessaoUsuario sessao);
    Task<SessaoUsuario?> ObterPorHashRenovacaoAsync(string hashTokenRenovacao);
    Task RevogarAsync(Guid sessaoId);
    Task RevogarTodasDoUsuarioAsync(Guid usuarioId);
    Task AtualizarUltimoUsoAsync(Guid sessaoId);
}
