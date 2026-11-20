using System;
using Npgsql;
class Program {
    static void Main() {
        using var conn = new NpgsqlConnection("Server=aws-1-sa-east-1.pooler.supabase.com;Port=6543;Database=postgres;User Id=postgres.jmutxqausafvqplervtw;Password=Andreense@Project#2026;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;No Reset On Close=true;");
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM imobiliaria_anuncios;", conn);
        Console.WriteLine(cmd.ExecuteScalar());
    }
}
