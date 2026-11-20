using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Empresas.Convite;

public class ResponderConviteEquipeModel : AppBasePageModel
{
    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public ResponderConviteEquipeModel(IMediator mediator) => _mediator = mediator;

    public Guid EmpresaId { get; private set; }
    public string EmpresaNome { get; private set; } = string.Empty;
    public string LetraEmpresa => string.IsNullOrEmpty(EmpresaNome) ? "?" : EmpresaNome[0].ToString().ToUpper();
    public bool ConviteInvalido { get; private set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? empresaId)
    {
        if (!empresaId.HasValue)
        {
            Logger.LogWarning(
                "Acesso à página de convite sem EmpresaId pelo usuário '{Email}'.",
                UsuarioEmail);
            ConviteInvalido = true;
            return Page();
        }

        EmpresaId = empresaId.Value;

        var convitesPendentes = await _mediator.Send(new ListarConvitesUsuarioQuery(UsuarioId));
        var convite = convitesPendentes.FirstOrDefault(v => v.EmpresaId == EmpresaId);

        if (convite == null)
        {
            Logger.LogWarning(
                "Nenhum convite pendente para usuário '{Email}' na empresa {EmpresaId}.",
                UsuarioEmail, EmpresaId);
            ConviteInvalido = true;
            return Page();
        }

        var empresa = await _mediator.Send(new ObterEmpresaPorIdQuery(EmpresaId));
        if (empresa == null)
        {
            Logger.LogWarning(
                "Empresa {EmpresaId} referenciada no convite não existe — usuário '{Email}'.",
                EmpresaId, UsuarioEmail);
            ConviteInvalido = true;
            return Page();
        }

        EmpresaNome = empresa.Nome;

        Logger.LogInformation(
            "Convite da empresa '{Nome}' (ID: {EmpresaId}) exibido para o usuário '{Email}'.",
            EmpresaNome, EmpresaId, UsuarioEmail);

        return Page();
    }

    public async Task<IActionResult> OnPostResponderAsync(Guid empresaId, bool aceitar)
    {
        await _mediator.Send(new ResponderConviteEquipeCommand(UsuarioId, empresaId, aceitar));

        if (aceitar)
        {
            Logger.LogInformation(
                "Usuário '{Email}' aceitou convite para empresa {EmpresaId}.",
                UsuarioEmail, empresaId);
        }
        else
        {
            Logger.LogInformation(
                "Usuário '{Email}' recusou convite para empresa {EmpresaId}.",
                UsuarioEmail, empresaId);
        }

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.ERP.Index);
    }
}
