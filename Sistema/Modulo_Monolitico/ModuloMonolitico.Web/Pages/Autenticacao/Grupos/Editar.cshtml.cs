using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Autenticacao.Grupos;

public class EditarModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditarModel> _logger;

    public EditarModel(IMediator mediator, ILogger<EditarModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override string? ChavePermissao => "Base.GrupoUsuario.Editar";

    [BindProperty]
    public CadastrarGrupoCommand Comando { get; set; } = new();

    public IEnumerable<RecursoSistema> TodosRecursos { get; private set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try
        {
            var grupo = await _mediator.Send(new ObterGrupoQuery(id, EmpresaAtiva!.Value));

            if (grupo == null)
            {
                _logger.LogWarning("Grupo ID '{GrupoId}' não encontrado para o usuário '{Email}'.", id, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoEncontrado;
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
                RecursosSelecionados = grupo.Recursos.Select(r => r.Id).ToList(),
                RecursosAtalhos = grupo.Recursos.Where(r => r.AtalhoDashboard).Select(r => r.Id).ToList()
            };

            _logger.LogInformation(
                "Formulário de edição do grupo '{GrupoNome}' (ID: {GrupoId}) acessado por '{Email}'.",
                grupo.Nome,
                id,
                UsuarioEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar grupo ID '{GrupoId}' para o usuário '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.ErroCarregar;
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
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
                    "Grupo '{GrupoNome}' (ID: {GrupoId}) atualizado na empresa {EmpresaId} pelo usuário '{Email}'.",
                    Comando.Nome,
                    Comando.Id,
                    Comando.EmpresaId,
                    UsuarioEmail);
                return RedirectToPage("Index");
            }

            MensagemErro = resultado.Mensagem;
            _logger.LogWarning(
                "Falha ao atualizar grupo '{GrupoNome}' (ID: {GrupoId}) pelo usuário '{Email}': {Mensagem}.",
                Comando.Nome,
                Comando.Id,
                UsuarioEmail,
                resultado.Mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar grupo ID '{GrupoId}' pelo usuário '{Email}'.", Comando.Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.ErroSalvarConfiguracoes;
        }

        TodosRecursos = await _mediator.Send(new ListarTodosRecursosQuery());
        return Page();
    }
}
