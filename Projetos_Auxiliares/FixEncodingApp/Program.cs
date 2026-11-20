using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// FixEncodingApp v4
// Estratégia correta: ler o arquivo como UTF-8 (que interpreta os bytes
// corretamente), então substituir as strings Unicode corrompidas que
// aparecem no .NET string space, e gravar de volta com BOM.
//
// Exemplos de corrupção visível (em .NET string após leitura UTF-8):
//   "â€""   -> "–"  (en-dash, U+2013)
//   "ðŸ'‹"  -> "👋"  (emoji)
//   "âœ""   -> "✔"  (checkmark)
//   "â"€"   -> "─"  (box drawing)

class Program
{
    static void Main()
    {
        // Necessário para usar encodings legados como Windows-1252 no .NET moderno
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string path = @"C:\Projeto\Projeto";
        Console.WriteLine($"FixEncodingApp v4 — iniciando em: {path}");

        Encoding utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

        // Todas as substituições são de strings .NET (Unicode) para strings .NET.
        // O padrão corrompido é: bytes UTF-8 do char correto foram lidos como Windows-1252,
        // gerando uma nova string UTF-8 com os byte values como code points separados.
        // Ex: U+2013 (–) = bytes E2 80 93.
        //     Lido como Windows-1252: E2='â', 80=€ (cp1252), 93=\u0093 (ctrl, mapeado p/ U+201C no cp1252)
        //     Então aparece como "â€"" (3 chars: U+00E2, U+20AC, U+201C) ou variações.
        //
        // Para CADA char unicode que queremos, derivamos a string corrompida assim:
        //   bytes = UTF8.GetBytes(char)
        //   corrompido = Windows1252.GetString(bytes)

        var cp1252 = Encoding.GetEncoding(1252);

        // Função auxiliar
        string Corrupt(string s) => cp1252.GetString(Encoding.UTF8.GetBytes(s));

        var replacements = new Dictionary<string, string>();

        // Acrescentar ao dicionário usando a função
        void Add(string correct) => replacements[Corrupt(correct)] = correct;

        // Vogais e consoantes com acento (latim)
        foreach (var c in "áâãçéêíóôõúüÁÇÉÊÓÔÕÚÍÀòÃÕ")
            Add(c.ToString());

        // Símbolos de pontuação tipográfica
        Add("–");    // en dash U+2013
        Add("—");    // em dash U+2014
        Add("\u201C"); // "
        Add("\u201D"); // "
        Add("\u2018"); // '
        Add("\u2019"); // '
        Add("…");    // ellipsis U+2026
        Add("·");    // middle dot U+00B7
        Add("✔");    // checkmark U+2714
        Add("✓");    // check U+2713
        Add("✗");    // cross U+2717
        Add("✨");   // sparkles U+2728
        Add("⚠");   // warning U+26A0
        Add("←");   // left arrow U+2190
        Add("→");   // right arrow U+2192
        Add("↓");   // down arrow U+2193
        Add("↑");   // up arrow U+2191
        Add("─");   // box drawing U+2500
        Add("│");   // box drawing U+2502
        Add("✔");   // checkmark
        Add("®");   // registered
        Add("©");   // copyright
        Add("™");   // trademark
        Add("°");   // degree
        Add("±");   // plus-minus
        Add("×");   // multiplication
        Add("÷");   // division
        Add("½");   // half
        Add("¼");   // quarter
        Add("¾");   // three-quarters

        // Emojis comuns (UTF-8 de 4 bytes, corrompem em 4 chars cp1252)
        Add("👋");  // U+1F44B
        Add("👍");  // U+1F44D
        Add("👎");  // U+1F44E
        Add("👀");  // U+1F440
        Add("👉");  // U+1F449
        Add("👌");  // U+1F44C
        Add("🔔");  // U+1F514
        Add("🔒");  // U+1F512
        Add("🔓");  // U+1F513
        Add("🔥");  // U+1F525
        Add("🔎");  // U+1F50E
        Add("🔄");  // U+1F504
        Add("🚀");  // U+1F680
        Add("🏠");  // U+1F3E0
        Add("📦");  // U+1F4E6
        Add("📋");  // U+1F4CB
        Add("📝");  // U+1F4DD
        Add("💡");  // U+1F4A1
        Add("💬");  // U+1F4AC
        Add("💳");  // U+1F4B3
        Add("💙");  // U+1F499
        Add("💚");  // U+1F49A
        Add("💜");  // U+1F49C
        Add("🌟");  // U+1F31F
        Add("🌐");  // U+1F310
        Add("🌍");  // U+1F30D
        Add("🎯");  // U+1F3AF
        Add("🎉");  // U+1F389
        Add("🤖");  // U+1F916
        Add("✅");  // U+2705
        Add("❌");  // U+274C
        Add("⭐");  // U+2B50

        // Substituições residuais por U+FFFD
        var fffdFixes = new List<(string From, string To)>
        {
            ("Gest\uFFFDo",     "Gestão"),
            ("gest\uFFFDo",     "gestão"),
            ("avan\uFFFDada",   "avançada"),
            ("emiss\uFFFDo",    "emissão"),
            ("r\uFFFDpidas",    "rápidas"),
            ("N\uFFFDo",        "Não"),
            ("n\uFFFDo",        "não"),
            ("opera\uFFFD\uFFFDo",    "operação"),
            ("opera\uFFFD\uFFFDes",   "operações"),
            ("sele\uFFFD\uFFFDo",     "seleção"),
            ("informa\uFFFD\uFFFDo",  "informação"),
            ("informa\uFFFD\uFFFDes", "informações"),
            ("aten\uFFFD\uFFFDo",     "atenção"),
            ("cria\uFFFD\uFFFDo",     "criação"),
            ("configura\uFFFD\uFFFDo","configuração"),
            ("configura\uFFFD\uFFFDes","configurações"),
            ("integra\uFFFD\uFFFDo",  "integração"),
            ("Aplica\uFFFD",          "Aplicaç"),
            ("Gesto avanada, emisso de NFs", "Gestão avançada, emissão de NFs"),
            ("Gesto avanada", "Gestão avançada"),
            ("emisso de NFs", "emissão de NFs"),
            ("vendas rpidas", "vendas rápidas"),
        };

        var extensions = new[] { ".cs", ".cshtml", ".css", ".js", ".md", ".txt" };
        int count = 0, total = 0;

        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
            bool validExt = false;
            foreach (var ext in extensions)
                if (file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) { validExt = true; break; }
            if (!validExt) continue;
            if (file.Contains(@"\bin\") || file.Contains(@"\obj\") || file.Contains("FixEncodingApp")) continue;

            total++;
            try
            {
                byte[] rawBytes = File.ReadAllBytes(file);
                bool hasBom = rawBytes.Length >= 3 && rawBytes[0] == 0xEF && rawBytes[1] == 0xBB && rawBytes[2] == 0xBF;
                int skip = hasBom ? 3 : 0;
                string content = Encoding.UTF8.GetString(rawBytes, skip, rawBytes.Length - skip);
                string original = content;

                foreach (var kv in replacements)
                    content = content.Replace(kv.Key, kv.Value);

                foreach (var (from, to) in fffdFixes)
                    content = content.Replace(from, to);

                byte[] bomBytes = new byte[] { 0xEF, 0xBB, 0xBF };
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                byte[] final = new byte[3 + contentBytes.Length];
                Buffer.BlockCopy(bomBytes, 0, final, 0, 3);
                Buffer.BlockCopy(contentBytes, 0, final, 3, contentBytes.Length);

                File.WriteAllBytes(file, final);

                if (content != original)
                {
                    Console.WriteLine($"[CORRIGIDO] {Path.GetRelativePath(path, file)}");
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        Console.WriteLine($"\nConcluído: {count}/{total} arquivo(s) corrigido(s).");
    }
}
