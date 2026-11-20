using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Estoque.ViewModels;

public class FormularioEntrada
{
    [Display(Name = "Empresa")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    public Guid EmpresaId { get; set; }

    [Display(Name = "Produto")]
    public Guid ProdutoId { get; set; }

    [Display(Name = "Variação")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    public Guid ProdutoVariacaoId { get; set; }

    [Display(Name = "Local de estoque")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    public Guid LocalEstoqueId { get; set; }

    [Display(Name = "Lote")]
    [MaxLength(50, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? Lote { get; set; }

    [Display(Name = "Data de validade")]
    public DateTime? DataValidade { get; set; }

    [Display(Name = "Data de fabricação")]
    public DateTime? DataFabricacao { get; set; }

    [Display(Name = "Quantidade")]
    [Range(0.0001, double.MaxValue, ErrorMessage = MensagensSistema.Estoque.QuantidadeEntradaInvalida)]
    public decimal Quantidade { get; set; } = 1;

    [Display(Name = "Motivo")]
    [MaxLength(200, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? Motivo { get; set; } = "Entrada de estoque";
}
