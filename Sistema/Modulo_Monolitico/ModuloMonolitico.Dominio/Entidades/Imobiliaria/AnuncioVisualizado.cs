using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class AnuncioVisualizado
{
    public Guid UsuarioId { get; set; }
    public Guid AnuncioId { get; set; }
    public DateTime DataVisualizacao { get; set; } = DateTime.UtcNow;
}
