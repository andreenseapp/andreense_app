using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.UltimosConectados;

public class IndexModel : AppBasePageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;
    private readonly IServicoIdentidadeJwt _identidade;

    protected override bool ExigeEmpresaAtiva => false;

    public IEnumerable<TentativaLoginListagemDto> Conexoes { get; private set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger, IServicoIdentidadeJwt identidade)
    {
        _mediator = mediator;
        _logger = logger;
        _identidade = identidade;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Acesso negado à tela de últimos conectados para o usuário '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            Conexoes = await _mediator.Send(new ListarUltimosConectadosQuery(50));

            _logger.LogInformation("Últimos conectados consultado por '{Email}'. {Total} registros retornados.",
                UsuarioEmail, Conexoes.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao listar últimos conectados para o usuário '{Email}'.", UsuarioEmail);
            Conexoes = [];
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Autenticacao.ErroListaConectados;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRevogarAcessoAsync(Guid usuarioId)
    {
        if (!UsuarioEhMaster)
        {
            _logger.LogWarning("Tentativa de revogar acesso por usuário não-master '{Email}'.", UsuarioEmail);
            return Forbid();
        }

        try
        {
            await _mediator.Send(new RevogarAcessoUsuarioCommand(usuarioId));

            _identidade.InvalidarCacheIdentidade(usuarioId);

            _logger.LogWarning("Acesso do usuário '{UsuarioId}' revogado pelo master '{Email}'. Cache invalidado imediatamente.",
                usuarioId, UsuarioEmail);

            MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao tentar revogar acesso do usuário '{UsuarioId}' pelo master '{Email}'.", usuarioId, UsuarioEmail);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Autenticacao.ErroRevogarAcesso;
        }

        return RedirectToPage();
    }
}
