namespace ModuloMonolitico.Dominio.Entidades;

public class PreferenciaTelaUsuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string TelaIdentificador { get; set; } = string.Empty;
    public string ColunasVisiveisJson { get; set; } = string.Empty;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
