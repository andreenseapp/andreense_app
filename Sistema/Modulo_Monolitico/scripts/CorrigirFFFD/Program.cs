using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var root = @"C:\Projeto\Projeto\Sistema\Modulo_Monolitico";
var utf8Bom = new UTF8Encoding(true);
const char R = '\uFFFD';

(string from, string to)[] map =
[
    ("ATEN" + R + R + "O", "ATENÇÃO"),
    ("a" + R + R + "o", "ação"),
    ("A" + R + R + "es", "Ações"),
    ("n" + R + "o", "não"),
    ("Pagina" + R + R + "o", "Paginação"),
    ("Navega" + R + R + "o", "Navegação"),
    ("confirma" + R + R + "o", "confirmação"),
    ("notifica" + R + R + "es", "notificações"),
    ("Observa" + R + R + "es", "Observações"),
    ("Informa" + R + R + "es", "Informações"),
    ("Integra" + R + R + "o", "Integração"),
    ("expira" + R + R + "o", "expiração"),
    ("Posi" + R + R + "o", "Posição"),
    ("VARIA" + R + R + "O", "VARIAÇÃO"),
    ("Varia" + R + R + "o", "Variação"),
    ("Fabrica" + R + R + "o", "Fabricação"),
    ("Observa" + R + R + "o", "Observação"),
    ("Devolu" + R + R + "o", "Devolução"),
    ("manuten" + R + R + "es", "manutenções"),
    ("Manuten" + R + R + "o", "Manutenção"),
    ("dura" + R + R + "o", "duração"),
    ("Funcion" + R + "rios", "Funcionários"),
    ("Funcion" + R + "rio", "Funcionário"),
    ("Refer" + R + "ncia", "Referência"),
    ("benef" + R + "cios", "benefícios"),
    ("Servi" + R + "os", "Serviços"),
    ("Servi" + R + "o", "Serviço"),
    ("Usu" + R + "rios", "Usuários"),
    ("Usu" + R + "rio", "Usuário"),
    ("usu" + R + "rio", "usuário"),
    ("Sess" + R + "o", "Sessão"),
    ("sess" + R + "es", "sessões"),
    ("An" + R + "lise", "Análise"),
    ("Hist" + R + "rico", "Histórico"),
    ("p" + R + "gina", "página"),
    ("Pr" + R + "xima", "Próxima"),
    ("Pr" + R + "ximo", "Próximo"),
    ("Gen" + R + "rico", "Genérico"),
    ("poder" + R, "poderá"),
    ("ir" + R, "irá"),
    ("receber" + R, "receberá"),
    ("voltar" + R, "voltará"),
    ("ser" + R, "será"),
    ("est" + R, "está"),
    ("s" + R + "o", "são"),
    ("agend" + R + "veis", "agendáveis"),
    ("Hor" + R + "rios", "Horários"),
    ("hor" + R + "rios", "horários"),
    ("Hor" + R + "rio", "Horário"),
    ("hor" + R + "rio", "horário"),
    ("dispon" + R + "vel", "disponível"),
    ("Calend" + R + "rio", "Calendário"),
    ("Cabe" + R + "alho", "Cabeçalho"),
    ("navega" + R + R + "o", "navegação"),
    ("conex" + R + "o", "conexão"),
    ("Bot" + R + "o", "Botão"),
    ("P" + R + "nico", "Pânico"),
    ("F" + R + "sica", "Física"),
    ("Jur" + R + "dica", "Jurídica"),
    ("Voc" + R, "Você"),
    ("m" + R + "s", "mês"),
    ("M" + R + "s", "Mês"),
    ("Ter" + R + "a", "Terça"),
    ("S" + R + "b", "Sáb"),
    ("m" + R + "dulo", "módulo"),
    ("Pre" + R + "o", "Preço"),
    ("v" + R + "nculos", "vínculos"),
    ("V" + R + "nculos", "Vínculos"),
    ("per" + R + "odos", "períodos"),
    ("Per" + R + "odo", "Período"),
    ("f" + R + "rias", "férias"),
    ("F" + R + "rias", "Férias"),
    ("espec" + R + "fico", "específico"),
    ("espec" + R + "ficos", "específicos"),
    ("pr" + R + "-selecionada", "pré-selecionada"),
    ("pr" + R + "ximos", "próximos"),
    ("at" + R, "até"),
    ("In" + R + "cio", "Início"),
    ("Vis" + R + "vel", "Visível"),
    ("H" + R + " ", "Há "),
    (R + "ltimos", "Últimos"),
    (R + "ltimo", "Último"),
    ("1" + R, "1º"),
    (" " + R + " Andreense", " — Andreense"),
    (" " + R + " ", " — "),
    ("@" + R, "@ —"),
    ("\"" + R + " Selecione", "\"— Selecione"),
    ("?? \"" + R + "\"", "?? \"—\""),
    ("HOR" + R + "RIOS", "HORÁRIOS"),
    ("SERVI" + R + "OS", "SERVIÇOS"),
    ("N" + R + " do", "Nº do"),
    ("N" + R + "mero", "Número"),
];

var exts = new[] { ".cs", ".cshtml", ".css", ".js", ".md" };
var fixedCount = 0;

foreach (var file in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
{
    if (!exts.Any(e => file.EndsWith(e, StringComparison.OrdinalIgnoreCase))) continue;
    if (file.Contains(@"\bin\") || file.Contains(@"\obj\")) continue;

    var bytes = File.ReadAllBytes(file);
    var skip = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
    var content = Encoding.UTF8.GetString(bytes, skip, bytes.Length - skip);
    if (!content.Contains(R)) continue;

    var original = content;
    foreach (var (from, to) in map)
        content = content.Replace(from, to);

    content = content.Replace("ClienteNome ?? \"" + R + "\"", "ClienteNome ?? \"—\"");
    content = content.Replace("Email ?? \"" + R + "\"", "Email ?? \"—\"");
    content = content.Replace("Motivo ?? \"" + R + "\"", "Motivo ?? \"—\"");

    if (content != original)
    {
        File.WriteAllText(file, content, utf8Bom);
        Console.WriteLine("[OK] " + Path.GetRelativePath(root, file));
        fixedCount++;
    }
    else
    {
        Console.WriteLine("[?] " + Path.GetRelativePath(root, file));
    }
}

Console.WriteLine("Corrigidos: " + fixedCount);
