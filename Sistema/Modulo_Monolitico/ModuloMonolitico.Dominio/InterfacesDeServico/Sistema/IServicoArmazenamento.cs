using System.IO;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public interface IServicoArmazenamento
{
    Task<string> EnviarArquivoAsync(Stream stream, string nomeArquivo, string tipoConteudo);
    Task DeletarFisicoArquivoAsync(string nomeArquivo);
    string ObterUrlPublica(string nomeArquivo);
}
