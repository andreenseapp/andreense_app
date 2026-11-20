using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class Anuncio
{
    public Guid Id { get; set; }
    public Guid ImovelId { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string TipoNegocio { get; set; } = string.Empty;
    public decimal ValorPrincipal { get; set; }
    public decimal ValorCondominio { get; set; }
    public decimal ValorIptu { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? ContatoWhatsapp { get; set; }
    public bool MostrarLogradouro { get; set; } = true;
    public bool MostrarNumero { get; set; } = true;
    public string Situacao { get; set; } = "ativo";
    public bool EhDestaque { get; set; }
    public string? CodigoPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
