# Referência de Conformidade — Aplicação / Casos de Uso (MediatR)

> **Projeto canônico:** `Projeto/Sistema/Modulo_Monolitico/ModuloMonolitico.Aplicacao`
>
> **Arquivos relacionados:** [web-pages](web-pages.md) · [domain](domain.md) · [infrastructure](infrastructure.md)

Documento de referência com os critérios de conformidade a verificar em cada Command e Query. O controle de status é feito por classe individualmente, não neste arquivo.

---

## Commands (`Aplicacao/CasosDeUso/{Contexto}/Comandos/`)

Commands = **intenções de mudança de estado**. CRUD simples tende a migrar para EF Core direto na Page.

Ao auditar ou criar um Command, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Pasta `Comandos/` no contexto correto |
| **1 Arquivo** | Command + Handler no mesmo arquivo, isolado de outros |
| **Nomenclatura** | `{Verbo}{Entidade}Command` — ex.: `CadastrarProdutoCommand` |
| **Verbos Corretos** | `Cadastrar`, `Atualizar`, `Deletar` (soft), `DeletarFisico` (hard), ou verbo de domínio rico (`Cancelar`, `Registrar`) |
| **Retorno Padronizado** | `ResultadoDto` / `ResultadoOperacao` quando expõe mensagem ao usuário |
| **Validação no Handler** | Regras no Handler + FluentValidation — não na Razor Page |
| **Sem Acesso Direto a DB** | Usa repositório ou `DbContext` via interface/injeção |
| **Entidade Própria** | Sem anti-leakage entre contextos |
| **ILogger** | Handler registra início, sucesso e falha com `ILogger<T>` |
| **Sem Comentários** | Sem `//` ou `///` supérfluos |
| **EF Escrita** | Se CRUD simples: considerar remover Command e usar EF na Page |

### Verbos permitidos

| Tipo | Verbos |
|------|--------|
| CRUD padrão | `Cadastrar`, `Atualizar`, `Deletar` (intenção do usuário), `DeletarFisico` (quando físico) |
| Domínio rico | `Cancelar`, `Registrar`, `Convidar`, `Resgatar`, `Creditar`, `Definir`, `Inativar`, `Revogar` |
| **Evitar** | `Criar`, `Alterar`, `Excluir`, `Salvar`, `Inserir`, `Incluir`, `Adicionar` |

---

## Queries (`Aplicacao/CasosDeUso/{Contexto}/Consultas/`)

Queries = **leituras sem efeito colateral**.

Ao auditar ou criar uma Query, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Pasta `Consultas/` no contexto correto |
| **1 Arquivo** | Query + Handler isolados |
| **Nomenclatura** | `{Listar\|Obter}{Entidade}Query` |
| **Sem Side Effects** | Apenas leitura |
| **Sem Acesso Direto a DB** | Via repositório ou `DbContext` |
| **Entidade Própria** | Sem vazamento entre contextos |
| **Paginação** | Listagens com página e tamanho (ou ➖) |
| **ILogger** | Log de consultas pesadas ou erros |
| **Sem Comentários** | Código limpo |

---

## Observações Gerais

- Commands com verbo de domínio (`CancelarAgendamento`, `RegistrarLeituraComunicado`) **não** devem ser renomeados para `Cadastrar`/`Deletar`.
- Para CRUD novo: avaliar EF na Page antes de criar Command (regra do desenvolvedor).
- Pasta `SistemaBack/` ainda contém verbos legados; não usar como referência.
