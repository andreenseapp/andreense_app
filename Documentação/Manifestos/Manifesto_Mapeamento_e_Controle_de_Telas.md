# 📺 Manifesto de Mapeamento e Controle de Telas (Checklist Master de UI/UX)

> **Premissa**: A consistência da interface com o usuário (UI/UX), segurança e componentização não podem depender apenas da herança de classes bases. Precisamos de um mapeamento explícito, tela por tela, auditando o que já foi implementado, o que falta ajustar e as travas específicas de cada tela.

Este manifesto define a estrutura organizacional das telas no projeto e fornece a planilha/checklist de controle master para auditar cada tela individualmente contra o nosso design system e regras de negócio.

---

## 📁 1. Regra de Organização: Estrutura de Pastas e Rotas

Para manter rotas limpas e organização lógica, o projeto Razor Pages segue as seguintes regras de diretório:

### ❌ O que NÃO fazer:
- **Uma pasta por arquivo `.cshtml`**: Criar `Pages/CRM/Contatos/Listagem/Index.cshtml` e `Pages/CRM/Contatos/NovoCadastro/Index.cshtml` gera redundância física e rotas excessivamente profundas.
- **Telas soltas na raiz**: Encher a pasta `Pages/` com arquivos órfãos sem contexto de negócio.
- **Sobrecarga de Responsabilidade (Puxadinhos)**: Não use uma tela para fazer alguma operação de outra responsabilidade. O correto é termos uma tela própria de responsabilidade "única" para isso, ou certificar-se de que faça MUITO sentido a operação se encaixar naquele Hub/Perfil.

###  Como deve ser (Padrão por Entidade/Recurso):
Agrupamos as telas relacionadas a um mesmo **Recurso/Entidade** dentro de uma pasta única que descreve o recurso. Os arquivos dentro da pasta refletem suas ações:

```
📁 Pages/
└── 📁 CRM/
    └── 📁 Contatos/                  ← Pasta unificada do recurso
        ├── Index.cshtml              ← Rota: /Contatos (Listagem)
        ├── Index.cshtml.cs           ← Model (Herdando AppListBasePageModel)
        ├── Novo.cshtml               ← Rota: /Contatos/Novo (Cadastro)
        ├── Novo.cshtml.cs            ← Model
        ├── Editar.cshtml             ← Rota: /Contatos/Editar (Edição)
        ├── Editar.cshtml.cs          ← Model
        ├── Detalhes.cshtml           ← Rota: /Contatos/Detalhes (Visualização/Histórico)
        └── Detalhes.cshtml.cs        ← Model
```

> 💡 **Exceções**: Telas completamente isoladas e de propósito único (ex: `MeuPerfil.cshtml`, `SeletorDemo/Index.cshtml`) podem viver em suas respectivas pastas dedicadas ou diretamente sob o módulo correspondente.

---

## 🛡️ 2. Matriz de Auditoria: Os 9 Pilares de Conformidade de uma Tela

Toda tela adicionada ou modificada deve ser avaliada contra estes 9 pilares. Mesmo que a tela herde de uma classe base (como `AppListBasePageModel`), o desenvolvedor deve marcar e atestar o comportamento na tabela de controle.

| # | Pilar | O que verificar |
|---|---|---|
| **1** | **Classe Base** | Herda de `AppBasePageModel` ou `AppListBasePageModel<T, F>`? Nunca de `PageModel` puro. |
| **2** | **Isolamento de Tenant** | Filtra e isola os dados utilizando `ContaId` em todas as queries e comandos back-end? |
| **3** | **Componentização** | Substituiu 100% dos inputs nativos por customizados (`<app-input>`, `<app-select>`, `<app-button>`)? |
| **4** | **UX de Filtros** | Painel de busca expansível com `<details>/<summary>` (fechado por padrão, exceto se houver filtros ativos)? |
| **5** | **Persistência** | Usa `RestaurarOuSalvarFiltros("key")` para salvar filtros em cookies e reflete-os como QueryStrings na URL? |
| **6** | **Mensagens** | As mensagens de sucesso usam estritamente o termo simplificado **"Concluído"** no TempData/UI? |
| **7** | **Segurança / RBAC** | Possui trava de segurança ou regra específica (ex: permissões, auditoria de escrita, autorização expressa)? |
| **8** | **Testes** | O Handler correspondente possui teste de Caminho Feliz e de Regra de Negócio? |
| **9** | **Rascunho (Snapshot)** | O recurso `form-snapshot.js` (Auto-save) deve ser usado **APENAS** em telas de Criação (`Novo.cshtml`). **NUNCA** adicione em telas de Edição (`Editar.cshtml`) para evitar que dados antigos do cache do navegador sobreescrevam os dados atualizados do banco. |

---

## 📊 3. Checklist Master de Telas do Sistema

Use esta tabela para acompanhar e auditar o estado de cada tela ativa na aplicação.

> **Legenda de Status**:
> - `[x] OK` - Totalmente implementado e aderente aos padrões
> - `[/] AJUSTAR` - Parcialmente implementado, necessita de correções/refatoração
> - `[ ] PENDENTE` - Tela vazia, protótipo ou não-componetizada

### 👤 Módulo: Autenticação & Perfil (`/Auth`, `/Base`)

| Tela / Rota | Arquivos Relacionados | Classe Base | Tenant | Comp. | Filtros | Sucesso "Concluído" | Trava Seg. / RBAC | Status | Observações / Pendências |
|---|---|---|---|---|---|---|---|---|---|
| **Login** (`/Auth/Login`) | `Auth/Login.cshtml` | `AppBasePageModel` | N/A | `[x]` | N/A | N/A | Cookie HTTPOnly | `[x] OK` | Tela de autenticação base funcionando. |
| **Cadastro** (`/Auth/Cadastro`) | `Auth/Cadastro.cshtml` | `AppBasePageModel` | N/A | `[x]` | N/A | N/A | N/A | `[x] OK` | Cadastro inicial de contas/tenants. |
| **Meu Perfil** (`/MeuPerfil`) | `MeuPerfil.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | `[x]` | Valida usuário ativo | `[x] OK` | Central de preferências e troca de tema. |
| **Meus Vínculos** (`/Perfil/MeusVinculos`) | `Perfil/MeusVinculos/Index.cshtml` | `AppBasePageModel` | N/A | `[x]` | N/A | `[x]` | Apenas Authorize | `[x] OK` | Central de gestão de empresas aceitas e convites pendentes. |

---

### 🤝 Módulo: CRM (`/CRM`)

| Tela / Rota | Arquivos Relacionados | Classe Base | Tenant | Comp. | Filtros | Sucesso "Concluído" | Trava Seg. / RBAC | Status | Observações / Pendências |
|---|---|---|---|---|---|---|---|---|---|
| **Dashboard CRM** (`/CRM`) | `CRM/Index.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | N/A | Apenas ERP/CRM role | `[x] OK` | Hub inicial do módulo CRM. |
| **Listagem Contatos** (`/Contatos`) | `CRM/Contatos/Index.cshtml` | `AppListBasePageModel` | `[x]` | `[/]` | `[x]` | `[x]` | Valida `dono_id` | `[/] AJUSTAR` | Refatorar inputs de filtro (`type="date"`) para o tag helper `<app-input>`. |
| **Novo Contato** (`/Contatos/Novo`) | `CRM/Contatos/Novo.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | `[x]` | Write permission | `[x] OK` | Utiliza componentes do Design System. |
| **Editar Contato** (`/Contatos/Editar`) | `CRM/Contatos/Editar.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | `[x]` | Valida pertença da `ContaId` | `[x] OK` | Completamente migrado e auditado. |
| **Detalhes Contato** (`/Contatos/Detalhes`) | `CRM/Contatos/Detalhes.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | N/A | Auditoria activa | `[x] OK` | Visualização compacta com histórico de logs. |

---

### 📦 Módulo: ERP (`/ERP`)

| Tela / Rota | Arquivos Relacionados | Classe Base | Tenant | Comp. | Filtros | Sucesso "Concluído" | Trava Seg. / RBAC | Status | Observações / Pendências |
|---|---|---|---|---|---|---|---|---|---|
| **Dashboard ERP** (`/ERP`) | `ERP/Index.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | N/A | Role Base | `[x] OK` | Painel geral de vendas e inventário. |
| **Listagem Produtos** (`/ERP/Produtos`) | `ERP/Produtos/Index.cshtml` | `AppListBasePageModel` | `[x]` | `[ ]` | `[x]` | `[x]` | N/A | `[/] AJUSTAR` | Migrar tabela clássica para usar Tag Helpers de formulário e filtros unificados. |
| **Editar Produto** (`/ERP/Produtos/Editar`) | `ERP/Produtos/Editar.cshtml` | `AppBasePageModel` | `[x]` | `[ ]` | N/A | `[x]` | Estoque Manager | `[/] AJUSTAR` | Substituir inputs nativos antigos por `<app-input>` e `<app-select>`. |

---

### 🏢 Módulo: Base & Corporativo (`/Base`)

| Tela / Rota | Arquivos Relacionados | Classe Base | Tenant | Comp. | Filtros | Sucesso "Concluído" | Trava Seg. / RBAC | Status | Observações / Pendências |
|---|---|---|---|---|---|---|---|---|---|
| **Seletor Empresas** (`/Base/Empresas/Selecionar`) | `Base/Empresas/Selecionar.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | `[x]` | Valida vínculo usuário | `[x] OK` | Painel de troca rápida de tenant ativa. |
| **Lista Vendedores** (`/Base/Vendedores`) | `Base/Vendedores/Index.cshtml` | `AppListBasePageModel` | `[x]` | `[x]` | `[x]` | `[x]` | Financeiro/Comercial | `[x] OK` | Concluido nos padrões da plataforma. |
| **Lista Tipos Vendedor** (`/Base/TiposVendedor`) | `Base/TiposVendedor/Index.cshtml` | `AppListBasePageModel` | `[x]` | `[x]` | `[x]` | `[x]` | Admin/Comercial | `[x] OK` | Totalmente componentizado. |
| **Configurações** (`/Base/Configuracoes`) | `Base/Configuracoes/Index.cshtml` | `AppBasePageModel` | `[x]` | `[x]` | N/A | `[x]` | Somente Administrador | `[x] OK` | Gerenciador de parâmetros globais. |

---

### 🛠️ Módulo: Administrativo & Segurança (`/InfraAdmin`)

| Tela / Rota | Arquivos Relacionados | Classe Base | Tenant | Comp. | Filtros | Sucesso "Concluído" | Trava Seg. / RBAC | Status | Observações / Pendências |
|---|---|---|---|---|---|---|---|---|---|
| **Dashboard Admin** (`/InfraAdmin`) | `InfraAdmin/Index.cshtml` | `AppBasePageModel` | N/A | `[x]` | N/A | N/A | SuperAdmin apenas | `[x] OK` | Painel geral de monitoramento e auditoria. |
| **Histórico de Logins** (`/InfraAdmin/HistoricoLogin`) | `InfraAdmin/HistoricoLogin/Index.cshtml` | `AppListBasePageModel` | N/A | `[x]` | `[x]` | N/A | SuperAdmin apenas | `[x] OK` | Auditado. |
| **Gestão de Grupos/RBAC** (`/InfraAdmin/Grupos`) | `InfraAdmin/Grupos/Index.cshtml` | `AppListBasePageModel` | `[x]` | `[x]` | `[x]` | `[x]` | SuperAdmin apenas | `[x] OK` | Controle de acessos e perfis de segurança. |
| **Últimos Conectados** (`/InfraAdmin/UltimosConectados`) | `InfraAdmin/UltimosConectados/Index.cshtml` | `AppListBasePageModel` | N/A | `[x]` | N/A | N/A | SuperAdmin apenas | `[x] OK` | Monitor de sessões ativas do servidor. |

---

## 📝 4. Template para Adicionar Nova Tela ao Controle

Toda vez que criar uma tela no sistema, copie o template abaixo, preencha as colunas e adicione ao respectivo módulo acima.

```markdown
| Nome da Tela (`/Rota`) | [Modulo/Pasta/Nome.cshtml](file:///c:/Projeto/Projeto/Sistema/Modulo_Monolitico/ModuloMonolitico.Web/Pages/Pasta/Nome.cshtml) | Classe Base Usada | Tenant ID ativo? [Sim/Não] | Usa TagHelpers customizados? [Sim/Não] | Filtro expansível com Cookie? [Sim/Não/NA] | Usa "Concluído" no sucesso? [Sim/Não/NA] | Travas Específicas / RBAC | `[ ] PENDENTE` | Detalhes de pendências, ideias de refatoração ou notas do dev |
```

---

## 🔒 5. Segurança e Auditoria Dinâmica de Telas

Mesmo com a herança de classes bases (como `AppBasePageModel`), algumas travas precisam ser explícitas e validadas de forma estrita em cada caso de uso:

### 1. Travas no `OnGetAsync` e `OnPostAsync`
Não dependa apenas da anotação `[Authorize]`. Certifique-se de que os Handlers e consultas Dapper e EF Core filtram o recurso baseado no ID do usuário logado E no Tenant ativo:
```csharp
// Exemplo de segurança reativa obrigatória
var contato = await _contatoRepositorio.ObterPorIdAsync(id);
if (contato == null || contato.ContaId != ContaId)
{
    // Ação proibida ou registro inexistente para este tenant
    MensagemErro = "Recurso não encontrado ou acesso não autorizado.";
    return RedirectToPage("./Index");
}
```

### 2. Controle de Permissões Específicas por Ação
Ao atualizar ou deletar dados, certifique-se de validar se o usuário pertence a um grupo com a permissão necessária. Isso deve ser verificado explicitamente no `OnPost`:
```csharp
if (!await _autorizacaoServico.UsuarioPossuiPermissaoAsync(UsuarioId, "crm_contatos_excluir"))
{
    MensagemErro = "Você não tem permissão para realizar esta exclusão.";
    return RedirectToPage();
}
```

### 3. Logs de Auditoria
Ações de escrita (exclusão, atualização e inserção) em telas críticas devem disparar explicitamente uma gravação no log de auditoria do registro:
```csharp
await _auditoriaServico.RegistrarAcaoAsync(
    UsuarioId, 
    "Exclusão de Contato", 
    $"Contato '{contato.Nome}' foi excluído por solicitação do usuário.");
```

---

## 🏁 Ciclo de Vida da Tela (A "Definition of Done")

Uma tela só sai de `[/] AJUSTAR` para `[x] OK` no Manifesto quando:
1. **Componetização completa**: Nenhuma tag `<input>`, `<select>` ou `<button>` nativa fora do padrão do Design System foi esquecida na tela.
2. **Filtros aderentes**: Se a tela lista dados, o filtro está dentro de um `<details>` com cookies e URL sincronizados.
3. **Consistência de texto**: Mensagens de sucesso mostram apenas **"Concluído"**.
4. **Sem bypass de segurança**: O código do back-end valida rigorosamente a `ContaId` E o escopo de atuação do usuário logado.
5. **Rascunhos limpos**: A tela de Edição NÃO possui a tag de `form-snapshot.js`.
6. **Auto-atualização**: A tabela deste manifesto foi marcada como `[x] OK`.
