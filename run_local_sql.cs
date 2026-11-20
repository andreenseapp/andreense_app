using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main()
    {
        string[] senhas = { "501203357aA@", "postgres", "admin", "root", "123456", "" };
        string sqlVeiculos = File.Exists(@"C:\Projeto\Projeto\veiculos_script_local.sql") ? File.ReadAllText(@"C:\Projeto\Projeto\veiculos_script_local.sql") : "";
        string sqlFix = File.Exists(@"C:\Projeto\Projeto\fix_status.sql") ? File.ReadAllText(@"C:\Projeto\Projeto\fix_status.sql") : "";
        
        bool success = false;
        
        foreach (var senha in senhas)
        {
            string connStr = $"Host=localhost;Port=5432;Database=erp_db;Username=postgres;Password={senha}";
            try
            {
                await using var conn = new NpgsqlConnection(connStr);
                await conn.OpenAsync();
                Console.WriteLine($"Conectado com sucesso usando a senha: {(string.IsNullOrEmpty(senha) ? "vazia" : "***")}");
                
                if (!string.IsNullOrEmpty(sqlVeiculos))
                {
                    try {
                        await using var cmd1 = new NpgsqlCommand(sqlVeiculos, conn);
                        await cmd1.ExecuteNonQueryAsync();
                        Console.WriteLine("Tabelas base de Veículos criadas no localhost com sucesso.");
                    } catch (Exception ex1) {
                        Console.WriteLine("Erro ao rodar script base (as tabelas já podem existir): " + ex1.Message);
                    }
                }
                
                if (!string.IsNullOrEmpty(sqlFix))
                {
                    try {
                        await using var cmd2 = new NpgsqlCommand(sqlFix, conn);
                        await cmd2.ExecuteNonQueryAsync();
                        Console.WriteLine("Fix do status (TEXT) aplicado no localhost com sucesso.");
                    } catch (Exception ex2) {
                        Console.WriteLine("Erro ao rodar script de fix: " + ex2.Message);
                    }
                }
                
                success = true;
                break;
            }
            catch (Exception)
            {
                // Ignora erro de login e tenta a próxima
            }
        }
        
        if (!success)
        {
            Console.WriteLine("FALHA: Nenhuma das senhas padrão funcionou para acessar o localhost.");
        }
    }
}
