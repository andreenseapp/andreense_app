using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class CertificadoDigital
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string? NomeArquivo { get; set; }
    public string ConteudoPfxBase64 { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    public DateTime? DataVencimento { get; set; }
    public string Situacao { get; set; } = "ativo";
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimaAtualizacao { get; set; }
    public DateTime? DataExclusao { get; set; }
    public bool EstaVencido => DataVencimento.HasValue && DataVencimento.Value < DateTime.UtcNow;
    public bool VenceEmBreve => DataVencimento.HasValue && DataVencimento.Value > DateTime.UtcNow
                                 && DataVencimento.Value <= DateTime.UtcNow.AddDays(30);
}

