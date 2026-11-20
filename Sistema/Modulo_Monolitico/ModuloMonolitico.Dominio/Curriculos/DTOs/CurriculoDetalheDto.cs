using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Curriculos.DTOs;

public class CurriculoDetalheDto
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
    public bool AceitaViagem { get; set; }
    public bool AceitaMudanca { get; set; }
    public string? ResumoProfissional { get; set; }
    public string? CnhCategorias { get; set; }
    public bool Pcd { get; set; }
    public string? TipoDeficiencia { get; set; }
    public string? UrlFotoPerfil { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public IList<ExperienciaDto> Experiencias { get; set; } = new List<ExperienciaDto>();
    public IList<FormacaoDto> Formacoes { get; set; } = new List<FormacaoDto>();
    public IList<CursoDto> Cursos { get; set; } = new List<CursoDto>();
    public IList<IdiomaDto> Idiomas { get; set; } = new List<IdiomaDto>();
    public IList<string> Habilidades { get; set; } = new List<string>();
}

public class ExperienciaDto
{
    public Guid Id { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public bool EmAndamento { get; set; }
    public string? Descricao { get; set; }
    public string? Cidade { get; set; }
}

public class FormacaoDto
{
    public Guid Id { get; set; }
    public string Instituicao { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;
    public string Grau { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public bool EmAndamento { get; set; }
}

public class CursoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Instituicao { get; set; }
    public int? CargaHoraria { get; set; }
    public short? AnoConclusao { get; set; }
}

public class IdiomaDto
{
    public Guid Id { get; set; }
    public string Idioma { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
}
