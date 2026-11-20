using ModuloMonolitico.Web.Core.Constantes;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Fiscal.Parametros.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fiscal.Parametros;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Fiscal.Parametros.Cadastrar";

    private readonly IMediator _mediator;

    public NovoModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty] public FormularioConfiguracaoEmissao Formulario { get; set; } = new();

    [TempData] public string? MensagemErro { get; set; }

    public IActionResult OnGet()
    {
        if (!EmpresaAtiva.HasValue) return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!EmpresaAtiva.HasValue) return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);

        if (!ModelState.IsValid)
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.VerifiqueDados;
            return Page();
        }

        try
        {
            var resultado = await _mediator.Send(new CadastrarConfiguracaoEmissaoCommand(
                EmpresaAtiva.Value,
                Formulario.Ambiente,
                Formulario.Serie,
                Formulario.Modelo,
                Formulario.ProximoNumeroNf,
                Formulario.CscId,
                Formulario.CscToken,
                Formulario.TipoEmissaoPadrao,
                Formulario.TipoImpressaoPadrao,
                Formulario.TratamentoDesconto,
                Formulario.Ativo));

            await RegistrarAuditoriaAsync("configuracoes_emissoes", EmpresaAtiva.Value.ToString(),
                string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.CriacaoSerieFiscal, Formulario.Modelo, Formulario.Serie));

            Logger.LogInformation("Nova serie fiscal Modelo {Modelo} / Serie {Serie} cadastrada na Empresa {EmpresaId} pelo usuario '{Email}'.",
                Formulario.Modelo, Formulario.Serie, EmpresaAtiva.Value, UsuarioEmail);
            NotificarSucesso();
            return ModuloRotas.Redirecionar(ModuloRotas.Fiscal.Parametros);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao cadastrar serie fiscal na Empresa {EmpresaId} pelo usuario '{Email}'.", EmpresaAtiva.Value, UsuarioEmail);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.ErroSalvarSerie, ex.Message);
            return Page();
        }
    }
}
