namespace ModuloMonolitico.Dominio.Servicos;

public record ResultadoEndereco(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Cidade,
    string Uf,
    bool Encontrado
)
{
    public static ResultadoEndereco NaoEncontrado =>
        new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false);
}
public interface IServicoEndereco
{
    Task<ResultadoEndereco> BuscarPorCepAsync(string cep);
}
