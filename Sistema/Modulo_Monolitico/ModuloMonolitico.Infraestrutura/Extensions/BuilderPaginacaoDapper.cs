namespace ModuloMonolitico.Infraestrutura.Extensions;

public static class BuilderPaginacaoDapper
{
    public static string ConstruirQuery(string queryBase, string? colunaOrdenacao, bool descendente, int limite, int offset)
    {
        var colSegura = !string.IsNullOrWhiteSpace(colunaOrdenacao) && System.Text.RegularExpressions.Regex.IsMatch(colunaOrdenacao, @"^[a-zA-Z0-9_]+$")
            ? colunaOrdenacao
            : "1"; 

        var direcao = descendente ? "DESC" : "ASC";
        return $"{queryBase} ORDER BY {colSegura} {direcao} LIMIT {limite} OFFSET {offset}";
    }
}
