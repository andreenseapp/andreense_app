using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class DocumentoLegal
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Versao { get; set; } = string.Empty;
    public string? Titulo { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? UrlImagem { get; set; }
    public string Situacao { get; set; } = "ativo";
    public int Ordem { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public DateTime? DataExclusao { get; set; }
}

