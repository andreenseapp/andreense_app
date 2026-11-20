using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.MeusAgendamentos;

public class DetalhesClienteModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<DetalhesClienteModel> _logger;

    public DetalhesClienteModel(IMediator mediator, ILogger<DetalhesClienteModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public Agendamento? Agendamento { get; private set; }
    public bool PodeCancelar { get; private set; }

    [BindProperty]
    [Display(Name = "Motivo do cancelamento")]
    [MaxLength(500)]
    public string MotivoCancelamento { get; set; } = string.Empty;

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation(
            "Cliente '{Email}' visualizando agendamento {Id}.",
            UsuarioEmail,
            id);

        Agendamento = await _mediator.Send(new ObterAgendamentoPorClienteQuery(id, UsuarioId));

        if (Agendamento == null)
        {
            _logger.LogWarning(
                "Agendamento {Id} não encontrado para o cliente '{Email}'.",
                id,
                UsuarioEmail);
            return NotFound();
        }

        PodeCancelar = Agendamento.Situacao != "cancelado"
            && Agendamento.Situacao != "concluido"
            && (Agendamento.DataHoraInicio - DateTime.UtcNow).TotalHours > 2;

        return Page();
    }

    public async Task<IActionResult> OnPostCancelarAsync(Guid id)
    {
        _logger.LogInformation(
            "Cliente '{Email}' solicitando cancelamento do agendamento {Id}.",
            UsuarioEmail,
            id);

        var ag = await _mediator.Send(new ObterAgendamentoPorClienteQuery(id, UsuarioId));

        if (ag == null)
        {
            _logger.LogWarning(
                "Agendamento {Id} não encontrado para cancelamento pelo cliente '{Email}'.",
                id,
                UsuarioEmail);
            return NotFound();
        }

        var retorno = await _mediator.Send(
            new CancelarAgendamentoCommand(id, ag.EmpresaId, MotivoCancelamento, "cliente"));

        if (retorno.Sucesso)
        {
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Agendamento {Id} cancelado pelo cliente '{Email}'.",
                id,
                UsuarioEmail);
            return RedirectToPage("./Index");
        }

        MensagemErro = retorno.Mensagem;
        _logger.LogWarning(
            "Falha ao cancelar agendamento {Id} pelo cliente '{Email}': {Mensagem}.",
            id,
            UsuarioEmail,
            retorno.Mensagem);
        return RedirectToPage(new { id });
    }
}
