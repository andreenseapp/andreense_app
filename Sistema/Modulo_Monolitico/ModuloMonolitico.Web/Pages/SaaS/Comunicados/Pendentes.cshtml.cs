using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.SaaS.Comunicados;

public class PendentesModel : AppBasePageModel
{
    private readonly IMediator _mediator;

    public PendentesModel(IMediator mediator) => _mediator = mediator;

    public ComunicadoSaaSDto? ComunicadoPendente { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    public async Task<IActionResult> OnGetAsync([FromQuery] string moduloAtual = "Todos")
    {
        if (ContaId == Guid.Empty)
            return new NoContentResult();

        ComunicadoPendente = await _mediator.Send(new ConsultarComunicadosPendentesQuery
        {
            ContaId = ContaId,
            UsuarioId = UsuarioId,
            ModuloAtual = moduloAtual
        });

        if (ComunicadoPendente == null)
            return new NoContentResult();

        Logger.LogInformation(
            "Comunicado pendente '{Titulo}' (ID: {Id}) exibido para usuário '{Email}' no módulo {Modulo}.",
            ComunicadoPendente.Titulo, ComunicadoPendente.Id, UsuarioEmail, moduloAtual);

        return Page();
    }

    public async Task<IActionResult> OnPostResponderAsync(Guid comunicadoId, Guid? opcaoSelecionadaId)
    {
        await _mediator.Send(new RegistrarLeituraComunicadoCommand
        {
            ComunicadoId = comunicadoId,
            ContaId = ContaId,
            UsuarioId = UsuarioId,
            OpcaoSelecionadaId = opcaoSelecionadaId
        });

        Logger.LogInformation(
            "Usuário '{Email}' registrou leitura/resposta no comunicado {ComunicadoId}.",
            UsuarioEmail, comunicadoId);

        return new NoContentResult();
    }
}
