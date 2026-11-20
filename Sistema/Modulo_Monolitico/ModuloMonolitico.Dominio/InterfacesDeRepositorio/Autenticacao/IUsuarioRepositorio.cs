using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Guid?> ObterContaIdPrincipalAsync(Guid usuarioId);
    Task<bool> EmailJaExisteAsync(string email);
    Task CadastrarComContaEEmpresaAsync(Usuario usuario, Conta conta, Empresa empresa);
    Task AtualizarAsync(Usuario usuario);
    Task InativarAsync(Guid id);
    Task RedefinirAdministradorAsync(Usuario usuario);
    Task<IEnumerable<Usuario>> ListarPorContaIdAsync(Guid contaId);
    Task<IEnumerable<UsuarioProfissao>> ListarProfissoesAsync(Guid usuarioId);
    Task CadastrarProfissaoAsync(UsuarioProfissao profissao);
    Task DeletarFisicoProfissaoAsync(Guid profissaoId, Guid usuarioId);
    Task<(IEnumerable<MembroEquipeDto> Itens, int Total)> ListarEquipeDaEmpresaAsync(Guid empresaId, int pagina, int tamanhoPagina);
    Task<IEnumerable<MinhaEmpresaDto>> ListarEmpresasDoUsuarioAsync(Guid usuarioId);
    Task<IEnumerable<ConviteEquipeDto>> ListarConvitesPendentesDoUsuarioAsync(Guid usuarioId);
    Task<bool> ExisteVinculoAsync(Guid usuarioId, Guid empresaId);
    Task ConvidarMembroAsync(Guid usuarioId, Guid empresaId, Guid grupoId);
    Task ResponderConviteAsync(Guid usuarioId, Guid empresaId, bool aceitar);
    Task AtualizarGrupoMembroAsync(Guid usuarioId, Guid empresaId, Guid novoGrupoId);
    Task DeletarFisicoMembroAsync(Guid usuarioId, Guid empresaId);
}

public class MembroEquipeDto
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? UrlImagem { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataInclusao { get; set; }
    public bool EhTitular { get; set; }
}

public class ConviteEquipeDto
{
    public Guid EmpresaId { get; set; }
    public string EmpresaNome { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; }
}

public class MinhaEmpresaDto
{
    public Guid EmpresaId { get; set; }
    public string EmpresaNome { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; }
}


