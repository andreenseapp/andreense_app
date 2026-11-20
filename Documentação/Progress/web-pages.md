# Referência de Conformidade — Web / Pages

> **Projeto canônico:** `Projeto/Sistema/Modulo_Monolitico/ModuloMonolitico.Web`
>
> **Arquivos relacionados:** [domain](domain.md) · [infrastructure](infrastructure.md) · [services](services.md)
>
> **Checklist de UX por módulo:** `Documentação/Manifestos/Manifesto_Mapeamento_e_Controle_de_Telas.md`

Documento de referência com os critérios de conformidade a verificar em cada Razor Page. O controle de status é feito por tela individualmente, não neste arquivo.

---

## Parte A — Arquitetura, Roteamento e Segurança

Ao auditar ou criar uma Razor Page, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Arquivo em `Pages/{Contexto}/` alinhado ao `Program.cs` e pastas de Domínio/Infra |
| **Herança de Base** | Page Model herda `AppBasePageModel` ou `AppListBasePageModel` |
| **Rota Canônica** | URL em `ModuloRotas.Telas.cs` — sem path hardcoded no `@page` |
| **Mapeamento de Página** | Entrada em `ModuloRotas.Paginas.cs` + `ConfigurarRotasCanonicas()` |
| **Empresa Ativa** | `ExigeEmpresaAtiva` correto (true / false / N/A) |
| **Autenticação** | Anônima / Autenticada / Apenas Master |
| **Permissão Declarada** | Chave de `RecursosSistemaDefinicoes` valida o acesso |
| **Recurso Registrado** | Chave existe em `RecursosSistemaDefinicoes.cs` |
| **Visível no Menu** | Item em `MenuSistemaDefinicoes.cs` (ou N/A se interna) |
| **Resposta JSON** | AJAX usa `ModuloRotas.JsonSucesso` / `JsonErro` (ou N/A) |
| **Limite de Acesso** | `[LimiteAcesso]` em login/forms sensíveis (ou N/A) |

---

## Parte B — UI, Código e Operação

Ao auditar ou criar uma Razor Page, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Assets Isolados** | `.css` e `.js` em `wwwroot/Contextos/{Modulo}/`, mesmo nome da page; sem `<style>`/`<script>` inline |
| **TagHelpers** | 100% dos campos com `<app-input>`, `<app-select>`, `<app-button>` etc. |
| **Paginação** | Listagens com OFFSET/LIMIT + `_ListPagination` (ou N/A) |
| **Feedback Visual** | `TempData["MensagemSucesso"]` / `MensagemErro`; sucesso = **"Concluído"** |
| **Seletor Colunas** | `data-seletor-colunas="true"` + `seletor-colunas.js` (ou N/A) |
| **Código PT-BR** | Métodos, variáveis e propriedades de negócio em português |
| **Sem Comentários** | Sem `//` ou `///` no código (exceção: nota pontual sobre formato de variável) |
| **ILogger** | `ILogger<T>` injetado; POSTs com `LogInformation` / `LogWarning` / `LogError` |
| **Nomenclatura** | Commands: `Cadastrar` / `Atualizar` / `Deletar*` conforme [services.md](services.md) |
| **UTF-8** | Arquivo sem corrupção (`Ã£`, `??`); salvo em UTF-8 |
| **Tipografia** | Plus Jakarta Sans via layout/`comum.css` — sem `font-family` inline |
| **EF Escrita** | CRUD simples persiste via EF Core (`DbContext`) com tracking de alterações (ou N/A) |
| **Interações via data-\*** | Sem `onclick` inline; interatividade delegada via `data-*` no `.js` isolado |

---

## Referências Rápidas

**Assets por tela:** `wwwroot/Contextos/{Modulo}/{nome-da-page}.css` e `.js` + `comum.css` / `comum.js` do contexto.

**Fonte padrão (layout):**
```html
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=Instrument+Serif:ital@0;1&display=swap" rel="stylesheet">
```

**Login para testes no browser:** `andreense.app@gmail.com` / `123456` (apenas dev).

**Seletor de colunas:**
```html
<table class="table" data-seletor-colunas="true">
<script src="~/js/seletor-colunas.js" asp-append-version="true"></script>
```

**Paginação:** `AppListBasePageModel` + `OFFSET @Offset LIMIT @Limite` no repositório ou EF.

---

## Mapeamento de Paths (doc → pasta real em `Pages/`)

| Nomenclatura no Doc | Caminho físico |
|---------------------|----------------|
| `Auth/*` | `Autenticacao/*` |
| `CRM/Contatos/*` | `Contatos/Cadastro/*` |
| `CRM/Vendedores/*` | `Gestao/Vendedores/*` |
| `CRM/TiposVendedor/*` | `Gestao/TiposVendedor/*` |
| `Servicos/Fidelidade/*` | `Fidelizacao/Fidelidade/*` |
| `Servicos/Cashback/*` | `Fidelizacao/Cashback/*` |
| `ERP/Produtos/*` | `Produto/*` |
| `ERP/Estoque/*` | `Estoque/*` |
| `Base/Configuracoes` | `Gestao/Configuracoes` |
| `Principal/Index` | `Principal/Index` (hub de módulos) |

---

## Observações Gerais

- **Critério de Conclusão:** Uma tela só pode ser considerada OK quando atende 100% dos critérios arquiteturais, de segurança, visuais e de negócio aplicáveis.
- **Níveis de Log:** Nunca usar `LogError` para falhas de validação de negócio ou comportamento do usuário (ex: e-mail inválido, credenciais incorretas). Usar `LogWarning` ou `LogInformation`. `LogError` é reservado para falhas críticas de infraestrutura ou exceções não tratadas.
- `SistemaBack/` é cópia legada — não usar como referência.
- CRUD novo: preferir EF Core na escrita; Commands permanecem para fluxos com regra de negócio rica.
