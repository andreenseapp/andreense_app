using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao.Fidelidade;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Fidelidade.Cadastrar";

    private readonly IMediator _mediator;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public FidelidadeCartao Cartao { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public void OnGet()
    {
        Cartao.PontosNecessarios = 10;
        Cartao.Situacao = "ativo";
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
                "Cadastro de cartão fidelidade bloqueado: usuário '{Email}' sem empresa.",
                UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NaoEncontrada;
            return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Fidelidade);
        }

        await _mediator.Send(new CadastrarCartaoFidelidadeCommand(Cartao, empresaId.Value, UsuarioId));

        Logger.LogInformation(
            "Cartão fidelidade '{Titulo}' cadastrado pelo usuário '{Email}'.",
            Cartao.Titulo, UsuarioEmail);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Fidelizacao.Fidelidade);
    }
}
