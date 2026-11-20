using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Produto.ViewModels;

public class ProdutoFiltro
{
    [Display(Name = "Busca")]
    public string? Busca { get; set; }

    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }
}
