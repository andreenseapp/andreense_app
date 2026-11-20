namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public class EmailEnvioResultado
{
    public bool Sucesso { get; init; }

    public bool EntregueAoServidorSmtp { get; init; }

    public string? CategoriaFalha { get; init; }

    public string? MotivoFalha { get; init; }

    public string? Remetente { get; init; }

    public static EmailEnvioResultado Entregue(string remetente) => new()
    {
        Sucesso = true,
        EntregueAoServidorSmtp = true,
        Remetente = remetente
    };

    public static EmailEnvioResultado Falha(string categoria, string motivo, string? remetente = null) => new()
    {
        Sucesso = false,
        EntregueAoServidorSmtp = false,
        CategoriaFalha = categoria,
        MotivoFalha = motivo,
        Remetente = remetente
    };
}
