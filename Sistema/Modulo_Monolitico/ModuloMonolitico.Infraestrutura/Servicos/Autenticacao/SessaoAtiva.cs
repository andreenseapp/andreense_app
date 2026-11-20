namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public class SessaoAtiva
{
    public string IdSessao { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string GrupoCodigo { get; set; } = string.Empty;
    public string GrupoDescricao { get; set; } = string.Empty;
    public DateTime UltimoAcesso { get; set; }
    public string EnderecoIp { get; set; } = string.Empty;
    public string Navegador { get; set; } = string.Empty;
    public bool Revogada { get; set; }
}
