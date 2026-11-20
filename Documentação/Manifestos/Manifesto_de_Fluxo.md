# Manifesto de Fluxo — ERP Web (Definição Geral do Sistema)

> ⚠️ **ESCOPO DESTE DOCUMENTO**: Este manifesto descreve os fluxos de negócio e o mapeamento de telas/tabelas do ERP de forma geral. Para o projeto ativo `c:\Projeto\Sistema\Modulo_Monolitico`, o **manifesto de referência único é o [`Manifesto_Monolitico.md`](./Manifesto_Monolitico.md)**. O schema do banco em DBML completo está em: **[Manifesto_DBML.md](./Manifesto_DBML.md)**



---

## ⚠️ Regra de Ouro: Idioma e Nomenclatura
> **Toda a base de código, banco de dados, variáveis e logs DEVERÃO ser escritos em Português.**
> O uso de termos em inglês (ex: `client`, `product`, `order`) para o domínio do sistema é **expressamente proibido**. Essa regra visa facilitar a interpretação imediata, a auditoria de regras de negócio e a revisão de códigos (mesmo aqueles gerados por IA) por parte do proprietário. O inglês só é aceitável quando exigido por bibliotecas externas, frameworks ou sintaxe de linguagens.

---

## Legenda de Cores (VS Code Preview)

| Símbolo | Significado |
|---|---|
| 🔵 <span style="color:#4FC3F7">**Azul**</span> | Nome da Tabela |
| 🟢 <span style="color:#81C784">**Verde**</span> | Coluna(s) relevante(s) |
| 🟡 <span style="color:#FFD54F">**Amarelo**</span> | Tela do sistema |
| 🟣 <span style="color:#CE93D8">**Roxo**</span> | Exemplo de dado real |

## Nota Arquitetural — Identidade, Consumidor e Endereços

> **`pessoas_fisicas` e `pessoas_juridicas` são cadastros OFFLINE**, scoped por `conta_id`.
> São criados manualmente pela empresa para clientes que não têm (ou nunca terão) login na plataforma.
>
> **`usuarios` são identidades ONLINE**, universais, sem vínculo direto com as tabelas offline.
> Os dois mundos são separados intencionalmente — não existe FK entre eles.
>
> **Como isso se resolve no agendamento?** Pelo padrão polimórfico `cliente_id + cliente_tipo`:
>
> | `cliente_tipo` | Que tipo de cliente é |
> |---|---|
> | `'pessoas_fisicas'` | Cadastro offline — criado manualmente pela empresa |
> | `'pessoas_juridicas'` | Empresa cliente — cadastrada offline |
> | `'usuarios'` | Usuário da plataforma com login online |
> | `null` + `nome_cliente_avulso` preenchido | Walk-in — sem nenhum cadastro |
>
> **Exemplo de fluxo:** A recepcionista abre o modal de novo agendamento e busca o cliente. Se ele tiver login na plataforma, aparece em `usuarios`. Se for cadastrado manualmente, aparece em `pessoas_fisicas`. Se nenhum dos dois, ela digita só o nome no campo `nome_cliente_avulso`. O mesmo `agendamentos.cliente_id` carrega os três casos via `cliente_tipo`.

**Endereços — onde cada um fica:**

| Necessidade | Onde fica |
|---|---|
| Endereço pessoal de cliente/fornecedor offline | `enderecos_pf` / `enderecos_pj` |
| Endereço de cobrança do plano SaaS | Colunas diretas em `contas` |
| Endereço da empresa (matriz/filial) | Colunas diretas em `empresas` |

---

> 🔗 **Matching entre mundo offline e plataforma — via CNPJ/CPF com Autorização**
>
> **Regra de unicidade:** O mesmo CNPJ não pode aparecer duas vezes na mesma conta → `UNIQUE INDEX (conta_id, cnpj)` em `pessoas_juridicas`. Idem CPF em `pessoas_fisicas`.
>
> **Regra de privacidade:** Nenhuma conta pode vincular outra sem consentimento explícito. O campo `conta_plataforma_id` / `usuario_plataforma_id` só é preenchido após aceite.
>
> **Fluxo de autorização** (tabela: <span style="color:#4FC3F7">`solicitacoes_vinculo`</span>):
>
> ```
> 1. Conta A cadastra fornecedor com CNPJ X
> 2. Sistema detecta: esse CNPJ pertence à Conta B na plataforma
> 3. Sistema cria solicitacoes_vinculo { status: 'pendente', papel_proposto: 'fornecedor' }
>    → Conta B recebe notificação: "Salão do João quer te vincular como fornecedor. Aceitar?"
> 4a. Conta B ACEITA → pessoas_juridicas.conta_plataforma_id = id-da-conta-B ✅
> 4b. Conta B REJEITA → link não acontece. Conta A não sabe o motivo ❌
> ```
>
> Quando aceito, abre caminhos para: importação automática de XMLs, histórico cruzado B2B, pedidos entre contas da plataforma.



---




## [ASSINATURAS E FINANCEIRO SAAS]

**A plataforma vai oferecer opções de planos de assinatura que limitam a quantidade de usuários com login e empresas (filiais) cadastradas.**
- 🔵 Tabela: <span style="color:#4FC3F7">`saas_planos`</span>
- 🟢 Colunas: <span style="color:#81C784">`limite_usuarios`, `limite_empresas`, `valor_mensal`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Landing page de preços / CRUD Admin*</span>
- 🟣 Exemplos de planos:
  - <span style="color:#CE93D8">`{ nome: "Starter",    limite_usuarios: 3,  limite_empresas: 1, valor_mensal: 49.90  }` _// 1 CNPJ, dono + 2 funcionários com login_</span>
  - <span style="color:#CE93D8">`{ nome: "Business",   limite_usuarios: 10, limite_empresas: 3, valor_mensal: 149.90 }` _// Grupo com matriz + 2 filiais, até 10 logins_</span>
  - <span style="color:#CE93D8">`{ nome: "Enterprise", limite_usuarios: 50, limite_empresas: 20, valor_mensal: 499.90 }` _// Rede grande, franquias_</span>

> ⚠️ **`limite_usuarios` ≠ limite de profissionais. E `usuario` é uma identidade universal.**
> - Um **`usuario`** é como um e-mail: existe uma vez no sistema e pode pertencer a **N contas simultaneamente** — titular da própria empresa e funcionário de outras ao mesmo tempo. Igual à vida real.
> - **`usuarios_contas`** → registra o vínculo com a conta (`is_titular = true` se for o dono).
> - **`usuarios_empresas`** → registra em qual empresa dentro da conta ele opera e com qual perfil.
> - **`limite_usuarios`** restringe quantos usuários *o titular pode convidar para a conta dele*. Cada conta tem sua própria assinatura e seu próprio limite — independentes entre si.
> - **Profissionais da agenda** (`empresas_profissionais`) são pessoas que *executam serviços* na agenda — podem ou não ter login. **Não há limite de plano para profissionais cadastrados.**

---

**A plataforma vai permitir o uso de cupons promocionais para abater o valor da assinatura.**
- 🔵 Tabela: <span style="color:#4FC3F7">`saas_cupons_desconto`</span>
- 🟢 Colunas: <span style="color:#81C784">`codigo`, `tipo_desconto`, `valor_desconto`, `data_fim`, `quantidade_total`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Checkout de assinatura / CRUD Admin*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ codigo: "PROMO50", tipo_desconto: "percentual", valor_desconto: 50, data_fim: "2026-12-31" }`</span>

---

**A conta vai poder cadastrar seus métodos de pagamento da assinatura via gateway terceiro (ex: Pagar.me). O sistema jamais armazena dados reais de cartão — apenas tokens e metadados retornados pelo gateway.**
- 🔵 Tabela: <span style="color:#4FC3F7">`saas_metodos_pagamento`</span>
- 🟢 Colunas: <span style="color:#81C784">`gateway_customer_id`, `gateway_payment_method_id`, `ultimos_4_digitos`, `bandeira`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Carteira / Financeiro na configuração da conta logada*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ bandeira: "Visa", ultimos_4_digitos: "4242", gateway_payment_method_id: "pm_abc123XYZ" }`</span>

---

**A empresa vai possuir uma assinatura ativa que vincula o plano escolhido ao método de pagamento.**
- 🔵 Tabela: <span style="color:#4FC3F7">`saas_planos_assinaturas`</span>
- 🟢 Colunas: <span style="color:#81C784">`saas_plano_id`, `metodo_pagamento_id`, `status`, `proxima_fatura`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Painel "Minha Assinatura"*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ status: "ATIVA", valor: 149.90, data_inicio: "2026-01-01", proxima_fatura: "2026-04-01" }`</span>

---

**O sistema vai gerar as parcelas mensais (faturas) da assinatura, controlando o que está pago ou pendente.**
- 🔵 Tabela: <span style="color:#4FC3F7">`saas_planos_assinaturas_faturas`</span>
- 🟢 Colunas: <span style="color:#81C784">`numero_parcela`, `status`, `valor`, `data_vencimento`, `data_pagamento`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Histórico de faturas (baixar / pagar)*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ numero_parcela: 3, status: "pendente", valor: 149.90, data_vencimento: "2026-04-05" }`</span>

---

## [CLIENTES, FORNECEDORES E ENDEREÇOS]

> **Padrão de UI: Tela dividida em abas por papel**
> A tela de cadastro é única para a pessoa (física ou jurídica). Os dados de identificação ficam sempre visíveis no topo. Abaixo, abas separadas para cada papel que a pessoa pode ter. **Cada aba só aparece / é habilitada se o papel estiver ativo para aquela empresa.** Um botão "Ativar como Cliente" cria o registro correspondente.

---

**Os dados de identificação da pessoa ficam no topo da tela — são o master data compartilhado por todas as filiais da Conta.**

Para **Pessoa Física:**
- 🔵 Tabela: <span style="color:#4FC3F7">`pessoas_fisicas`</span> *(vinculada à `conta_id`)*
- 🟢 Colunas: <span style="color:#81C784">`nome`, `cpf`, `rg`, `data_nascimento`, `email`, `telefone`, `celular`, `observacao`</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "João da Silva", cpf: "123.456.789-00", telefone: "(11) 98888-0000" }`</span>

Para **Pessoa Jurídica:**
- 🔵 Tabela: <span style="color:#4FC3F7">`pessoas_juridicas`</span> *(vinculada à `conta_id`)*
- 🟢 Colunas: <span style="color:#81C784">`razao_social`, `nome_fantasia`, `cnpj`, `inscricao_estadual`, `email`, `telefone`, `celular`</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ razao_social: "Distribuidora ABC Ltda", cnpj: "12.345.678/0001-99", nome_fantasia: "ABC Dist." }`</span>

> Os flags `is_cliente`, `is_fornecedor`, `is_profissional` em `pessoas_fisicas`/`pessoas_juridicas` são **campos utilitários de filtro rápido** — derivados da existência dos registros nas tabelas abaixo. Não carregam dados de papel em si.

---

**Aba "Endereços" — comum às duas telas, exibida sempre.**
- 🔵 Tabelas: <span style="color:#4FC3F7">`enderecos_pf`</span> / <span style="color:#4FC3F7">`enderecos_pj`</span>
- 🟢 Coluna: <span style="color:#81C784">`tipo_endereco`</span> *(PF: PRINCIPAL, COBRANCA, ENTREGA — PJ: PRINCIPAL, FILIAL)*
- 🟡 Tela: <span style="color:#FFD54F">*Aba "Endereços" dentro do cadastro da pessoa*</span>
- 🟣 Exemplo PF: <span style="color:#CE93D8">`{ tipo_endereco: "ENTREGA", logradouro: "Rua das Flores", numero: "42", cidade: "São Paulo", uf: "SP" }`</span>

---

**Aba "Cliente" — aparece/é criada via botão "Ativar como Cliente". Parâmetros exclusivos do papel de cliente *para esta empresa*.**
- 🔵 Tabela: <span style="color:#4FC3F7">`empresas_clientes`</span> *(vinculada à `empresa_id` — cada filial pode ter configurações distintas para o mesmo cliente)*
- 🟢 Colunas: <span style="color:#81C784">`pessoa_id`, `pessoa_tipo`, `limite_credito`, `forma_pagamento_padrao_id`, `condicao_pagamento_padrao_id`, `natureza_operacao_padrao_id`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Aba "Cliente" dentro do cadastro da pessoa*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ limite_credito: 5000.00, forma_pagamento_padrao: "Pix", condicao_pagamento_padrao: "30 dias" }`</span>

---

**Aba "Fornecedor" — aparece/é criada via botão "Ativar como Fornecedor". Parâmetros exclusivos do papel de fornecedor *para esta empresa*.**
> Campos completamente independentes do papel de Cliente — o mesmo João pode pagar via Pix como cliente e receber via Boleto como fornecedor.
- 🔵 Tabela: <span style="color:#4FC3F7">`empresas_fornecedores`</span> *(vinculada à `empresa_id`)*
- 🟢 Colunas: <span style="color:#81C784">`pessoa_id`, `pessoa_tipo`, `forma_pagamento_padrao_id`, `condicao_pagamento_padrao_id`, `natureza_operacao_padrao_id`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Aba "Fornecedor" dentro do cadastro da pessoa*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ forma_pagamento_padrao: "Boleto", condicao_pagamento_padrao: "30/60/90 dias", natureza_operacao_padrao: "Compra" }`</span>

---


## [AGENDAMENTO E PROFISSIONAIS]

**A empresa vai definir quais pessoas (físicas ou jurídicas) atuarão como profissionais prestadores de serviço.**
- 🔵 Tabelas: <span style="color:#4FC3F7">`empresas_funcoes`</span> *(define o cargo: "Barbeiro", "Médico")* + <span style="color:#4FC3F7">`empresas_profissionais`</span>
- 🟢 Colunas: <span style="color:#81C784">`pessoa_id`, `funcao_id`, `pessoa_tipo`, `data_inicio`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de profissionais / CRUD de funções*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ funcao: "Barbeiro", pessoa: "João Silva", data_inicio: "2025-01-15" }`</span>

---

**A empresa vai configurar os horários de trabalho, dias da semana e pausas padrão para cada profissional.**
- 🔵 Tabela: <span style="color:#4FC3F7">`empresas_profissionais_horarios`</span>
- 🟢 Colunas: <span style="color:#81C784">`is_segunda..is_domingo`, `hora_inicio`, `hora_fim`, `pausa_inicio`, `pausa_fim`, `duracao_minutos`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Aba de grade de horários no cadastro do profissional*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ is_segunda: true, is_sexta: true, hora_inicio: "09:00", hora_fim: "18:00", pausa_inicio: "12:00", pausa_fim: "13:00", duracao_minutos: 30 }`</span>

---

**O profissional ou a empresa poderão registrar momentos de exceção que impedem agendamentos (ex: médico, férias).**
- 🔵 Tabela: <span style="color:#4FC3F7">`bloqueios_agenda`</span>
- 🟢 Colunas: <span style="color:#81C784">`empresa_id`, `profissional_id`, `inicio`, `fim`, `motivo`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Botão "Bloquear horário" direto na agenda*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ motivo: "Consulta médica", inicio: "2026-03-20 14:00", fim: "2026-03-20 16:00" }`</span>

---

**O sistema manterá um calendário de dias não úteis para alertar durante marcações.**
- 🔵 Tabela: <span style="color:#4FC3F7">`feriados`</span>
- 🟢 Colunas: <span style="color:#81C784">`data`, `nome`, `uf`, `tipo`</span> *(tipo: nacional, estadual, municipal)*
- 🟡 Tela: <span style="color:#FFD54F">*CRUD global de feriados / alertas no calendário*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Carnaval", data: "2026-02-16", tipo: "nacional", uf: null }`</span>

---

**A empresa terá um painel de regras locais, como o texto padrão para mensagens de WhatsApp aos clientes.**
- 🔵 Tabela: <span style="color:#4FC3F7">`agendamento_config`</span> *(um registro único por empresa — coluna `empresa_id [unique]`)*
- 🟢 Coluna: <span style="color:#81C784">`texto_whatsapp_agendamento`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Configurações de agendamento*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ texto_whatsapp_agendamento: "Olá {nome}, lembrete do seu agendamento em {data} às {hora}!" }`</span>

---

**Vai ser possível criar um agendamento no formato de calendário vinculando cliente, profissional e previsão de pagamento.**
- 🔵 Tabela: <span style="color:#4FC3F7">`agendamentos`</span>
- 🟢 Colunas: <span style="color:#81C784">`cliente_id`, `profissional_id`, `inicio`, `fim`, `situacao`, `gera_financeiro`, `forma_pagamento_id`, `intervalo_dias`, `campos_customizados`, `qtd_parcelas`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Calendário principal / modal "Novo agendamento"*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ situacao: "confirmado", inicio: "2026-03-15 10:00", fim: "2026-03-15 11:00", valor_total: 80.00, gera_financeiro: true }`</span>

---

**No agendamento, vai ser possível detalhar quais serviços ou produtos (variações) exatos foram consumidos e o tempo de cada um.**
- 🔵 Tabela: <span style="color:#4FC3F7">`agendamento_servicos`</span>
- 🟢 Colunas: <span style="color:#81C784">`produto_variacao_id`, `profissional_id`, `duracao_minutos`, `valor`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Grid de serviços dentro do modal de agendamento*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ produto_variacao: "Degradê", profissional: "João", duracao_minutos: 45, valor: 60.00 }`</span>

---

**No agendamento, vai ser possível aplicar abatimentos financeiros específicos sobre um dos serviços prestados.**
- 🔵 Tabela: <span style="color:#4FC3F7">`servico_desconto`</span>
- 🟢 Colunas: <span style="color:#81C784">`empresa_id`, `agendamento_servicos_id`, `valor`, `percentual`, `data_inicio`, `data_fim`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Modal de desconto na linha do serviço agendado*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ percentual: 10.0, valor: 6.00, data_inicio: "2026-03-15 10:00" }`</span>

---

## [PRODUTOS E PREÇOS]

**A empresa vai poder organizar seu portfólio de produtos e serviços agrupando-os de forma lógica.**
- 🔵 Tabela: <span style="color:#4FC3F7">`produto_categorias`</span> *(vinculada à `conta_id` — master data compartilhado)*
- 🟢 Colunas: <span style="color:#81C784">`nome`, `descricao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de categorias*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Coloração", descricao: "Serviços de tingimento e luzes" }`</span>

---

**A empresa vai poder registrar os fabricantes dos seus produtos para facilitar a triagem.**
- 🔵 Tabela: <span style="color:#4FC3F7">`marcas`</span> *(vinculada à `conta_id` — master data compartilhado)*
- 🟢 Colunas: <span style="color:#81C784">`nome`, `descricao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de marcas*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "L'Oréal Professional", descricao: "Linha profissional de coloração" }`</span>

---

**A empresa vai cadastrar o "cabeçalho" do item, centralizando as regras tributárias (NCM, CST, CFOP) e origem.**
> No ERP, "Produto" engloba tanto Mercadoria (física) quanto Serviço (abstrato). A coluna `tipo_item` é quem dita toda a lógica fiscal.
- 🔵 Tabela: <span style="color:#4FC3F7">`produtos`</span> *(vinculada à `conta_id` — master data compartilhado)*
- 🟢 Colunas: <span style="color:#81C784">`tipo_item`, `ncm`, `cfop_padrao`, `cst_padrao`, `origem`, `controla_lote`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Cabeçalho do cadastro de produto*</span>
- 🟣 Exemplo mercadoria: <span style="color:#CE93D8">`{ nome: "Pneu Aro 15", tipo_item: "mercadoria", ncm: "40111000", origem: 0 }`</span>
- 🟣 Exemplo serviço: <span style="color:#CE93D8">`{ nome: "Alinhamento 3D", tipo_item: "servico", ncm: null }`</span>

---

**A empresa vai cadastrar as especificações do que realmente é vendido (código de barras, SKU, tempo de execução se for serviço).**
> Estrutura Master/Detail. Ex: Produto = "Corte de Cabelo" → Variação 1: "Degradê" | Variação 2: "Máquina Zero".
- 🔵 Tabela: <span style="color:#4FC3F7">`produto_variacoes`</span>
- 🟢 Colunas: <span style="color:#81C784">`nome`, `sku`, `codigo_barras`, `unidade_medida`, `tempo_minutos`, `estoque_minimo`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Grid / aba de variações dentro do cadastro de produto*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Degradê", sku: "COR-DEG-01", codigo_barras: "7891234567890", tempo_minutos: 45 }`</span>

---

**A empresa vai poder criar diferentes cenários comerciais (ex: "Tabela Padrão", "Tabela Atacado", "Promoção").**
- 🔵 Tabela: <span style="color:#4FC3F7">`contextos_preco`</span> *(vinculada à `conta_id` — master data compartilhado)*
- 🟢 Coluna: <span style="color:#81C784">`nome`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de tabelas de preço*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Tabela Atacado", descricao: "Preços para pedidos acima de 100 unidades" }`</span>

---

**A empresa vai definir exatamente quanto custa cada variação de produto/serviço em cada cenário comercial.**
- 🔵 Tabela: <span style="color:#4FC3F7">`precos`</span>
- 🟢 Colunas: <span style="color:#81C784">`produto_variacao_id`, `contexto_preco_id`, `valor`, `percentual_comissao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Aba de precificação dentro do cadastro da variação*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ variacao: "Degradê", contexto: "Tabela Padrão", valor: 80.00, percentual_comissao: 15.0 }`</span>

---

## [ESTOQUE]

**A empresa vai poder dividir seu espaço físico em grandes galpões ou setores.**
- 🔵 Tabela: <span style="color:#4FC3F7">`areas_estoque`</span> *(vinculada à `empresa_id` — estoque é por filial)*
- 🟢 Colunas: <span style="color:#81C784">`nome`, `descricao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Árvore de locais — nó pai*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Loja Principal", descricao: "Área de vendas e atendimento" }`</span>

---

**A empresa vai poder criar subdivisões exatas dentro das áreas indicando prioridade de saída.**
- 🔵 Tabela: <span style="color:#4FC3F7">`locais_estoque`</span>
- 🟢 Colunas: <span style="color:#81C784">`area_estoque_id`, `tipo`, `prioridade_de_saida`, `codigo_visual`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Árvore de locais — nó filho*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Prateleira A1", tipo: "pulmao", prioridade_de_saida: 1, codigo_visual: "PA-001" }`</span>

---

**O sistema vai apontar a "foto" exata do estoque: quantos itens de uma variação, lote e validade existem em cada local.**
- 🔵 Tabela: <span style="color:#4FC3F7">`estoque_saldo`</span>
- 🟢 Colunas: <span style="color:#81C784">`produto_variacao_id`, `local_estoque_id`, `lote`, `data_validade`, `quantidade_atual`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Consulta / Dashboard de saldos*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ lote: "L2026A", data_validade: "2027-06-30", quantidade_atual: 150.0, status: "disponivel" }`</span>

---

**O sistema vai registrar o "filme": cada vez que um item entrou, saiu ou trocou de lugar.**
- 🔵 Tabela: <span style="color:#4FC3F7">`estoque_movimentacoes`</span> *(imutável — `bigserial`, sem soft delete)*
- 🟢 Colunas: <span style="color:#81C784">`tipo_movimentacao`, `quantidade`, `motivo`, `data_movimentacao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Tela de apontamento (entrada/saída) / Extrato do item (Kardex)*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ tipo_movimentacao: "SAIDA_VENDA", quantidade: 2, motivo: "Venda #1042", data_movimentacao: "2026-03-11T20:00:00" }`</span>

---

## [SISTEMA E LEGAL]

**A plataforma vai guardar e versionar as Políticas de Privacidade e Termos de Uso vigentes.**
- 🔵 Tabela: <span style="color:#4FC3F7">`documentos_legais`</span>
- 🟢 Colunas: <span style="color:#81C784">`tipo`, `versao`, `titulo`, `texto`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Editor de texto rico para o admin do sistema*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ tipo: "TERMOS_USO", versao: "2.0", titulo: "Termos de Uso — versão 2026" }`</span>

---

**A plataforma vai registrar incontestavelmente qual usuário assinou/aceitou qual versão dos termos e quando.**
- 🔵 Tabela: <span style="color:#4FC3F7">`documentos_aceites`</span>
- 🟢 Colunas: <span style="color:#81C784">`usuario_id`, `documento_legal_id`, `data_aceite`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Modal de bloqueio pós-login exigindo o aceite*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ usuario: "joao@email.com", documento: "Termos v2.0", data_aceite: "2026-03-11T20:30:00Z" }`</span>

---

**O sistema vai gravar silenciosamente tudo o que é alterado no banco de dados (antes e depois) para rastreabilidade.**
- 🔵 Tabela: <span style="color:#4FC3F7">`auditoria_registros`</span> *(imutável — `bigserial`, sem soft delete, populada por trigger)*
- 🟢 Colunas: <span style="color:#81C784">`conta_id`, `table_name`, `action`, `record_id`, `old_values`, `new_values`, `user_id`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Grid gerencial de consulta de logs do sistema*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ table_name: "produtos", action: "UPDATE", old_values: {preco: 50.00}, new_values: {preco: 60.00} }`</span>

---

## [VENDAS E CAIXA (PDV)]

**A empresa vai poder realizar vendas diretas (balcão) ou concluir um agendamento transformando-o em venda.**
- 🔵 Tabela: <span style="color:#4FC3F7">`vendas`</span>
- 🟢 Colunas: <span style="color:#81C784">`caixa_id`, `cliente_id`, `agendamento_id`, `valor_total`, `status`</span>
- 🟡 Tela: <span style="color:#FFD54F">*PDV (Ponto de Venda) / Checkout de agendamento*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ valor_total: 130.00, status: "concluida", agendamento_id: "uuid-agend-xyz" }`</span>

---

**A venda vai detalhar exatamente quais variações de produtos ou serviços foram consumidos, quantidades e valores.**
- 🔵 Tabela: <span style="color:#4FC3F7">`venda_itens`</span>
- 🟢 Colunas: <span style="color:#81C784">`produto_variacao_id`, `profissional_id`, `quantidade`, `valor_unitario`, `valor_desconto`, `valor_total`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Grid de itens no PDV*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ produto: "Degradê", quantidade: 1, valor_unitario: 80.00, valor_desconto: 0, valor_total: 80.00 }`</span>

---

**A empresa vai registrar os meios de pagamento disponibilizados para seus clientes (ex: Pix, Cartão, Dinheiro).**
- 🔵 Tabela: <span style="color:#4FC3F7">`formas_pagamento`</span> *(PDV — vinculada à `conta_id` — master data compartilhado)*
- 🟢 Colunas: <span style="color:#81C784">`nome`, `tipo`, `taxa_percentual`, `dias_recebimento`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de formas de pagamento internas*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ nome: "Pix", tipo: "pix", taxa_percentual: 0, dias_recebimento: 1 }`</span>

---

**A empresa precisará abrir um caixa (turno) para registrar entradas e saídas e fechá-lo ao fim do dia.**
- 🔵 Tabela: <span style="color:#4FC3F7">`controle_caixa`</span>
- 🟢 Colunas: <span style="color:#81C784">`usuario_id`, `saldo_inicial`, `saldo_final`, `data_abertura`, `data_fechamento`, `status`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Modal de abertura e fechamento de caixa*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ saldo_inicial: 200.00, status: "aberto", data_abertura: "2026-03-11T08:00:00" }`</span>

---

## [FINANCEIRO (CONTAS A PAGAR E RECEBER)]

**A conclusão de uma venda a prazo ou um agendamento vai gerar automaticamente uma previsão de recebimento.**
- 🔵 Tabela: <span style="color:#4FC3F7">`contas_receber`</span>
- 🟢 Colunas: <span style="color:#81C784">`origem_id`, `origem_tipo`, `cliente_id`, `valor`, `data_vencimento`, `situacao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Dashboard financeiro / Grid de recebíveis*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ origem_tipo: "venda", valor: 130.00, data_vencimento: "2026-03-20", situacao: "pendente" }`</span>

---

**A empresa vai poder lançar suas despesas e compras com fornecedores para controlar o fluxo de caixa.**
- 🔵 Tabela: <span style="color:#4FC3F7">`contas_pagar`</span>
- 🟢 Colunas: <span style="color:#81C784">`fornecedor_id`, `descricao`, `valor`, `data_vencimento`, `situacao`</span>
- 🟡 Tela: <span style="color:#FFD54F">*Grid de contas a pagar*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ descricao: "Compra tintas L'Oréal", valor: 850.00, data_vencimento: "2026-03-25", situacao: "pendente" }`</span>

---

> ℹ️ **Modelo financeiro em duas camadas:**
>
> | Camada | Tabelas | Quando é usada |
> |---|---|---|
> | **PDV / Agendamento** (simples) | `contas_receber` / `contas_pagar` | Vendas de balcão, agendamentos finalizados, despesas avulsas |
> | **ERP Completo** (credor/devedor) | `contas_a_receber` / `contas_a_pagar` | Pedidos de venda ERP, notas fiscais, operações B2B com parcelamento |
>
> As tabelas simples cobrem o dia a dia do PDV. As tabelas ERP carregam dados completos de credor/devedor, nota fiscal e conta bancária.

---

## [CONFIGURAÇÕES FISCAIS E COMERCIAIS]

**A empresa vai poder criar modelos de operação ("Venda", "Devolução", "Compra") que definem quais impactos uma transação terá — se gera financeiro, se movimenta estoque e qual CFOP padrão.**
- 🔵 Tabela: <span style="color:#4FC3F7">`naturezas_operacao`</span> *(vinculada à `conta_id` — master data compartilhado)*
- 🟢 Colunas: <span style="color:#81C784">`descricao`, `cfop_padrao`, `gera_financeiro`, `movimenta_estoque`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de Naturezas de Operação*</span>
- 🟣 Exemplo: <span style="color:#CE93D8">`{ descricao: "Venda de Mercadoria", cfop_padrao: "5102", gera_financeiro: true, movimenta_estoque: true }`</span>

---

**A empresa vai poder definir condições de parcelamento que serão associadas a clientes e fornecedores (ex: à vista, 30/60/90 dias).**
- 🔵 Tabela: <span style="color:#4FC3F7">`condicoes_pagamento`</span> *(vinculada à `conta_id` — master data compartilhado)* + <span style="color:#4FC3F7">`condicao_pagamento_parcelas`</span>
- 🟢 Colunas: <span style="color:#81C784">`nome`, `descricao`</span> + parcelas: <span style="color:#81C784">`dias_para_vencimento`, `percentual`</span>
- 🟡 Tela: <span style="color:#FFD54F">*CRUD de Condições de Pagamento*</span>
- 🟣 Exemplo condição: <span style="color:#CE93D8">`{ nome: "30/60/90 dias" }`</span>
- 🟣 Exemplo parcelas: <span style="color:#CE93D8">`[{ dias: 30, percentual: 33.33 }, { dias: 60, percentual: 33.33 }, { dias: 90, percentual: 33.34 }]`</span>

---

## Jornada do Usuário (Passo a Passo de Uso)

```
1. ONBOARDING        → Cadastro → cria Conta (Tenant) → escolhe Plano SaaS
                                                        → aceita Termos de Uso
2. SETUP ESTRUTURAL  → Configura Segmento / Atividade → Formas de Pagamento
                                                       → Regras de Agendamento
3. CADASTROS BASE    → Insere Profissionais + Horários → Clientes + Fornecedores
4. CATÁLOGO/ESTOQUE  → Cria Áreas/Locais → Produtos + Variações
                                         → Tabelas de Preço
                                         → Apontamento de Saldo Inicial
5. OPERAÇÃO DIÁRIA   → Cliente marca horário na Agenda
                     → Sistema valida feriados, bloqueios, grade do profissional
6. EXECUÇÃO/PDV      → Serviço realizado → Agendamento vira Venda
                     → Baixa automática no Estoque (estoque_movimentacoes)
7. ACERTO FINANCEIRO → Pagamento registrado no Caixa
                     → Parcelado/prazo → alimenta Contas a Receber
                     → Despesas → lançadas em Contas a Pagar
```

---

## Índice de Módulos e Tabelas

### Módulo Núcleo e SaaS (Workspace / Tenant)
| Tabela | Vínculo | Descrição |
|---|---|---|
| `segmentos` | — | Segmentos de mercado (ex: Saúde, Beleza) |
| `atividades` | — | Atividades específicas por segmento |
| `saas_planos` | — | Planos de assinatura da plataforma |
| `saas_cupons_desconto` | — | Cupons de desconto na assinatura |
| `contas` | — | **Workspace Titular** (Tenant raiz) |
| `empresas` | `conta_id` | Filiais / CNPJs dentro da Conta |
| `usuarios` | — | Identidade de acesso do usuário |
| `usuarios_contas` | `conta_id` | Vínculo usuário ↔ conta (`is_titular`) |
| `usuarios_empresas` | `empresa_id` | Vínculo usuário ↔ empresa (perfil/papel) |
| `saas_metodos_pagamento` | `conta_id` | Tokens do gateway (nunca dados reais) |
| `saas_planos_assinaturas` | `conta_id` | Assinatura ativa |
| `saas_planos_assinaturas_faturas` | `conta_id` | Faturas mensais geradas |

### Módulo de Pessoas (Master Data da Conta)
| Tabela | Vínculo | Descrição |
|---|---|---|
| `pessoas_fisicas` | `conta_id` | CPF — cliente / fornecedor / profissional |
| `enderecos_pf` | `pessoa_fisica_id` | Endereços da pessoa física |
| `pessoas_juridicas` | `conta_id` | CNPJ — cliente / fornecedor |
| `enderecos_pj` | `pessoa_juridica_id` | Endereços da pessoa jurídica |
| `empresas_clientes` | `empresa_id` | Parâmetros de cliente por empresa (limite crédito, forma pgto padrão) |
| `empresas_fornecedores` | `empresa_id` | Parâmetros de fornecedor por empresa |
| `solicitacoes_vinculo` | `conta_solicitante_id` | Fluxo de autorização para vincular cadastro offline a conta/usuário real |

### Módulo de Agendamentos e Profissionais
| Tabela | Vínculo | Descrição |
|---|---|---|
| `empresas_funcoes` | `empresa_id` | Cargos/especialidades (Barbeiro, Médico...) |
| `empresas_profissionais` | `empresa_id` | Quem executa o serviço |
| `empresas_profissionais_horarios` | `profissional_id` | Grade de horários semanais |
| `bloqueios_agenda` | `empresa_id` | Exceções que bloqueiam horários |
| `agendamentos` | `empresa_id` | Marcações no calendário |
| `agendamento_servicos` | `agendamento_id` | O que vai ser feito no agendamento |
| `servico_desconto` | `agendamento_servicos_id` | Desconto por serviço |
| `agendamento_config` | `empresa_id [unique]` | Configuração de agendamento (1 por empresa) |
| `feriados` | — | Calendário nacional/estadual/municipal |

### Módulo Sistema e Legal
| Tabela | Vínculo | Descrição |
|---|---|---|
| `documentos_legais` | — | Termos / Políticas versionados |
| `documentos_aceites` | `usuario_id` | Registro imutável de aceite |
| `auditoria_registros` | `conta_id` | Log de todas as alterações no BD (trigger) |

### Módulo de Produtos, Estoque e Preços (Master Data da Conta)
| Tabela | Vínculo | Descrição |
|---|---|---|
| `produto_categorias` | `conta_id` | Grupos de produtos |
| `marcas` | `conta_id` | Fabricantes / marcas |
| `produtos` | `conta_id` | Cabeçalho — inf. fiscal (NCM, CFOP, CST) |
| `produto_variacoes` | `produto_id` | O que realmente é vendido (SKU, barras) |
| `contextos_preco` | `conta_id` | Tabelas de preço (Padrão, Atacado...) |
| `precos` | `produto_variacao_id` + `contexto_preco_id` | Valor por variação por contexto |
| `areas_estoque` | `empresa_id` | Galpões / setores |
| `locais_estoque` | `area_estoque_id` | Prateleiras / subdivisões |
| `estoque_saldo` | `local_estoque_id` | "Foto" atual do estoque por lote |
| `estoque_movimentacoes` | `estoque_saldo_id` | "Filme" histórico (Kardex) |

### Módulo de Configurações Fiscais
| Tabela | Vínculo | Descrição |
|---|---|---|
| `config_responsavel_tecnico` | — | Dados do responsável técnico (CSRT para SEFAZ) |
| `certificados_digitais` | `empresa_id` | Certificado A1 (PFX) por empresa |
| `configuracoes_emissoes` | `empresa_id` | Série, modelo, ambiente, CSC, próximo nº |
| `naturezas_operacao` | `conta_id` | Define se gera financeiro, movimenta estoque |
| `regras_fiscais` | `empresa_id` + `natureza_operacao_id` | Matriz NCM × UF × CFOP × CST × alíquotas |
| `formas_pagamento_fiscal` | `conta_id` | Com `codigo_sefaz` para TAG `<tPag>` XML |
| `condicoes_pagamento` | `conta_id` | Parcelamentos (ex: 30/60/90 dias) |
| `condicao_pagamento_parcelas` | `condicao_pagamento_id` | Detalhamento das parcelas |
| `contas_bancarias` | `empresa_id` | Dados bancários para boleto/TED |

### Módulo de Vendas e Caixa (PDV)
| Tabela | Vínculo | Descrição |
|---|---|---|
| `formas_pagamento` | `conta_id` | Modos de recebimento simples (PDV) |
| `controle_caixa` | `empresa_id` | Abertura / fechamento de turno |
| `vendas` | `empresa_id` | Venda concluída (balcão ou de agendamento) |
| `venda_itens` | `venda_id` | Itens da venda |

### Módulo de Orçamentos e Pedidos
| Tabela | Vínculo | Descrição |
|---|---|---|
| `orcamentos` | `empresa_id` | Proposta comercial |
| `orcamento_itens` | `orcamento_id` | Itens do orçamento com tributos |
| `pedidos` | `empresa_id` | Pedido de venda confirmado |
| `pedido_itens` | `pedido_id` | Itens do pedido com tributos |
| `pedido_parcelas` | `pedido_id` | Parcelas do pedido |
| `embarques` | `pedido_id` | Separação física dos itens |
| `embarque_itens` | `embarque_id` | Itens separados por embarque |

### Módulo de Faturamento Fiscal
| Tabela | Vínculo | Descrição |
|---|---|---|
| `faturas` | `pedido_id` | NFe/NFCe — header com chave SEFAZ |
| `fatura_itens` | `fatura_id` | Itens com tributos detalhados (ICMS/IPI/PIS/COFINS/IBS/CBS/IS) |

### Módulo Financeiro
| Tabela | Vínculo | Descrição |
|---|---|---|
| `contas_receber` | `empresa_id` | Recebíveis simples (PDV/agendamento) |
| `contas_pagar` | `empresa_id` | Contas a pagar simples |
| `contas_a_receber` | `empresa_id` | Recebíveis ERP completo (credor/devedor) |
| `contas_a_pagar` | `empresa_id` | Contas a pagar ERP completo |
| `contas_a_receber_movimentacao` | `conta_a_receber_id` | Histórico de movimentos de recebíveis |
| `contas_a_pagar_movimentacao` | `conta_a_pagar_id` | Histórico de movimentos de pagáveis |
| `contas_a_receber_comissao` | `conta_a_receber_id` | Comissão gerada por recebível |
| `contas_a_receber_comissao_deducao` | `comissao_id` | Deduções aplicadas à comissão |
| `comissao_deducao_regra` | `conta_id` | Regras de dedução padrão |
| `conta_a_receber_credito` | `empresa_id` | Crédito disponível a receber |
| `conta_a_pagar_credito` | `empresa_id` | Crédito disponível a pagar |

### Módulo App (Andreense — Assinaturas do App Mobile)
> ⚠️ *[TODO: confirmar se devem estar no mesmo banco do ERP ou em banco separado]*

| Tabela | Descrição |
|---|---|
| `planos_app` | Planos de assinatura do App mobile |
| `assinaturas_app` | Assinaturas ativas dos usuários no App |
| `servicos_disponiveis_app` | Serviços avulsos disponíveis no App |
| `servico_contratacoes_app` | Contratações de serviços avulsos |
| `pagamentos_app` | Pagamentos processados no App |
| `pagamentos_logs_app` | Log de eventos do gateway de pagamento |
| `configuracoes_taxas_app` | Taxas por método/parcelamento |
| `usuario_recursos_app` | Recursos ativos por usuário (limites/quotas) |
