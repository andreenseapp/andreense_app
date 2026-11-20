using ModuloMonolitico.Web.Core.Constantes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Web.Core;

using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Web.Pages.Autenticacao;
[AllowAnonymous]
[LimiteAcesso("redefinir-senha-rate-limit", CampoUsuario = "Email")]
public class RedefinirSenhaModel : ContextoStandalonePageModel
{
    public override string ContextoPasta => "Autenticacao";
    public override string PaginaSlug => "redefinir-senha";

    private readonly IMediator _mediator;
    private readonly ILogger<RedefinirSenhaModel> _logger;
    private readonly ICodigoValidacaoRepositorio _codigoRepo;
    private readonly IWebHostEnvironment _environment;

    public RedefinirSenhaModel(
        IMediator mediator,
        ILogger<RedefinirSenhaModel> logger,
        ICodigoValidacaoRepositorio codigoRepo,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _logger = logger;
        _codigoRepo = codigoRepo;
        _environment = environment;
    }
    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "O {0} não é válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Display(Name = "Código de recuperação")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "A senha deve ter no mínimo 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
    [Display(Name = "Nova senha")]
    public string NovaSenha { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
    [Display(Name = "Confirmar nova senha")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;

    public string? MensagemErro { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
            return ModuloRotas.Redirecionar(ModuloRotas.Principal.Hub);

        if (string.IsNullOrWhiteSpace(Email))
            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.EsqueciSenha);

        Email = Email.Trim().ToLowerInvariant();

        _logger.LogInformation("RecuperacaoSenha: GET RedefinirSenha para '{Email}'.", Email);

        var temCodigoPendente = await _codigoRepo.PossuiCodigoPendenteAsync(Email, "recuperacao_senha");

        if (temCodigoPendente)
        {
            _logger.LogInformation(
                "RecuperacaoSenha: '{Email}' já possui código pendente válido — nenhum reenvio automático.",
                Email);
            await RegistrarCodigoDevAsync(Email, "código pendente existente");
            NotificarSucesso("Use o código enviado ao seu e-mail. Você pode solicitar um novo código abaixo.");
        }
        else
        {
            _logger.LogInformation(
                "RecuperacaoSenha: nenhum código pendente para '{Email}' — disparando envio.",
                Email);

            try
            {
                var resultado = await _mediator.Send(new SolicitarRecuperacaoSenhaCommand
                {
                    Email = Email,
                    AgenteUsuario = Request.Headers["User-Agent"].ToString(),
                    EnderecoIp = Request.ObterIpCliente()
                });

                if (!resultado.Sucesso)
                {
                    _logger.LogWarning(
                        "RecuperacaoSenha: envio automático falhou para '{Email}'. Motivo: {Mensagem}",
                        Email,
                        resultado.Mensagem);
                    MensagemErro = resultado.Mensagem;
                }
                else
                {
                    _logger.LogInformation(
                        "RecuperacaoSenha: envio automático concluído para '{Email}'.",
                        Email);
                    NotificarSucesso(resultado.Mensagem);
                    await RegistrarCodigoDevAsync(Email, "após envio automático");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RecuperacaoSenha: exceção no envio automático para '{Email}'.", Email);
                MensagemErro = MensagensSistema.Autenticacao.FalhaEnvioCodigo;
            }
        }

        return Page();
    }

    private async Task RegistrarCodigoDevAsync(string email, string contexto)
    {
        if (!_environment.IsDevelopment())
            return;

        var codigo = await _codigoRepo.ObterCodigoPendenteMaisRecenteAsync(email, "recuperacao_senha");
        if (string.IsNullOrEmpty(codigo))
        {
            _logger.LogWarning("[DEV] RecuperacaoSenha ({Contexto}): nenhum código pendente para '{Email}'.", contexto, email);
            return;
        }

        _logger.LogWarning("[DEV] RecuperacaoSenha ({Contexto}): código ativo para '{Email}' = {Codigo}", contexto, email, codigo);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var requisicaoAjax = RequisicaoAjax();
        Email = Email.Trim().ToLowerInvariant();

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Tentativa de redefinição de senha bloqueada para '{Email}'. Falha na validação do formulário.", Email);
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
            var comando = new RedefinirSenhaCommand
            {
                Email = Email,
                Codigo = Codigo,
                NovaSenha = NovaSenha,
                ConfirmarNovaSenha = ConfirmarNovaSenha,
                AgenteUsuario = Request.Headers["User-Agent"].ToString(),
                EnderecoIp = Request.ObterIpCliente()
            };

            var resultado = await _mediator.Send(comando);

            if (!resultado.Sucesso)
            {
                _logger.LogWarning("Falha ao redefinir a senha para '{Email}'. Motivo: {Mensagem}", Email, resultado.Mensagem);
                if (requisicaoAjax)
                    return ResponderErroAjax(resultado.Mensagem ?? "Não foi possível redefinir a senha.", StatusCodes.Status400BadRequest);

                MensagemErro = resultado.Mensagem;
                return Page();
            }

            _logger.LogInformation("Senha redefinida com sucesso para '{Email}'.", Email);
            NotificarSucesso(resultado.Mensagem);

            if (requisicaoAjax)
                return ModuloRotas.JsonSucesso(ModuloRotas.Autenticacao.Login);

            return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado durante a redefinição de senha para '{Email}'.", Email);
            
            var mensagemErroInterno = "Ocorreu um erro interno no servidor ao tentar redefinir a senha.";
            if (requisicaoAjax)
                return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

            MensagemErro = mensagemErroInterno;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostReenviarCodigoAsync()
    {
        var requisicaoAjax = RequisicaoAjax();

        if (string.IsNullOrWhiteSpace(Email))
        {
            _logger.LogWarning("Tentativa de reenvio de código falhou. E-mail não informado.");
            if (requisicaoAjax)
                return ResponderErroAjax("Por favor, informe seu e-mail.");

            MensagemErro = MensagensSistema.Autenticacao.InformeEmail;
            return Page();
        }

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
                _logger.LogWarning("RecuperacaoSenha: reenvio falhou para '{Email}'. Motivo: {Mensagem}", Email, resultado.Mensagem);
                if (requisicaoAjax)
                    return ResponderErroAjax(resultado.Mensagem ?? "Não foi possível reenviar o código.", StatusCodes.Status400BadRequest);

                MensagemErro = resultado.Mensagem;
                return Page();
            }

            _logger.LogInformation("RecuperacaoSenha: código reenviado para '{Email}'.", Email);
            await RegistrarCodigoDevAsync(Email, "após reenvio manual");

            if (requisicaoAjax)
                return ModuloRotas.JsonSucessoMensagem("Código reenviado com sucesso!");
            NotificarSucesso("Novo código reenviado com sucesso!");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado ao reenviar código de recuperação de senha para '{Email}'.", Email);
            
            var mensagemErroInterno = "Ocorreu um erro interno ao tentar reenviar o código.";
            if (requisicaoAjax)
                return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

            MensagemErro = mensagemErroInterno;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostValidarCodigoPrevioAsync(string email, string codigo)
    {
        var requisicaoAjax = RequisicaoAjax();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(codigo))
        {
            if (requisicaoAjax)
                return ResponderErroAjax("E-mail e código são obrigatórios.");
            return Page();
        }

        try
        {
            var existe = await _codigoRepo.ExisteCodigoValidoAsync(email, codigo, "recuperacao_senha");
            if (!existe)
            {
                if (requisicaoAjax)
                    return ResponderErroAjax("Código inválido ou expirado. Verifique e tente novamente.");
                return Page();
            }

            if (requisicaoAjax)
                return ModuloRotas.JsonSucessoMensagem("Código verificado com sucesso.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico inesperado durante a pré-validação do código de recuperação para '{Email}'.", email);
            
            var mensagemErroInterno = "Ocorreu um erro interno no servidor ao tentar validar o código.";
            if (requisicaoAjax)
                return ResponderErroAjax(mensagemErroInterno, StatusCodes.Status500InternalServerError);

            return Page();
        }
    }
}
