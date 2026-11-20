using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Dominio.Entidades;

public class FidelidadeCartao
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }

    [Display(Name = "Título do cartão")]
    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Display(Name = "Selos necessários")]
    [Range(1, 1000, ErrorMessage = "A quantidade de selos deve estar entre {1} e {2}.")]
    public int PontosNecessarios { get; set; }

    [Display(Name = "Recompensa")]
    [Required]
    [MaxLength(200)]
    public string Recompensa { get; set; } = string.Empty;

    [Display(Name = "Texto das regras")]
    [MaxLength(2000)]
    public string? RegraTexto { get; set; }

    [Display(Name = "Validade de cada selo (dias)")]
    [Range(1, 3650, ErrorMessage = "A validade do selo deve estar entre {1} e {2} dias.")]
    public int? DiasValidadeCarimbo { get; set; }

    [Display(Name = "Zerar cartão se inativo por (dias)")]
    [Range(1, 3650, ErrorMessage = "O prazo de inatividade deve estar entre {1} e {2} dias.")]
    public int? DiasInatividadeCartao { get; set; }

    [Display(Name = "Início da campanha")]
    public DateTime? DataInicioVigencia { get; set; }

    [Display(Name = "Término da campanha")]
    public DateTime? DataFimVigencia { get; set; }

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
