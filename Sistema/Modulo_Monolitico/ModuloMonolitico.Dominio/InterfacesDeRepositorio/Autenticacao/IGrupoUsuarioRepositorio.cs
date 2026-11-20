using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IGrupoUsuarioRepositorio
{
    Task CadastrarAsync(GrupoUsuario grupo);
    Task AtualizarAsync(GrupoUsuario grupo);
    Task<ResultadoPaginado<GrupoUsuario>> ListarPorEmpresaIdAsync(Guid empresaId, string? ordenarPor = "nome", bool descendente = false, int limite = 50, int pagina = 1);
    Task<GrupoUsuario?> ObterPorIdEEmpresaAsync(Guid id, Guid empresaId);
    Task<IEnumerable<RecursoSistema>> ListarTodosRecursosAsync();
    Task VincularPermissoesAsync(Guid grupoId, IEnumerable<Guid> recursoIds, IEnumerable<Guid> atalhoIds);
    Task<bool> UsuarioPossuiPermissaoAsync(Guid usuarioId, Guid empresaId, string chavePermissao);
    Task<IEnumerable<MenuItemAtalhoDto>> ListarAtalhosDashboardAsync(Guid usuarioId, Guid empresaId);
    Task<int> ContarUsuariosVinculadosAsync(Guid grupoId);
    Task DeletarFisicoAsync(Guid id, Guid empresaId);
}

public class MenuItemAtalhoDto
{
    public string Chave { get; set; } = string.Empty;
}
