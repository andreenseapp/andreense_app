using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Grupos;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditarModel> _logger;

    public EditarModel(IMediator mediator, ILogger<EditarModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [TempData]
    public string? MensagemErro { get; set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [BindProperty]
    public CadastrarGrupoCommand Comando { get; set; } = new();

    public IEnumerable<RecursoSistema> TodosRecursos { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à edição de grupo do InfraAdmin para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var grupo = await _mediator.Send(new ObterGrupoQuery(id, EmpresaAtiva!.Value));

            if (grupo == null)
            {
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.NaoEncontrado;
                _logger.LogWarning("Grupo ID '{GrupoId}' não encontrado para o usuário Master '{Email}'.", id, UsuarioEmail);
                return RedirectToPage("Index");
            }

            TodosRecursos = await _mediator.Send(new ListarTodosRecursosQuery());

            Comando = new CadastrarGrupoCommand
            {
                Id = grupo.Id,
                EmpresaId = grupo.EmpresaId,
                Nome = grupo.Nome,
                Descricao = grupo.Descricao,
                EhAdministrador = grupo.EhAdministrador,
                Ativo = grupo.Ativo,
                RecursosSelecionados = grupo.Recursos.Select(r => r.Id).ToList()
            };

            _logger.LogInformation(
                "Formulário de edição de grupo '{GrupoNome}' (ID: {GrupoId}) carregado por '{Email}'.",
                Comando.Nome,
                id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao carregar dados do grupo ID '{GrupoId}' para o usuário Master '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroCarregarDados;
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de atualizar grupo do InfraAdmin pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            TodosRecursos = await _mediator.Send(new ListarTodosRecursosQuery());
            return Page();
        }

        try
        {
            Comando.EmpresaId = EmpresaAtiva!.Value;

            var resultado = await _mediator.Send(Comando);

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Grupo '{GrupoNome}' (ID: '{GrupoId}') atualizado com sucesso pelo usuário Master '{Email}'.",
                    Comando.Nome,
                    Comando.Id,
                    UsuarioEmail);
                return RedirectToPage("Index");
            }

            MensagemErro = resultado.Mensagem;
            _logger.LogWarning(
                "Falha ao atualizar grupo ID '{GrupoId}' pelo usuário Master '{Email}': {Mensagem}",
                Comando.Id,
                UsuarioEmail,
                resultado.Mensagem);
            TodosRecursos = await _mediator.Send(new ListarTodosRecursosQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar grupo ID '{GrupoId}' pelo usuário Master '{Email}'.", Comando.Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
            TodosRecursos = await _mediator.Send(new ListarTodosRecursosQuery());
        }

        return Page();
    }
}
