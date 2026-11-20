using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

var connectionString = "Server=aws-1-sa-east-1.pooler.supabase.com;Port=6543;Database=postgres;User Id=postgres.jmutxqausafvqplervtw;Password=Andreense@Project#2026;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;No Reset On Close=true;";

var sql = """
CREATE TABLE IF NOT EXISTS imobiliaria_perfis_anunciante (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conta_id UUID NOT NULL UNIQUE,
    nome_exibicao VARCHAR(200) NOT NULL,
    descricao TEXT,
    foto_logo_url TEXT,
    whatsapp VARCHAR(20),
    site_url TEXT,
    instagram VARCHAR(100),
    creci VARCHAR(50),
    data_inclusao TIMESTAMPTZ DEFAULT NOW(),
    data_atualizacao TIMESTAMPTZ DEFAULT NOW()
);
""";

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

await using var command = new NpgsqlCommand(sql, connection);
await command.ExecuteNonQueryAsync();

Console.WriteLine("Tabela imobiliaria_perfis_anunciante criada com sucesso!");
