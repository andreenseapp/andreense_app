using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.GruposSaaS;

public class UsuariosModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsuariosModel> _logger;

    public UsuariosModel(IMediator mediator, ILogger<UsuariosModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public GrupoUsuarioSaaS? GrupoAtual { get; private set; }
    public IEnumerable<UsuarioGrupoSaaS> Membros { get; private set; } = [];

    [BindProperty]
    public string EmailBusca { get; set; } = string.Empty;

    public Usuario? UsuarioEncontrado { get; private set; }
    public bool BuscaRealizada { get; private set; }

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado aos membros do grupo SaaS para o usuário '{Email}'. Não é Master.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            await CarregarDados();

            if (GrupoAtual is null)
            {
                _logger.LogWarning("Grupo SaaS ID '{GrupoId}' não encontrado para o usuário '{Email}'.", Id, UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.NaoEncontrado;
                return RedirectToPage("./Index");
            }

            _logger.LogInformation(
                "Membros do grupo SaaS '{GrupoNome}' (ID: {GrupoId}) consultados por '{Email}'. Total: {Total}.",
                GrupoAtual.Nome,
                Id,
                UsuarioEmail,
                Membros.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar membros do grupo SaaS ID '{GrupoId}' para o usuário '{Email}'.", Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroCarregarDados;
            return RedirectToPage("./Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostBuscarAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de buscar usuário por e-mail no grupo SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        BuscaRealizada = true;

        try
        {
            if (!string.IsNullOrWhiteSpace(EmailBusca))
                UsuarioEncontrado = await _mediator.Send(new ObterUsuarioPorEmailQuery(EmailBusca.Trim().ToLower()));

            await CarregarDados();

            _logger.LogInformation(
                "Busca de colaborador por e-mail '{EmailBusca}' no grupo SaaS ID '{GrupoId}' realizada por '{Email}'. Encontrado: {Encontrado}.",
                EmailBusca,
                Id,
                UsuarioEmail,
                UsuarioEncontrado is not null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por e-mail no grupo SaaS ID '{GrupoId}' para o usuário '{Email}'.", Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Grupos.ErroProcessarBusca;
        }

        if (GrupoAtual is null)
            return RedirectToPage("./Index");

        return Page();
    }

    public async Task<IActionResult> OnPostVincularAsync(Guid usuarioId)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de vincular usuário ao grupo SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var resultado = await _mediator.Send(new VincularUsuarioGrupoSaaSCommand
            {
                UsuarioId = usuarioId,
                GrupoSaaSId = Id,
                OperadorId = UsuarioId
            });

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Usuário ID '{VincUsuarioId}' vinculado ao grupo SaaS ID '{GrupoId}' com sucesso pelo usuário Master '{Email}'.",
                    usuarioId,
                    Id,
                    UsuarioEmail);
            }
            else
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning(
                    "Falha ao vincular usuário ID '{VincUsuarioId}' ao grupo SaaS ID '{GrupoId}' pelo usuário Master '{Email}': {Mensagem}",
                    usuarioId,
                    Id,
                    UsuarioEmail,
                    resultado.Mensagem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao vincular usuário ID '{VincUsuarioId}' ao grupo SaaS ID '{GrupoId}' pelo usuário '{Email}'.", usuarioId, Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostRemoverAsync(Guid usuarioId)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa não autorizada de desvincular usuário do grupo SaaS pelo usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            var resultado = await _mediator.Send(new DesvincularUsuarioGrupoSaaSCommand
            {
                UsuarioId = usuarioId,
                GrupoSaaSId = Id
            });

            if (resultado.Sucesso)
            {
                MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
                _logger.LogInformation(
                    "Usuário ID '{VincUsuarioId}' desvinculado do grupo SaaS ID '{GrupoId}' com sucesso pelo usuário Master '{Email}'.",
                    usuarioId,
                    Id,
                    UsuarioEmail);
            }
            else
            {
                MensagemErro = resultado.Mensagem;
                _logger.LogWarning(
                    "Falha ao desvincular usuário ID '{VincUsuarioId}' do grupo SaaS ID '{GrupoId}' pelo usuário Master '{Email}': {Mensagem}",
                    usuarioId,
                    Id,
                    UsuarioEmail,
                    resultado.Mensagem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao desvincular usuário ID '{VincUsuarioId}' do grupo SaaS ID '{GrupoId}' pelo usuário '{Email}'.", usuarioId, Id, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.ErroProcessarRequisicao;
        }

        return RedirectToPage(new { Id });
    }

    private async Task CarregarDados()
    {
        GrupoAtual = await _mediator.Send(new ObterGrupoSaaSQuery(Id));
        Membros = await _mediator.Send(new ListarUsuariosDoGrupoSaaSQuery(Id));
    }
}
