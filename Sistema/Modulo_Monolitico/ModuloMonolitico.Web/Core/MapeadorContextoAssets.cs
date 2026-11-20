namespace ModuloMonolitico.Web.Core;

public static class MapeadorContextoAssets
{
    private static readonly Dictionary<string, string> AliasesModuloRaiz =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["CRM"] = "Contatos",
            ["AreaServicos"] = "Principal",
            ["Cidade"] = "Cidadania",
            ["MeuPerfil"] = "Perfil",
            ["MeuPerfilDesativar"] = "Perfil",
        };

    private static readonly HashSet<string> PastasIgnoradas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Shared",
            "Api",
            "Error",
        };

    public static string? ResolverPastaContexto(string? caminho)
    {
        if (string.IsNullOrWhiteSpace(caminho))
            return null;

        var segmentos = caminho.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segmentos.Length == 0)
            return null;

        string pasta;
        if (segmentos[0].Equals("Contextos", StringComparison.OrdinalIgnoreCase))
        {
            if (segmentos.Length < 2)
                return null;
            pasta = segmentos[1];
        }
        else
        {
            pasta = segmentos[0];
        }

        if (AliasesModuloRaiz.TryGetValue(pasta, out var alias))
            pasta = alias;

        if (PastasIgnoradas.Contains(pasta))
            return null;

        return pasta;
    }
}
