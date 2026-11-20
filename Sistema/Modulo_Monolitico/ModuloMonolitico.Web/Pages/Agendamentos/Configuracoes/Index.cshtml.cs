using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.Configuracoes;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Agendamentos.Configurar";

    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<ServicoAgendavel> Servicos { get; private set; } = [];
    public IEnumerable<Prestador> Prestadores { get; private set; } = [];
    public IEnumerable<ConfiguracaoHorario> ConfiguracoesHorarios { get; private set; } = [];
    public IEnumerable<HorarioBloqueado> HorariosBloqueados { get; private set; } = [];
    public IEnumerable<SelectListItem> OpcoesPrestadorHorarios { get; private set; } = [];
    public IEnumerable<SelectListItem> OpcoesPrestadorBloqueio { get; private set; } = [];

    public static string[] DiasNomes { get; } =
        ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"];

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Profissional")]
    public string? PrestadorIdComposto { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Voltar { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Aba")]
    public string Aba { get; set; } = "horarios";

    public string AbaAtiva => string.IsNullOrWhiteSpace(Aba) ? "horarios" : Aba;

    public string UrlVoltar { get; private set; } = ModuloRotas.Agendamentos.Painel;
    public string TextoVoltar { get; private set; } = "Agendamentos";

    public string QueryVoltar =>
        string.IsNullOrEmpty(Voltar) ? string.Empty : $"&voltar={Uri.EscapeDataString(Voltar)}";

    public string UrlAba(string aba) => $"?aba={aba}{QueryVoltar}";

    public async Task<IActionResult> OnGetAsync()
    {
        ResolverVoltar();
        await CarregarAsync();

        _logger.LogInformation(
            "Configurações de agendamentos acessadas por '{Email}'. Aba: {Tab}.",
            UsuarioEmail,
            AbaAtiva);

        return Page();
    }

    public async Task<IActionResult> OnPostVincularServicosPrestadorAsync(
        Guid? usuarioId, Guid? contatoId, List<Guid>? produtoVariacaoIds)
    {
        try
        {
            _logger.LogInformation(
                "Salvando vínculos de serviços para prestador U:{UsuarioId} C:{ContatoId} por '{Email}'.",
                usuarioId,
                contatoId,
                UsuarioEmail);

            await _mediator.Send(new VincularServicosPrestadorCommand(
                EmpresaAtiva!.Value, usuarioId, contatoId, produtoVariacaoIds ?? []));

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao salvar vínculos do prestador por '{Email}'.",
                UsuarioEmail);
            MensagemErro = ex.Message;
        }

        return RedirectToPage(new { aba = "profissionais", voltar = Voltar });
    }

    public async Task<IActionResult> OnPostAtualizarHorariosAsync(string? prestadorIdComposto)
    {
        ChavesSistema.ExtrairPrestadorIdComposto(prestadorIdComposto, out var usuarioId, out var contatoId);

        var horarios = new List<ConfiguracaoHorario>();
        for (int dia = 0; dia <= 6; dia++)
        {
            var ativo = Request.Form[ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.FormAgendamentoAtivo(dia)] == "true";
            var horaInStr = Request.Form[ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.FormAgendamentoInicio(dia)].ToString();
            var horaFimStr = Request.Form[ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.FormAgendamentoFim(dia)].ToString();
            var intervStr = Request.Form[ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.FormAgendamentoIntervalo(dia)].ToString();

            if (!ativo || string.IsNullOrEmpty(horaInStr) || string.IsNullOrEmpty(horaFimStr)) continue;
            if (!TimeOnly.TryParse(horaInStr, out var horaIn) || !TimeOnly.TryParse(horaFimStr, out var horaFim)) continue;
            int.TryParse(intervStr, out var interv);

            horarios.Add(new ConfiguracaoHorario
            {
                DiaSemana = dia,
                HoraInicio = horaIn,
                HoraFim = horaFim,
                IntervaloMinutos = interv > 0 ? interv : 30,
                Ativo = true
            });
        }

        _logger.LogInformation(
            "Salvando {Count} regras de horários para '{PrestadorIdComposto}' por '{Email}'.",
            horarios.Count,
            prestadorIdComposto,
            UsuarioEmail);

        await _mediator.Send(new AtualizarConfiguracoesHorariosCommand(EmpresaAtiva!.Value, horarios, usuarioId, contatoId));
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage(new { aba = "horarios", prestadorIdComposto, voltar = Voltar });
    }

    public async Task<IActionResult> OnPostCadastrarBloqueioAsync(
        string? prestadorIdComposto, DateTime dataInicio, DateTime dataFim, string? motivo)
    {
        ChavesSistema.ExtrairPrestadorIdComposto(prestadorIdComposto, out var usuarioId, out var contatoId);

        _logger.LogInformation(
            "Cadastrando bloqueio de {Inicio} a {Fim} por '{Email}'.",
            dataInicio,
            dataFim,
            UsuarioEmail);

        await _mediator.Send(new CadastrarHorarioBloqueadoCommand(
            EmpresaAtiva!.Value, usuarioId, contatoId, dataInicio, dataFim, motivo));

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage(new { aba = "bloqueios", voltar = Voltar });
    }

    public async Task<IActionResult> OnPostDeletarFisicoAsync(Guid id)
    {
        _logger.LogInformation(
            "Removendo bloqueio {Id} por '{Email}'.",
            id,
            UsuarioEmail);

        await _mediator.Send(new DeletarFisicoHorarioBloqueadoCommand(id, EmpresaAtiva!.Value));
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return RedirectToPage(new { aba = "bloqueios", voltar = Voltar });
    }

    private void ResolverVoltar()
    {
        if (!string.IsNullOrWhiteSpace(Voltar) && Voltar.StartsWith('/') && !Voltar.StartsWith("//"))
        {
            UrlVoltar = Voltar;
            TextoVoltar = "Voltar";
            return;
        }

        UrlVoltar = ModuloRotas.Agendamentos.Painel;
        TextoVoltar = "Agendamentos";
    }

    private async Task CarregarAsync()
    {
        ChavesSistema.ExtrairPrestadorIdComposto(PrestadorIdComposto, out var usuarioId, out var contatoId);

        Servicos = await _mediator.Send(new ListarServicosAgendaQuery(ContaId, false));
        Prestadores = await _mediator.Send(new ListarPrestadoresQuery(EmpresaAtiva!.Value, ContaId, false));
        ConfiguracoesHorarios = await _mediator.Send(new ObterConfiguracoesHorariosQuery(EmpresaAtiva.Value, usuarioId, contatoId));
        HorariosBloqueados = await _mediator.Send(new ListarHorariosBloqueadosQuery(
            EmpresaAtiva.Value, DateTime.Today, DateTime.Today.AddMonths(3)));

        MontarOpcoesPrestador();
    }

    private void MontarOpcoesPrestador()
    {
        OpcoesPrestadorHorarios = Prestadores.Select(p =>
        {
            var rawVal = p.UsuarioId.HasValue ? ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("U", p.UsuarioId) : ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("C", p.ContatoId);
            return new SelectListItem
            {
                Value = rawVal,
                Text = p.NomeExibicao,
                Selected = rawVal == PrestadorIdComposto
            };
        });

        OpcoesPrestadorBloqueio = Prestadores.Select(p =>
        {
            var rawVal = p.UsuarioId.HasValue ? ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("U", p.UsuarioId) : ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("C", p.ContatoId);
            return new SelectListItem
            {
                Value = rawVal,
                Text = $"Apenas {p.NomeExibicao}"
            };
        });
    }
}
