using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Gestao.Vendedores;

public class DetalhesModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.Vendedores.Visualizar";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public DetalhesModel(IMediator mediator) => _mediator = mediator;

    public Vendedor? Vendedor { get; private set; }

    public string? NomeUsuarioVinculado { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Vendedor = await _mediator.Send(new ObterVendedorQuery(id, ContaId));

        if (Vendedor == null)
        {
            Logger.LogWarning(
                "Vendedor {VendedorId} não encontrado para visualização pelo usuário '{Email}'.",
                id, UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.Gestao.Vendedores);
        }

        if (Vendedor.UsuarioId.HasValue)
        {
            var usuarios = await _mediator.Send(new ListarUsuariosPorContaQuery(ContaId));
            NomeUsuarioVinculado = usuarios.FirstOrDefault(u => u.Id == Vendedor.UsuarioId.Value)?.Nome;
        }

        Logger.LogInformation(
            "Detalhes do vendedor '{Nome}' (ID: {Id}) visualizados pelo usuário '{Email}'.",
            Vendedor.NomeFantasia, Vendedor.Id, UsuarioEmail);

        return Page();
    }
}
