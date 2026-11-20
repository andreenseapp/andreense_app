# Manifesto Oficial — Banco de Dados do ERP Web

> ⚠️ **ESCOPO DESTE DOCUMENTO**: Este manifesto faz parte do conjunto de documentação geral do projeto ERP. Para o projeto ativo `c:\Projeto\Sistema\Modulo_Monolitico`, o **manifesto de referência único é o [`Manifesto_Monolitico.md`](./Manifesto_Monolitico.md)**. Consulte este arquivo apenas para regras de nomenclatura de banco de dados e design de schema que se aplicam globalmente a todos os projetos.

Este documento define as diretrizes e regras absolutas arquiteturais para criação e modificação de tabelas no banco de dados do nosso ERP.


## 1. Regras Fundamentais

* **Extensões**: Toda vez que for incluída uma nova `EXTENSION` no PostgreSQL, ela deve obrigatoriamente ser adicionada na rotina de backup.
* **Relacionamentos**: O banco **usa FK físicas** (`FOREIGN KEY ... REFERENCES`) nas tabelas estruturais e relacionais (ex: `usuarios_empresas`, `empresas`, `contatos_pessoas_fisicas`). As constraints são aplicadas via `ALTER TABLE` após a criação das tabelas. Ao criar uma nova tabela, **avalie a necessidade de FK** nas colunas relacionais — em tabelas transacionais de alto volume pode-se optar por integridade apenas via código, mas nas tabelas estruturais a FK é padrão.

## 2. Padrão de Nomenclatura (Snake Case)

O PostgreSQL gerencia nomes em caracteres minúsculos por baixo dos panos. 
Em vez de utilizar *CamelCase* forçado (que exigiria o uso constante de aspas duplas, tornando as queries muito verbosas ou passíveis de erros), **o modelo inteiro utiliza `snake_case`**.
* **Exemplos válidos:** `usuario_interno`, `data_inclusao`, `valor_parcela`.
* Nunca gerar nada como `"usuarioInterno"`.

## 3. Campos Padrão (Colunas Base)

Absolutamente **toda tabela** no sistema deve herdar as seguintes colunas padrão, com exata semântica:

```sql
url_imagem TEXT,
status TEXT NOT NULL DEFAULT 'ativo',
ordem INT NOT NULL DEFAULT 0,
data_inclusao TIMESTAMPTZ NOT NULL DEFAULT now(),
data_ultima_atualizacao TIMESTAMPTZ,
data_exclusao TIMESTAMPTZ -- ou deleted_at TIMESTAMPTZ
```

### Explicação do Controle de Tempo (TIMESTAMPTZ)
*Usar sempre `TIMESTAMPTZ` para forçar o servidor a salvar como data e hora Universal (UTC), cabendo ao sistema operacional do cliente a responsabilidade de converter e apresentar o dado no fuso correto (Timezone).*
* `data_inclusao`: Garantida de nunca ser nula. Representa o _timestamp_ de momento exato de inserção.
* `data_ultima_atualizacao`: É iniciada como nula e automatizada por _TRIGGER_ do banco toda vez que houver um `UPDATE` na linha.
* `data_exclusao` / `deleted_at`: Usada exclusivamente para **Soft Delete**. O registro não é sumariamente deletado do bloco, apenas recebe um _timestamp_ nesta coluna evidenciando sua remoção lógica.

## 4. Chaves Primárias (Identificadores)

* Quando criar uma nova tabela, a coluna primária que recebe o `UUID` deverá chamar-se apenas e **estritamente `id`**.
* Jamais crie anomalias de nomenclatura como `id_produto` ou `produto_id` no campo primário da própria tabela. Os nomes estendidos são restritos apenas para relacionamentos.
* Alguns registros padrões globais são incluídos com UUID "fixo" pré-determinado para o isolamento correto dos dados (vide Sessão 6).

## 5. Gatilhos de Auditoria

* Criou ou alterou a estrutura de uma tabela? **Confira a necessidade de vincular** o trigger respectivo na engine de "auditoria" para a captura de log de mundanças (`auditoria_registros`).

## 6. Sementes e Usuário Padrão Globais (MASTER)

Registros bases preexistentes ou defaults da plataforma pertencem explicitamente ao **Usuário Master**, que possui o `UUID: 00000000-0000-0000-0000-000000000000`. 
* **Leitura**: Quando o sistema consulta, por exemplo, a tabela "Formas de Pagamento", o Back End buscará sempre os registros do Usuário Master _mais (+) os registros do Usuário Logado_. Dessa forma, o usuário novo já nasce com opções padronizadas inseridas pela equipe.

## 7. Performance (Índice de Registros Ativos)

Para toda tabela (sobretudo as grandes corporativas ou com soft delete), crie um índice de deleção lógica que acelerará as buscas ignorando os _Soft Deletes_.

```sql
-- Exemplos de Criação de Índice de Performance
CREATE INDEX IF NOT EXISTS idx_nome_tabela_not_deleted ON public.nome_tabela(deleted_at) WHERE deleted_at IS NULL;
-- ou usando data_exclusao:
CREATE INDEX IF NOT EXISTS idx_nome_tabela_not_deleted ON public.nome_tabela(data_exclusao) WHERE data_exclusao IS NULL;
```

## 8. Autorização Multi-Tenant e RBAC (Controle por Grupos)

No contexto de plataformas SaaS, **um Usuário pode pertencer a múltiplas Contas e Empresas**, agindo sob _Perfis_ (Papéis) distintos em cada uma delas (Admin na Empresa A, Colaborador na Empresa B).
Por isso, a modelagem de acessos e restrições nunca pode residir solitariamente na tabela `usuarios`. O controle rigoroso se constrói da seguinte forma hierárquica:

1. **`contas`**: O Workspace Titular. Engloba múltiplas Empresas (Matriz e Filiais).
2. **`empresas`**: Cada filial / CNPJ dentro de uma Conta.
3. **`usuarios_contas`**: Faz o vínculo entre um Usuário e uma Conta. O campo `is_titular` indica o proprietário do workspace.
4. **`usuarios_empresas`**: A tabela pivô definitiva que rege o acesso operacional. Une `empresa_id`, `usuario_id` e o perfil de acesso (`perfil` — do Enum `perfil`: Titular, Administrador, Recepcionista, Vendas, Profissional, Financeiro).

Isso engessa o sistema impedindo que as decisões de acesso vazem lateralmente entre inquílinos do SaaS (Conceito de Bounded Company Context).
