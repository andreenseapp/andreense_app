using System;

namespace ModuloMonolitico.Dominio.Entidades.Veiculos;

public class VeiculoFavorito
{
    public Guid UsuarioId { get; set; }
    public Guid AnuncioVeiculoId { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
