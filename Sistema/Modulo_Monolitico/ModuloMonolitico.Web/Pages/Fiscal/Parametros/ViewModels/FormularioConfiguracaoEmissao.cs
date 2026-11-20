using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Fiscal.Parametros.ViewModels;

public class FormularioConfiguracaoEmissao
{
    [Display(Name = "Ambiente SEFAZ")]
    [Range(1, 2, ErrorMessage = MensagensSistema.Fiscal.AmbienteInvalido)]
    public int Ambiente { get; set; } = 2;

    [Display(Name = "Série")]
    [Range(1, 999, ErrorMessage = MensagensSistema.Fiscal.SerieInvalida)]
    public int Serie { get; set; } = 1;

    [Display(Name = "Modelo")]
    public int Modelo { get; set; } = 65;

    [Display(Name = "Série ativa principal")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Próximo nº da NF (talonário)")]
    [Range(1, int.MaxValue, ErrorMessage = MensagensSistema.Fiscal.NumeroNfInvalido)]
    public int ProximoNumeroNf { get; set; } = 1;

    [Display(Name = "CSC ID")]
    [MaxLength(50, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? CscId { get; set; }

    [Display(Name = "CSC Token")]
    [MaxLength(200, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? CscToken { get; set; }

    [Display(Name = "Tipo de emissão")]
    [Range(1, 9, ErrorMessage = MensagensSistema.Fiscal.TipoEmissaoInvalido)]
    public int TipoEmissaoPadrao { get; set; } = 1;

    [Display(Name = "Impressão do DANFe")]
    [Range(0, 5, ErrorMessage = MensagensSistema.Fiscal.TipoImpressaoInvalido)]
    public int TipoImpressaoPadrao { get; set; } = 1;

    [Display(Name = "Tratamento de desconto no XML")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    [MaxLength(30, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string TratamentoDesconto { get; set; } = "RATEIO";
}
