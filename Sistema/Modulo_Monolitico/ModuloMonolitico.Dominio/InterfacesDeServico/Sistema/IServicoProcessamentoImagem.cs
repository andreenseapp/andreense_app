using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public interface IServicoProcessamentoImagem
{
    Task<(byte[] Bytes, string TipoConteudo, string Extensao)> OtimizarParaWebAsync(byte[] bytesOriginais, int larguraMaxima = 1920);
}
