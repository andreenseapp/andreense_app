using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Pages.Autenticacao;

[AllowAnonymous]
[LimiteAcesso("esqueci-senha-rate-limit", CampoUsuario = "Email")]
public class EsqueciSenhaModel : ContextoStandalonePageModel
{
    public override string ContextoPasta => "Autenticacao";
    public override string PaginaSlug => "esqueci-senha";

    private readonly IMediator _mediator;
    private readonly ILogger<EsqueciSenhaModel> _logger;

    public EsqueciSenhaModel(IMediator mediator, ILogger<EsqueciSenhaModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "E-mail")]
    [EmailAddress]
    public string? Email { get; set; }

    public string? MensagemErro { get; private set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var requisicaoAjax = RequisicaoAjax();

        if (string.IsNullOrWhiteSpace(Email))
        {
            _logger.LogWarning("RecuperacaoSenha: POST EsqueciSenha sem e-mail.");
            if (requisicaoAjax)
                return ResponderErroAjax("Por favor, informe seu e-mail.");

            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.InformeEmail;
            return Page();
        }

        Email = Email.Trim().ToLowerInvariant();

        try
        {
            var comando = new SolicitarRecuperacaoSenhaCommand
            {
                Email = Email,
                AgenteUsuario = Request.Headers["User-Agent"].ToString(),
                EnderecoIp = Request.ObterIpCliente()
            };
            var resultado = await _mediator.Send(comando);

            if (!resultado.Sucesso)
            {
                _logger.LogWarning("RecuperacaoSenha: POST EsqueciSenha falhou para '{Email}'. Motivo: {Mensagem}", Email, resultado.Mensagem);
                if (requisicaoAjax)
                    return ResponderErroAjax(resultado.Mensagem ?? "Não foi possível enviar o código.", StatusCodes.Status400BadRequest);

                MensagemErro = resultado.Mensagem;
                return Page();
            }

            _logger.LogInformation("RecuperacaoSenha: POST EsqueciSenha OK para '{Email}'.", Email);
            NotificarSucesso(resultado.Mensagem);
            var rotaDestino = ModuloRotas.Autenticacao.RedefinirSenha + QueryString.Create("email", Email).Value;

            if (requisicaoAjax)
                return ModuloRotas.JsonSucesso(rotaDestino);

            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.RedefinirSenha, new { email = Email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao solicitar recuperação de senha para '{Email}'.", Email);
            
            var mensagemErroInterno = "Ocorreu um erro interno no servidor ao tentar processar a solicitação.";
            if (requisicaoAjax)
                return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

            MensagemErro = mensagemErroInterno;
            return Page();
        }
    }
}
