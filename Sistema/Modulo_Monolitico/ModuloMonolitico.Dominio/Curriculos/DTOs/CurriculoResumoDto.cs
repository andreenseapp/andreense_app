using System;

namespace ModuloMonolitico.Dominio.Curriculos.DTOs;

public class CurriculoResumoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string CargoDesejado { get; set; } = string.Empty;
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string Modalidade { get; set; } = string.Empty;
    public decimal? PretensaoSalarial { get; set; }
    public string Disponibilidade { get; set; } = string.Empty;
    public bool Pcd { get; set; }
    public string? UrlFotoPerfil { get; set; }
    public string? UltimaEmpresa { get; set; }
    public string? UltimoCargoOcupado { get; set; }
    public string? UltimaFormacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
