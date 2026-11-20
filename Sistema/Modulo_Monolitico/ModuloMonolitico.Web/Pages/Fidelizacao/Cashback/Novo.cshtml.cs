using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Cashback;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Cashback.Cadastrar";

    private readonly IMediator _mediator;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public CashbackRegra Regra { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public void OnGet()
    {
        Regra.Percentual = 5.0m;
        Regra.DiasValidade = 30;
        Regra.Situacao = "ativo";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (UsuarioId == Guid.Empty)
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

        var empresaId = EmpresaAtiva ?? (await _mediator.Send(new ListarEmpresasContaQuery(ContaId))).FirstOrDefault()?.Id;
        if (empresaId == null || empresaId == Guid.Empty)
        {
            Logger.LogWarning(
                "Cadastro de regra cashback bloqueado: usuário '{Email}' sem empresa.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NaoEncontrada;
            return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Cashback);
        }

        await _mediator.Send(new CadastrarRegraCashbackCommand(Regra, empresaId.Value, UsuarioId));

        Logger.LogInformation(
            "Regra de cashback '{Titulo}' cadastrada pelo usuário '{Email}'.",
            Regra.Titulo, UsuarioEmail);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Cashback);
    }
}
