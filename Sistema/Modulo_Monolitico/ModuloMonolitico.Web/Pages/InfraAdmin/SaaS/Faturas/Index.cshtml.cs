using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS;

public class FaturasIndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<FaturasIndexModel> _logger;

    public FaturasIndexModel(IMediator mediator, ILogger<FaturasIndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public IEnumerable<FaturaSaaS> Faturas { get; private set; } = [];
    public Guid ContaIdFiltro { get; private set; }
    public string ContaNomeFiltro { get; private set; } = string.Empty;

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid contaId)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado às faturas SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            ContaIdFiltro = contaId;
            if (contaId != Guid.Empty)
            {
                Faturas = await _mediator.Send(new ListarFaturasContaQuery(contaId));
                var contas = await _mediator.Send(new ListarContasComAssinaturaQuery());
                ContaNomeFiltro = contas.FirstOrDefault(c => c.ContaId == contaId)?.ContaNome ?? string.Empty;

                _logger.LogInformation(
                    "Faturas da conta '{ContaNome}' (ID: {ContaId}) consultadas por '{Email}'. Total: {Total}.",
                    ContaNomeFiltro,
                    contaId,
                    UsuarioEmail,
                    Faturas.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar faturas da conta '{ContaId}' para o usuário '{Email}'.", contaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroCarregarPagina;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostBaixaAsync(Guid faturaId, Guid contaId)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de dar baixa em fatura pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            await _mediator.Send(new DarBaixaFaturaSaaSCommand(faturaId, UsuarioId));
            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Baixa realizada com sucesso na fatura ID '{FaturaId}' da conta '{ContaId}' pelo usuário Master '{Email}'.",
                faturaId,
                contaId,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao dar baixa na fatura ID '{FaturaId}' da conta '{ContaId}' pelo usuário '{Email}'.", faturaId, contaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Faturas.ErroDarBaixa;
        }

        return RedirectToPage(new { contaId });
    }

    public async Task<IActionResult> OnPostGerarAsync(Guid contaId, string mesReferencia, decimal valorTotal, DateTime dataVencimento, string? observacoes)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de gerar fatura pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            await _mediator.Send(new CadastrarFaturaSaaSCommand(new FaturaSaaS
            {
                Id = Guid.NewGuid(),
                ContaId = contaId,
                MesReferencia = mesReferencia,
                ValorTotal = valorTotal,
                Situacao = SituacaoFatura.Pendente,
                DataVencimento = dataVencimento.ToUniversalTime(),
                Observacoes = observacoes,
                DataInclusao = DateTime.UtcNow
            }));

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Fatura do mês '{MesReferencia}' no valor de R$ {ValorTotal} gerada para a conta '{ContaId}' pelo usuário Master '{Email}'.",
                mesReferencia,
                valorTotal,
                contaId,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao gerar fatura de '{MesReferencia}' para a conta '{ContaId}' pelo usuário '{Email}'.", mesReferencia, contaId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Faturas.ErroGerar;
        }

        return RedirectToPage(new { contaId });
    }
}
