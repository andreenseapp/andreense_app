using System;

namespace ModuloMonolitico.Dominio.Imobiliaria.DTOs;

public class AmenidadeDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Ordem { get; set; }
}
