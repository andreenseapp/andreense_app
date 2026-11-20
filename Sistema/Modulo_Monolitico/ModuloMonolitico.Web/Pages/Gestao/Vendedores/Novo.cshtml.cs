using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.Vendedores;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.Vendedores.Cadastrar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public NovoModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public CadastrarVendedorCommand Comando { get; set; } = new();

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public SelectList ListaTiposVendedor { get; set; } = new(Enumerable.Empty<SelectListItem>());

    public SelectList ListaUsuarios { get; set; } = new(Enumerable.Empty<SelectListItem>());

    public async Task OnGetAsync()
    {
        await CarregarListasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return Page();
        }

        Comando.ContaId = ContaId;

        var resultado = await _mediator.Send(Comando);

        if (resultado.Sucesso)
        {
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            Logger.LogInformation(
                "Vendedor '{Nome}' cadastrado pelo usuário '{Email}'.",
                Comando.NomeFantasia, UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.Gestao.Vendedores);
        }

        Logger.LogWarning(
            "Falha ao cadastrar vendedor '{Nome}' pelo usuário '{Email}'. Motivo: {Mensagem}.",
            Comando.NomeFantasia, UsuarioEmail, resultado.Mensagem);
        MensagemErro = resultado.Mensagem;
        await CarregarListasAsync();
        return Page();
    }

    private async Task CarregarListasAsync()
    {
        var tipos = await _mediator.Send(new ListarTiposVendedorQuery(ContaId));
        ListaTiposVendedor = new SelectList(tipos, "Id", "Nome");

        var usuarios = await _mediator.Send(new ListarUsuariosPorContaQuery(ContaId));
        ListaUsuarios = new SelectList(usuarios, "Id", "Nome");
    }
}
