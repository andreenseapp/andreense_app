using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        var dir = @"c:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages";
        var files = Directory.GetFiles(dir, "*.cshtml", SearchOption.AllDirectories);
        int alterados = 0;

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            bool changed = false;

            // Procurar por tabelas não marcadas
            if (content.Contains("<table ") && !content.Contains("data-seletor-colunas"))
            {
                content = Regex.Replace(content, @"<table\s+(.*?)>", match => {
                    var inner = match.Groups[1].Value;
                    if (inner.Contains("app-table") || inner.Contains("table")) {
                        changed = true;
                        return $"<table {inner} data-seletor-colunas=\"true\">";
                    }
                    return match.Value;
                });
            }

            if (changed)
            {
                // Injetar script se não existir
                if (!content.Contains("seletor-colunas.js"))
                {
                    // Procurar a tag <link rel="stylesheet" ou </head> para injetar o script na head
                    if (content.Contains("</head>"))
                    {
                        content = content.Replace("</head>", "  <script src=\"~/Contextos/Shared/js/seletor-colunas.js\" asp-append-version=\"true\"></script>\r\n</head>");
                    }
                    else if (content.Contains("<link rel=\"stylesheet\" href=\"~/Contextos/Shared/css/design-system.css\""))
                    {
                        content = content.Replace("<link rel=\"stylesheet\" href=\"~/Contextos/Shared/css/design-system.css\"", "<script src=\"~/Contextos/Shared/js/seletor-colunas.js\" asp-append-version=\"true\"></script>\r\n  <link rel=\"stylesheet\" href=\"~/Contextos/Shared/css/design-system.css\"");
                    }
                }

                // Injetar data-coluna-fixa no TH de Ações
                content = Regex.Replace(content, @"<th([^>]*)>Ações<\/th>", "<th$1 data-coluna-fixa>Ações</th>", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<th([^>]*)>\s*Ações\s*<\/th>", "<th$1 data-coluna-fixa>Ações</th>", RegexOptions.IgnoreCase);
                
                // Injetar data-coluna-fixa no TH de Checkbox (se tiver chkSelecionarTodos)
                // Isso é complexo com regex simples, vamos deixar para as ações primariamente.

                File.WriteAllText(file, content);
                alterados++;
                Console.WriteLine($"Atualizado: {file}");
            }
        }
        
        Console.WriteLine($"Concluido! {alterados} arquivos modificados com sucesso.");
    }
}
