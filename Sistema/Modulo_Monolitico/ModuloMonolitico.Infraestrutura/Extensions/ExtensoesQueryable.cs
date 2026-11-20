using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Infraestrutura.Extensions;

public static class ExtensoesQueryable
{
    public static async Task<int> CountWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "CountAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }

    public static async Task<List<T>> ToListWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "ToListAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }
    public static async Task<T?> FirstOrDefaultWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "FirstOrDefaultAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }

    public static async Task<T?> SingleOrDefaultWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "SingleOrDefaultAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.SingleOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }

    public static async Task<T> FirstWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "FirstAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.FirstAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }

    public static async Task<bool> AnyWithLoggingAsync<T>(
        this IQueryable<T> query,
        ILogger logger,
        string operacao = "AnyAsync",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            string? sql = null;
            try
            {
                sql = query.ToQueryString();
            }
            catch (Exception sqlEx)
            {
                logger.LogError(sqlEx, "Não foi possível obter o SQL da consulta para a operação '{Operacao}'", operacao);
            }

            if (sql != null)
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'. SQL:\n{Sql}", operacao, sql);
            }
            else
            {
                logger.LogError(ex, "Erro no banco de dados (EF) durante '{Operacao}'", operacao);
            }

            throw;
        }
    }
}
