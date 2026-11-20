using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

[LimiteAcesso("cadastro-rate-limit", CampoUsuario = "Formulario.Email")]
public class CadastroModel : ContextoStandalonePageModel
{
  public override string ContextoPasta => "Autenticacao";
  public override string PaginaSlug => "cadastro";
  private readonly IMediator _mediator;
  private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
  private readonly ILogger<CadastroModel> _logger;

  public CadastroModel(IMediator mediator, IServicoCookieAutenticacao cookiesAutenticacao, ILogger<CadastroModel> logger)
  {
    _mediator = mediator;
    _cookiesAutenticacao = cookiesAutenticacao;
    _logger = logger;
  }

  [BindProperty]
  public CadastroFormulario Formulario { get; set; } = new();

  public string? MensagemErro { get; private set; }
  
  public IEnumerable<DocumentoLegal> DocumentosAtivos { get; set; } = new List<DocumentoLegal>();

  public async Task OnGetAsync() 
  { 
      DocumentosAtivos = await _mediator.Send(new ObterDocumentosAtivosParaAceiteQuery());
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var requisicaoAjax = RequisicaoAjax();
    DocumentosAtivos = await _mediator.Send(new ObterDocumentosAtivosParaAceiteQuery());

    if (!ModelState.IsValid)
    {
      _logger.LogWarning("Tentativa de cadastro bloqueada. Falha na validação do formulário.");
      if (requisicaoAjax)
      {
        var mensagemValidacao = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
            ?? "Verifique os campos do formulário.";
        return ResponderErroAjax(mensagemValidacao);
      }
      return Page();
    }

    try
    {
      var comando = new CadastrarUsuarioCommand
      {
        Nome   = Formulario.Nome,
        Email  = Formulario.Email,
        Senha  = Formulario.Senha,
        ConfirmarSenha = Formulario.ConfirmarSenha
      };

      var resultadoCadastro = await _mediator.Send(comando);

      if (!resultadoCadastro.Sucesso)
      {
        _logger.LogWarning("Tentativa de cadastro falhou para o usuário '{Email}'. Motivo: {Mensagem}", Formulario.Email, resultadoCadastro.Mensagem);
        if (requisicaoAjax)
          return ResponderErroAjax(resultadoCadastro.Mensagem ?? "Não foi possível criar a conta.", StatusCodes.Status400BadRequest);

        MensagemErro = resultadoCadastro.Mensagem;
        return Page();
      }

      var comandoLogin = new LoginCommand
      {
        Email = Formulario.Email,
        Senha = Formulario.Senha,
        AgenteUsuario = Request.Headers["User-Agent"].ToString(),
        IdiomaAceito = Request.Headers["Accept-Language"].ToString(),
        Plataforma = Request.Headers["Sec-CH-UA-Platform"].ToString() ?? "",
        EnderecoIp = Request.ObterIpCliente()
      };

      var resultadoLogin = await _mediator.Send(comandoLogin);

      if (!resultadoLogin.Sucesso)
      {
        _logger.LogWarning("Cadastro bem-sucedido, mas o login automático falhou para '{Email}'. Motivo: {Mensagem}", Formulario.Email, resultadoLogin.Mensagem);
        if (requisicaoAjax)
          return ModuloRotas.JsonSucesso(ModuloRotas.Autenticacao.Login);

        MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.ErroLoginPosCadastro;
        return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
      }

      _logger.LogInformation("Usuário '{Email}' cadastrado e logado com sucesso.", Formulario.Email);

      _cookiesAutenticacao.DefinirTokensAutenticacao(resultadoLogin.TokenAcesso!, resultadoLogin.TokenRenovacao!);

      var tokenHandler = new JwtSecurityTokenHandler();
      var jwtToken = tokenHandler.ReadJwtToken(resultadoLogin.TokenAcesso);
      var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      
      if (Guid.TryParse(userIdClaim, out var usuarioId))
      {
          foreach (var doc in DocumentosAtivos)
          {
              await _mediator.Send(new RegistrarAceiteCommand 
              { 
                  UsuarioId = usuarioId, 
                  DocumentoLegalId = doc.Id 
              });
          }
      }

      if (requisicaoAjax)
        return ModuloRotas.JsonSucesso(ModuloRotas.Principal.Hub);

      return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro crítico inesperado durante o cadastro do usuário '{Email}'.", Formulario.Email);
      
      var mensagemErroInterno = "Ocorreu um erro interno no servidor ao tentar realizar o cadastro.";
      if (requisicaoAjax)
        return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

      MensagemErro = mensagemErroInterno;
      return Page();
    }
  }

  public class CadastroFormulario
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "O {0} não é válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      ErrorMessage = "A senha deve ter no mínimo 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
    [Display(Name = "Confirmação de senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Você deve ler e aceitar os termos de uso para continuar.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve ler e aceitar os termos de uso para continuar.")]
    [Display(Name = "Termos de uso")]
    public bool AceitouTermos { get; set; }
  }
}



