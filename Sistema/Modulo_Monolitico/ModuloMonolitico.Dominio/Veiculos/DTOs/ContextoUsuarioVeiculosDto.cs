using System;

namespace ModuloMonolitico.Dominio.Veiculos.DTOs;

public class ContextoUsuarioVeiculosDto
{
    public bool EhMaster { get; set; }
    public bool TemAnunciosAtivos { get; set; }
    public int TotalAnuncios { get; set; }
}
