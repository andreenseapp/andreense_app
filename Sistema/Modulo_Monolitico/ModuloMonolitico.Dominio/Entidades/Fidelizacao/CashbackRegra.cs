using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Dominio.Entidades;

public class CashbackRegra
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }

    [Display(Name = "Título da campanha")]
    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Display(Name = "Percentual de cashback")]
    [Range(0.01, 100, ErrorMessage = "O percentual deve estar entre {1} e {2}.")]
    public decimal Percentual { get; set; }

    [Display(Name = "Validade dos créditos (dias)")]
    [Range(1, 3650, ErrorMessage = "A validade dos créditos deve estar entre {1} e {2} dias.")]
    public int? DiasValidade { get; set; }

    [Display(Name = "Data início (vigência)")]
    public DateTime? DataInicioVigencia { get; set; }

    [Display(Name = "Data fim (vigência)")]
    public DateTime? DataFimVigencia { get; set; }

    [Display(Name = "Valor máximo por unidade")]
    public decimal? ValorMaximoUnidade { get; set; }

    [Display(Name = "Valor máximo total")]
    public decimal? ValorMaximoTotal { get; set; }

    [Display(Name = "Exige referência")]
    public bool TemReferencia { get; set; }

    [Display(Name = "URL da imagem")]
    [MaxLength(500)]
    public string? UrlImagem { get; set; }

    [Display(Name = "Situação")]
    [MaxLength(20)]
    public string Situacao { get; set; } = "ativo";

    [Display(Name = "Ordem")]
    public int Ordem { get; set; }

    public DateTime DataInclusao { get; set; }
    public Guid? OperadorInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
}
