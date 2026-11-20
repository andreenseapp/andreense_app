using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class SolicitacaoVinculo
{
    public Guid Id { get; set; }
    public Guid ContaSolicitanteId { get; set; }
    public string EmailAlvo { get; set; } = string.Empty;
    public Guid? ContaAlvoId { get; set; }
    public string TipoVinculo { get; set; } = "confianca";

    public string? Mensagem { get; set; }
    public string Situacao { get; set; } = "pendente";

    public DateTime DataInclusao { get; set; }
    public DateTime? DataResposta { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public string? NomeSolicitante { get; set; }
    public string? EmailSolicitante { get; set; }
}

