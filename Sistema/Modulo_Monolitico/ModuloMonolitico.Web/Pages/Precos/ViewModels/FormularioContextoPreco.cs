using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Pages.Precos.ViewModels;

public class FormularioContextoPreco
{
    public Guid? Id { get; set; }

    [Display(Name = "Empresa")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    public Guid EmpresaId { get; set; }

    [Display(Name = "Nome do contexto")]
    [Required(ErrorMessage = MensagensSistema.Validacao.CampoObrigatorio)]
    [MaxLength(100, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [MaxLength(500, ErrorMessage = MensagensSistema.Validacao.TamanhoMaximoExcedido)]
    public string? Descricao { get; set; }

    [Display(Name = "Tabela padrão")]
    public bool EhPadrao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Ordem de exibição")]
    [Range(0, 9999, ErrorMessage = MensagensSistema.Validacao.OrdemInvalida)]
    public int Ordem { get; set; }
}
