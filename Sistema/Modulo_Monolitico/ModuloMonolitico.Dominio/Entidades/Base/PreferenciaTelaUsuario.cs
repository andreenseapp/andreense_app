namespace ModuloMonolitico.Dominio.Entidades.Base;

public class PreferenciaTelaUsuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Guid EmpresaId { get; set; }
    public string CaminhoTela { get; set; } = string.Empty;
    public string TabelaId { get; set; } = string.Empty;
    public string ColunasOcultasJson { get; set; } = "[]";
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}
