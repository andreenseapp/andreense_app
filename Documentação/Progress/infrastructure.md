# Referência de Conformidade — Infraestrutura

> **Projeto canônico:** `Projeto/Sistema/Modulo_Monolitico/ModuloMonolitico.Infraestrutura`
>
> **Arquivos relacionados:** [web-pages](web-pages.md) · [domain](domain.md) · [services](services.md)

Documento de referência com os critérios de conformidade a verificar em cada artefato de infraestrutura. O controle de status é feito por classe individualmente, não neste arquivo.

---

## Repositórios (`Infraestrutura/AcessoADados/Repositorios/{Contexto}/`)

Ao auditar ou criar um Repositório, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Contexto** | Pasta alinhada ao `Program.cs` e ao Domínio |
| **Herança de Base** | Herda `RepositorioBase` (quando Dapper) |
| **Implementa Interface** | Implementa `IXxxRepositorio` do Domínio |
| **1 Classe por Arquivo** | Um repositório por `.cs` |
| **Nomenclatura** | `Cadastrar*`, `Atualizar*`, `DeletarSeguro*`, `DeletarFisico*` para CRUD |
| **Mapeamento Explícito** | SQL com aliases `coluna AS Propriedade` |
| **Paginação** | `OFFSET @Offset LIMIT @Limite` retornando `ResultadoPaginado<T>` |
| **Sem Lógica de Negócio** | Só acesso a dados |
| **Transação** | Multi-tabela usa `ExecutarEmTransacaoAsync` |
| **Datas UTC** | `DateTime.UtcNow` em persistência |
| **Sem Comentários** | Sem comentários supérfluos no código |
| **EF Escrita** | Escritas de CRUD simples migradas para EF quando aplicável |

---

## Injeção de Dependência (`Infraestrutura/InjecaoDependencia/Contextos/`)

Ao auditar ou criar um arquivo de Setup de contexto, verificar:

| Critério | O que verificar |
|----------|-----------------|
| **Arquivo Setup** | Existe `{Contexto}Setup.cs` ou extensão `AddContextoXxx` |
| **Registra Interface** | `AddScoped<IXxx, Xxx>()` para cada repositório do contexto |
| **Registrado no Program.cs** | Chamada no bootstrap (`Program.cs`) |

---

## Banco de Dados — Dapper + EF Core

| Camada | Uso |
|--------|-----|
| **Leitura (listagens, relatórios)** | Dapper — SQL explícito, paginação, `@contaId` obrigatório |
| **Escrita (CRUD simples)** | EF Core (`DbContext`) — tracking de campos alterados |
| **Escrita (fluxo complexo)** | Dapper em repositório + transação, ou EF conforme caso |

`DbContext` registrado em `Program.cs`:
```csharp
builder.Services.AddDbContext<DbContext>(options => options.UseNpgsql(connectionString));
```

---

## Convenção de Datas — UTC

| ✅ Correto | ❌ Incorreto |
|---|---|
| `DateTime.UtcNow` no repositório | `DateTime.Now` na persistência |
| `.ToLocalTime()` só na View | Data local salva no banco |

---

## Convenção de Paginação

Listagens retornam `ResultadoPaginado<T>` (não tupla solta). PageModels de listagem herdam `AppListBasePageModel`.

```sql
ORDER BY {colunaWhitelist} {direcao}
OFFSET @Offset LIMIT @Limite
```

---

## Observações Gerais

- Pasta `SistemaBack/` contém nomenclatura legada (`CriarAsync`, `DeletarAsync`); não usar como referência.
- Para CRUD novo: avaliar EF na Page antes de criar repositório específico para escrita.
