using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Cidadania.CodigoBarras;

[AllowAnonymous]
public class IndexModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IServicoGeradorCodigoBarras _servicoCodigoBarras;

    public IndexModel(IServicoGeradorCodigoBarras servicoCodigoBarras)
    {
        _servicoCodigoBarras = servicoCodigoBarras;
    }

    [BindProperty]
    [Required(ErrorMessage = "O conteúdo do código de barras é obrigatório.")]
    [Display(Name = "Conteúdo/Texto")]
    public string Conteudo { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Selecione o formato do código de barras.")]
    [Display(Name = "Formato")]
    public FormatoCodigoBarras Formato { get; set; } = FormatoCodigoBarras.Code128;

    [BindProperty]
    [Range(50, 1000, ErrorMessage = "A largura deve ser entre 50 e 1000 pixels.")]
    [Display(Name = "Largura (px)")]
    public int Largura { get; set; } = 300;

    [BindProperty]
    [Range(20, 500, ErrorMessage = "A altura deve ser entre 20 e 500 pixels.")]
    [Display(Name = "Altura (px)")]
    public int Altura { get; set; } = 120;

    [BindProperty]
    [Display(Name = "Incluir rótulo legível (texto inferior)")]
    public bool IncluirRotulo { get; set; } = true;

    public string? CodigoBarrasBase64 { get; set; }
    public string? MensagemErro { get; set; }
    public bool ExibirResultado { get; set; }

    public void OnGet()
    {
        ExibirResultado = false;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            ExibirResultado = false;
            return Page();
        }

        try
        {
            MensagemErro = null;
            CodigoBarrasBase64 = _servicoCodigoBarras.GerarCodigoBarrasComoBase64(
                Conteudo,
                Formato,
                Largura,
                Altura,
                IncluirRotulo);

            ExibirResultado = true;
            Logger.LogInformation("Código de barras '{Formato}' gerado com sucesso.", Formato);
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Validação falhou ao gerar código de barras no formato '{Formato}'.", Formato);
            MensagemErro = ex.Message;
            ExibirResultado = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro inesperado ao gerar código de barras no formato '{Formato}'.", Formato);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Cidadania.ErroGerarCodigoBarras, ex.Message);
            ExibirResultado = false;
        }

        return Page();
    }
}
