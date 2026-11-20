# Referência de Conformidade — Domínio

> **Projeto canônico:** `Projeto/Sistema/Modulo_Monolitico/ModuloMonolitico.Dominio`
>
> **Arquivos relacionados:** [web-pages](web-pages.md) · [infrastructure](infrastructure.md) · [services](services.md)

Documento de referência com os critérios de conformidade a verificar em cada artefato de domínio. O controle de status é feito por classe individualmente, não neste arquivo.

---

## Entidades (`Dominio/Entidades/{Contexto}/`)

Ao auditar ou criar uma Entidade, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Pasta `Entidades/{Contexto}/` alinhada ao `Program.cs` |
| **1 Classe por Arquivo** | Uma classe por `.cs` |
| **Sem Dependência Cruzada** | Sem referência a entidades de outro contexto |
| **Interface de Repositório** | Existe `IXxxRepositorio` em `InterfacesDeRepositorio/{Contexto}/` |
| **Propriedades Nomeadas** | Propriedades em português, sem abreviações obscuras |
| **Sem Comentários** | Sem `//` ou `///` (exceção: formato esperado de dado em caso raro) |

---

## Interfaces de Repositório (`Dominio/InterfacesDeRepositorio/{Contexto}/`)

Ao auditar ou criar uma Interface de Repositório, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Pasta correta por módulo de negócio |
| **Nomenclatura** | Interface `IXxxRepositorio` |
| **Métodos Padrão** | Inserção: `Cadastrar*Async` · Atualização: `Atualizar*Async` · Soft delete: `DeletarSeguro*Async` · Hard delete: `DeletarFisico*Async` |
| **Verbos de Domínio** | Fluxos ricos (`Registrar*`, `Executar*`, `Convidar*`) são aceitos quando não são CRUD simples |
| **Implementada** | Classe em `Infraestrutura/AcessoADados/Repositorios/{Contexto}/` |
| **Registrada na DI** | `AddContextoXxx()` no `Program.cs` |
| **Sem Comentários** | Interface limpa, sem comentários supérfluos |

---

## Constantes e Definições (`Dominio/Constantes/`)

Ao auditar ou criar constantes, verificar:

| Arquivo | O que verificar |
|---------|-----------------|
| `RecursosSistemaDefinicoes.cs` | Telas com permissão declaradas |
| `MenuSistemaDefinicoes.cs` | Itens apontam para rotas existentes |
| `MenuSistemaDefinicoes.cs` | Chaves batem com `RecursosSistemaDefinicoes` |

---

## Convenção de Nomenclatura

### Código em português

Funções, métodos, variáveis e propriedades de **lógica de negócio** em **português**. Inglês apenas para sintaxe de framework e contratos externos.

| ✅ Português | ❌ Inglês reservado |
|---|---|
| `CadastrarAsync`, `AtualizarAsync`, `DeletarSeguroAsync`, `DeletarFisicoAsync` | `OnGet`, `OnPost`, `PageModel`, `DbContext` |
| `ObterUsuarioAtivo`, `CalcularDesconto` | `async`, `return`, `class`, keywords C# |
| `var totalItens`, `var precoFinal` | `Dapper`, `MediatR`, bibliotecas |

### Verbos de repositório (intenção × ação técnica)

| Intenção | Método | Efeito no banco |
|----------|--------|-----------------|
| Inserir registro | `Cadastrar*Async` | INSERT |
| Alterar registro | `Atualizar*Async` | UPDATE |
| Exclusão lógica | `DeletarSeguro*Async` | UPDATE `data_exclusao` |
| Exclusão física | `DeletarFisico*Async` | DELETE |
| Fluxo de negócio rico | `Registrar*`, `Executar*`, `Cancelar*` etc. | conforme regra |

> **Proibidos para CRUD:** `Criar`, `Inserir`, `Incluir`, `Salvar`, `Adicionar`, `Excluir` (usar `Cadastrar` / `DeletarSeguro` / `DeletarFisico`).

---

## Observações Gerais

- Verbos de domínio ricos (`CancelarAgendamento`, `RegistrarCarimbo`) **não** precisam virar `Cadastrar`/`Deletar`.
- Pasta `SistemaBack/` mantém nomenclatura legada; não usar como referência.
