using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string dir = @""c:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages"";
        var files = Directory.GetFiles(dir, ""*.*"", SearchOption.AllDirectories)
                             .Where(f => f.EndsWith("".cshtml"") || f.EndsWith("".cs""));
        
        char fffd = '\uFFFD';
        int totalFiles = 0;
        int totalLines = 0;

        foreach (var file in files)
        {
            string[] lines = File.ReadAllLines(file);
            bool fileHasIssues = false;
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(fffd))
                {
                    if (!fileHasIssues) { Console.WriteLine(""\n--- "" + Path.GetFileName(file) + "" ---""); fileHasIssues = true; }
                    Console.WriteLine($""[{i+1}] {lines[i].Trim()}"");
                    totalLines++;
                }
            }
            if (fileHasIssues) totalFiles++;
        }
        Console.WriteLine($""\nTotal arquivos: {totalFiles}, Total linhas: {totalLines}"");
    }
}
