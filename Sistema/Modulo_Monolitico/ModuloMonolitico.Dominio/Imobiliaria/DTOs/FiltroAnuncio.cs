using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class FiltroAnuncio
{
    public string? TipoNegocio { get; set; }
    public string? TipoImovel { get; set; }
    public string? Bairro { get; set; }
    public short? QuartosMinimo { get; set; }
    public short? BanheirosMinimo { get; set; }
    public short? VagasMinimo { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public decimal? ValorCondominioMinimo { get; set; }
    public decimal? ValorCondominioMaximo { get; set; }
    public decimal? AreaMinima { get; set; }
    public decimal? AreaMaxima { get; set; }
    public string? SituacaoObra { get; set; }
    public string? CodigoPublico { get; set; }
    public List<string> CodigosAmenidade { get; set; } = new();
    public bool BuscarPorPacote { get; set; } = false;
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
    public int Offset => (Pagina - 1) * TamanhoPagina;
}
