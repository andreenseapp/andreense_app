using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

var connectionString = "Server=aws-1-sa-east-1.pooler.supabase.com;Port=6543;Database=postgres;User Id=postgres.jmutxqausafvqplervtw;Password=Andreense@Project#2026;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;No Reset On Close=true;";

var scriptPath = @"c:\Projeto\Projeto\Scripts\curriculos_migration.sql";
var sql = await System.IO.File.ReadAllTextAsync(scriptPath);

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

await using var command = new NpgsqlCommand(sql, connection);
await command.ExecuteNonQueryAsync();

Console.WriteLine("Tabelas do módulo de Currículos criadas com sucesso no banco de dados!");

