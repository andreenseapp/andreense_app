using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.GruposSaaS;

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

    public IEnumerable<RecursoSistemaSaaS> TodosRecursos { get; private set; } = [];
    public IEnumerable<IGrouping<string, RecursoSistemaSaaS>> RecursosPorModulo { get; private set; } = Enumerable.Empty<IGrouping<string, RecursoSistemaSaaS>>();

    [BindProperty]
    public string Nome { get; set; } = string.Empty;

    [BindProperty]
    public string? Descricao { get; set; }

    [BindProperty]
    public bool Ativo { get; set; } = true;

    [BindProperty]
    public List<Guid> RecursosSelecionados { get; set; } = [];

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à criação de grupos SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        await CarregarRecursos();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de criar grupo SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await CarregarRecursos();
            return Page();
        }

        try
        {
            var comando = new CadastrarGrupoSaaSCommand
            {
                Nome = Nome,
                Descricao = Descricao,
                Ativo = Ativo,
                OperadorId = UsuarioId,
                RecursosSelecionados = RecursosSelecionados
            };

            var resultado = await _mediator.Send(comando);

            if (!resultado.Sucesso)
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning("Falha ao criar grupo SaaS pelo usuário Master '{Email}': {Mensagem}", UsuarioEmail, resultado.Mensagem);
                await CarregarRecursos();
                return Page();
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Grupo SaaS '{GrupoNome}' criado com sucesso pelo usuário Master '{Email}'.", Nome, UsuarioEmail);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao criar grupo SaaS pelo usuário Master '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
            await CarregarRecursos();
            return Page();
        }
    }

    private async Task CarregarRecursos()
    {
        TodosRecursos = await _mediator.Send(new ListarRecursosSaaSQuery());
        RecursosPorModulo = TodosRecursos
            .GroupBy(r => r.Modulo ?? "Geral");
    }
}
