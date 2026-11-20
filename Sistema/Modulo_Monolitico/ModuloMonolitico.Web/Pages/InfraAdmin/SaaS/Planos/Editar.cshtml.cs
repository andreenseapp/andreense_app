using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.SaaS;

public class PlanosEditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlanosEditarModel> _logger;

    public PlanosEditarModel(IMediator mediator, ILogger<PlanosEditarModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    public PlanoSaaS Plano { get; private set; } = new();
    public IEnumerable<RecursoPlanoSaaS> RecursosDisponiveis { get; private set; } = [];

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public string Nome { get; set; } = string.Empty;

    [BindProperty]
    public string? Descricao { get; set; }

    [BindProperty]
    public string ValorMensalStr { get; set; } = "0";

    [BindProperty]
    public bool Ativo { get; set; } = true;

    [BindProperty]
    public List<string> RecursosSelecionados { get; set; } = [];

    [BindProperty]
    public Dictionary<string, string> ValoresRecurso { get; set; } = new();

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à edição de plano SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var plano = await _mediator.Send(new ObterPlanoSaaSQuery(id));
            if (plano == null)
            {
                _logger.LogWarning("Plano SaaS ID '{PlanoId}' não encontrado para o usuário Master '{Email}'.", id, UsuarioEmail);
                return NotFound();
            }

            Id = id;
            Plano = plano;
            Nome = plano.Nome;
            Descricao = plano.Descricao;
            ValorMensalStr = plano.ValorMensal.ToString("F2", CultureInfo.InvariantCulture);
            Ativo = plano.Ativo;
            RecursosSelecionados = plano.Recursos.Select(f => f.RecursoId.ToString()).ToList();
            ValoresRecurso = plano.Recursos.ToDictionary(f => f.RecursoId.ToString(), f => f.Valor);

            RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());

            _logger.LogInformation(
                "Formulário de edição do plano SaaS '{PlanoNome}' (ID: {PlanoId}) carregado por '{Email}'.",
                Nome,
                id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar plano SaaS ID '{PlanoId}' para o usuário Master '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroCarregarPagina;
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.SaaS_Planos);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atualizar plano SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await RecarregarFormularioAsync();
            return Page();
        }

        if (!TentarConverterValorMensal(out var valor))
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.SaaS.Planos.ValorMensalInvalido;
            await RecarregarFormularioAsync();
            return Page();
        }

        try
        {
            var resultado = await _mediator.Send(new AtualizarPlanoSaaSCommand
            {
                Id = Id,
                Nome = Nome,
                Descricao = Descricao,
                ValorMensal = valor,
                Ativo = Ativo,
                RecursosSelecionados = RecursosSelecionados,
                ValoresRecurso = ValoresRecurso
            });

            if (!resultado.Sucesso)
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning(
                    "Falha ao atualizar plano SaaS ID '{PlanoId}' pelo usuário Master '{Email}': {Mensagem}",
                    Id,
                    UsuarioEmail,
                    resultado.Mensagem);
                await RecarregarFormularioAsync();
                return Page();
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation(
                "Plano SaaS '{PlanoNome}' (ID: '{PlanoId}') atualizado com sucesso pelo usuário Master '{Email}'.",
                Nome,
                Id,
                UsuarioEmail);
            return ModuloRotas.Redirecionar(ModuloRotas.InfraAdmin.SaaS_Planos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar o plano SaaS ID '{PlanoId}' pelo usuário Master '{Email}'.", Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
            await RecarregarFormularioAsync();
            return Page();
        }
    }

    private async Task RecarregarFormularioAsync()
    {
        RecursosDisponiveis = await _mediator.Send(new ListarRecursosPlanoSaaSQuery());
        Plano = await _mediator.Send(new ObterPlanoSaaSQuery(Id)) ?? new PlanoSaaS { Id = Id };
    }

    private bool TentarConverterValorMensal(out decimal valor) =>
        decimal.TryParse(ValorMensalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
}
