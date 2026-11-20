using System;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main()
    {
        string connStr = "Server=aws-1-sa-east-1.pooler.supabase.com;Port=6543;Database=postgres;User Id=postgres.jmutxqausafvqplervtw;Password=Andreense@Project#2026;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;No Reset On Close=true;";
        try
        {
            await using var conn = new NpgsqlConnection(connStr);
            await conn.OpenAsync();
            
            await using var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_name LIKE 'veiculos%';", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            Console.WriteLine("Tables found:");
            while (await reader.ReadAsync())
            {
                Console.WriteLine("- " + reader.GetString(0));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
