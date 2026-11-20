using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class ConfiguracaoEmissao
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid? CertificadoId { get; set; }
    public int Ambiente { get; set; } = 2;
    public int Serie { get; set; } = 1;
    public int Modelo { get; set; } = 65;
    public int ProximoNumeroNf { get; set; } = 1;
    public string? CscId { get; set; }
    public string? CscToken { get; set; }
    public int TipoEmissaoPadrao { get; set; } = 1;
    public int TipoImpressaoPadrao { get; set; } = 1;

    public DateTime? DataUltimaEmissao { get; set; }
    public string TratamentoDesconto { get; set; } = "RATEIO";

    public string Situacao { get; set; } = "ativo";
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimaAtualizacao { get; set; }
    public CertificadoDigital? Certificado { get; set; }
    public string AmbienteDescricao => Ambiente == 1 ? "Produção" : "Homologação";
    public string ModeloDescricao => Modelo == 55 ? "NF-e (55)" : "NFC-e (65)";
}

