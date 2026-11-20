using System;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class PerfilAnuncianteDto
{
    public Guid ContaId { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? UrlFotoLogo { get; set; }
    public string? Whatsapp { get; set; }
    public string? UrlSite { get; set; }
    public string? Instagram { get; set; }
    public string? Creci { get; set; }
    public int TotalAnuncios { get; set; }
}
