using System;

namespace ModuloMonolitico.Dominio.Veiculos.DTOs;

public class BuscaRecenteVeiculoDto
{
    public Guid Id { get; set; }
    public string ChaveFiltro { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public string FiltroJson { get; set; } = string.Empty;
    public DateTime DataUltimaBusca { get; set; }
}
