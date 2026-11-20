using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Produto.ViewModels;

public class ProdutoVariacaoFormViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Descrição da variação")]
    [Required]
    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Display(Name = "Código de barras")]
    [MaxLength(50)]
    public string? CodigoBarras { get; set; }

    [Display(Name = "SKU")]
    [MaxLength(50)]
    public string? CodigoSku { get; set; }

    [Display(Name = "Preço de venda")]
    public decimal PrecoVenda { get; set; }

    [Display(Name = "Custo")]
    public decimal Custo { get; set; }

    [Display(Name = "Estoque atual")]
    public decimal EstoqueAtual { get; set; }

    [Display(Name = "Estoque mínimo")]
    public decimal EstoqueMinimo { get; set; }
}
