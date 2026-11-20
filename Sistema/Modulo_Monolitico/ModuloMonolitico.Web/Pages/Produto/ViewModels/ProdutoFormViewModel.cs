using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Produto.ViewModels;

public class ProdutoFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Código")]
    [MaxLength(50)]
    public string? Codigo { get; set; }

    [Display(Name = "Descrição")]
    [Required]
    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Display(Name = "Descrição complementar")]
    [MaxLength(500)]
    public string? DescricaoComplementar { get; set; }

    [Display(Name = "Tipo de item")]
    [Required]
    [MaxLength(20)]
    public string TipoProduto { get; set; } = "insumo";

    [Display(Name = "Grupo / categoria")]
    [MaxLength(100)]
    public string? Grupo { get; set; }

    [Display(Name = "Marca")]
    [MaxLength(100)]
    public string? Marca { get; set; }

    [Display(Name = "Unidade de medida")]
    [Required]
    [MaxLength(10)]
    public string UnidadeMedida { get; set; } = "UN";

    [Display(Name = "NCM")]
    [MaxLength(20)]
    public string? Ncm { get; set; }

    [Display(Name = "CEST")]
    [MaxLength(20)]
    public string? Cest { get; set; }

    [Display(Name = "Observações")]
    [MaxLength(1000)]
    public string? Observacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    public List<ProdutoVariacaoFormViewModel> Variacoes { get; set; } = [];
}
