using System.Linq.Expressions;

namespace ModuloMonolitico.Infraestrutura.Extensions;

public static class ExtensoesOrdenacaoEF
{
    public static IQueryable<T> AplicarOrdenacao<T>(this IQueryable<T> source, string? propriedadeOrdenacao, bool descendente)
    {
        if (string.IsNullOrWhiteSpace(propriedadeOrdenacao))
            return source;

        var tipo = typeof(T);
        var propriedade = tipo.GetProperty(propriedadeOrdenacao, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (propriedade == null)
            return source; 

        var parametro = Expression.Parameter(tipo, "p");
        var acessoPropriedade = Expression.MakeMemberAccess(parametro, propriedade);
        var expressaoOrdenacao = Expression.Lambda(acessoPropriedade, parametro);

        var nomeMetodo = descendente ? "OrderByDescending" : "OrderBy";
        
        var expressaoResultado = Expression.Call(
            typeof(Queryable),
            nomeMetodo,
            [tipo, propriedade.PropertyType],
            source.Expression,
            Expression.Quote(expressaoOrdenacao)
        );

        return source.Provider.CreateQuery<T>(expressaoResultado);
    }
}
