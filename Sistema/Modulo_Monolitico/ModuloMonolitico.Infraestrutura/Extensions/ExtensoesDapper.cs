using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Infraestrutura.Extensions;

public static class ExtensoesDapper
{
    public static async Task<IEnumerable<T>> ConsultarComLogAsync<T>(
        this IDbConnection connection,
        string sql,
        object? param,
        ILogger logger,
        string operacao = "QueryAsync",
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        try
        {
            return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro no banco de dados (Dapper) durante '{Operacao}'. SQL:\n{Sql}\nParâmetros: {@Parametros}", 
                operacao, sql, param);
            throw;
        }
    }

    public static async Task<int> ExecutarComLogAsync(
        this IDbConnection connection,
        string sql,
        object? param,
        ILogger logger,
        string operacao = "ExecuteAsync",
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        try
        {
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro no banco de dados (Dapper) durante '{Operacao}'. SQL:\n{Sql}\nParâmetros: {@Parametros}", 
                operacao, sql, param);
            throw;
        }
    }
}
