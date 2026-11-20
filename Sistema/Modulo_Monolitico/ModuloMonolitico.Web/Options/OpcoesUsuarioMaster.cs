namespace ModuloMonolitico.Web.Options;

public class OpcoesUsuarioMaster
{
    public const string Secao = "Sistema:UsuarioMaster";

    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
}
