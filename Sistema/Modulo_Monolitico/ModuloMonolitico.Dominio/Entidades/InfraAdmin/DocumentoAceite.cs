using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class DocumentoAceite
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid DocumentoLegalId { get; set; }
    public DateTime DataAceite { get; set; }
    public string? UrlImagem { get; set; }
    public string Situacao { get; set; } = "ativo";
    public int Ordem { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public DateTime? DataExclusao { get; set; }
}

