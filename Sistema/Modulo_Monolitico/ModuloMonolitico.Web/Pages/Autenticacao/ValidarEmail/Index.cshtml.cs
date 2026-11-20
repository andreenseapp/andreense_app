using ModuloMonolitico.Web.Core.Constantes;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Web.Pages.Shared;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;

namespace ModuloMonolitico.Web.Pages.Autenticacao
{
    public class ValidarEmailModel : AppBasePageModel
    {
        private readonly IServicoEmail _servicoEmail;
        private readonly IMediator _mediator;
        private readonly IServicoCookieAutenticacao _cookiesAutenticacao;
        private readonly ILogger<ValidarEmailModel> _logger;

        public ValidarEmailModel(
            IServicoEmail servicoEmail,
            IMediator mediator,
            IServicoCookieAutenticacao cookiesAutenticacao,
            ILogger<ValidarEmailModel> logger)
        {
            _servicoEmail = servicoEmail;
            _mediator = mediator;
            _cookiesAutenticacao = cookiesAutenticacao;
            _logger = logger;
        }

        protected override bool ExigeEmpresaAtiva => false;

        [BindProperty]
        [Display(Name = "Código de validação")]
        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (EmailValidado)
            {
                return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);
            }

            var cookieEnviado = Request.Cookies[EsquemaCookieAutenticacao.EmailCodigoEnviado];
            if (string.IsNullOrEmpty(cookieEnviado))
            {
                await EnviarNovoCodigoAsync();
                _cookiesAutenticacao.DefinirThrottleEmailValidacao();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var valido = await _mediator.Send(new ValidarCodigoEmailCommand(UsuarioEmail, Codigo));
            
            if (valido)
            {
                var refreshToken = Request.Cookies[EsquemaCookieAutenticacao.TokenRenovacao];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var comandoRenovar = new RenovarTokenCommand
                    {
                        TokenRenovacao = refreshToken,
                        AgenteUsuario = Request.Headers["User-Agent"].ToString(),
                        IdiomaAceito = Request.Headers["Accept-Language"].ToString(),
                        Plataforma = Request.Headers["Sec-CH-UA-Platform"].ToString() ?? "",
                        EnderecoIp = Request.ObterIpCliente()
                    };

                    var resultado = await _mediator.Send(comandoRenovar);
                    if (resultado.Sucesso)
                        _cookiesAutenticacao.DefinirTokensAutenticacao(resultado.TokenAcesso!, resultado.TokenRenovacao!);
                }
                
                NotificarSucesso();
                _logger.LogInformation("E-mail '{Email}' validado com sucesso.", UsuarioEmail);
                return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);
            }

            _logger.LogWarning("Tentativa de validação falhou para '{Email}'. Código inválido ou expirado.", UsuarioEmail);
            NotificarErro("Código inválido ou expirado. Tente novamente.");
            return Page();
        }

        public async Task<IActionResult> OnPostReenviarAsync()
        {
            await EnviarNovoCodigoAsync();
            _cookiesAutenticacao.DefinirThrottleEmailValidacao();

            _logger.LogInformation("Novo código de validação solicitado para '{Email}'.", UsuarioEmail);
            NotificarSucesso();
            return Page();
        }

        private async Task EnviarNovoCodigoAsync()
        {
            var novoCodigo = new Random().Next(100000, 999999).ToString();
            await _mediator.Send(new CadastrarCodigoValidacaoCommand(UsuarioEmail, novoCodigo, "validacao_email", DateTime.UtcNow.AddHours(2)));

            var assunto = "Código de Validação - Andreense";
            var corpo = $"""
                <h1>Seu código de validação chegou!</h1>
                <p>Use o código abaixo para validar sua conta:</p>
                <h2 style='color: #8ab4f8; letter-spacing: 5px;'>{novoCodigo}</h2>
                """;

            await _servicoEmail.EnviarEmailAsync(UsuarioEmail, assunto, corpo);
        }
    }
}



