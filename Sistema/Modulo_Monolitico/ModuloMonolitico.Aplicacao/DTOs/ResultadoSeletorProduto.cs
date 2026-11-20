using System;

namespace ModuloMonolitico.Aplicacao.DTOs;
public class ResultadoSeletorProduto
{
    public Guid Id { get; set; }
    public string TipoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public string NomeVariacao { get; set; } = string.Empty;
    public string? CodigoSku { get; set; }
    public string? CodigoProduto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int DuracaoMinutos { get; set; }
    public string? CorHexadecimal { get; set; }
    public string CodigoExibicao => !string.IsNullOrWhiteSpace(CodigoSku) ? CodigoSku : (CodigoProduto ?? string.Empty);

    public string NomeExibicao
    {
        get
        {
            if (string.IsNullOrWhiteSpace(NomeVariacao) || NomeProduto.Equals(NomeVariacao, StringComparison.OrdinalIgnoreCase))
                return NomeProduto;
            return $"{NomeProduto} - {NomeVariacao}";
        }
    }
}
