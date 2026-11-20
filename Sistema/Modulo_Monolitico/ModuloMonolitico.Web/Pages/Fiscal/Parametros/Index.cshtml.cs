using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fiscal.Parametros;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Fiscal.Parametros.Visualizar";

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<ConfiguracaoEmissao> Configuracoes { get; set; } = new List<ConfiguracaoEmissao>();

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!EmpresaAtiva.HasValue) return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);

        Configuracoes = await _mediator.Send(new ListarConfiguracoesEmissaoQuery(EmpresaAtiva.Value));

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarSeguroAsync(Guid id)
    {
        if (!EmpresaAtiva.HasValue) return ModuloRotas.RedirecionarSelecaoEmpresa(Request.Path + Request.QueryString);

        try
        {
            var resultado = await _mediator.Send(new DeletarSeguroConfiguracaoEmissaoCommand(EmpresaAtiva.Value, id));
            await RegistrarAuditoriaAsync("configuracoes_emissoes", id.ToString(), ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.SerieRemovida);
            Logger.LogInformation("Usuario {UsuarioId} inativou serie fiscal {Id} na empresa {EmpresaId}", UsuarioId, id, EmpresaAtiva.Value);
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao remover serie fiscal {Id} pelo usuario {UsuarioId}", id, UsuarioId);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fiscal.ErroRemoverSerie, ex.Message);
        }

        return RedirectToPage();
    }
}
