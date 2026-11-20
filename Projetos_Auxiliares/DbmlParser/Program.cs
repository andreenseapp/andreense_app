using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DbmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbmlPath = @"c:\Projeto\Projeto\Documentação\Manifestos\Manifesto_DBML.md";
            string outPath = @"c:\Projeto\Projeto\Scripts\banco_script_completo.sql";

            if (!File.Exists(dbmlPath))
            {
                Console.WriteLine("Arquivo DBML não encontrado.");
                return;
            }

            var lines = File.ReadAllLines(dbmlPath);
            var sql = new List<string>();

            sql.Add("-- Script de Inicialização - ERP DB");
            sql.Add("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
            sql.Add("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\";");
            sql.Add("CREATE SCHEMA IF NOT EXISTS erp;");
            sql.Add("CREATE SCHEMA IF NOT EXISTS servicos;");
            sql.Add("CREATE SCHEMA IF NOT EXISTS cliente;");
            sql.Add("");
            
            sql.Add(@"CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.data_ultima_atualizacao = now();
    RETURN NEW;
END;
$$ language 'plpgsql';");
            sql.Add("");

            var enums = new Dictionary<string, List<string>>();
            var tables = new Dictionary<string, List<ColumnDef>>();
            var refs = new List<RefDef>();
            var sequences = new HashSet<string>();

            string currentEnum = null;
            string currentTable = null;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("//") || line.StartsWith("#") || line.StartsWith(">"))
                    continue;

                var enumMatch = Regex.Match(line, @"Enum\s+(\w+)\s*\{?");
                if (enumMatch.Success)
                {
                    currentEnum = enumMatch.Groups[1].Value;
                    enums[currentEnum] = new List<string>();
                    continue;
                }

                var tableMatch = Regex.Match(line, @"Table\s+(\w+)\s*\{?");
                if (tableMatch.Success)
                {
                    currentTable = tableMatch.Groups[1].Value;
                    tables[currentTable] = new List<ColumnDef>();
                    continue;
                }

                var refMatch = Regex.Match(line, @"Ref:\s*([\w\.]+)\s*([><-])\s*([\w\.]+)");
                if (refMatch.Success)
                {
                    var p1 = refMatch.Groups[1].Value.Split('.');
                    var op = refMatch.Groups[2].Value;
                    var p2 = refMatch.Groups[3].Value.Split('.');
                    if (p1.Length == 2 && p2.Length == 2)
                    {
                        if (op == ">") refs.Add(new RefDef { FkTable = p1[0], FkCol = p1[1], RefTable = p2[0], RefCol = p2[1] });
                        else if (op == "<") refs.Add(new RefDef { FkTable = p2[0], FkCol = p2[1], RefTable = p1[0], RefCol = p1[1] });
                        else if (op == "-") refs.Add(new RefDef { FkTable = p1[0], FkCol = p1[1], RefTable = p2[0], RefCol = p2[1] });
                    }
                    continue;
                }

                if (line == "}" || line.StartsWith("}"))
                {
                    currentEnum = null;
                    currentTable = null;
                    continue;
                }

                if (currentEnum != null)
                {
                    if (line != "{") enums[currentEnum].Add(line.Trim());
                    continue;
                }

                if (currentTable != null)
                {
                    if (line == "{") continue;
                    var parts = line.Split(new[] { "//" }, 2, StringSplitOptions.None);
                    var defPart = parts[0].Trim();
                    if (defPart.EndsWith(",")) defPart = defPart.Substring(0, defPart.Length - 1).Trim();
                    
                    string attrs = "";
                    var attrMatch = Regex.Match(defPart, @"\[(.*?)\]$");
                    if (attrMatch.Success)
                    {
                        attrs = attrMatch.Groups[1].Value;
                        defPart = defPart.Substring(0, attrMatch.Index).Trim();
                    }

                    var colMatch = Regex.Match(defPart, @"^(\w+)\s+([\w\.\(\)]+)");
                    if (colMatch.Success)
                    {
                        tables[currentTable].Add(new ColumnDef
                        {
                            Name = colMatch.Groups[1].Value,
                            Type = colMatch.Groups[2].Value,
                            Attrs = attrs
                        });
                        
                        var seqMatch = Regex.Match(attrs, @"nextval\('public\.(\w+)'\)");
                        if(seqMatch.Success)
                        {
                            sequences.Add(seqMatch.Groups[1].Value);
                        }
                    }
                }
            }

            foreach(var seq in sequences)
            {
                sql.Add($"CREATE SEQUENCE IF NOT EXISTS public.{seq};");
            }
            sql.Add("");

            foreach (var kvp in enums)
            {
                var vals = string.Join(", ", kvp.Value.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => $"'{v}'"));
                sql.Add($"DROP TYPE IF EXISTS public.{kvp.Key} CASCADE;");
                sql.Add($"CREATE TYPE public.{kvp.Key} AS ENUM ({vals});");
            }
            sql.Add("");

            foreach (var kvp in tables)
            {
                var schema = DetermineSchema(kvp.Key);
                sql.Add($"CREATE TABLE IF NOT EXISTS {schema}.{kvp.Key} (");
                var colLines = new List<string>();
                bool hasUpdatedAt = false;

                foreach (var col in kvp.Value)
                {
                    string attrStr = "";
                    if (!string.IsNullOrEmpty(col.Attrs))
                    {
                        var attrs = col.Attrs.Split(',').Select(a => a.Trim());
                        foreach (var a in attrs)
                        {
                            if (a.Equals("primary key", StringComparison.OrdinalIgnoreCase)) attrStr += " PRIMARY KEY";
                            else if (a.Equals("not null", StringComparison.OrdinalIgnoreCase)) attrStr += " NOT NULL";
                            else if (a.Equals("unique", StringComparison.OrdinalIgnoreCase)) attrStr += " UNIQUE";
                            else if (a.StartsWith("default:", StringComparison.OrdinalIgnoreCase))
                            {
                                var defVal = a.Substring(8).Trim();
                                if (defVal.StartsWith("`") && defVal.EndsWith("`")) defVal = defVal.Substring(1, defVal.Length - 2);
                                attrStr += $" DEFAULT {defVal}";
                            }
                        }
                    }

                    var colType = col.Type;
                    if (enums.ContainsKey(colType)) colType = $"public.{colType}";

                    colLines.Add($"  {col.Name} {colType}{attrStr}");
                    
                    if (col.Name == "data_ultima_atualizacao") hasUpdatedAt = true;
                }

                sql.Add(string.Join(",\n", colLines));
                sql.Add(");");
                
                if(hasUpdatedAt)
                {
                    sql.Add($"DROP TRIGGER IF EXISTS update_{kvp.Key}_updated_at ON {schema}.{kvp.Key};");
                    sql.Add($"CREATE TRIGGER update_{kvp.Key}_updated_at BEFORE UPDATE ON {schema}.{kvp.Key} FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();");
                }
                
                sql.Add("");
            }

            foreach (var r in refs)
            {
                var fkSchema = DetermineSchema(r.FkTable);
                var refSchema = DetermineSchema(r.RefTable);
                if (tables.ContainsKey(r.FkTable) && tables.ContainsKey(r.RefTable))
                {
                    sql.Add($"ALTER TABLE {fkSchema}.{r.FkTable} DROP CONSTRAINT IF EXISTS fk_{r.FkTable}_{r.FkCol};");
                    sql.Add($"ALTER TABLE {fkSchema}.{r.FkTable} ADD CONSTRAINT fk_{r.FkTable}_{r.FkCol} FOREIGN KEY ({r.FkCol}) REFERENCES {refSchema}.{r.RefTable}({r.RefCol});");
                }
            }

            File.WriteAllLines(outPath, sql);
            Console.WriteLine($"SQL Gerado em {outPath}");
        }

        static string DetermineSchema(string table)
        {
            return "public"; // Everything public for simplicity unless requested
        }

        class ColumnDef { public string Name; public string Type; public string Attrs; }
        class RefDef { public string FkTable; public string FkCol; public string RefTable; public string RefCol; }
    }
}
