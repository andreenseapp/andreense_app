# Manifesto Oficial — Como Trabalhar (Guia Operacional de Infraestrutura)

> **Premissa**: Todo ajuste de infraestrutura, integração ou configuração de ambiente **deve ser rastreável**. Se alguém novo entrar na equipe amanhã, deve conseguir subir o projeto do zero lendo apenas esta documentação — sem precisar perguntar "onde fica?" ou "como faz?".

Este manifesto não trata de regras de código ou arquitetura (isso vive nos outros manifestos). Ele trata de **como operar, configurar e documentar** o ecossistema do projeto no dia a dia.

---

## 📁 1. A Pasta `/docs` (Documentação Viva Junto ao Projeto)

Todo projeto na solução **deve conter** uma pasta `docs/` na raiz, versionada no Git junto ao código. Ela é o "cofre de conhecimento operacional".

```
📁 MinhaSolucao/
├── 📁 src/
│   └── 📁 Apresentacao.Web/
├── 📁 docs/                          ← OBRIGATÓRIA
│   ├── 📁 configs/                   ← Cópias sanitizadas de arquivos de configuração
│   │   ├── appsettings.exemplo.json  ← Modelo sem segredos reais
│   │   ├── google-services.exemplo.json
│   │   └── supabase-config.exemplo.json
│   ├── 📁 integracoes/              ← Um .md por integração externa
│   │   ├── Supabase.md
│   │   ├── Google_Auth.md
│   │   ├── Email_SMTP.md
│   │   └── Sefaz_NFe.md
│   ├── 📁 ambientes/                ← Como cada ambiente funciona
│   │   ├── Local.md
│   │   ├── Homologacao.md
│   │   └── Producao.md
│   ├── 📁 decisoes/                 ← Registro de Decisões Arquiteturais (ADRs)
│   │   ├── 001_banco_local_vs_supabase.md
│   │   └── 002_autenticacao_google.md
│   └── SETUP.md                      ← Guia "do zero ao rodando" (passo a passo)
└── 📁 tests/
```

### Regras da pasta `/docs`:

- **Nunca** colocar segredos reais (senhas, tokens, chaves API). Usar `.exemplo` com placeholders.
- **Sempre** versionar no Git — documentação que não acompanha o código apodrece.
- **Atualizar** na mesma PR/commit que altera a integração. Documentação defasada é pior que nenhuma.

---

## 🔑 2. Gestão de Segredos e Configurações

### O Que Nunca Vai Pro Git

Arquivos com dados sensíveis **ficam no `.gitignore`** e são documentados na pasta `docs/configs/` com versões sanitizadas:

| Arquivo Real (ignorado) | Cópia Documentada (versionada) |
|---|---|
| `appsettings.json` | `docs/configs/appsettings.exemplo.json` |
| `appsettings.Production.json` | `docs/configs/appsettings.Production.exemplo.json` |
| `google-services.json` | `docs/configs/google-services.exemplo.json` |
| `.env` | `docs/configs/.env.exemplo` |
| Certificados `.pfx` | Apenas referência no `docs/integracoes/Certificado_Digital.md` |

### Modelo do Arquivo `.exemplo`

```json
{
  "ConnectionStrings": {
    "BancoPrincipal": "Host=SEU_HOST;Port=5432;Database=SEU_BANCO;Username=SEU_USER;Password=SUA_SENHA"
  },
  "Jwt": {
    "Secret": "SUA_CHAVE_SECRETA_AQUI_MINIMO_32_CHARS",
    "Issuer": "WebCidadeLegal",
    "Audience": "WebCidadeLegal"
  },
  "Email": {
    "Smtp": "smtp.SEU_PROVEDOR.com",
    "Porta": 587,
    "Usuario": "seu@email.com",
    "Senha": "SUA_SENHA_SMTP"
  },
  "Google": {
    "ClientId": "SEU_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "SEU_GOOGLE_CLIENT_SECRET"
  }
}
```

> ⚠️ **O arquivo `.exemplo` deve conter TODAS as chaves que o sistema usa**, com valores placeholder que indicam claramente o que preencher. Se um dev novo clonar o repo, ele copia o `.exemplo`, renomeia e preenche.

---

## 🔀 3. Chave de Ambiente (A "Chavinha" de Troca)

### Como o Sistema Sabe Onde Conectar

O .NET já possui mecanismo nativo de ambientes. O sistema **nunca hardcoda** a string de conexão ou provider:

```
📁 Apresentacao.Web/
├── appsettings.json                    ← Configs compartilhadas (sem segredos)
├── appsettings.Development.json        ← PostgreSQL local (dev)
├── appsettings.Staging.json            ← Supabase de homologação
└── appsettings.Production.json         ← Supabase de produção
```

**A variável de ambiente `ASPNETCORE_ENVIRONMENT`** é a "chavinha":

| Valor | Conecta em | Arquivo lido |
|---|---|---|
| `Development` | PostgreSQL local (localhost:5432) | `appsettings.Development.json` |
| `Staging` | Supabase de homologação | `appsettings.Staging.json` |
| `Production` | Supabase de produção | `appsettings.Production.json` |

### Documentar no `docs/ambientes/Local.md`:

```markdown
# Ambiente Local (Development)

## Pré-requisitos
- PostgreSQL 16+ instalado localmente
- Porta 5432 liberada

## Como configurar
1. Copie `docs/configs/appsettings.exemplo.json` para `src/Apresentacao.Web/appsettings.Development.json`
2. Preencha `ConnectionStrings:BancoPrincipal` com seus dados locais
3. Execute os scripts de `scripts_db/` na ordem numérica
4. Rode `dotnet run` — o sistema lê automaticamente o arquivo Development

## Variável de ambiente
$env:ASPNETCORE_ENVIRONMENT = "Development"   # PowerShell
```

---

## 📝 4. Registro de Decisão Arquitetural (ADR)

Toda decisão de infraestrutura **não-óbvia** ganha um arquivo curto na pasta `docs/decisoes/`. O formato é:

```markdown
# ADR-001: Banco Local PostgreSQL vs Supabase em Produção

## Data
2026-04-25

## Status
Aceita

## Contexto
Precisamos de PostgreSQL local para desenvolvimento rápido sem custo,
mas Supabase em produção pelo managed hosting e backups automáticos.

## Decisão
- Desenvolvimento: PostgreSQL local via Docker ou instalação nativa
- Produção: Supabase (PostgreSQL gerenciado)
- A troca é feita exclusivamente pela variável ASPNETCORE_ENVIRONMENT
- Nenhuma linha de código muda entre ambientes

## Onde Fica no Código
- `Program.cs` → lê `ConnectionStrings:BancoPrincipal` do appsettings ativo
- `docs/ambientes/Local.md` → passo a passo do ambiente dev
- `docs/ambientes/Producao.md` → passo a passo do ambiente prod

## Consequências
- Dev não precisa de internet para trabalhar
- Migrations devem ser testadas em ambos os ambientes antes do deploy
```

### Quando Criar um ADR?

- Conectou num serviço externo novo (Supabase, Google, SEFAZ, SMTP)
- Mudou estratégia de deploy
- Alterou provider de autenticação
- Migrou de uma lib para outra
- Qualquer decisão que alguém perguntaria "por que fizeram assim?"

---

## 🔌 5. Documentação de Integrações Externas

Cada serviço externo ganha um arquivo em `docs/integracoes/`. O formato padrão:

```markdown
# Integração: [Nome do Serviço]

## O que faz
Descrição clara e curta do papel dessa integração no sistema.

## Credenciais necessárias
| Chave | Onde configurar | Como obter |
|---|---|---|
| `ClientId` | `appsettings.json → Google:ClientId` | Console Google Cloud → APIs & Services |
| `ClientSecret` | `appsettings.json → Google:ClientSecret` | Mesmo lugar |

## Arquivos envolvidos no código
| Arquivo | Função |
|---|---|
| `Program.cs` (linha X) | Registra o middleware de autenticação Google |
| `Infraestrutura/Servicos/GoogleAuthService.cs` | Implementação do fluxo OAuth |
| `wwwroot/google-services.json` | Config client-side (se aplicável) |

## Arquivo de configuração original
Cópia sanitizada em: `docs/configs/google-services.exemplo.json`

## Passo a passo para configurar do zero
1. Acesse console.cloud.google.com
2. Crie um projeto ou selecione o existente
3. Vá em "APIs & Services" → "Credentials"
4. Crie um OAuth 2.0 Client ID
5. Copie ClientId e ClientSecret para o appsettings do ambiente desejado

## Observações / Armadilhas
- O redirect URI deve ser exato, incluindo o trailing slash
- Em produção o domínio deve estar verificado no Google
```

---

## 🖥️ 6. O Arquivo SETUP.md (Do Zero ao Rodando)

Na raiz de `docs/`, o `SETUP.md` é o **guia definitivo** para subir o projeto do zero. Deve conter:

1. **Pré-requisitos**: .NET 8 SDK, PostgreSQL, Node.js (se HTMX precisar de build), etc.
2. **Clonar e configurar**: `git clone` → copiar `.exemplo` → preencher segredos
3. **Banco de dados**: Executar scripts na ordem (`01_schema.sql`, `02_seeds.sql`...)
4. **Rodar**: `dotnet run` e como verificar que funcionou
5. **Troubleshooting**: Erros comuns e como resolver (porta ocupada, migration falhou, etc.)

> 💡 **Teste do SETUP.md**: Se um desenvolvedor novo consegue seguir o documento e subir o projeto em menos de 30 minutos sem perguntar nada, o SETUP.md está bom.

---

## 🏷️ 7. Regras de Commit para Mudanças de Infraestrutura

Quando a mudança é de **infraestrutura/integração** (não é uma tela ou feature de negócio), o commit deve:

- **Prefixo**: `infra:` ou `config:` no título do commit
- **Incluir** a atualização do respectivo arquivo em `docs/` no mesmo commit
- **Exemplos**:
  - `infra: configurar Supabase como provider de produção`
  - `config: adicionar autenticação Google OAuth`
  - `infra: migrar SMTP de MailGun para SendGrid`

**Regra de Ouro**: Se o commit altera uma integração mas NÃO atualiza o `docs/`, o commit está incompleto.

---

## 🤖 8. O Antigravity e Contexto de Projeto

Para que a IA (Antigravity ou qualquer assistente) "enxergue" corretamente as estruturas do projeto:

### O que a IA precisa saber
- Os **Manifestos** (este e os outros) fornecem o "como deve ser feito"
- A pasta `docs/integracoes/` fornece o "o que está conectado e onde"
- A pasta `docs/ambientes/` fornece o "como rodar em cada lugar"
- Os arquivos `.exemplo` fornecem o "formato esperado das configs"

### Como referenciar para a IA
Ao iniciar uma conversa envolvendo infraestrutura, aponte:
1. O manifesto relevante (ex: este documento)
2. O ADR se existir (ex: `docs/decisoes/001_banco_local_vs_supabase.md`)
3. O doc de integração (ex: `docs/integracoes/Supabase.md`)

> A IA nunca deve inventar credenciais ou assumir configurações. Ela deve **perguntar** ou **ler** o que existe na pasta `docs/`.

> 🚫 **PROIBIÇÃO DE ALTERAÇÃO**: A IA é terminantemente PROIBIDA de criar, editar, apagar ou manipular o arquivo `Documentação/Proximos_Passos.md`. Este documento serve unicamente como controle cronológico/estratégico de tarefas para o desenvolvedor humano. Qualquer ação de IA sobre ele resultará em rejeição imediata.

---

## 📋 Checklist de Nova Integração

Toda vez que uma integração nova for adicionada ao sistema, verificar:

- [ ] Arquivo de integração criado em `docs/integracoes/[NomeServico].md`
- [ ] Arquivo `.exemplo` com todas as chaves em `docs/configs/`
- [ ] ADR criado em `docs/decisoes/` se for decisão não-óbvia
- [ ] `appsettings.json` atualizado com as chaves (valores placeholder)
- [ ] `.gitignore` atualizado se houver novo arquivo sensível
- [ ] `SETUP.md` atualizado com novo pré-requisito se aplicável
- [ ] Commit com prefixo `infra:` ou `config:` incluindo os docs

---

## 🏁 O Foco Final

> Código sem documentação operacional é uma **bomba-relógio**. Ele funciona apenas na máquina de quem configurou. A pasta `docs/` garante que qualquer pessoa — humana ou IA — consiga entender, configurar e operar o sistema em qualquer ambiente, a qualquer momento.

---

## 🗄️ 9. Banco de Dados — Scripts e Migrations (Regra Canônica)

> **Regra de Ouro**: Toda alteração no schema do banco — nova tabela, coluna, trigger, índice ou função — **deve ser consolidada no arquivo canônico `Scripts/banco_script_completo.sql`**. Nunca alterar o banco diretamente pelo Supabase UI sem registrar a alteração no script canônico.

### 9.1 Onde ficam os scripts

**Estrutura simplificada de arquivos e scripts**:

```
📁 c:/Projeto/
├── Projetos_Auxiliares/
│   ├── DbSync/                     ← Robo de sincronização SQL multi-banco
│   ├── DbmlParser/                 ← Parser de Manifesto_DBML.md -> SQL
│   ├── FixEncodingApp/             ← Corretor de Encoding para UTF-8 com BOM
│   └── PowerShell/                 ← Scripts operacionais e atalhos de console
└── Scripts/
    ├── INSERT/                     ← Pasta com planilhas e scripts de inserts antigos (referência)
    └── banco_script_completo.sql   ← ÚNICO ARQUIVO — todo o schema e seeds aqui
```

O script é organizado internamente em seções comentadas:

```sql
-- Script de Inicialização - ERP DB          (extensões, schemas, enums)
-- [tabelas do ERP / Serviços / Cliente]     (geradas pelo DbmlParser)
-- SEGURANÇA: SESSÕES E TENTATIVAS DE LOGIN  (sessoes_usuario, tentativas_login)
-- SISTEMA DE MENSAGENS E NOTIFICAÇÕES       (7 tabelas + trigger)
```

> ✅ **Regra**: novos módulos SQL são sempre **appendados** ao final do `Scripts/banco_script_completo.sql`, nunca em arquivos separados.

Os arquivos e planilhas com inserts do sistema antigo estão mantidos sob a pasta `Scripts/INSERT/` apenas para fins de consulta e migração de dados antigos.

### 9.2 Como executar no Supabase (padrão obrigatório)

O `psql` do PostgreSQL instalado localmente é o executor padrão. **Nunca** usar o SQL Editor do Supabase UI para scripts longos (limite de tempo e sem rastreabilidade).

```powershell
# Variável de ambiente para senha (evita prompt interativo)
$env:PGPASSWORD = 'SUA_SENHA_SUPABASE'

# Executar um script único
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" `
    "host=SEU_POOLER.pooler.supabase.com port=6543 dbname=postgres user=postgres.SEU_ID sslmode=require" `
    --echo-errors `
    -f "caminho\para\script.sql"

# Executar o script canônico completo
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" `
    "host=SEU_POOLER.pooler.supabase.com port=6543 dbname=postgres user=postgres.SEU_ID sslmode=require" `
    --echo-errors `
    -f "Scripts\banco_script_completo.sql"
```

> ⚠️ **NOTICE vs ERROR**: O psql retorna `NOTICE` (não erros reais) quando usa `IF NOT EXISTS` e o objeto já existe. Isso é comportamento esperado e seguro — os scripts são **idempotentes**.

### 9.3 Regras de escrita dos scripts

| Regra | Motivo |
|---|---|
| Usar `CREATE TABLE IF NOT EXISTS` | Idempotência: re-executar não quebra |
| Usar `CREATE INDEX IF NOT EXISTS` | Idem |
| Usar `CREATE OR REPLACE FUNCTION` | Permite atualizar funções sem DROP |
| Usar `DROP TRIGGER IF EXISTS` antes de `CREATE TRIGGER` | Triggers não têm `OR REPLACE` no PG |
| Adicionar `COMMENT ON TABLE/FUNCTION` | Documenta o banco inline |
| Consolidado no banco canônico | Centraliza todo o schema e seeds em um ponto único (`Scripts/banco_script_completo.sql`) |

### 9.4 Fluxo de nova migration

```
1. Adicionar o DDL/DML idempotente ao final de Scripts/banco_script_completo.sql
2. Testar localmente (PostgreSQL local ou Supabase dev)
3. Executar no Supabase via psql (seção 9.2)
4. Verificar output: apenas CREATE/ALTER/NOTICE — nenhum ERROR
5. Registrar no Diário de Bordo (seção 10) com data e descrição
6. Commit com prefixo: "infra: atualizar banco_script_completo.sql"
```

---

## 📅 10. Diário de Bordo (Changelog de Ambiente e Ferramentas)

Esta seção serve para datar e justificar **criações de ferramentas, scripts ou mudanças estruturais** no ambiente de desenvolvimento, garantindo que o histórico de "por que isso existe" não se perca.

| Data | Ferramenta / Mudança | Motivo / Contexto |
|---|---|---|
| **2026-04-25** | Criação do banco `erp_db` local | A IA conectou-se via `psql` para provisionar o banco de dados oficial de desenvolvimento na máquina local. |
| **2026-04-25** | Criação do `DbmlParser` (C#) | Como o ambiente local não possuía Python instalado (necessário para scripts antigos como `dbml2sql.py`), a IA Antigravity criou um projeto C# nativo (`dotnet run`) chamado **DbmlParser** (sob `Projetos_Auxiliares/DbmlParser`). Ele lê o `Manifesto_DBML.md` e gera automaticamente o script SQL com schemas, tabelas, enums, FKs e as triggers de `data_ultima_atualizacao`. Ele substitui o antigo script em Python. |
| **2026-04-26** | `seguranca_sessoes.sql` aplicado ao Supabase | Script de sessões e tentativas de login executado via psql contra o Supabase de produção (pooler `aws-1-sa-east-1`). Tabelas: `sessoes_usuario`, `tentativas_login`. |
| **2026-04-26** | `mensagens_notificacoes_schema.sql` criado e aplicado | Schema completo do sistema de inbox (estilo OLX/e-mail) e notificações globais/individuais. 7 tabelas + trigger `trg_update_topico_ao_enviar`. Executado via psql no Supabase. |
