using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class AnuncioFoto
{
    public Guid Id { get; set; }
    public Guid AnuncioId { get; set; }
    public string UrlImagem { get; set; } = string.Empty;
    public short Ordem { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
}
