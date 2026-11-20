using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Base.Equipe;

public class IndexModel : AppListBasePageModel<MembroEquipeDto, IndexModel.FiltroPesquisa>
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public class FiltroPesquisa { }

    [BindProperty]
    public string EmailConvite { get; set; } = string.Empty;

    [BindProperty]
    public Guid GrupoIdConvite { get; set; }

    [BindProperty]
    public Guid UsuarioIdEditar { get; set; }

    [BindProperty]
    public Guid NovoGrupoId { get; set; }

    public IEnumerable<GrupoUsuario> GruposDisponiveis { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        if (PaginaAtual < 1)
            PaginaAtual = 1;

        try
        {
            var (itens, total) = await _mediator.Send(new ListarEquipeQuery(EmpresaAtiva!.Value, PaginaAtual, TamanhoPagina));
            Itens = itens;
            TotalItens = total;
            GruposDisponiveis = await _mediator.Send(new ListarGruposPorEmpresaQuery(EmpresaAtiva.Value));

            _logger.LogInformation(
                "Equipe consultada por '{Email}'. Empresa: {EmpresaId}, Página: {Pagina}, Total: {Total}.",
                UsuarioEmail,
                EmpresaAtiva.Value,
                PaginaAtual,
                TotalItens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar equipe para o usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroCarregar;
            Itens = [];
            TotalItens = 0;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostConvidarAsync()
    {
        if (string.IsNullOrWhiteSpace(EmailConvite))
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.EmailObrigatorio;
            return RedirectToPage();
        }

        try
        {
            var resultado = await _mediator.Send(new ConvidarEquipeCommand(
                UsuarioId,
                EmpresaAtiva!.Value,
                EmailConvite.ToLowerInvariant().Trim(),
                GrupoIdConvite));

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Convite enviado por '{Email}' para '{EmailConvite}' na empresa {EmpresaId}.",
                    UsuarioEmail,
                    EmailConvite,
                    EmpresaAtiva.Value);
            }
            else
            {
                MensagemErro = resultado.Erro;
                _logger.LogWarning(
                    "Falha ao convidar '{EmailConvite}' pelo usuário '{Email}': {Erro}.",
                    EmailConvite,
                    UsuarioEmail,
                    resultado.Erro);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao convidar membro pelo usuário '{Email}'.", UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroEnviarConvite;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeletarFisicoAsync(Guid usuarioId)
    {
        if (usuarioId == UsuarioId)
        {
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroRemoverSiMesmo;
            return RedirectToPage();
        }

        try
        {
            var resultado = await _mediator.Send(new DeletarFisicoMembroEquipeCommand(usuarioId, EmpresaAtiva!.Value));

            if (resultado)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Membro {RemovidoId} removido da equipe pelo usuário '{Email}'. Empresa: {EmpresaId}.",
                    usuarioId,
                    UsuarioEmail,
                    EmpresaAtiva.Value);
            }
            else
            {
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroRemover;
                _logger.LogWarning(
                    "Falha ao remover membro {RemovidoId} da empresa {EmpresaId} pelo usuário '{Email}'.",
                    usuarioId,
                    EmpresaAtiva.Value,
                    UsuarioEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao remover membro ID '{MembroId}' pelo usuário '{Email}'.", usuarioId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroRemover;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditarGrupoAsync()
    {
        try
        {
            var resultado = await _mediator.Send(new AtualizarGrupoEquipeCommand(EmpresaAtiva!.Value, UsuarioIdEditar, NovoGrupoId));

            if (resultado)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Grupo do membro {UsuarioIdEditar} atualizado para {NovoGrupoId} pelo usuário '{Email}'.",
                    UsuarioIdEditar,
                    NovoGrupoId,
                    UsuarioEmail);
            }
            else
            {
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroAtualizar;
                _logger.LogWarning(
                    "Falha ao atualizar grupo do membro {UsuarioIdEditar} pelo usuário '{Email}'.",
                    UsuarioIdEditar,
                    UsuarioEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao atualizar perfil do membro ID '{MembroId}' pelo usuário '{Email}'.", UsuarioIdEditar, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroAtualizar;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReenviarConviteAsync(Guid usuarioId)
    {
        try
        {
            var resultado = await _mediator.Send(new ReenviarConviteEquipeCommand(UsuarioId, EmpresaAtiva!.Value, usuarioId));

            if (resultado)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Convite reenviado para membro {UsuarioId} pelo usuário '{Email}'.",
                    usuarioId,
                    UsuarioEmail);
            }
            else
            {
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroReenviarConvite;
                _logger.LogWarning(
                    "Falha ao reenviar convite para membro {UsuarioId} pelo usuário '{Email}'.",
                    usuarioId,
                    UsuarioEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao reenviar convite para '{EmailDestino}' pelo usuário '{Email}'.", usuarioId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Base.Equipe.ErroReenviarConvite;
        }

        return RedirectToPage();
    }
}
