using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.Painel;

public class IndexModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Agendamentos.Visualizar";

    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<Agendamento> AgendamentosMes { get; private set; } = [];
    public IEnumerable<Prestador> Prestadores { get; private set; } = [];
    public IEnumerable<ConfiguracaoHorario> Configuracoes { get; private set; } = [];
    public IEnumerable<SelectListItem> OpcoesPrestador { get; private set; } = [];
    public IEnumerable<ModuloMonolitico.Dominio.Entidades.Servicos.ServicoAgendavel> Servicos { get; private set; } = [];

    [BindProperty] public Guid ProdutoVariacaoId { get; set; }
    [BindProperty] public string? ClienteNome { get; set; }
    [BindProperty] public string? ClienteTelefone { get; set; }
    [BindProperty] public string? DataAgendamento { get; set; }
    [BindProperty] public string? Horario { get; set; }
    [BindProperty] public string? Observacoes { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ModoVisualizacao { get; set; } = "mes";

    public DateTime MesAtual { get; private set; }
    public DateTime DiaAtual { get; private set; }
    public DateTime InicioSemanaAtual => DiaAtual.AddDays(-(((int)DiaAtual.DayOfWeek + 6) % 7));
    public int Ano { get; private set; }
    public int Mes { get; private set; }
    public int Dia { get; private set; }

    public IEnumerable<Agendamento> AgendamentosExibicao =>
        string.IsNullOrEmpty(SituacaoFiltro)
            ? AgendamentosMes.Where(a => !string.Equals(a.Situacao, "cancelado", StringComparison.OrdinalIgnoreCase))
            : AgendamentosMes.Where(a => string.Equals(a.Situacao, SituacaoFiltro, StringComparison.OrdinalIgnoreCase));

    public string UrlMesAnterior { get; private set; } = string.Empty;
    public string UrlMesProximo { get; private set; } = string.Empty;
    public string UrlNovoAgendamento { get; private set; } = ModuloRotas.Agendamentos.Painel_Novo;
    public string UrlConfiguracoes { get; private set; } = ModuloRotas.Agendamentos.Configuracoes;

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Profissional")]
    public string? PrestadorIdComposto { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SituacaoFiltro { get; set; }

    public async Task<IActionResult> OnGetAsync(int? ano, int? mes, int? dia)
    {
        Ano = ano ?? DateTime.Now.Year;
        Mes = mes ?? DateTime.Now.Month;
        Dia = dia ?? (ano == null && mes == null ? DateTime.Today.Day : 1);
        MesAtual = new DateTime(Ano, Mes, 1);
        DiaAtual = new DateTime(Ano, Mes, Dia);

        DateTime de;
        DateTime ate;

        if(ModoVisualizacao == "dia")
        {
            de = DiaAtual.Date;
            ate = de.AddDays(1).AddSeconds(-1);
        }
        else if(ModoVisualizacao == "semana")
        {
            de = InicioSemanaAtual;
            ate = de.AddDays(7).AddSeconds(-1);
        }
        else
        {
            de = MesAtual;
            ate = MesAtual.AddMonths(1).AddSeconds(-1);
        }

        ChavesSistema.ExtrairPrestadorIdComposto(PrestadorIdComposto, out var usuarioId, out var contatoId);
        if (!string.IsNullOrEmpty(PrestadorIdComposto))
        {
            if (usuarioId.HasValue || contatoId.HasValue)
            {
                _logger.LogInformation(
                    "Filtro de prestador aplicado por '{Email}': {PrestadorIdComposto}.",
                    UsuarioEmail,
                    PrestadorIdComposto);
            }
            else
            {
                _logger.LogWarning(
                    "Filtro de prestador mal formatado informado por '{Email}': {PrestadorIdComposto}.",
                    UsuarioEmail,
                    PrestadorIdComposto);
            }
        }

        _logger.LogInformation(
            "Consultando agendamentos de {De:yyyy-MM-dd} a {Ate:yyyy-MM-dd} por '{Email}'.",
            de,
            ate,
            UsuarioEmail);

        AgendamentosMes = await _mediator.Send(new ListarAgendamentosQuery(EmpresaAtiva!.Value, de, ate, usuarioId, contatoId));
        Servicos = await _mediator.Send(new ListarServicosAgendaQuery(ContaId));

        Prestadores = await _mediator.Send(new ListarPrestadoresQuery(EmpresaAtiva.Value, ContaId));

        Configuracoes = await _mediator.Send(new ObterConfiguracoesHorariosQuery(EmpresaAtiva.Value, usuarioId, contatoId));
        if (!Configuracoes.Any())
            Configuracoes = await _mediator.Send(new ObterConfiguracoesHorariosQuery(EmpresaAtiva.Value));

        MontarUrls();
        MontarOpcoesPrestador();

        return Page();
    }

    private void MontarUrls()
    {
        var prestadorQuery = string.IsNullOrEmpty(PrestadorIdComposto)
            ? string.Empty
            : $"&prestadorIdComposto={PrestadorIdComposto}";

        var situacaoQuery = string.IsNullOrEmpty(SituacaoFiltro)
            ? string.Empty
            : $"&situacaoFiltro={SituacaoFiltro}";

        DateTime DataAnt;
        DateTime DataProx;
        
        if (ModoVisualizacao == "dia")
        {
           DataAnt = new DateTime(Ano, Mes, Dia).AddDays(-1);
           DataProx = new DateTime(Ano, Mes, Dia).AddDays(1);
        }
        else if (ModoVisualizacao == "semana")
        {
            DataAnt = new DateTime(Ano, Mes, Dia).AddDays(-7);
            DataProx = new DateTime(Ano, Mes, Dia).AddDays(7);
        }
        else
        {
            DataAnt = new DateTime(Ano, Mes, Dia).AddMonths(-1);
            DataProx = new DateTime(Ano, Mes, Dia).AddMonths(1);
        }
        

        UrlMesAnterior = $"{ModuloRotas.Agendamentos.Painel}?ano={DataAnt.Year}&mes={DataAnt.Month}&dia={DataAnt.Day}&modoVisualizacao={ModoVisualizacao}{prestadorQuery}{situacaoQuery}";
        UrlMesProximo = $"{ModuloRotas.Agendamentos.Painel}?ano={DataProx.Year}&mes={DataProx.Month}&dia={DataProx.Day}&modoVisualizacao={ModoVisualizacao}{prestadorQuery}{situacaoQuery}";

        UrlNovoAgendamento = string.IsNullOrEmpty(PrestadorIdComposto)
            ? ModuloRotas.Agendamentos.Painel_Novo
            : $"{ModuloRotas.Agendamentos.Painel_Novo}?prestadorIdComposto={PrestadorIdComposto}";

        UrlConfiguracoes = $"{ModuloRotas.Agendamentos.Configuracoes}?prestadorIdComposto={PrestadorIdComposto}&voltar={ModuloRotas.Agendamentos.Painel}";
    }

    
    public async Task<IActionResult> OnGetHorariosLivresAsync(DateTime data)
    {
        var config = await _mediator.Send(new ObterConfiguracoesHorariosQuery(EmpresaAtiva.Value));
        int diaSemana = (int)data.DayOfWeek;
        
        var cfgDia = config.FirstOrDefault(c => (int)c.DiaSemana == diaSemana);
        if (cfgDia == null || !cfgDia.Ativo)
            return new JsonResult(new List<string>()); // Dia fechado

        var horarios = new List<string>();
        var atual = cfgDia.HoraInicio;
        while (atual.Add(TimeSpan.FromMinutes(cfgDia.IntervaloMinutos)) <= cfgDia.HoraFim)
        {
            horarios.Add(atual.ToString("HH:mm"));
            atual = atual.Add(TimeSpan.FromMinutes(cfgDia.IntervaloMinutos));
        }
        
        return new JsonResult(horarios);
    }

    private void MontarOpcoesPrestador()
    {
        OpcoesPrestador = Prestadores.Select(p =>
        {
            var rawVal = p.UsuarioId.HasValue ? ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("U", p.UsuarioId) : ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("C", p.ContatoId);
            return new SelectListItem
            {
                Value = rawVal,
                Text = p.NomeExibicao,
                Selected = rawVal == PrestadorIdComposto
            };
        });
    }

    public async Task<IActionResult> OnPostSalvarModalAsync()
    {
        if (string.IsNullOrWhiteSpace(ClienteNome) || ProdutoVariacaoId == Guid.Empty || string.IsNullOrWhiteSpace(Horario))
        {
            MensagemErro = MensagensSistema.Validacao.CampoObrigatorio;
            return RedirectToPage();
        }

        var servicos = await _mediator.Send(new ListarServicosAgendaQuery(ContaId));
        var servico = servicos.FirstOrDefault(s => s.Id == ProdutoVariacaoId);
        if (servico == null)
        {
            MensagemErro = MensagensSistema.Agendamentos.ServicoNaoEncontrado;
            return RedirectToPage();
        }

        if (!DateTime.TryParse(DataAgendamento, out var dataBase) || !TimeOnly.TryParse(Horario, out var horaInicio))
        {
            MensagemErro = MensagensSistema.Agendamentos.DataHoraInvalida;
            return RedirectToPage();
        }

        var dataHoraInicio = dataBase.Date.Add(horaInicio.ToTimeSpan());
        var dataHoraFim = dataHoraInicio.AddMinutes(servico.DuracaoMinutos);

        ChavesSistema.ExtrairPrestadorIdComposto(PrestadorIdComposto, out var profUsuarioId, out var profContatoId);

        var resultado = await _mediator.Send(new CadastrarAgendamentoCommand(
            EmpresaId: EmpresaAtiva!.Value,
            ProdutoVariacaoId: ProdutoVariacaoId,
            ProfissionalUsuarioId: profUsuarioId,
            ProfissionalContatoId: profContatoId,
            DataHoraInicio: dataHoraInicio,
            DataHoraFim: dataHoraFim,
            ClienteUsuarioId: null,
            ClienteNome: ClienteNome,
            ClienteEmail: null,
            ClienteTelefone: ClienteTelefone,
            Observacoes: Observacoes,
            OperadorInclusao: UsuarioId,
            EnviaEmailConfirmacao: false
        ));

        if (!resultado.Sucesso)
        {
            _logger.LogWarning(
                "Conflito de horário ao cadastrar agendamento pelo modal do painel por '{Email}' para {DataHoraInicio:yyyy-MM-dd HH:mm}.",
                UsuarioEmail,
                dataHoraInicio);
            MensagemErro = resultado.Mensagem;
            return RedirectToPage(new { ano = dataBase.Year, mes = dataBase.Month, dia = dataBase.Day });
        }

        MensagemSucesso = MensagensSistema.Geral.Sucesso;
        return RedirectToPage(new { ano = dataBase.Year, mes = dataBase.Month, dia = dataBase.Day });
    }

}
