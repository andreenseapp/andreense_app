using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Curriculos.DTOs;

public class CurriculoCompletoDto
{
    public Guid Id { get; set; }
    public string CargoDesejado { get; set; } = string.Empty;
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Nivel { get; set; } = "pleno";
    public string Modalidade { get; set; } = "clt";
    public decimal? PretensaoSalarial { get; set; }
    public string Disponibilidade { get; set; } = "imediata";
    public bool AceitaViagem { get; set; }
    public bool AceitaMudanca { get; set; }
    public string? ResumoProfissional { get; set; }
    public string? CnhCategorias { get; set; }
    public bool Pcd { get; set; }
    public string? TipoDeficiencia { get; set; }
    public bool Visivel { get; set; }
    public string Status { get; set; } = "ativo";
    public DateTime DataAtualizacao { get; set; }
    public IList<ExperienciaDto> Experiencias { get; set; } = new List<ExperienciaDto>();
    public IList<FormacaoDto> Formacoes { get; set; } = new List<FormacaoDto>();
    public IList<CursoDto> Cursos { get; set; } = new List<CursoDto>();
    public IList<IdiomaDto> Idiomas { get; set; } = new List<IdiomaDto>();
    public IList<string> Habilidades { get; set; } = new List<string>();
    public int PercentualPreenchimento { get; set; }
}
