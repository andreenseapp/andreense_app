using System;

namespace ModuloMonolitico.Dominio.Veiculos.DTOs;

public class PerfilVendedorDto
{
    public Guid ContaId { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? UrlFotoLogo { get; set; }
    public string? Whatsapp { get; set; }
    public string? UrlSite { get; set; }
    public string? Instagram { get; set; }
}
