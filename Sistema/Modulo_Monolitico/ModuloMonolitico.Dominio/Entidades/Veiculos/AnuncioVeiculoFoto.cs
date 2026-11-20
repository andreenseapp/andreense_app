using System;

namespace ModuloMonolitico.Dominio.Entidades.Veiculos;

public class AnuncioVeiculoFoto
{
    public Guid Id { get; set; }
    public Guid AnuncioVeiculoId { get; set; }
    public string UrlImagem { get; set; } = string.Empty;
    public short Ordem { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}
