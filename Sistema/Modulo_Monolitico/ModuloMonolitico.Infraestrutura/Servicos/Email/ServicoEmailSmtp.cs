using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Infraestrutura.Servicos.Email;

public class ServicoEmailSmtp : IServicoEmail
{
    private const string CategoriaConfiguracao = "ConfiguracaoAusente";
    private const string CategoriaAutenticacao = "AutenticacaoRemetente";
    private const string CategoriaConexao = "ConexaoServidor";
    private const string CategoriaServidor = "RejeicaoServidor";
    private const string CategoriaDesconhecida = "ErroDesconhecido";

    private readonly IConfiguration _configuration;
    private readonly ILogger<ServicoEmailSmtp> _logger;

    public ServicoEmailSmtp(IConfiguration configuration, ILogger<ServicoEmailSmtp> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<EmailEnvioResultado> EnviarEmailAsync(string para, string assunto, string corpo, bool ehHtml = true)
    {
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderPassword = _configuration["EmailSettings:SenderPassword"]?.Replace(" ", "");

        if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
        {
            var motivo = "EmailSettings:SenderEmail ou SenderPassword não configurados no appsettings.";
            _logger.LogError(
                "SmtpContaRemetente: CONFIGURAÇÃO AUSENTE — e-mail para '{Para}' NÃO enviado. {Motivo}",
                para,
                motivo);
            return EmailEnvioResultado.Falha(CategoriaConfiguracao, motivo);
        }

        _logger.LogInformation(
            "SmtpContaRemetente: tentando enviar de '{Remetente}' para '{Para}'. Assunto='{Assunto}'",
            senderEmail,
            para,
            assunto);

        try
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "Equipe Andreense"),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = ehHtml
            };

            mailMessage.To.Add(para);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation(
                "SmtpContaRemetente: SUCESSO — Gmail aceitou a mensagem. Remetente='{Remetente}' Destino='{Para}' Assunto='{Assunto}'",
                senderEmail,
                para,
                assunto);

            return EmailEnvioResultado.Entregue(senderEmail);
        }
        catch (Exception ex)
        {
            var (categoria, motivo, detalheLog) = ClassificarFalha(ex, senderEmail);

            _logger.LogError(
                ex,
                "SmtpContaRemetente: FALHA [{Categoria}] — conta remetente '{Remetente}' ou servidor SMTP com problema. " +
                "Destino='{Para}' Assunto='{Assunto}'. {Detalhe}",
                categoria,
                senderEmail,
                para,
                assunto,
                detalheLog);

            return EmailEnvioResultado.Falha(categoria, motivo, senderEmail);
        }
    }

    private static (string Categoria, string Motivo, string DetalheLog) ClassificarFalha(Exception ex, string remetente)
    {
        if (ex is SmtpException smtpEx)
        {
            var status = smtpEx.StatusCode.ToString();
            var msg = smtpEx.Message;

            if (msg.Contains("Authentication", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("535", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("534", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("credentials", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("not authenticated", StringComparison.OrdinalIgnoreCase))
            {
                return (
                    CategoriaAutenticacao,
                    "A conta remetente SMTP foi rejeitada (senha de app inválida, expirada ou 2FA não configurado no Gmail).",
                    $"Gmail recusou login da conta '{remetente}'. StatusCode={status}. Verifique EmailSettings:SenderPassword (senha de app).");
            }

            if (msg.Contains("Mailbox unavailable", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("550", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("553", StringComparison.OrdinalIgnoreCase))
            {
                return (
                    CategoriaServidor,
                    "O servidor SMTP rejeitou o envio (remetente ou destinatário inválido).",
                    $"StatusCode={status}. Mensagem={msg}");
            }

            return (
                CategoriaServidor,
                $"Erro SMTP: {msg}",
                $"StatusCode={status}.");
        }

        if (ex is System.Net.Sockets.SocketException or TimeoutException or IOException)
        {
            return (
                CategoriaConexao,
                "Não foi possível conectar ao servidor SMTP (smtp.gmail.com:587). Verifique rede/firewall.",
                ex.Message);
        }

        if (ex is InvalidOperationException)
        {
            return (
                CategoriaConfiguracao,
                ex.Message,
                "Configuração ou estado inválido do cliente SMTP.");
        }

        return (
            CategoriaDesconhecida,
            ex.Message,
            $"Tipo={ex.GetType().Name}");
    }
}
