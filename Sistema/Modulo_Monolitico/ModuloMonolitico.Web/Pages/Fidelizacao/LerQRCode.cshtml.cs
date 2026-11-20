using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Fidelizacao;

public class LerQRCodeModel : BaseScannerPageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<LerQRCodeModel> _logger;

    public LerQRCodeModel(IMediator mediator, ILogger<LerQRCodeModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override string TituloPagina => "Escanear Benefício";
    public override string SubtituloPagina => "Aponte para o código de Cashback ou Fidelidade.";

    public override async Task<IActionResult> OnPostProcessarCodigoAsync(string payload)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Leitura de QR Code rejeitada: payload vazio. Usuário '{Email}'.", UsuarioEmail);
            return new JsonResult(new { sucesso = false, mensagem = "Código inválido." });
        }

        if (!ChavesSistema.ExtrairTipoETokenQrCode(payload, out var tipo, out var token))
        {
            _logger.LogWarning("Leitura de QR Code rejeitada: formato inválido. Usuário '{Email}'.", UsuarioEmail);
            return new JsonResult(new { sucesso = false, mensagem = "Este QR Code não é válido no sistema Andreense." });
        }

        if (tipo == "cashback")
            return await ProcessarCashback(token);

        if (tipo == "fidelidade")
            return await ProcessarFidelidade(token);

        _logger.LogWarning("Leitura de QR Code rejeitada: tipo desconhecido '{Tipo}'. Usuário '{Email}'.", tipo, UsuarioEmail);
        return new JsonResult(new { sucesso = false, mensagem = "Tipo de benefício desconhecido." });
    }

    private async Task<IActionResult> ProcessarCashback(string token)
    {
        var resultado = await _mediator.Send(new ConsumirCodigoCashbackCommand { Codigo = token, UsuarioId = UsuarioId });
        if (resultado.Sucesso)
        {
            _logger.LogInformation("Cashback consumido via QR Code pelo usuário '{Email}'.", UsuarioEmail);
            var valorCredito = resultado.Dados;
            return new JsonResult(new
            {
                sucesso = true,
                mensagem = resultado.Mensagem,
                tipo = "cashback",
                valorCredito = valorCredito
            });
        }

        _logger.LogWarning("Falha ao consumir cashback via QR Code pelo usuário '{Email}': {Mensagem}", UsuarioEmail, resultado.Mensagem);
        return new JsonResult(new { sucesso = false, mensagem = resultado.Mensagem });
    }

    private async Task<IActionResult> ProcessarFidelidade(string token)
    {
        var resultado = await _mediator.Send(new ConsumirCodigoFidelidadeCommand { Codigo = token, UsuarioId = UsuarioId });
        if (resultado.Sucesso && resultado.Dados != null)
        {
            _logger.LogInformation("Fidelidade consumida via QR Code pelo usuário '{Email}'.", UsuarioEmail);
            var saldo = resultado.Dados;
            bool completou = saldo.RecompensasAcumuladas > 0 && saldo.PontosAcumulados == 0;
            return new JsonResult(new
            {
                sucesso = true,
                mensagem = resultado.Mensagem,
                tipo = "fidelidade",
                tituloCartao = saldo.TituloCartao,
                pontosAcumulados = saldo.PontosAcumulados,
                pontosNecessarios = saldo.PontosNecessarios,
                recompensasAcumuladas = saldo.RecompensasAcumuladas,
                recompensa = saldo.Recompensa,
                completouCartao = completou
            });
        }

        _logger.LogWarning("Falha ao consumir fidelidade via QR Code pelo usuário '{Email}': {Mensagem}", UsuarioEmail, resultado.Mensagem);
        return new JsonResult(new { sucesso = false, mensagem = resultado.Mensagem });
    }
}
