# ✅ Checklist do Desenvolvedor — Sistema ModuloMonolitico

> Leia isso antes de escrever qualquer linha de código.
> Um conceito por linha. Marque mentalmente cada item.

---

## 🔑 CREDENCIAIS DE DESENVOLVIMENTO
Para realizar login em ambiente de desenvolvimento/teste local, utilize as credenciais abaixo:
* **Usuário Padrão (ERP)**:
  * **E-mail**: `igorjacopidutra@gmail.com`
  * **Senha**: `123456`
* **Administrador Master (Global / InfraAdmin / Testes)**:
  * **E-mail**: `andreense.app@gmail.com`
  * **Senha**: `123456`

---

## 🧩 MÓDULOS DO SISTEMA — Onde cada coisa mora

**Regra de ouro**: toda feature nova pertence a um módulo. Se serve a mais de um módulo, é `Base`.

### Base (sem dono, compartilhado)
| Contexto | O que contém |
|---|---|
| Auth | Login, token, validação de código |
| Contatos | Pessoa Física e Jurídica |
| Empresas | Contas, Empresas, Multi-Tenancy |
| Usuários / RBAC | Usuários, Grupos, Permissões |
| Configurações | Parâmetros globais |
| Vendedores | Vendedores e tipos de vendedor |

### Módulos de Negócio
| Módulo | Rota | Conteúdo principal |
|---|---|---|
| **Área do Cliente** | `/Cliente` | Cashback/Fidelidade (consultar), Vagas (currículo), Agendamento |
| **Serviços** | `/Servicos` | Cashback/Fidelidade (emitir), OS, Nota Serviço, Vagas (publicar) |
| **Varejo** | `/Varejo` | PDV, Caixa |
| **ERP** | `/ERP` | Pedido, Estoque |
| **Financeiro** | `/Financeiro` | Contas a Pagar/Receber, Bancos |
| **Logística** | `/Logistica` | Expedição, Rotas |
| **Cidade** | `/Cidade` | Achados e Perdidos, Vagas (busca pública) |
| **CRM** | `/CRM` | Ligações, Prospects, Marketing |
| **Help Desk** | `/HelpDesk` | Chamados, Incidentes |
| **Fiscal** | `/Fiscal` | Impostos, NFe |
| **Admin** | `/Administracao` | Logs, Usuários, Notificações |

> **Vagas** é distribuída: Serviços (publicar) · Cliente (currículo) · Cidade (buscar)

---

## 🏗️ ARQUITETURA — Como o sistema é organizado

- [ ] O projeto é um **Monolito SSR** (.NET / Razor Pages) preparado para virar API Mobile no futuro sem reescrever regras de negócio
- [ ] Toda regra de negócio vive na camada **Aplicação** (Handlers MediatR) — nunca em Pages, Controllers ou Views
- [ ] A camada de Aplicação **não pode saber** que existe HTTP, formulário, cookie ou sessão
- [ ] Pages e Controllers recebem dados do formulário, montam um `Command` ou `Query`, enviam ao `MediatR`, e renderizam o resultado
- [ ] O mesmo Handler que a Page usa hoje é o mesmo que o futuro App Mobile vai chamar via ApiController
- [ ] Nunca passe `HttpContext`, `Session` ou `ClaimsPrincipal` para dentro de um Handler ou Serviço — passe apenas `Guid usuarioId`

---

## 📦 CAMADAS — O que vai em cada uma

- [ ] **Domínio**: Entidades puras e interfaces de repositório — sem referência a frameworks
- [ ] **Aplicação**: Commands, Queries, Handlers (MediatR), DTOs, Validadores (FluentValidation)
- [ ] **Infraestrutura**: Implementações dos repositórios, acesso a banco (Dapper + EF Core), serviços externos
- [ ] **Web**: Razor Pages (`.cshtml`), ApiControllers, Middlewares, wwwroot (CSS/JS)
- [ ] **DTOs**: Saem da Aplicação — nunca ViewModels. A Page transforma o DTO em HTML; o ApiController serializa em JSON
- [ ] Fluxo de criação obrigatório: **DTOs → Repositório → Handler → Teste → Page**

---

## 🗃️ BANCO DE DADOS — Regras PostgreSQL

- [ ] SGBD: **PostgreSQL**. Leitura: **Dapper** (SQL puro). Escrita: **EF Core** (commands com tracking)
- [ ] **FKs físicas existem** no banco para tabelas estruturais (ex: `usuarios_empresas`, `empresas`, `contatos`). O schema usa `FOREIGN KEY ... REFERENCES` com `ALTER TABLE`. Ao criar uma nova tabela, avaliar se deve ter FK nas colunas relacionais
- [ ] **Soft Delete**: campo `data_exclusao TIMESTAMPTZ` — nunca deletar fisicamente registros de negócio
- [ ] **UUID gerado no banco**: `INSERT ... RETURNING id`
- [ ] Toda tabela tem colunas padrão: `url_imagem`, `status`, `ordem`, `data_inclusao`, `data_ultima_atualizacao`, `data_exclusao`
- [ ] `data_ultima_atualizacao` é atualizada por **TRIGGER** automático a cada `UPDATE`
- [ ] A coluna primária UUID se chama sempre **`id`** — nunca `id_produto` ou `produto_id`
- [ ] Criar índice `WHERE data_exclusao IS NULL` em toda tabela grande
- [ ] Banco usa `snake_case`; C# usa `PascalCase`; JSON usa `camelCase`
- [ ] Toda tabela com campo categórico (tipo, status, modalidade) deve ter a opção **"SEM INFORMAÇÃO"** (`NULL` ou `'sem_informacao'`) como estado legítimo e tratado
- [ ] Enums em C# sempre incluem `NaoInformado = 0` como primeiro membro
- [ ] Toda nova `EXTENSION` PostgreSQL deve ser incluída na rotina de backup
- [ ] Criar ou alterar tabela? Verificar se é necessário vincular o **trigger de auditoria**
- [ ] Toda query Dapper de listagem paginada retorna uma tupla `(IEnumerable<TItem> Itens, int Total)` sob a mesma conexão, executando o `COUNT` e o `SELECT LIMIT/OFFSET` de forma eficiente na mesma rotina


---

## 🔐 SEGURANÇA — Blindagem do sistema

- [ ] Autenticação por **JWT** com validação estrita de `Issuer` e `Audience`
- [ ] SSR usa **Cookie HTTPOnly**; API Mobile usa **Bearer Token**
- [ ] **Cookies centralizados**: nomes/paths/durações só em `AuthCookieSchema`; emissão/limpeza só via `IAuthCookieService` — nunca `CookieOptions` espalhado nas Pages
- [ ] Leitura de token: `AuthCookieSchema.AccessToken` — nunca string `"auth_token_v2"` hardcoded
- [ ] **Rate limit centralizado**: `IRateLimitService` + `RateLimitPoliticas` — nunca helper ad hoc por fluxo; IP via `HttpRequestExtensions.ObterIpCliente()` no Command
- [ ] Headers de segurança + **CSP** via `UseSecurityHeaders` (`SecurityHeadersExtensions.cs`)
- [ ] Mensagens de auth/recuperação **genéricas** — não revelar se e-mail existe
- [ ] **Revogar sessões** antigas no login e na redefinição de senha
- [ ] **XSS**: não usar `innerHTML` com dados dinâmicos — `escHtml()` / DOM seguro em `app.js`
- [ ] Toda escrita verifica `dono_id == usuarioIdLogado` — não basta ter token válido
- [ ] Toda validação do front-end **obrigatoriamente existe no back-end** via FluentValidation (nunca confiar no cliente)
- [ ] O Handler trata toda requisição como se viesse direto de um terminal, sem formulário
- [ ] **LGPD — Consentimento Explícito**: dados de um usuário **jamais** são expostos a outro sem autorização registrada
  - Candidatura a vaga: candidato autoriza ou não o envio de contato ao recrutador — esse consentimento é salvo no banco
  - Localização precisa, currículo, histórico médico e dados financeiros só trafegam com autorização ativa
  - Logs com dados pessoais: acessíveis apenas a perfis Administrador de Conta
- [ ] Anomalias de comportamento (UBA) geram redução de TTL de sessão ou bloqueio temporário
- [ ] Nunca expor detalhes de `TryCatch` para o usuário — mensagens humanizadas apenas
- [ ] Mensagens de erro padrão do .NET em inglês são **proibidas** na interface — sempre em PT-BR com `[Required(ErrorMessage = "...")]`
- [ ] Senha forte obrigatória: mín. 8 caracteres, maiúscula + minúscula + número + caractere especial

---

## 🏢 MULTI-TENANCY — Isolamento entre empresas

- [ ] Hierarquia: `Conta (Tenant)` → `Empresa(s)` → `Usuário(s) com Perfil`
- [ ] Toda query Dapper **obrigatoriamente** recebe `@contaId` como parâmetro — PR sem isso é rejeitado
- [ ] EF Core aplica `Global Query Filter` com `conta_id` automaticamente
- [ ] Um usuário pode pertencer a múltiplas Contas/Empresas com perfis diferentes em cada uma
- [ ] Acesso White-label: o Middleware resolve o tenant pelo domínio/subdomínio antes de qualquer lógica
- [ ] Login White-label exige **duas validações**: senha correta E vínculo com aquela conta específica
- [ ] `MasterId = Guid.Empty` para dados padrão da plataforma — queries incluem `OR conta_id = @masterId`

---

## ⚡ CQRS com MediatR — O padrão de comunicação

- [ ] **Command**: toda operação que muda estado (criar, atualizar, deletar)
- [ ] **Query**: toda operação de leitura — nunca altera estado
- [ ] Handlers ficam em `CasosDeUso/[Contexto]/Comandos/` e `CasosDeUso/[Contexto]/Consultas/`
- [ ] Um Handler resolve **um único problema** — sem misturar responsabilidades
- [ ] Handler cresceu demais? Decompor em sub-handlers ou serviços de domínio

---

## ✏️ NOMENCLATURA — Como nomear tudo

- [ ] Todo código em **PT-BR** — inglês apenas para sintaxe de frameworks (`async`, `IRequest`, `PageModel`)
- [ ] Verbos obrigatórios: `Criar`, `Atualizar`, `Deletar`, `Obter`, `Listar`, `Verificar`, `Vincular`
- [ ] **Proibidos**: `Inserir`, `Incluir`, `Salvar`, `Adicionar`, `Novo` para criação de dados
- [ ] **Nenhuma** tela, classe, relatório ou caso de uso tem apenas o nome do cliente/empresa — o nome reflete a **metodologia**: `ListagemCandidaturasPorVaga`, não `TelaAndreense`
- [ ] Pluralização para gerência: `RegistrosHandler`, `RegistrosRepositorio`
- [ ] Singularidade para transferência: `RegistroDto`, `RegistroCommand`
- [ ] Todo novo termo de negócio deve ser formalizado no Manifesto antes de entrar no código

---

## 🚫 PROIBIÇÕES ABSOLUTAS — Nunca faça isso

- [ ] Nunca criar **God Classes** — classes que validam, salvam, disparam e-mail, emitem nota e fazem log juntos
- [ ] Nunca colocar regra de negócio em Pages, Controllers ou Views
- [ ] Nunca usar `HttpContext` ou `Session` dentro de Handlers ou Serviços
- [ ] Nunca passar `ClaimsPrincipal` para a camada de Aplicação — apenas `Guid usuarioId`
- [ ] Nunca deixar rotina grande em uma única tela — se o botão dispara múltiplas etapas, cada etapa vai para um Handler isolado
- [ ] Nunca criar scripts PowerShell para ler/reescrever arquivos `.cshtml`, `.cs` ou `.json` — causa corrupção de encoding (ERR-001)
- [ ] Nunca hardcodar `connection strings`, `JWT secrets` ou `API keys` no código — apenas variáveis de ambiente
- [ ] Nunca remover um endpoint de API sem o ciclo de depreciação (mínimo 6 meses de coexistência)
- [ ] Nunca copiar código de regra de negócio em duas telas — extraia para um serviço centralizado (DRY)
- [ ] Nunca ter rotinas que fazem a mesma coisa em funções separadas, por menor que elas sejam
- [ ] Nunca nomear algo exclusivamente com o nome do cliente (`RelatorioXYZ`) — use a metodologia (`RelatorioFaturamentoPorPeriodo`)
- [ ] Nunca deixar mensagem de erro em inglês visível para o usuário
- [ ] Sempre responder em português brasileiro (PT-BR) ao interagir com o usuário
- [ ] Nunca editar ou sugerir alterações no arquivo `Proximos_Passos.md` (controle exclusivo do usuário)
- [ ] Nunca utilizar `using static` para importar classes utilitárias de forma oculta; referencie explicitamente a classe de utilidades (ex: `UFuncoes.RetornNumerosStr(texto)`) ou estruture a função como um **Método de Extensão** direto do tipo (ex: `texto.RetornNumerosStr()`)

---

## 🧱 HERANÇA CUSTOMIZADA — Base classes obrigatórias

- [ ] Razor Pages herdam de `AppBasePageModel` — nunca de `PageModel` direto
- [ ] Telas de listagem, consultas e relatórios de dados obrigatoriamente herdam de `AppListBasePageModel<TItem, TFiltro>` para usufruir de paginação universal e persistência automática de filtros
- [ ] API Controllers herdam de `ApiBaseController` — nunca de `ControllerBase` direto
- [ ] Scanner pages herdam de `BaseScannerPageModel` para comportamento comum de leitura de QR Code
- [ ] Componentes Razor reutilizáveis são wrappers dos nativos: `<app-input>` em vez de `<input>` avulso


---

## 🧩 RESPONSABILIDADE ÚNICA (SRP)

- [ ] Uma classe, um problema. Uma função, um propósito
- [ ] Nada de função grande fazendo várias coisas
- [ ] Funções com mais de ~40 linhas devem ser questionadas e provavelmente divididas
- [ ] Se o fluxo for complexo, a função principal é "Orquestradora" que delega para serviços isolados
- [ ] Telas com comportamentos distintos (ex: QR Code para cashback vs fidelidade) usam especialização — base genérica + implementação específica, não `if/else` empilhados

---

## 🔌 MODULARIDADE DE INTEGRAÇÕES EXTERNAS

- [ ] Toda integração externa (pagamento, e-mail, armazenamento, geolocalização) fica atrás de uma **interface de contrato**
- [ ] A troca de fornecedor exige apenas trocar o **adaptador** — nunca alterar Handlers ou Pages
- [ ] Implementações externas ficam em `Infraestrutura/Servicos/` com interface definida em `Aplicacao/Interfaces/`

---

## 🧪 TESTES — Cobertura mínima obrigatória

- [ ] Todo novo `CommandHandler` ou `QueryHandler` nasce com sua classe de teste
- [ ] **Caminho Feliz**: fluxo completo funcionando e retornando o DTO esperado
- [ ] **Validação de Negócio**: pelo menos 1 teste que garante a regra impeditiva principal
- [ ] **Teste do "Sim" e do "Não"**: fluxo com confirmação E fluxo com cancelamento — ambos retornam estado correto sem efeitos colaterais
- [ ] **Teste de Repetição (Idempotência)**: rodar a mesma rotina 2x seguidas com a tela aberta não pode quebrar, duplicar ou lançar exceção
- [ ] **Testes em Lote**: para processamento de múltiplos registros, testar com (a) 1 registro, (b) todos (N), (c) todos menos 1 (N-1)
- [ ] A Definition of Done: funcionalidade só está "concluída" quando tem testes de Caminho Feliz e Validação de Negócio
- [ ] Handlers de login usam `IRateLimitService` — nos testes, mockar `rateLimitMock`, não instanciar helpers de proteção removidos

---

## 📂 REFERÊNCIA RÁPIDA — Infra de Auth

| Recurso | Onde fica |
|---|---|
| Cookies (schema) | `Web/Configuracoes/AuthCookieSchema.cs` |
| Cookies (emissão) | `Web/Servicos/AuthCookieService.cs` |
| Rate limit | `Aplicacao/Servicos/RateLimitService.cs` + `RateLimitPoliticas.cs` |
| CSP / headers | `Web/Configuracoes/SecurityHeadersExtensions.cs` |
| IP do cliente | `Web/Extensions/HttpRequestExtensions.cs` |
| JS/CSS Auth | `wwwroot/Contextos/Autenticacao/` — compartilhado: `comum.css` / `comum.js`; por tela: `login.css`, `login.js`, etc. |
| Rotas canônicas | `Web/Core/ModuloRotas.Telas.cs` — **todas** as telas; `.cshtml` usa só `@page` |
| Mapa de rotas | `ModuloRotas.Paginas` + `ConfigurarRotasCanonicas()` no `Program.cs` |
| Tela standalone (Auth) | Herdar `AutenticacaoStandalonePageModel` + partials `_ContextoStandaloneHead/Foot` |


## 📝 LOGGING — Rastreabilidade obrigatória

- [ ] Primeira linha de todo método habilitado: log com nome do método e variáveis recebidas
- [ ] Tag padrão: `private const string LogTag = "LOG :: [NomeDaClasse]";`
- [ ] Requisição OK → logs em RAM (zero I/O de disco). Requisição falhou → flush completo ao disco
- [ ] Logs de banco expõem `record_id` afetado e `affectedRows`
- [ ] Logs de integração HTTP expõem endpoint acionado e payload
- [ ] StackTrace de erro: completo, sem truncamento

---

## 📖 DOCUMENTAÇÃO — Auto-documentação no código

- [ ] Toda Classe Mestra ou Rotina Pública tem `/// <summary>` com propósito claro
- [ ] Proibido criar arquivos `.txt` soltos para explicar o que o código faz — use XML Comments
- [ ] Toda classe de infraestrutura ou Handler encapsula o `LogTag` com o nome da classe

---

## 🖥️ INTERFACE (UI/UX) — Padrões visuais

- [ ] **Rotas:** `.cshtml` com `@page` apenas (sem path); links/redirects via `ModuloRotas.*`
- [ ] **CSS/JS de tela** em `wwwroot/Contextos/{Modulo}/` — **um `.css` + um `.js` por Page**; compartilhado do contexto: `comum.css` + `comum.js` (nunca prefixar o nome do módulo no arquivo)
- [ ] Dados do servidor para JS: atributos `data-*` no HTML — nunca concatenar no arquivo `.js` estático
- [ ] Migração incremental: telas novas já externalizadas; legado migra ao editar a tela
- [ ] Filtros de listagem/relatório: sempre dentro de `<details>/<summary>` (painel expansível fechado por padrão, exceto se houver filtros ativos informados na URL/QueryString)
- [ ] Recuperação e salvamento de filtros de listagem gerenciados automaticamente por cookies através do método `RestaurarOuSalvarFiltros("nome_do_modulo")` no `OnGetAsync`
- [ ] Todo `<input type="date">` usa `<app-input type="date">` que já inclui o botão "HOJE"
- [ ] Funcionalidade não implementada: exibir **"Em Breve"** — nunca tela em branco
- [ ] Listas grandes: paginação obrigatória com `LIMIT/OFFSET` — nunca renderizar tudo de uma vez
- [ ] Barra de paginação responsiva no final da listagem, cujos links dinâmicos obrigatoriamente preservam todos os filtros de pesquisa ativos na query string
- [ ] Layout widescreen responsivo: as listagens e painéis de filtro devem ocupar a largura útil da tela em harmonia com a toolbar superior, utilizando contêineres principais configurados com `max-width: 1400px` responsivo
- [ ] Apresentação de dados de cadastros densos e listagens em formato de tabela clássica responsiva (com `.app-table` e `.table-container` com efeitos glassmorphic) em vez de grids de cartões, que espalham os dados de forma ineficiente
- [ ] Labels e textos visuais repetidos referentes a Nota Fiscal (ex: "Nota Fiscal", "NF", "NF-e", "NFC-e") **jamais** devem ser hardcoded em strings literais espalhadas pelas Views, Hubs ou Controllers. É **obrigatório** utilizar as propriedades estáticas da classe centralizadora de nomenclatura `@TerminologiaErp` (ex: `TerminologiaErp.AbreviacaoNotaFiscalEletronica`, `TerminologiaErp.NomeNotaFiscalPlural`).
- [ ] Filtros de busca persistidos na URL como query strings — recarregar a página mantém o estado
- [ ] Filtros favoritos: se referência deletada, descartar silenciosamente o parâmetro e notificar sem bloquear
- [ ] O óbvio tem que ser dito: sempre avisar o usuário da sua regra de negócio
- [ ] **Navegação Canônica de Módulos (UX)**: Dashboards ou Hubs de primeiro nível (como ERP, CRM, Varejo, etc.) nunca devem conter atalhos ou cards de cadastro direto (`/Novo`). A jornada correta orienta o usuário a acessar primeiro a listagem do recurso (ex: `/Contatos`) e, de dentro dela, utilizar o botão de novo cadastro na toolbar superior.
- [ ] **Separação Canônica de Cabeçalhos (UI/UX)**: A Topbar superior (`_ToolbarBase.cshtml`) é estritamente global (marca + controles de sistema e avatar de perfil). Títulos de tela, subtítulos, botões voltar breadcrumb e botões de ação específicos (Novo, Histórico, Salvar) nunca devem ficar espremidos no topo da barra. Eles devem ser renderizados automaticamente pela partial como um elegante Page Header (`.page-header`) posicionado no início do conteúdo da página, preservando a harmonia e o espaço útil.
- [ ] **Mensagens de Sucesso Simplificadas (UI/UX)**: Mensagens ou termos como "gravação realizada", "registro salvo" ou "linha salva" são proibidos na interface com o usuário por serem técnicos e poluídos. Use sempre **"Concluído"** — simples, objetivo e flexível para qualquer ação concluída com sucesso.
- [ ] **Evitar Flexbox Pai em Filhos Grid Layout (UI/UX)**: Nunca aplique `display: flex` em contêineres que envolvem grades de formulário (`form-grid` / `.form-grid-2`), pois o contexto de flexbox anula a capacidade dos filhos de respeitarem regras de `grid-column` (como `span 2`), empurrando os elementos adjacentes e quebrando o alinhamento. Prefira `display: block` no elemento pai.
- [ ] **Isolamento de Stacking Context com Fundos Fixos (UI)**: Layouts e contêineres principais (como `.app-layout` e `.main-content`) que abrigam os formulários de negócio precisam explicitamente possuir `position: relative` e `z-index: 1` (ou superior) para flutuarem corretamente acima de fundos decorativos como o `.bg-animated` (que usa `position: fixed; z-index: 0`), prevenindo que o conteúdo fique invisível ("tela preta/branca") no tema dark ou light.
- [ ] **Injeção de Estilo inline de Posicionamento nos TagHelpers**: Todos os TagHelpers de formulário (`AppInput`, `AppSelect`, `AppTextarea`) devem obrigatoriamente capturar o atributo `style` fornecido na view e injetá-lo no elemento contêiner externo (`form-group`), garantindo que regras inline de posicionamento de grade (ex: `style="grid-column: span 2;"`) sejam respeitadas no layout final.
- [ ] **Mapeamento e Organização de Telas (UI/UX)**: Toda tela do sistema deve pertencer a uma pasta correspondente ao seu recurso/entidade (ex: `Pages/CRM/Contatos/`) e estar devidamente catalogada no [Manifesto de Mapeamento e Controle de Telas](file:///c:/Projeto/Projeto/Documenta%C3%A7%C3%A3o/Manifestos/Manifesto_Mapeamento_e_Controle_de_Telas.md).

---

## 🚀 DIRETRIZES DE DEPLOY E VPS — KingHost & Produção

- [ ] **Segurança de Chaves Privadas no Windows (`id_rsa`)**: Chaves SSH privadas usadas no Windows para deploy automatizado não podem ter permissões muito abertas. Para evitar rejeição pelo OpenSSH, desative a herança e conceda privilégios exclusivos de leitura/gravação apenas ao usuário ativo com os comandos:
  `icacls "caminho_chave" /inheritance:r`
  `icacls "caminho_chave" /grant "${env:USERNAME}:(R,W)"`
- [ ] **Caminhos de Runtime Absolutos em VPS Multiversão**: Nunca confie no comando global `dotnet` em servidores que contêm múltiplos runtimes instalados (como conflitos entre .NET 8 e .NET 10 na KingHost). Use sempre o caminho do executável absoluto do runtime específico do usuário (ex: `/root/.dotnet/dotnet`) para iniciar a aplicação de forma previsível.
- [ ] **Bloqueio de Canal SSH em Execuções em Segundo Plano**: Ferramentas de conexão automatizada SSH (como `SSH.NET`) bloqueiam o fluxo de execução se o terminal esperar pela finalização de um processo de background. Ao iniciar serviços com `nohup`, chame o comando de forma assíncrona (`BeginExecute()`) ou certifique-se de redirecionar todos os descritores de arquivo (incluindo entrada padrão) com `< /dev/null &`.

---

## 🔄 AUDITORIA — Rastreio de mudanças

- [ ] Movimentações importantes (criação, aprovação, exclusão, mudança de permissão) emitem trace para `auditoria_registros`
- [ ] `auditoria_registros` é imutável — sem soft delete nessa tabela
- [ ] A tela de auditoria responde em dois modos: ciclo de vida de um registro OU histórico do usuário

---

## 🤖 DIRETRIZES E REGRAS PARA INTELIGÊNCIA ARTIFICIAL (IA)

- [ ] **Limpeza de Sujeiras de Desenvolvimento**: A IA nunca deve deixar arquivos temporários, zips de backup (ex: `app.zip`), arquivos `.txt` com logs, ou scripts `.sql` / `.csx` dispersos. Todo script pontual deve ser removido após a execução.
- [ ] **Rastreabilidade e Migração de Banco**: A IA nunca deve sugerir alterações estruturais de banco em arquivos de scripts avulsos. Qualquer DDL/DML novo deve ser anexado ao arquivo canônico `Scripts/banco_script_completo.sql` em formato idempotente (`CREATE TABLE IF NOT EXISTS`, `ADD COLUMN IF NOT EXISTS`, `ON CONFLICT DO NOTHING`).
- [ ] **Proibição de Alterar Arquivo de Próximos Passos**: A IA é terminantemente proibida de modificar o arquivo `Documentação/Proximos_Passos.md`. Este arquivo é de uso exclusivo do desenvolvedor humano para controle de tarefas.
- [ ] **Design e Interface Premium**: Ao criar ou refatorar UIs, a IA deve sempre aplicar a paleta de cores institucional baseada em variáveis HSL (design premium/glassmorphic). O uso de placeholders visuais (caixas cinzas, textos "Lorem Ipsum", imagens quebradas) é terminantemente proibido.
- [ ] **CQRS e Padrão Razor Pages**: A IA deve seguir estritamente o padrão MediatR para manipulação de regras de negócio. Handlers devem ser unitários e focados. Razor Pages devem herdar de `AppBasePageModel` e fazer o mapeamento correto.
- [ ] **Sem Scripts Destrutivos**: A IA não pode criar ou rodar scripts do tipo PowerShell que manipulem de forma descuidada a codificação de arquivos `.cshtml` ou `.cs` para evitar corrupção de encoding (ERR-001).
- [ ] **Autenticação e Multi-Tenancy**: Qualquer query Dapper criada pela IA em repositórios deve obrigatoriamente incluir filtros por `@contaId` para manter o isolamento absoluto de dados.
- [ ] **Proibição de Executores Redundantes (SqlRunner)**: É expressamente proibido recriar ou introduzir projetos/scripts adicionais de execução de SQL (como `SqlRunner`, `TempSqlRunner` ou executores locais com credenciais hardcoded). Toda a sincronização e execução de scripts de banco deve usar exclusivamente a ferramenta canônica `DbSync` (`Projetos_Auxiliares/DbSync`).

---

## 🏁 CHECKLIST ANTES DE FAZER PR

- [ ] O código compila sem erros?
- [ ] Existe teste de Caminho Feliz para o Handler?
- [ ] Existe teste de Validação de Negócio?
- [ ] A query Dapper filtra por `@contaId`?
- [ ] Não há `HttpContext` na camada de Aplicação?
- [ ] As mensagens para o usuário estão em PT-BR?
- [ ] O nome da classe reflete a metodologia, não o cliente?
- [ ] Arquivos temporários de desenvolvimento foram removidos (scripts de migração, logs de build)?
- [ ] Toda integração externa está atrás de uma interface?
- [ ] Dados pessoais de terceiros exigem consentimento registrado?
- [ ] A nova tela foi mapeada, auditada e documentada no [Manifesto de Mapeamento e Controle de Telas](file:///c:/Projeto/Projeto/Documenta%C3%A7%C3%A3o/Manifestos/Manifesto_Mapeamento_e_Controle_de_Telas.md)?
- [ ] O código passou pelas diretrizes de conduta de IA (sem arquivos temporários espalhados e com alterações consolidadas no `Scripts/banco_script_completo.sql`)?
