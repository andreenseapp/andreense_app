using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Veiculos.DTOs;

public class AnuncioVeiculoDetalheDto
{
    public Guid AnuncioVeiculoId { get; set; }
    public Guid VeiculoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid ContaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string? Versao { get; set; }
    public int AnoFabricacao { get; set; }
    public int AnoModelo { get; set; }
    public int Quilometragem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public string Cambio { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string? Placa { get; set; }
    public string TipoVeiculo { get; set; } = string.Empty;
    public short? NumeroPortas { get; set; }
    public decimal Preco { get; set; }
    public bool AceitaTroca { get; set; }
    public string? ContatoWhatsapp { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public bool EhDestaque { get; set; }
    public string? CodigoPublico { get; set; }
    public DateTime DataInclusao { get; set; }
    public List<string> Fotos { get; set; } = new();
    public string NomeVendedor { get; set; } = string.Empty;
    public string? LogoVendedor { get; set; }
    public string? UrlSiteVendedor { get; set; }
    public string? InstagramVendedor { get; set; }
}
