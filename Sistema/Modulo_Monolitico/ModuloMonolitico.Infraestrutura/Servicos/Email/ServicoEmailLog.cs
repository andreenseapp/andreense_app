using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Infraestrutura.Servicos.Email;

public class ServicoEmailLog : IServicoEmail
{
    private readonly ILogger<ServicoEmailLog> _logger;

    public ServicoEmailLog(ILogger<ServicoEmailLog> logger)
    {
        _logger = logger;
    }

    public Task<EmailEnvioResultado> EnviarEmailAsync(string para, string assunto, string corpo, bool ehHtml = true)
    {
        _logger.LogInformation(
            "SmtpContaRemetente: SIMULADO (ServicoEmailLog) — Para={Para} Assunto={Assunto}",
            para,
            assunto);

        return Task.FromResult(EmailEnvioResultado.Entregue("log-email-service@simulado"));
    }
}
