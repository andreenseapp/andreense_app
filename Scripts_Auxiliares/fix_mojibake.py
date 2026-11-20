import os

replacements = {
    "Ã¡": "á",
    "Ã¢": "â",
    "Ã£": "ã",
    "Ã§": "ç",
    "Ã©": "é",
    "Ãª": "ê",
    "Ã­": "í",
    "Ã³": "ó",
    "Ã´": "ô",
    "Ãµ": "õ",
    "Ãº": "ú",
    "Ã ": "À",
    "Ã\x81": "Á",
    "Ã‡": "Ç",
    "Ã‰": "É",
    "ÃŠ": "Ê",
    "Ã“": "Ó",
    "Ã”": "Ô",
    "Ã•": "Õ",
    "Ãš": "Ú",
    "Ã\x8D": "Í",
    "Ãrea": "Área",
    "InformaÃ§Ãµes": "Informações",
    "pÃºblicas": "públicas",
    "operaÃ§Ãµes": "operações",
    "diÃ¡rias": "diárias",
    "GestÃ£o": "Gestão",
    "avanÃ§ada": "avançada",
    "emissÃ£o": "emissão",
    "cartÃµes": "cartões",
    "ServiÃ§os": "Serviços",
    "NotificaÃ§Ãµes": "Notificações",
    "notificaÃ§Ã£o": "notificação",
    "NotificaÃ§Ã£o": "Notificação",
    "VocÃª": "Você"
}

def fix_file(filepath):
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
            
        modified = False
        for k, v in replacements.items():
            if k in content:
                content = content.replace(k, v)
                modified = True
                
        if modified:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            print(f"Fixed: {filepath}")
    except Exception as e:
        print(f"Error {filepath}: {e}")

for root, _, files in os.walk(r'c:\Projeto\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\Pages'):
    for file in files:
        if file.endswith('.cshtml') or file.endswith('.cs'):
            fix_file(os.path.join(root, file))
