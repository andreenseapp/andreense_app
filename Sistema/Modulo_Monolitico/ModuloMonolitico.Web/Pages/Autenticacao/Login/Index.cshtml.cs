using ModuloMonolitico.Web.Core.Constantes;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Servicos;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

[ModuloMonolitico.Web.Core.LimiteAcesso("login-rate-limit", CampoUsuario = "Formulario.Email")]
public class LoginModel : ContextoStandalonePageModel
{
  public override string ContextoPasta => "Autenticacao";
  public override string PaginaSlug => "login";
  private readonly IMediator _mediator;
  private readonly IServicoAutenticacaoGoogle _autenticacaoGoogle;
  private readonly IConfiguration _config;
  private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
  private readonly ILogger<LoginModel> _logger;

  public LoginModel(
      IMediator mediator,
      IServicoAutenticacaoGoogle autenticacaoGoogle,
      IConfiguration config,
      IServicoCookieAutenticacao cookiesAutenticacao,
      ILogger<LoginModel> logger)
  {
    _mediator = mediator;
    _autenticacaoGoogle = autenticacaoGoogle;
    _config = config;
    _cookiesAutenticacao = cookiesAutenticacao;
    _logger = logger;
  }

  [BindProperty]
  public FormularioLogin Formulario { get; set; } = new();

  [BindProperty(SupportsGet = true, Name = "ReturnUrl")]
  public string? UrlRetorno { get; set; }
  
  [BindProperty(SupportsGet = true, Name = "exp")]
  public int? Expiracao { get; set; }

  public string? MensagemErro { get; private set; }

  private void PreencherViewData()
  {
    ViewData[ViewDataKeys.GoogleClientId] = _autenticacaoGoogle.Habilitado
      ? _config["Google:ClientId"] ?? string.Empty
      : string.Empty;
  }

  public void OnGet()
  {
      if (Expiracao == 1) MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.SessaoExpirada;
      if (Expiracao == 2) MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.SessaoEncerradaAdmin;
      PreencherViewData();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var requisicaoAjax = RequisicaoAjax();

    if (!ModelState.IsValid)
    {
      _logger.LogWarning("Tentativa de login bloqueada. Falha na validação do formulário (E-mail ou Senha vazios).");
      var mensagemValidacao = ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
          ?? "Informe e-mail e senha válidos.";

      if (requisicaoAjax)
        return ResponderErroAjax(mensagemValidacao, StatusCodes.Status400BadRequest);

      PreencherViewData();
      return Page();
    }

    var comando = new LoginCommand
    {
      Email = Formulario.Email,
      Senha = Formulario.Senha,
      AgenteUsuario = Request.Headers["User-Agent"].ToString(),
      IdiomaAceito = Request.Headers["Accept-Language"].ToString(),
      Plataforma = Request.Headers["Sec-CH-UA-Platform"].ToString() ?? "",
      EnderecoIp = Request.ObterIpCliente()
    };

    try
    {
      var resultado = await _mediator.Send(comando);

      if (!resultado.Sucesso)
      {
        _logger.LogWarning("Tentativa de login falhou para o usuário '{Email}'. Motivo: {Mensagem}", Formulario.Email, resultado.Mensagem);

        if (requisicaoAjax)
          return ResponderErroAjax(resultado.Mensagem ?? "Credenciais inválidas. Verifique seu e-mail e senha.", StatusCodes.Status401Unauthorized);

        MensagemErro = resultado.Mensagem;
        PreencherViewData();
        return Page();
      }

      _logger.LogInformation("Usuário '{Email}' realizou login com sucesso via credenciais.", Formulario.Email);
      _cookiesAutenticacao.DefinirTokensAutenticacao(resultado.TokenAcesso!, resultado.TokenRenovacao!, Formulario.ManterConectado);

      var rotaDestino = !string.IsNullOrEmpty(UrlRetorno) && Url.IsLocalUrl(UrlRetorno)
          ? UrlRetorno
          : ModuloRotas.Principal.Hub;

      if (requisicaoAjax)
        return ModuloRotas.JsonSucesso(rotaDestino);

      return ModuloRotas.Redirecionar(rotaDestino);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro crítico inesperado durante o login do usuário '{Email}'.", Formulario.Email);
      
      var mensagemErroInterno = "Ocorreu um erro interno no servidor ao tentar realizar o login.";
      if (requisicaoAjax)
        return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

      MensagemErro = mensagemErroInterno;
      PreencherViewData();
      return Page();
    }
  }

  public class FormularioLogin
  {
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool ManterConectado { get; set; } = false;
  }
}


