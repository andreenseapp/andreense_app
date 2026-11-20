namespace ModuloMonolitico.Dominio.Servicos;

public record DadosGoogleUsuario(
    string Email,
    string Nome,
    string? UrlFoto,
    string IdGoogle
);

public interface IServicoAutenticacaoGoogle
{
    Task<DadosGoogleUsuario?> ValidarTokenIdentidadeAsync(string tokenIdentidade);
    bool Habilitado { get; }
}
