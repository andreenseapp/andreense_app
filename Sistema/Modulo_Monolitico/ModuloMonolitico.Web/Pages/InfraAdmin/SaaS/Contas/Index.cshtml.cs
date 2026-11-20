using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS;

public class ContasIndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContasIndexModel> _logger;

    public ContasIndexModel(IMediator mediator, ILogger<ContasIndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<ContaResumoDto> Contas { get; private set; } = new List<ContaResumoDto>();
    public IEnumerable<PlanoSaaS> Planos { get; private set; } = new List<PlanoSaaS>();
    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à listagem de contas SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Contas = await _mediator.Send(new ListarContasComAssinaturaQuery());
            Planos = await _mediator.Send(new ListarPlanosAtivosQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar listagem de contas SaaS para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroCarregarPagina;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAtualizarSituacaoAsync(Guid contaId, string novaSituacao)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atualizar status de conta SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var assinatura = await _mediator.Send(new ObterAssinaturaContaQuery(contaId));
            if (assinatura == null)
            {
                _logger.LogWarning("Assinatura da conta ID '{ContaId}' não encontrada para alteração de status.", contaId);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Contas.AssinaturaNaoEncontrada;
                return RedirectToPage();
            }

            assinatura.Situacao = novaSituacao;
            assinatura.DataUltimaAtualizacao = DateTime.UtcNow;
            await _mediator.Send(new AtualizarAssinaturaContaCommand(assinatura));

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Status da assinatura da conta '{ContaId}' alterado para '{NovoStatus}' pelo usuário Master '{Email}'.", contaId, novaSituacao, UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar status da conta '{ContaId}' pelo usuário '{Email}'.", contaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Contas.ErroAtualizarSituacao;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAtribuirPlanoAsync(Guid contaId, Guid planoId, DateTime dataVencimento, decimal valorContratado)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atribuir plano de conta SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var plano = await _mediator.Send(new ObterPlanoSaaSQuery(planoId));
            if (plano == null)
            {
                _logger.LogWarning("Plano ID '{PlanoId}' não encontrado para atribuição à conta '{ContaId}'.", planoId, contaId);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Planos.NaoEncontrado;
                return RedirectToPage();
            }

            var existente = await _mediator.Send(new ObterAssinaturaContaQuery(contaId));
            if (existente != null)
            {
                existente.PlanoId = planoId;
                existente.DataVencimento = dataVencimento.ToUniversalTime();
                existente.ValorContratado = valorContratado;
                existente.Situacao = SituacaoAssinatura.Ativo;
                existente.DataUltimaAtualizacao = DateTime.UtcNow;
                await _mediator.Send(new AtualizarAssinaturaContaCommand(existente));
            }
            else
            {
                await _mediator.Send(new CadastrarAssinaturaContaCommand(new ContaAssinatura
                {
                    Id = Guid.NewGuid(),
                    ContaId = contaId,
                    PlanoId = planoId,
                    Situacao = SituacaoAssinatura.Ativo,
                    DataInicio = DateTime.UtcNow,
                    DataVencimento = dataVencimento.ToUniversalTime(),
                    ValorContratado = valorContratado,
                    OperadorInclusao = UsuarioId
                }));
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Plano '{PlanoNome}' atribuído à conta '{ContaId}' pelo usuário Master '{Email}'.", plano.Nome, contaId, UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atribuir plano à conta '{ContaId}' pelo usuário '{Email}'.", contaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Contas.ErroAtribuirPlano;
        }

        return RedirectToPage();
    }
}
