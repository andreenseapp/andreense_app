using System;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class BuscaRecenteDto
{
    public Guid Id { get; set; }
    public string ChaveFiltro { get; set; } = string.Empty;
    public string FiltroJson { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public DateTime DataUltimaBusca { get; set; }
}
