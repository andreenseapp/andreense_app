using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.Painel;

public class DetalhesModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Agendamentos.Detalhes";

    private readonly IMediator _mediator;
    private readonly ILogger<DetalhesModel> _logger;

    public DetalhesModel(IMediator mediator, ILogger<DetalhesModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Agendamento? Agendamento { get; private set; }

    [BindProperty]
    [Display(Name = "Motivo do cancelamento")]
    [MaxLength(500)]
    public string MotivoCancelamento { get; set; } = string.Empty;

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation(
            "Visualizando detalhes do agendamento {Id} por '{Email}'.",
            id,
            UsuarioEmail);

        Agendamento = await _mediator.Send(new ObterAgendamentoQuery(id, EmpresaAtiva!.Value));
        if (Agendamento == null)
        {
            _logger.LogWarning(
                "Agendamento {Id} não encontrado para '{Email}'.",
                id,
                UsuarioEmail);
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAtualizarSituacaoAsync(Guid id, string novaSituacao)
    {
        _logger.LogInformation(
            "Atualizando status do agendamento {Id} para '{Status}' por '{Email}'.",
            id,
            novaSituacao,
            UsuarioEmail);

        var ok = await _mediator.Send(new AtualizarSituacaoAgendamentoCommand(id, EmpresaAtiva!.Value, novaSituacao));
        if (ok)
        {
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Status do agendamento {Id} alterado para '{Status}' por '{Email}'.",
                id,
                novaSituacao,
                UsuarioEmail);
        }
        else
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.ErroAlterarSituacao;
            _logger.LogWarning(
                "Falha ao alterar status do agendamento {Id} por '{Email}'.",
                id,
                UsuarioEmail);
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelarAsync(Guid id)
    {
        _logger.LogInformation(
            "Cancelando agendamento {Id} por '{Email}'. Motivo: {Motivo}.",
            id,
            UsuarioEmail,
            MotivoCancelamento);

        var (sucesso, mensagem) = await _mediator.Send(
            new CancelarAgendamentoCommand(id, EmpresaAtiva!.Value, MotivoCancelamento, "operador"));

        if (sucesso)
        {
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Agendamento {Id} cancelado por '{Email}'.",
                id,
                UsuarioEmail);
        }
        else
        {
            MensagemErro = mensagem;
            _logger.LogWarning(
                "Falha ao cancelar agendamento {Id} por '{Email}': {Mensagem}.",
                id,
                UsuarioEmail,
                mensagem);
        }

        return RedirectToPage(new { id });
    }
}
