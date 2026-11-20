# Regras e Padrões de Desenvolvimento do Projeto

> **Fonte canônica:** este arquivo (`.agents/AGENTS.md`). Cursor (`.cursorrules`) e Trae (`.trae/rules/`) devem conter **apenas** um aviso apontando para cá — não duplicar estas regras.

Este documento consolida as diretrizes de arquitetura, nomenclatura, frontend, persistência e boas práticas que devem ser seguidas de forma estrita pela IA em todas as modificações e criações de código neste workspace. Aplica-se a **todos** os contextos (incluindo Imobiliária), sem cópia por módulo.

---

## 1. Idioma e Nomenclatura

*   **Código em Português:** Todo o código de lógica de negócios (variáveis, propriedades, classes, métodos, parâmetros, tabelas) deve ser escrito obrigatoriamente em **Português**.
*   **Inglês apenas para Syntax/Framework:** O uso de inglês é restrito a palavras reservadas da linguagem ou do framework (ex: `async`, `return`, `class`, keywords C#), convenções padrão do ASP.NET Core (`OnGet`, `OnPost`, `PageModel`, `DbContext`) ou contratos de bibliotecas externas (ex: `Dapper`, `MediatR`).
*   **Refatoração Imediata:** Caso a tela ou classe que esteja sendo modificada possua trechos em inglês, faça a refatoração para português de forma imediata.
*   **Nomenclatura de Commands:** Devem seguir o padrão `{Verbo}{Entidade}Command` (ex.: `CadastrarProdutoCommand`).
*   **Nomenclatura de Queries:** Devem seguir o padrão `{Listar|Obter}{Entidade}Query` (ex.: `ListarProdutosQuery`).
*   **Verbos CRUD Proibidos:** Nunca use termos como `Criar`, `Inserir`, `Incluir`, `Salvar`, `Adicionar` ou `Excluir` em commands e métodos de persistência CRUD. Use estritamente:
    *   **Inserção:** `Cadastrar*Async` (ex.: `CadastrarUsuarioAsync`)
    *   **Atualização:** `Atualizar*Async` (ex.: `AtualizarUsuarioAsync`)
    *   **Exclusão Lógica:** `DeletarSeguro*Async` (ex.: `DeletarSeguroUsuarioAsync` — atualiza coluna `data_exclusao`)
    *   **Exclusão Física:** `DeletarFisico*Async` (ex.: `DeletarFisicoUsuarioAsync` — remove o registro do banco)
*   **Verbos de Domínio Rico:** Verbos ricos que representam fluxos de negócio reais (como `CancelarAgendamento`, `RegistrarCarimbo`, `ConvidarMembro`, `VincularPermissoes`, `DarBaixaFatura`, `RegistrarLeitura`) são bem-vindos e não devem ser substituídos por verbos CRUD genéricos.
*   **Textos e Mensagens:** É estritamente proibido o uso de textos *hardcoded* espalhados pelo código (como mensagens de erro, validação, labels e avisos no C#). Todas as mensagens devem ser extraídas para classes de terminologia (ex: `TerminologiaErp`) ou classes de definições de mensagens centralizadas no Domínio.
*   **Proibição Absoluta de Hardcodes:** É terminantemente proibido qualquer tipo de hardcode em interpolação de strings (mesmo parcial) para construção de chaves de cache, cookies, sessões, identificadores de componentes ou payloads (ex: NUNCA faça `var cacheKey = $"Suporte_{sessionId}_Administrador";`). Toda lógica de geração de chaves deve residir exclusivamente de forma centralizada em `ModuloMonolitico.Dominio.Configuracoes.ChavesSistema`.

---

## 2. Comentários e Formatação

*   **Sem Comentários no Código:** É proibido deixar linhas de comentário (`//` ou `///`) no código. O código deve ser limpo e autoexplicativo.
*   **Única Exceção:** É permitida apenas uma única linha explicativa descrevendo o "formato esperado de dados" armazenados em uma variável ou string em lógicas de tratamento de dados extremamente específicas e não triviais.
*   **Codificação UTF-8:** Garanta que todos os arquivos sejam salvos com codificação **UTF-8** para evitar corrupção de caracteres especiais e acentuações (evitar erros como `Ã£` ou `??`).
*   **Renomeação segura de identificadores:** **Não** use scripts PowerShell em lote (loops sobre dezenas de arquivos com `-replace` global) para renomear identificadores ou refatorar texto em massa — isso corrompe UTF-8 e pode alterar substrings indesejadas (ex.: `De` dentro de `ObjetosDeValor`, `Id` dentro de `Guid`). Prefira `StrReplace`/`Write` pontual por arquivo. Se precisar de PowerShell, use **lista explícita de arquivos** e encoding UTF-8 explícito na leitura e na gravação:

    ```powershell
    $utf8 = New-Object System.Text.UTF8Encoding $false
    $content = [System.IO.File]::ReadAllText($path, $utf8)
    # alterações pontuais no $content
    [System.IO.File]::WriteAllText($path, $content, $utf8)
    ```

    Após cada bloco de renomeação, valide com `dotnet build` e grep de sanidade (`hor?rio`, `usu?rio`, `MoiuloMonolitico`, `gnt; snt`).

---

## 3. Organização por Contextos e Arquitetura

*   **Uma Classe por Arquivo:** Cada classe, repositório, interface, command ou query deve residir em seu próprio arquivo `.cs`.
*   **Command + Handler:** O Command e seu respectivo Handler podem (e devem) ficar declarados no mesmo arquivo físico, desde que isolados de outras regras de aplicação.
*   **Estrutura de Pastas por Contexto:** O projeto é estritamente segmentado por contextos de negócio (ex: `Pages/{Contexto}/`, `Entidades/{Contexto}/`, `InterfacesDeRepositorio/{Contexto}/`, `Repositorios/{Contexto}/`, `Comandos/{Contexto}/`).
*   **Sincronização de Contexto:** Sempre verifique as dependências registradas no bootstrap `Program.cs` para identificar a arquitetura correta dos contextos de negócio. As entidades, repositórios e páginas devem seguir rigorosamente o alinhamento de seu respectivo contexto.
*   **Sem Dependência Cruzada (Anti-leakage):** Entidades pertencentes a um contexto nunca devem referenciar diretamente entidades de outros contextos.
*   **Injeção de Dependências:** O registro de repositórios deve ser feito em arquivos setup específicos por contexto (ex: `{Contexto}Setup.cs` ou extensão `AddContextoXxx`) dentro do diretório `Infraestrutura/InjecaoDependencia/Contextos/` e chamados no `Program.cs`.

---

## 4. Banco de Dados e Persistência (Dapper + EF Core)

*   **Modelo Híbrido de ORM:**
    *   **Leitura (Listagens, Relatórios):** Deve utilizar **Dapper** com SQL explícito.
        *   Sempre realize mapeamento explícito com aliases de coluna (ex: `SELECT nome_usuario AS NomeUsuario`).
        *   Toda listagem em grid deve retornar um `ResultadoPaginado<T>` (nunca retorne tuplas brutas).
        *   Implemente paginação server-side obrigatória usando `OFFSET @Offset LIMIT @Limite`.
        *   Sempre filtre por `@contaId` obrigatoriamente para segurança de concorrência e isolamento.
    *   **Escrita (CRUD simples):** Deve utilizar **Entity Framework Core (EF Core)** através do `DbContext` direto no `PageModel` para tracking automático de campos alterados (evitando a criação de Commands/Handlers desnecessários para operações triviais).
    *   **Escrita (Fluxos Complexos):** Utilizar Dapper encapsulado no repositório executado sob transação (`ExecutarEmTransacaoAsync`) ou EF Core, conforme o caso e complexidade da regra.
*   **Uso de Datas (UTC):** Salve e manipule as datas no banco de dados e repositórios sempre em UTC via `DateTime.UtcNow`. A conversão para data local (`ToLocalTime()`) deve ocorrer **apenas** no momento da exibição final na View/HTML. Nunca salve data local no banco de dados.
*   **Limpeza pós-refatoração:** Sempre que um Command, Handler ou Query se tornar obsoleto ou parar de ser utilizado após um ajuste, delete o arquivo fisicamente para não deixar lixo no repositório.

---

## 5. Frontend e UI (Razor Pages)

*   **Herança de Base:** Os PageModels devem obrigatoriamente herdar de `AppBasePageModel` ou `AppListBasePageModel` (no caso de telas de listagem).
*   **Rotas Canônicas:** Nunca declare caminhos hardcoded na diretiva `@page` no topo das Razor Pages. Registre e gerencie as rotas de forma centralizada em `ModuloRotas.Telas.cs`, `ModuloRotas.Paginas.cs` e configure através do método `ConfigurarRotasCanonicas()`.
*   **Assets Isolados (.css/.js):** Os códigos CSS e JavaScript de uma página devem ficar salvos em arquivos independentes que possuam o **mesmo nome** da Razor Page e residam em `wwwroot/Contextos/{Modulo}/`. Nunca utilize tags `<style>` ou `<script>` inline dentro do arquivo `.cshtml`. Cada módulo deve ter também um arquivo `comum.css` e `comum.js` para estilos/scripts compartilhados dentro do contexto.
*   **Tipografia Padrão:** Utilizar a fonte **Plus Jakarta Sans** importada globalmente no layout/`comum.css` com o seguinte link:
    `<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=Instrument+Serif:ital@0;1&display=swap" rel="stylesheet">`
    *   É proibido definir propriedades `font-family` inline nas tags HTML das páginas.
*   **TagHelpers:** Todos os campos de entrada, formulários e botões devem utilizar TagHelpers customizados (`<app-input>`, `<app-select>`, `<app-button>`). Não use marcações HTML puras para componentes que já possuam TagHelpers implementados.
*   **ViewData / TempData:** Chaves de `ViewData` e `TempData` devem usar exclusivamente `ViewDataKeys` e `TempDataKeys` em `Core/Constantes/TerminologiaDicionario.cs` — nunca literais `ViewData["..."]` ou `TempData["..."]`. Se um conjunto chave+valor for sempre o mesmo entre telas, encapsule em helper na `AppBasePageModel` (ou no PageModel de contexto) em vez de repetir o bloco. Títulos e descrições de página continuam locais (`DefinirMetaTags` / `ViewData[Title]` por tela).
*   **Feedback Visual:** Notifique o usuário com `NotificarSucesso()` / `NotificarErro()` (ou propriedades `[TempData]` alinhadas a `TempDataKeys`). Sucesso padrão: `MensagensSistema.Geral.Sucesso` (**"Concluído"**).
*   **Respostas AJAX/JSON:** Chamadas assíncronas de backend devem retornar respostas formatadas via `ModuloRotas.JsonSucesso` ou `ModuloRotas.JsonErro`.
*   **Seleção Dinâmica de Colunas:** Para tabelas dinâmicas, utilize o atributo `data-seletor-colunas="true"` integrado ao arquivo script `seletor-colunas.js`.
*   **Controle de Acesso e Limitação:** Use a anotação `[LimiteAcesso]` em rotinas ou formulários sensíveis como login e recuperação de credenciais.
*   **Interações via `data-*`:** Nunca use atributos `onclick` inline no HTML. Toda interatividade deve ser delegada via atributos `data-*` capturados no arquivo `.js` isolado da página.
*   **Permissões e Visibilidade no Menu:** Toda tela que exige permissão deve ter sua chave declarada em `RecursosSistemaDefinicoes.cs` e o item de menu correspondente em `MenuSistemaDefinicoes.cs` deve apontar para essa chave.

---

## 6. Repositórios — Regras Complementares

*   **Herança de Base:** Repositórios que utilizam Dapper devem herdar de `RepositorioBase`.
*   **Sem Lógica de Negócio:** A classe de repositório deve conter **apenas** lógica de acesso a dados. Regras de negócio pertencem ao Handler ou à Page Model.
*   **Transações Multi-tabela:** Operações que envolvem múltiplas tabelas devem obrigatoriamente ser executadas dentro de `ExecutarEmTransacaoAsync`.

---

## 7. Commands e Queries (MediatR)

*   **Commands = intenções de mudança de estado.** Antes de criar um Command para CRUD simples, avalie se o EF Core direto na Page é suficiente. Commands permanecem para fluxos com regra de negócio rica.
*   **Validação no Handler:** Regras de validação de negócio devem estar no Handler (com FluentValidation quando aplicável) — nunca diretamente na Razor Page.
*   **Sem Acesso Direto ao Banco:** Handlers nunca devem acessar o banco diretamente; devem utilizar repositórios ou `DbContext` via injeção de dependência.
*   **Retorno Padronizado:** Handlers devem retornar `ResultadoDto` ou `ResultadoOperacao` quando precisam expor mensagem ao usuário.
*   **ILogger no Handler:** Todo Handler deve registrar início, sucesso e falha usando `ILogger<T>`.
*   **Queries = leituras sem efeito colateral.** Estrutura em `Aplicacao/CasosDeUso/{Contexto}/Consultas/`.

---

## 8. Monitoramento, Logs e Testes

*   **Logging Estruturado:** Sempre injete `ILogger<T>` nas classes de backend. Toda operação importante (POSTs, Handlers de Command, início de fluxos, erros e avisos) deve registrar logs contendo informações úteis classificadas corretamente entre `LogInformation`, `LogWarning`, `LogError` ou alertas.
*   **Níveis de Log Corretos:** Nunca utilize `LogError` para falhas de validação de negócio ou comportamento esperado do usuário (ex: e-mail inválido, credenciais incorretas, rate-limit atingido). Estas situações devem ser registradas como `LogWarning` ou `LogInformation`. O nível `LogError` é estritamente reservado para falhas críticas de infraestrutura, integrações externas ou exceções não tratadas.
*   **Credenciais de Teste (Browser):** Ao testar fluxos no navegador, utilize as seguintes credenciais padrão:
    *   **Usuário/E-mail:** `andreense.app@gmail.com`
    *   **Senha:** `123456`
