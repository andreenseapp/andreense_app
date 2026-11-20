using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Curriculos.DTOs;

public class FiltroCurriculo
{
    public string? AreaAtuacao { get; set; }
    public string? CargoDesejado { get; set; }
    public string? Nivel { get; set; }
    public string? Modalidade { get; set; }
    public string? Disponibilidade { get; set; }
    public bool? Pcd { get; set; }
    public string? Idioma { get; set; }
    public decimal? PretensaoMinima { get; set; }
    public decimal? PretensaoMaxima { get; set; }
    public bool? AceitaViagem { get; set; }
    public bool? AceitaMudanca { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
    public int Offset => (Pagina - 1) * TamanhoPagina;
}
