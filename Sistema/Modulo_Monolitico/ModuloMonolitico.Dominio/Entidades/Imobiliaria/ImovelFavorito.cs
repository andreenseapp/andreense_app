using System;

namespace ModuloMonolitico.Dominio.Entidades.Imobiliaria;

public class ImovelFavorito
{
    public Guid UsuarioId { get; set; }
    public Guid ImovelId { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Imovel Imovel { get; set; } = null!;
}
