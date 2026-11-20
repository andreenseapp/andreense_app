using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;

public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly time)
    {
        parameter.Value = time.ToTimeSpan();
    }

    public override TimeOnly Parse(object value)
    {
        if (value is TimeOnly timeOnly)
            return timeOnly;
        if (value is TimeSpan timeSpan)
            return TimeOnly.FromTimeSpan(timeSpan);
        if (value is DateTime dateTime)
            return TimeOnly.FromDateTime(dateTime);
        throw new InvalidCastException($"Cannot convert {value?.GetType()} to TimeOnly");
    }
}

public abstract class RepositorioBase
{
    static RepositorioBase()
    {
        SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
    }

    private readonly string _connectionString;

    protected RepositorioBase(string conexaoString)
    {
        _connectionString = conexaoString;
    }

    protected NpgsqlConnection CriarConexao() => new(_connectionString);
    protected async Task ExecutarEmTransacaoAsync(Func<NpgsqlConnection, Task> acao)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync("BEGIN;");

        try
        {
            await acao(conexao);
            await conexao.ExecuteAsync("COMMIT;");
        }
        catch
        {
            try { await conexao.ExecuteAsync("ROLLBACK;"); } catch { }
            throw;
        }
    }
    protected async Task<T> ExecutarEmTransacaoAsync<T>(Func<NpgsqlConnection, Task<T>> acao)
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        await conexao.ExecuteAsync("BEGIN;");

        try
        {
            var resultado = await acao(conexao);
            await conexao.ExecuteAsync("COMMIT;");
            return resultado;
        }
        catch
        {
            try { await conexao.ExecuteAsync("ROLLBACK;"); } catch { }
            throw;
        }
    }
}
