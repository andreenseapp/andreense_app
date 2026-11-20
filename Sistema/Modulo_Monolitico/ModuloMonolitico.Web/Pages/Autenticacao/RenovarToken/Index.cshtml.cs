using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

public class RenovarTokenModel : PageModel
{
  private readonly IMediator _mediator;
  private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
  private readonly ILogger<RenovarTokenModel> _logger;

  public RenovarTokenModel(IMediator mediator, IServicoCookieAutenticacao cookiesAutenticacao, ILogger<RenovarTokenModel> logger)
  {
    _mediator = mediator;
    _cookiesAutenticacao = cookiesAutenticacao;
    _logger = logger;
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var refreshToken = Request.Cookies[EsquemaCookieAutenticacao.TokenRenovacao];
    if (string.IsNullOrEmpty(refreshToken))
    {
      _logger.LogWarning("Tentativa de renãovação de token falhou: RefreshToken ausente nãos cookies.");
      return Unauthorized();
    }

    var comando = new RenovarTokenCommand
    {
      TokenRenovacao = refreshToken,
      AgenteUsuario = Request.Headers["User-Agent"].ToString(),
      IdiomaAceito = Request.Headers["Accept-Language"].ToString(),
      Plataforma = Request.Headers["Sec-CH-UA-Platform"].ToString() ?? "",
      EnderecoIp = Request.ObterIpCliente()
    };

    var resultado = await _mediator.Send(comando);

    if (!resultado.Sucesso)
    {
      _logger.LogWarning("Falha ao Renovar token. Motivo: {Mensagem}", resultado.Mensagem);
      _cookiesAutenticacao.LimparSessao();
      return Unauthorized();
    }

    _logger.LogInformation("Token renãovado silenciosamente com sucesso.");
    _cookiesAutenticacao.DefinirTokensAutenticacao(resultado.TokenAcesso!, resultado.TokenRenovacao!);
    return new OkResult();
  }
}



