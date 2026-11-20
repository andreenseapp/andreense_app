using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Autenticacao.Grupos;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override string? ChavePermissao => "Base.GrupoUsuario.Visualizar";

    public IEnumerable<GrupoUsuario> Grupos { get; private set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var resultado = await _mediator.Send(new ListarGruposQuery(EmpresaAtiva!.Value));
            Grupos = resultado.Itens;

            _logger.LogInformation(
                "Grupos de usuários consultados por '{Email}'. Empresa: {EmpresaId}, Total: {Total}.",
                UsuarioEmail,
                EmpresaAtiva.Value,
                Grupos.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar grupos de usuários para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.ErroCarregarLista;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeletarFisicoAsync(Guid id)
    {
        try
        {
            var resultado = await _mediator.Send(new DeletarGrupoCommand
            {
                Id = id,
                EmpresaId = EmpresaAtiva!.Value
            });

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Grupo ID '{GrupoId}' excluído com sucesso pelo usuário '{Email}'. Empresa: {EmpresaId}.",
                    id,
                    UsuarioEmail,
                    EmpresaAtiva.Value);
            }
            else
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning(
                    "Falha ao excluir grupo ID '{GrupoId}' pelo usuário '{Email}': {Mensagem}.",
                    id,
                    UsuarioEmail,
                    resultado.Mensagem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao excluir grupo ID '{GrupoId}' pelo usuário '{Email}'.", id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.ErroExcluir;
        }

        return RedirectToPage();
    }
}
