using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Agendamentos.Painel;

public class NovoModel : AppBasePageModel
{
    protected override string? ChavePermissao => "Servicos.Agendamentos.Cadastrar";

    private readonly IMediator _mediator;
    private readonly ILogger<NovoModel> _logger;

    public NovoModel(IMediator mediator, ILogger<NovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public IEnumerable<ServicoAgendavel> Servicos { get; private set; } = [];
    public IEnumerable<Prestador> Prestadores { get; private set; } = [];

    [BindProperty]
    [Display(Name = "Serviço")]
    [Required]
    public Guid ProdutoVariacaoId { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Profissional")]
    public string PrestadorIdComposto { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Data e horário")]
    [Required]
    public string DataHora { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Data")]
    public string DataAgendamento { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Cliente")]
    public string ClienteIdRaw { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Nome do cliente")]
    [MaxLength(200)]
    public string? ClienteNome { get; set; }

    [BindProperty]
    [Display(Name = "E-mail")]
    [EmailAddress]
    [MaxLength(200)]
    public string? ClienteEmail { get; set; }

    [BindProperty]
    [Display(Name = "Telefone")]
    [MaxLength(30)]
    public string? ClienteTelefone { get; set; }

    [BindProperty]
    [Display(Name = "Observações")]
    [MaxLength(1000)]
    public string? Observacoes { get; set; }

    [BindProperty]
    [Display(Name = "Enviar e-mail de confirmação")]
    public bool EnviaEmailConfirmacao { get; set; } = true;

    public string? PrestadorNome { get; private set; }

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Voltar { get; set; }

    public string UrlVoltar { get; private set; } = ModuloRotas.Agendamentos.Painel;
    public string TextoVoltar { get; private set; } = "Agendamentos";

    public async Task<IActionResult> OnGetAsync(string? data)
    {
        ResolverVoltar();
        DataAgendamento = string.IsNullOrWhiteSpace(data)
            ? DateTime.Today.ToString("yyyy-MM-dd")
            : data;

        await CarregarDadosAsync();

        if (!string.IsNullOrEmpty(PrestadorIdComposto))
        {
            var p = Prestadores.FirstOrDefault(x =>
                (x.UsuarioId.HasValue && ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("U", x.UsuarioId) == PrestadorIdComposto) ||
                (x.ContatoId.HasValue && ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.PrestadorIdComposto("C", x.ContatoId) == PrestadorIdComposto));

            if (p != null)
                PrestadorNome = p.NomeExibicao;
            else
                _logger.LogWarning(
                    "Prestador não encontrado para o ID '{PrestadorId}' solicitado por '{Email}'.",
                    PrestadorIdComposto,
                    UsuarioEmail);
        }

        _logger.LogInformation("Formulário de novo agendamento acessado por '{Email}'.", UsuarioEmail);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ResolverVoltar();

        if (!DateTime.TryParse(DataHora, out var dataHoraInicio))
        {
            _logger.LogWarning(
                "Data/hora inválida ao cadastrar agendamento por '{Email}': {DataHora}.",
                UsuarioEmail,
                DataHora);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.DataHoraInvalida;
            await CarregarDadosAsync();
            return Page();
        }

        var servicos = await _mediator.Send(new ListarServicosAgendaQuery(ContaId));
        var ts = servicos.FirstOrDefault(t => t.Id == ProdutoVariacaoId);
        if (ts == null)
        {
            _logger.LogWarning(
                "Serviço não encontrado ao cadastrar agendamento por '{Email}'. ID: {Id}.",
                UsuarioEmail,
                ProdutoVariacaoId);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.ServicoNaoEncontrado;
            await CarregarDadosAsync();
            return Page();
        }

        var dataHoraFim = dataHoraInicio.AddMinutes(ts.DuracaoMinutos);

        ChavesSistema.ExtrairPrestadorIdComposto(PrestadorIdComposto, out var profUsuarioId, out var profContatoId);

        Guid? clienteUsuarioId = null;
        if (!string.IsNullOrEmpty(ClienteIdRaw) && Guid.TryParse(ClienteIdRaw, out var cid))
            clienteUsuarioId = cid;

        try
        {
            _logger.LogInformation(
                "Cadastrando agendamento para '{ClienteNome}' por '{Email}'.",
                ClienteNome,
                UsuarioEmail);

            var resultado = await _mediator.Send(new CadastrarAgendamentoCommand(
                EmpresaId: EmpresaAtiva!.Value,
                ProdutoVariacaoId: ProdutoVariacaoId,
                ProfissionalUsuarioId: profUsuarioId,
                ProfissionalContatoId: profContatoId,
                DataHoraInicio: dataHoraInicio,
                DataHoraFim: dataHoraFim,
                ClienteUsuarioId: clienteUsuarioId,
                ClienteNome: ClienteNome,
                ClienteEmail: ClienteEmail,
                ClienteTelefone: ClienteTelefone,
                Observacoes: Observacoes,
                OperadorInclusao: UsuarioId,
                EnviaEmailConfirmacao: EnviaEmailConfirmacao
            ));

            if (!resultado.Sucesso)
            {
                _logger.LogWarning(
                    "Conflito de horário ao cadastrar agendamento para '{ClienteNome}' por '{Email}'.",
                    ClienteNome,
                    UsuarioEmail);
                MensagemErro = resultado.Mensagem;
                await CarregarDadosAsync();
                return Page();
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            return ModuloRotas.Redirecionar(ModuloRotas.Agendamentos.Painel_Detalhes, new { id = resultado.Dados });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao cadastrar agendamento para '{ClienteNome}' por '{Email}'.",
                ClienteNome,
                UsuarioEmail);
            MensagemErro = string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Agendamentos.ErroCriarAgendamento, ex.Message);
            await CarregarDadosAsync();
            return Page();
        }
    }

    private async Task CarregarDadosAsync()
    {
        Servicos = await _mediator.Send(new ListarServicosAgendaQuery(ContaId));
        Prestadores = await _mediator.Send(new ListarPrestadoresQuery(EmpresaAtiva!.Value, ContaId));
    }

    private void ResolverVoltar()
    {
        if (!string.IsNullOrWhiteSpace(Voltar) && Voltar.StartsWith('/') && !Voltar.StartsWith("//"))
        {
            UrlVoltar = Voltar;
            TextoVoltar = "Voltar";
            return;
        }

        UrlVoltar = string.IsNullOrEmpty(PrestadorIdComposto)
            ? ModuloRotas.Agendamentos.Painel
            : $"{ModuloRotas.Agendamentos.Painel}?prestadorIdComposto={PrestadorIdComposto}";
        TextoVoltar = "Agendamentos";
    }
}
