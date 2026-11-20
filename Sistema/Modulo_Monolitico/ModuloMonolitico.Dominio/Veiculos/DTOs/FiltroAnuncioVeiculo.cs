using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Veiculos.DTOs;

public class FiltroAnuncioVeiculo
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? TipoVeiculo { get; set; }
    public string? Combustivel { get; set; }
    public string? Cambio { get; set; }
    public int? AnoMinimo { get; set; }
    public int? AnoMaximo { get; set; }
    public int? KmMaximo { get; set; }
    public decimal? PrecoMinimo { get; set; }
    public decimal? PrecoMaximo { get; set; }
    public bool? AceitaTroca { get; set; }
    public string? CodigoPublico { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
    public int Offset => (Pagina - 1) * TamanhoPagina;
}
