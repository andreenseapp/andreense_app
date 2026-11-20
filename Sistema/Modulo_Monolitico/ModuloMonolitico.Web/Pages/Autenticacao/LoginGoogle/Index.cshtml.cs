using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

[IgnoreAntiforgeryToken]
public class LoginGoogleModel : PageModel
{
  private const string TagLog = "LOG :: [LoginGoogleModel]";

  private readonly IMediator _mediator;
  private readonly ILogger<LoginGoogleModel> _logger;
  private readonly IServicoCookieAutenticacao _cookiesAutenticacao;

  public LoginGoogleModel(
      IMediator mediator,
      ILogger<LoginGoogleModel> logger,
      IServicoCookieAutenticacao cookiesAutenticacao)
  {
    _mediator = mediator;
    _logger = logger;
    _cookiesAutenticacao = cookiesAutenticacao;
  }

  public async Task<IActionResult> OnPostAsync([FromBody] RequisicaoLoginGoogle requisicao)
  {
    _logger.LogInformation("{TagLog} Requisição de login via Google recebida.", TagLog);

    if (!RequisicaoOrigemValida())
    {
      _logger.LogWarning("{TagLog} Requisição rejeitada: origem ou cabeçalho inválido.", TagLog);
      return new JsonResult(ModuloRotas.JsonErro("Não foi possível entrar com Google. Tente novamente.").Value)
      {
        StatusCode = 403
      };
    }

    if (string.IsNullOrWhiteSpace(requisicao?.TokenIdentidade))
    {
      _logger.LogWarning("{TagLog} idToken ausente na requisição.", TagLog);
      return new JsonResult(ModuloRotas.JsonErro("Não foi possível entrar com Google. Tente novamente.").Value)
      {
        StatusCode = 400
      };
    }

    var comando = new LoginGoogleCommand
    {
      TokenIdentidade = requisicao.TokenIdentidade,
      AgenteUsuario = Request.Headers["User-Agent"].ToString(),
      EnderecoIp = Request.ObterIpCliente()
    };

    var resultado = await _mediator.Send(comando);

    if (!resultado.Sucesso)
    {
      _logger.LogWarning("{TagLog} Login Google falhou: {Mensagem}", TagLog, resultado.Mensagem);
      return new JsonResult(ModuloRotas.JsonErro(resultado.Mensagem ?? "Falha no login Google.").Value)
      {
        StatusCode = 401
      };
    }

    _cookiesAutenticacao.DefinirTokensAutenticacao(resultado.TokenAcesso!, resultado.TokenRenovacao!);

    _logger.LogInformation("{TagLog} Login Google bem-sucedido. Redirecionando para hub.", TagLog);

    return ModuloRotas.JsonSucesso(ModuloRotas.Principal.Hub);
  }

  public IActionResult OnGet() => ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

  private bool RequisicaoOrigemValida()
  {
    if (!string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal))
      return false;

    var hostBase = $"{Request.Scheme}://{Request.Host.Value}";

    var origin = Request.Headers.Origin.FirstOrDefault();
    if (!string.IsNullOrEmpty(origin))
      return origin.StartsWith(hostBase, StringComparison.OrdinalIgnoreCase);

    var referer = Request.Headers.Referer.FirstOrDefault();
    if (!string.IsNullOrEmpty(referer))
      return referer.StartsWith(hostBase, StringComparison.OrdinalIgnoreCase);

    return false;
  }
}

public sealed record RequisicaoLoginGoogle([property: System.Text.Json.Serialization.JsonPropertyName("idToken")] string? TokenIdentidade);
