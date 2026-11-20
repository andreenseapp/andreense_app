using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Fiscal.Parametros.ViewModels;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fiscal.Parametros;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Fiscal.Parametros.Editar";

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty] public Guid Id { get; set; }
    [BindProperty] public FormularioConfiguracaoEmissao Formulario { get; set; } = new();

    [TempData] public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!EmpresaAtiva.HasValue) return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);

        var config = await _mediator.Send(new ObterConfiguracaoEmissaoQuery(EmpresaAtiva.Value, id));
        if (config == null) return ModuloRotas.Redirecionar(ModuloRotas.Fiscal.Parametros);

        Id = config.Id;
        Formulario = new FormularioConfiguracaoEmissao
        {
            Ambiente             = config.Ambiente,
            Serie                = config.Serie,
            Modelo               = config.Modelo,
            ProximoNumeroNf      = config.ProximoNumeroNf,
            CscId                = config.CscId,
            CscToken             = config.CscToken,
            TipoEmissaoPadrao    = config.TipoEmissaoPadrao,
            TipoImpressaoPadrao  = config.TipoImpressaoPadrao,
            TratamentoDesconto   = config.TratamentoDesconto,
            Ativo                = config.Situacao == "ativo"
        };

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
            var resultado = await _mediator.Send(new AtualizarConfiguracaoEmissaoCommand(
                EmpresaAtiva.Value,
                Id,
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

            if (!resultado.Sucesso)
                return ModuloRotas.Redirecionar(ModuloRotas.Fiscal.Parametros);

            await RegistrarAuditoriaAsync("configuracoes_emissoes", EmpresaAtiva.Value.ToString(),
                string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.EdicaoSerieFiscal, Id));

            Logger.LogInformation("Serie fiscal ID {Id} editada com sucesso na Empresa {EmpresaId} pelo usuario '{Email}'.", Id, EmpresaAtiva.Value, UsuarioEmail);
            NotificarSucesso();
            return ModuloRotas.Redirecionar(ModuloRotas.Fiscal.Parametros);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao atualizar serie fiscal ID {Id} na Empresa {EmpresaId} pelo usuario '{Email}'.", Id, EmpresaAtiva.Value, UsuarioEmail);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.ErroSalvarSerie, ex.Message);
            return Page();
        }
    }
}
