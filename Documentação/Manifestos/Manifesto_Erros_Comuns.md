# Manifesto de Erros Comuns e Soluções (SaaS Backoffice & Monolítico)

Este documento atua como um guia de referência rápida para mapear, prevenir e resolver erros comuns identificados na arquitetura e desenvolvimento do sistema multi-tenant, prevenindo regressões e retrabalho.

---

## 1. Mapeamento de Dados e ORM (Dapper & PostgreSQL)

### O Caso do Mapeamento Incorreto de Propriedades (snake_case vs PascalCase)

* **Sintoma**: Um formulário ou listagem realiza operações de escrita com sucesso no banco de dados, mas após o recarregamento da página (GET), os valores numéricos/monetários são zerados e coleções associativas (ex: features, permissões) retornam limpas ou vazias.
* **Causa**: O banco de dados PostgreSQL utiliza nomenclatura em `snake_case` para tabelas e colunas (ex: `valor_mensal`, `plano_id`, `feature_id`). O C# utiliza propriedades em `PascalCase` (ex: `ValorMensal`, `PlanoId`, `FeatureId`). O Dapper faz uma comparação exata de strings e, se não encontrar equivalência direta para propriedades compostas, deixa as variáveis C# com seus valores padrões (`0`, `null` ou `Guid.Empty`). Adicionalmente, quando IDs de chaves estrangeiras (`plano_id`) são lidos como `Guid.Empty` no multi-mapping, filtros de validação descartam os registros.
* **Solução**:
  1. **Evitar o uso de `SELECT *`** em consultas com multi-mapping (`QueryAsync<T1, T2, TR>`).
  2. **Definir aliases com aspas duplas** nas colunas do SQL para forçar a saída exata esperada pelo Dapper:
     ```sql
     SELECT 
         p.id            AS "Id",
         p.nome          AS "Nome",
         p.valor_mensal  AS "ValorMensal",
         pf.plano_id     AS "PlanoId",
         pf.feature_id   AS "FeatureId"
     FROM planos_saas p
     LEFT JOIN planos_features_saas pf ON pf.plano_id = p.id
     ```
  3. **Ajustar o parâmetro `splitOn`** do Dapper para usar o nome do alias correto em PascalCase:
     ```csharp
     await conn.QueryAsync<PlanoSaaS, PlanoFeatureSaaS, PlanoSaaS>(sql, 
         (p, f) => { ... }, 
         splitOn: "PlanoId"
     );
     ```

### O Caso da Violação de Chave Estrangeira (FK) por Mapeamento Incompleto

* **Sintoma**: Ao atualizar o status de uma entidade associada (ex: alterar assinatura de uma conta para "Bloqueada"), o sistema lança a exceção:
  `Npgsql.PostgresException: 23503: insert or update on table "contas_assinaturas_saas" violates foreign key constraint "contas_assinaturas_saas_plano_id_fkey"`.
* **Causa**: O método que carrega a entidade do banco de dados (ex: `ObterAssinaturaDaContaAsync`) retorna o campo com underscore (`ca.plano_id`) sem alias. Como consequência, o Dapper instancia a propriedade C# `PlanoId` como `Guid.Empty` (`00000000-0000-0000-0000-000000000000`). Quando o método de atualização é disparado (`AtualizarAssinaturaAsync`), ele tenta salvar o objeto contendo `PlanoId = Guid.Empty`. Como não há nenhum plano cadastrado com esse ID na tabela pai `planos_saas`, o banco de dados rejeita a transação por violar a chave estrangeira (FK).
* **Solução**: Sempre adicione aliases explícitos com aspas duplas nas propriedades mapeadas no C# ao realizar buscas SQL com Dapper:
  ```sql
  SELECT 
      ca.id                  AS "Id",
      ca.conta_id            AS "ContaId",
      ca.plano_id            AS "PlanoId", -- Alias que previne o Guid.Empty
      ca.status              AS "Status",
      ca.data_vencimento     AS "DataVencimento",
      ca.valor_contratado    AS "ValorContratado",
      ca.data_inclusao       AS "DataInclusao",
      ca.data_ultima_atualizacao AS "DataUltimaAtualizacao"
  FROM contas_assinaturas_saas ca
  ```

---

## 2. Bloqueio de DLLs em Ambiente de Desenvolvimento (Windows)

### Falha de Atualização Silenciosa / Erros MSB3027 na Compilação

* **Sintoma**: Você altera arquivos `.cs` em bibliotecas de classe (`ModuloMonolitico.Infraestrutura` ou `ModuloMonolitico.Aplicacao`), recompila o projeto web, mas as alterações do backend parecem nunca fazer efeito na execução do site, ou ocorrem erros de travamento na pasta `bin/`.
* **Causa**: O IIS Express, o executável `.NET Host` ou o processo de depuração do Visual Studio mantêm os assemblies (`.dll`) sob trava exclusiva de leitura enquanto a aplicação web está no ar. O compilador (`dotnet build` ou MSBuild) não consegue sobrescrever a DLL de infraestrutura antiga e avisa o erro no log, fazendo com que o servidor web continue rodando com a versão desatualizada da biblioteca.
* **Solução**:
  1. Fechar o IIS Express / parar o processo de debug no IDE.
  2. Identificar e encerrar processos remanescentes pelo terminal se necessário:
     ```powershell
     Stop-Process -Name "dotnet" -Force
     Stop-Process -Name "iisexpress" -Force
     ```
  3. Limpar a pasta de saída e reconstruir o projeto:
     ```powershell
     dotnet clean
     dotnet build ModuloMonolitico.Web/ModuloMonolitico.Web.csproj
     ```

---

## 3. Roteamento e Binding no Razor Pages

### Parâmetros de Query String não Vinculados no POST

* **Sintoma**: Ao submeter um formulário HTTP POST em uma URL que possui parâmetros na query string (ex: `Editar?id=GUID`), o argumento correspondente na assinatura do método `OnPostAsync(Guid id)` vem zerado/default (`00000000-0000-0000-0000-000000000000`).
* **Causa**: O model binder padrão do Razor Pages não faz bind automático de parâmetros de query string para assinaturas de handlers no POST a menos que estejam explicitamente mapeados na rota física da página ou marcados com atributos específicos.
* **Solução**:
  1. **Preferir o mapeamento de rota** no topo do arquivo `.cshtml` para tornar o parâmetro parte do path:
     ```html
     @page "{id:guid}"
     ```
  2. **Ou usar propriedade bindada** com um campo oculto (Hidden Field) dentro do formulário HTTP:
     ```csharp
     [BindProperty]
     public Guid Id { get; set; }
     ```
     ```html
     <form method="post">
         <input type="hidden" asp-for="Id" />
         <!-- Restante do form -->
     </form>
     ```
