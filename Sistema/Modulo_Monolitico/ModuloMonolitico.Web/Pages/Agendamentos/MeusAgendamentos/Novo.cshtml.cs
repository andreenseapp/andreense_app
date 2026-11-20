using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.MeusAgendamentos;

public class NovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<NovoModel> _logger;

    public NovoModel(IMediator mediator, ILogger<NovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public int Passo { get; private set; } = 1;

    public IEnumerable<EmpresaVinculada> EmpresasVinculadas { get; private set; } = [];
    public IEnumerable<ServicoAgendavel> TiposServico { get; private set; } = [];
    public IEnumerable<Prestador> Profissionais { get; private set; } = [];

    [TempData] public Guid? AssistenteEmpresaId { get; set; }
    [TempData] public string? AssistenteEmpresaNome { get; set; }
    [TempData] public Guid? AssistenteTipoServicoId { get; set; }
    [TempData] public Guid? AssistenteProfissionalId { get; set; }
    [TempData] public string? AssistenteProfissionalTipo { get; set; }
    [TempData] public string? AssistenteDataHoraInicio { get; set; }
    [TempData] public string? AssistenteDataHoraFim { get; set; }
    [TempData] public string? AssistenteServicoNome { get; set; }
    [TempData] public string? AssistenteProfNome { get; set; }

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    public string UrlPasso(int passo) => $"?passo={passo}";

    public async Task<IActionResult> OnGetAsync(int passo = 1)
    {
        Passo = passo;
        await CarregarPassoAsync(passo);

        _logger.LogInformation(
            "Wizard de agendamento (passo {Passo}) acessado por '{Email}'.",
            Passo,
            UsuarioEmail);

        return Page();
    }

    public async Task<IActionResult> OnPostPasso1Async(Guid empresaId, string empresaNome)
    {
        AssistenteEmpresaId = empresaId;
        AssistenteEmpresaNome = empresaNome;
        Passo = 2;

        _logger.LogInformation(
            "Cliente '{Email}' selecionou empresa {EmpresaId} no wizard.",
            UsuarioEmail,
            empresaId);

        TiposServico = await _mediator.Send(new ListarServicosAgendaQuery(empresaId));
        Profissionais = await _mediator.Send(new ListarPrestadoresQuery(empresaId, empresaId));
        EmpresasVinculadas = await ObterEmpresasVinculadasAsync();
        return Page();
    }

    public IActionResult OnPostPasso2(
        Guid tipoServicoId,
        string profissionalIdRaw,
        string dataHoraInicio,
        string dataHoraFim,
        string servicoNome,
        string profNome)
    {
        AssistenteTipoServicoId = tipoServicoId;

        ChavesSistema.ExtrairTipoEIdPrestador(profissionalIdRaw, out var profTipo, out var profId);

        AssistenteProfissionalId = profId ?? Guid.Empty;
        AssistenteProfissionalTipo = profTipo;
        AssistenteDataHoraInicio = dataHoraInicio;
        AssistenteDataHoraFim = dataHoraFim;
        AssistenteServicoNome = servicoNome;
        AssistenteProfNome = string.IsNullOrWhiteSpace(profNome) ? "Qualquer disponível" : profNome;
        Passo = 3;

        _logger.LogInformation(
            "Cliente '{Email}' avançou para confirmação no wizard. Serviço: {ServicoId}.",
            UsuarioEmail,
            tipoServicoId);

        return Page();
    }

    public async Task<IActionResult> OnPostConfirmarAsync()
    {
        if (!AssistenteEmpresaId.HasValue || !AssistenteTipoServicoId.HasValue || string.IsNullOrEmpty(AssistenteDataHoraInicio))
        {
            _logger.LogWarning("Sessão do wizard expirada para '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.SessaoExpirada;
            return RedirectToPage(new { passo = 1 });
        }

        if (!DateTime.TryParse(AssistenteDataHoraInicio, out var inicio) || !DateTime.TryParse(AssistenteDataHoraFim, out var fim))
        {
            _logger.LogWarning("Data/hora inválida no wizard para '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.DataHoraInvalida;
            return RedirectToPage(new { passo = 1 });
        }

        var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));

        try
        {
            _logger.LogInformation(
                "Confirmando agendamento do cliente '{Email}' na empresa {EmpresaId}.",
                UsuarioEmail,
                AssistenteEmpresaId);

            var resultado = await _mediator.Send(new CadastrarAgendamentoCommand(
                EmpresaId: AssistenteEmpresaId.Value,
                ProdutoVariacaoId: AssistenteTipoServicoId.Value,
                ProfissionalUsuarioId: AssistenteProfissionalTipo == "U" && AssistenteProfissionalId.GetValueOrDefault() != Guid.Empty ? AssistenteProfissionalId : null,
                ProfissionalContatoId: AssistenteProfissionalTipo == "C" && AssistenteProfissionalId.GetValueOrDefault() != Guid.Empty ? AssistenteProfissionalId : null,
                DataHoraInicio: inicio,
                DataHoraFim: fim,
                ClienteUsuarioId: UsuarioId,
                ClienteNome: usuario?.Nome,
                ClienteEmail: usuario?.Email,
                ClienteTelefone: null,
                Observacoes: null,
                OperadorInclusao: null
            ));

            if (!resultado.Sucesso)
            {
                _logger.LogWarning("Conflito de horário ao confirmar agendamento do cliente '{Email}'.", UsuarioEmail);
                MensagemErro = resultado.Mensagem;
                return RedirectToPage(new { passo = 3 });
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar agendamento do cliente '{Email}'.", UsuarioEmail);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.ErroConfirmarAgendamento, ex.Message);
            return RedirectToPage(new { passo = 3 });
        }
    }

    private async Task CarregarPassoAsync(int passo)
    {
        if (passo == 1 || passo == 2)
            EmpresasVinculadas = await ObterEmpresasVinculadasAsync();

        if ((passo == 2 || Passo == 2) && AssistenteEmpresaId.HasValue)
        {
            TiposServico = await _mediator.Send(new ListarServicosAgendaQuery(AssistenteEmpresaId.Value));
            Profissionais = await _mediator.Send(new ListarPrestadoresQuery(AssistenteEmpresaId.Value, AssistenteEmpresaId.Value));
        }
    }

    private async Task<IEnumerable<EmpresaVinculada>> ObterEmpresasVinculadasAsync()
    {
        var vinculos = await _mediator.Send(new ListarVinculosAtivosPorContaQuery(ContaId));
        return vinculos.Select(v => new EmpresaVinculada
        {
            Id = v.ContaBId,
            Nome = v.NomeContaParceira ?? "—"
        }).ToList();
    }
}

public class EmpresaVinculada
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Segmento { get; set; }
}
