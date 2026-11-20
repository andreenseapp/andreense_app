using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public interface IServicoEmail
{
    Task<EmailEnvioResultado> EnviarEmailAsync(string para, string assunto, string corpo, bool ehHtml = true);
}
