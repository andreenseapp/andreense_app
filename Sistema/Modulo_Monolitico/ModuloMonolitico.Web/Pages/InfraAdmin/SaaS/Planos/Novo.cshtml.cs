using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS;

public class PlanosNovoModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlanosNovoModel> _logger;

    public PlanosNovoModel(IMediator mediator, ILogger<PlanosNovoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public IEnumerable<RecursoPlanoSaaS> RecursosDisponiveis { get; private set; } = [];

    [BindProperty]
    public string Nome { get; set; } = string.Empty;

    [BindProperty]
    public string? Descricao { get; set; }

    [BindProperty]
    public string ValorMensalStr { get; set; } = "0";

    [BindProperty]
    public List<string> RecursosSelecionados { get; set; } = [];

    [BindProperty]
    public Dictionary<string, string> ValoresRecurso { get; set; } = new();

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à criação de planos SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar features SaaS para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroCarregarPagina;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de criar plano SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
            return Page();
        }

        if (!TentarConverterValorMensal(out var valor))
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Planos.ValorMensalInvalido;
            RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
            return Page();
        }

        try
        {
            var resultado = await _mediator.Send(new CadastrarPlanoSaaSCommand
            {
                Nome = Nome,
                Descricao = Descricao,
                ValorMensal = valor,
                RecursosSelecionados = RecursosSelecionados,
                ValoresRecurso = ValoresRecurso
            });

            if (!resultado.Sucesso)
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning("Falha ao criar plano SaaS pelo usuário Master '{Email}': {Mensagem}", UsuarioEmail, resultado.Mensagem);
                RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
                return Page();
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Plano SaaS '{PlanoNome}' criado com sucesso pelo usuário Master '{Email}'.", Nome, UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.SaaS_Planos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao criar o plano SaaS pelo usuário Master '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
            RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
            return Page();
        }
    }

    private bool TentarConverterValorMensal(out decimal valor) =>
        decimal.TryParse(ValorMensalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
}
