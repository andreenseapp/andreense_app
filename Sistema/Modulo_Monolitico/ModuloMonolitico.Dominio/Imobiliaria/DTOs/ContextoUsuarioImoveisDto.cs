using System;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class ContextoUsuarioImoveisDto
{
    public List<Guid> FavoritosIds { get; set; } = [];
    public List<Guid> VisualizadosIds { get; set; } = [];
    public List<BuscaRecenteResumoDto> BuscasRecentes { get; set; } = [];
}

public class BuscaRecenteResumoDto
{
    public string ChaveFiltro { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
}
