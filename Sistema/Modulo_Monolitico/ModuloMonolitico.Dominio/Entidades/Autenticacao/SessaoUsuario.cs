namespace ModuloMonolitico.Dominio.Entidades;

public class SessaoUsuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Guid ContaId { get; set; }
    public string HashTokenRenovacao { get; set; } = string.Empty;
    public string HashImpressaoDigital { get; set; } = string.Empty;

    public string? AgenteUsuario { get; set; }
    public string? EnderecoIp { get; set; }
    public string? Plataforma { get; set; }

    public bool Ativa { get; set; } = true;
    public DateTimeOffset DataInclusao { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset DataExpiracao { get; set; }
    public DateTimeOffset? DataUltimoUso { get; set; }
    public DateTimeOffset? DataExclusao { get; set; }
}
