using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IGrupoUsuarioSaaSRepositorio
{
    Task CadastrarAsync(GrupoUsuarioSaaS grupo);
    Task AtualizarAsync(GrupoUsuarioSaaS grupo);
    Task<IEnumerable<GrupoUsuarioSaaS>> ListarTodosAsync();
    Task<GrupoUsuarioSaaS?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<RecursoSistemaSaaS>> ListarTodosRecursosAsync();
    Task VincularPermissoesAsync(Guid grupoSaaSId, IEnumerable<Guid> recursoSaaSIds, Guid operadorId);
    Task VincularUsuarioAoGrupoAsync(Guid usuarioId, Guid grupoSaaSId, Guid operadorId);
    Task DesvincularUsuarioDoGrupoAsync(Guid usuarioId, Guid grupoSaaSId);
    Task<IEnumerable<UsuarioGrupoSaaS>> ListarUsuariosDoGrupoAsync(Guid grupoSaaSId);
    Task<IEnumerable<GrupoUsuarioSaaS>> ListarGruposDoUsuarioAsync(Guid usuarioId);
    Task<bool> UsuarioPossuiPermissaoAsync(Guid usuarioId, string chavePermissao);
    Task<IEnumerable<string>> ListarPermissoesDoUsuarioAsync(Guid usuarioId);
}



