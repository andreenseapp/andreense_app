using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.GruposSaaS;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditarModel> _logger;

    public EditarModel(IMediator mediator, ILogger<EditarModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    public GrupoUsuarioSaaS? GrupoAtual { get; private set; }
    public IEnumerable<RecursoSistemaSaaS> TodosRecursos { get; private set; } = [];
    public IEnumerable<IGrouping<string, RecursoSistemaSaaS>> RecursosPorModulo { get; private set; } = Enumerable.Empty<IGrouping<string, RecursoSistemaSaaS>>();
    public HashSet<Guid> RecursosVinculados { get; private set; } = [];

    [BindProperty] public string Nome { get; set; } = string.Empty;
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public bool Ativo { get; set; } = true;
    [BindProperty] public List<Guid> RecursosSelecionados { get; set; } = [];

    [TempData] public string? MensagemSucesso { get; set; }
    [TempData] public string? MensagemErro { get; set; }

    protected override bool ExigeEmpresaAtiva => false;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à edição de grupo SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            GrupoAtual = await _mediator.Send(new ObterGrupoSaaSQuery(Id));
            if (GrupoAtual is null)
            {
                _logger.LogWarning("Grupo SaaS ID '{GrupoId}' não encontrado para o usuário Master '{Email}'.", Id, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.NaoEncontrado;
                return RedirectToPage("./Index");
            }

            Nome = GrupoAtual.Nome;
            Descricao = GrupoAtual.Descricao;
            Ativo = GrupoAtual.Ativo;
            RecursosVinculados = GrupoAtual.Recursos.Select(r => r.Id).ToHashSet();

            await CarregarRecursos();

            _logger.LogInformation(
                "Formulário de edição de grupo SaaS '{GrupoNome}' (ID: {GrupoId}) carregado por '{Email}'.",
                Nome,
                Id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar grupo SaaS ID '{GrupoId}' para o usuário Master '{Email}'.", Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroCarregarDados;
            return RedirectToPage("./Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atualizar grupo SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            GrupoAtual = await _mediator.Send(new ObterGrupoSaaSQuery(Id));
            await CarregarRecursos();
            return Page();
        }

        try
        {
            var comando = new CadastrarGrupoSaaSCommand
            {
                Id = Id,
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
                _logger.LogWarning("Falha ao atualizar grupo SaaS ID '{GrupoId}' pelo usuário Master '{Email}': {Mensagem}", Id, UsuarioEmail, resultado.Mensagem);
                GrupoAtual = await _mediator.Send(new ObterGrupoSaaSQuery(Id));
                await CarregarRecursos();
                return Page();
            }

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
            _logger.LogInformation("Grupo SaaS '{GrupoNome}' (ID: '{GrupoId}') atualizado com sucesso pelo usuário Master '{Email}'.", Nome, Id, UsuarioEmail);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar grupo SaaS ID '{GrupoId}' pelo usuário Master '{Email}'.", Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
            GrupoAtual = await _mediator.Send(new ObterGrupoSaaSQuery(Id));
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
