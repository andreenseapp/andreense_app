using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class BuscaRecente
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string ChaveFiltro { get; set; } = string.Empty;
    public string FiltroJson { get; set; } = string.Empty;
    public string Rotulo { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaBusca { get; set; } = DateTime.UtcNow;
}
