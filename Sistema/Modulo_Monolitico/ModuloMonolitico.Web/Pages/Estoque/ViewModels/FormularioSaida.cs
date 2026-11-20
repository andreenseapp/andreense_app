using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Estoque.ViewModels;

public class FormularioSaida
{
    [Display(Name = "Saldo")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    public Guid SaldoId { get; set; }

    [Display(Name = "Quantidade")]
    [Range(0.0001, double.MaxValue, ErrorMessage = MensagensSistema.Estoque.QuantidadeSaidaInvalida)]
    public decimal Quantidade { get; set; } = 1;

    [Display(Name = "Motivo")]
    [MaxLength(200, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? Motivo { get; set; }
}
