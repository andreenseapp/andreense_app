using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages;

public class MeuPerfilDesativarModel : AppBasePageModel
{
    [TempData] public string? MensagemErro { get; set; }
    [TempData] public string? MensagemSucesso { get; set; }

    private readonly IMediator _mediator;
    private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
    private readonly ILogger<MeuPerfilDesativarModel> _logger;

    public MeuPerfilDesativarModel(
        IMediator mediator,
        IServicoCookieAutenticacao cookiesAutenticacao,
        ILogger<MeuPerfilDesativarModel> logger)
    {
        _mediator = mediator;
        _cookiesAutenticacao = cookiesAutenticacao;
        _logger = logger;
    }

    protected override bool ExigeEmpresaAtiva => false;

    [BindProperty]
    [Display(Name = "Senha de confirmação")]
    [Required]
    [DataType(DataType.Password)]
    public string? SenhaConfirmacao { get; set; }

    public string? LetraAvatar { get; private set; }
    public string? EmailAtual { get; private set; }
    public string? NomeAtual { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));
        if (usuario is null) return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

        CarregarDados(usuario);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));
        if (usuario is null) return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

        if (string.IsNullOrWhiteSpace(SenhaConfirmacao))
        {
            _logger.LogWarning(
                "Tentativa de desativação de conta do usuário '{UsuarioId}' rejeitada: senha de confirmação não informada.",
                UsuarioId);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Perfil.SenhaConfirmacaoObrigatoria;
            CarregarDados(usuario);
            return Page();
        }

        if (!BCrypt.Net.BCrypt.Verify(SenhaConfirmacao, usuario.SenhaHash))
        {
            _logger.LogWarning(
                "Tentativa de desativação de conta do usuário '{UsuarioId}' rejeitada: senha de confirmação incorreta.",
                UsuarioId);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Perfil.SenhaConfirmacaoIncorreta;
            CarregarDados(usuario);
            return Page();
        }

        await _mediator.Send(new InativarUsuarioCommand(UsuarioId));

        _cookiesAutenticacao.LimparSessao();

        _logger.LogInformation("Conta do usuário '{UsuarioId}' desativada com sucesso.", UsuarioId);

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
        return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
    }

    private void CarregarDados(Usuario usuario)
    {
        NomeAtual = usuario.Nome;
        EmailAtual = usuario.Email;
        LetraAvatar = usuario.Nome.Length > 0 ? usuario.Nome.Substring(0, 1).ToUpper() : "U";
    }
}
