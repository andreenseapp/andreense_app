using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public class TentativaLoginListagemDto
{
    public long Id { get; set; }
    public Guid? UsuarioId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? EnderecoIp { get; set; }
    public string? AgenteUsuario { get; set; }
    public string Provedor { get; set; } = string.Empty;
    public bool Sucesso { get; set; }
    public string? MotivoFalha { get; set; }
    public DateTime DataInclusao { get; set; }
}

public record RegistroTentativaLogin(
    string Email,
    string? EnderecoIp,
    string? AgenteUsuario,
    bool Sucesso,
    string Provedor = "local",
    string? MotivoFalha = null
);

public interface ITentativaLoginRepositorio
{
    Task RegistrarAsync(RegistroTentativaLogin registro);
    Task<int> ContarFalhasRecentesAsync(string email, string ip, TimeSpan janela);
    Task<int> ContarFalhasRecentesPorIpAsync(string ip, TimeSpan janela);
    Task<int> ContarSolicitacoesRecentesAsync(string email, string ip, string provedor, TimeSpan janela);
    Task<int> ContarSolicitacoesRecentesPorIpAsync(string ip, string provedor, TimeSpan janela);
    Task<int> ContarFalhasRecentesPorProvedorAsync(string email, string ip, string provedor, TimeSpan janela);
    Task<int> ContarFalhasRecentesPorIpEProvedorAsync(string ip, string provedor, TimeSpan janela);
    Task<ResultadoPaginado<TentativaLoginListagemDto>> ListarPaginadoAsync(int pagina, int tamanhoPagina, string? filtroEmail = null);
    Task<IEnumerable<TentativaLoginListagemDto>> ListarUltimosConectadosAsync(int quantidade = 50);
}
