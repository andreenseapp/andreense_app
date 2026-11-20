using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.Vendedores;

public class EditarModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.Vendedores.Editar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public EditarModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public CadastrarVendedorCommand Comando { get; set; } = new();

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public SelectList ListaTiposVendedor { get; set; } = new(Enumerable.Empty<SelectListItem>());

    public SelectList ListaUsuarios { get; set; } = new(Enumerable.Empty<SelectListItem>());

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        await CarregarListasAsync();

        var vendedor = await _mediator.Send(new ObterVendedorQuery(id, ContaId));

        if (vendedor == null)
        {
            Logger.LogWarning(
                "Vendedor {VendedorId} não encontrado para edição pelo usuário '{Email}'.",
                id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Gestao.Vendedores.NaoEncontrado;
            return ModuloRotas.Redirecionar(ModuloRotas.Gestao.Vendedores);
        }

        Comando = new CadastrarVendedorCommand
        {
            Id = vendedor.Id,
            ContaId = vendedor.ContaId,
            UsuarioId = vendedor.UsuarioId,
            TipoVendedorId = vendedor.TipoVendedorId,
            NomeFantasia = vendedor.NomeFantasia,
            Observacao = vendedor.Observacao,
            Ativo = vendedor.Ativo
        };

        return Page();
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
                "Vendedor '{Nome}' (ID: {Id}) atualizado pelo usuário '{Email}'.",
                Comando.NomeFantasia, Comando.Id, UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.Gestao.Vendedores);
        }

        Logger.LogWarning(
            "Falha ao atualizar vendedor '{Nome}' (ID: {Id}) pelo usuário '{Email}'. Motivo: {Mensagem}.",
            Comando.NomeFantasia, Comando.Id, UsuarioEmail, resultado.Mensagem);
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
