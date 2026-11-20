using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class AnuncioResumoDto
{
    public Guid AnuncioId { get; set; }
    public Guid ImovelId { get; set; }
    public Guid ContaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoNegocio { get; set; } = string.Empty;
    public string TipoImovel { get; set; } = string.Empty;
    public decimal ValorPrincipal { get; set; }
    public decimal ValorCondominio { get; set; }
    public decimal ValorIptu { get; set; }
    public decimal PacoteTotal { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public short Quartos { get; set; }
    public short Banheiros { get; set; }
    public short VagasGaragem { get; set; }
    public decimal? AreaTotal { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public bool EhDestaque { get; set; }
    public string? FotoCapa { get; set; }
    public DateTime DataInclusao { get; set; }
    public string NomeAnunciante { get; set; } = string.Empty;
    public string? LogoAnunciante { get; set; }
}

