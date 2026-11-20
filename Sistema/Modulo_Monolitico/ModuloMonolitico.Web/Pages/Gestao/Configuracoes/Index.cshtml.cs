using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.Configuracoes;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Base.Configuracoes.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public Guid? EmpresaId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (EmpresaAtiva.HasValue)
        {
            EmpresaId = EmpresaAtiva;
        }
        else
        {
            var empresas = await _mediator.Send(new ListarEmpresasContaQuery(ContaId));
            EmpresaId = empresas.FirstOrDefault()?.Id;
        }

        Logger.LogInformation(
            "Hub de configurações acessado pelo usuário '{Email}'. Empresa: {EmpresaId}.",
            UsuarioEmail, EmpresaId?.ToString() ?? "(nenhuma)");

        return Page();
    }
}
