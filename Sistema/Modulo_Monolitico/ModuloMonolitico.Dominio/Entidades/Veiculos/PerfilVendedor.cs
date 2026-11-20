using System;

namespace ModuloMonolitico.Dominio.Entidades.Veiculos;

public class PerfilVendedor
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? UrlFotoLogo { get; set; }
    public string? Whatsapp { get; set; }
    public string? UrlSite { get; set; }
    public string? Instagram { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}
