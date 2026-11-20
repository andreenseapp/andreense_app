# Manifesto DBML — ERP Web (Schema do Banco de Dados)

> ⚠ï¸ **ESCOPO DESTE DOCUMENTO**: Este é o schema completo em DBML de todos os módulos do ERP. Para o projeto ativo `c:\Projeto\Sistema\Modulo_Monolitico`, o **manifesto de referência único é o [`Manifesto_Monolitico.md`](./Manifesto_Monolitico.md)**. Este arquivo é consultado pela IA e pelo `DbmlParser` para gerar o `Scripts/banco_script_completo.sql`.

Este arquivo contém o schema completo em formato DBML (Database Markup Language).
Foi extraído do Manifesto de Fluxo para manter o documento narrativo limpo e legível.
O arquivo de referência narrativa é: **Manifesto_De_Fluxo.md**


> ⚠ï¸ **REGRA DE OURO:** Todo o schema, nome de tabelas, colunas e enums **DEVE ser mantido em português**. É expressamente proibido traduzir conceitos para inglês (ex: alterar `cliente` para `client`, `faturas` para `invoices`). A integridade orgânica em pt-br é o que garante a manutenibilidade para a equipe atual e para as revisões de inteligência artificial.

> ðŸ§  **Nota de Contexto (Gerada por IA - Documentação Cognitiva):** 
> Todos os comentários injetados lado a lado nas declarações (iniciados por `// [Por que existe?]`) servem de blindagem contra "alucinações" de interpretação e revisões vazias de outras matrizes de Inteligência Artificial. Elas documentam o porquê de uma coluna de dados existir, em vez de assumir sua literalidade. Ademais, se achar que uma coluna está faltando, vide o bloco de "Omissões Propositais" no rodapé deste dbml, tudo é feito conscientemente!


---

Enum situacao_agendamento {
  pendente
  confirmado
  concluido
  cancelado
  bloqueado
}

Enum perfil {
  Administrador
  Recepcionista
  Vendas
  Profissional
  Financeiro
}

Enum situacao_conta {
  pendente
  paga
  cancelada
  atrasada
}


Table segmentos {
  id integer [primary key] // [Por que existe?] Identificador PK universal (imutável)
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
}

Table atividades {
  id integer [primary key] // [Por que existe?] Identificador PK universal (imutável)
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
}

Table saas_planos {
  id integer [primary key] // [Por que existe?] Identificador PK universal (imutável)
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  valor_mensal numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  limite_usuarios integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos
  limite_empresas integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Table config_responsavel_tecnico {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  cnpj varchar(14) [not null] // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  x_contato varchar(60) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de config_responsavel_tecnico
  email varchar(60) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  fone varchar(14) // [Por que existe?] Armazena característica pertinente e nativa do domínio de config_responsavel_tecnico
  id_csrt varchar(2) // [Por que existe?] Armazena característica pertinente e nativa do domínio de config_responsavel_tecnico
  hash_csrt varchar(28) // [Por que existe?] Armazena característica pertinente e nativa do domínio de config_responsavel_tecnico
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table certificados_digitais {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  nome_arquivo text // [Por que existe?] Armazena característica pertinente e nativa do domínio de certificados_digitais
  pfx_base64 text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de certificados_digitais
  senha text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de certificados_digitais
  data_vencimento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de certificados_digitais
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: certificados_digitais.empresa_id > empresas.id

Table configuracoes_emissoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  certificado_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  ambiente integer [not null, default: 2] // 1=Producao, 2=Homologacao
  serie integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  modelo integer [not null, default: 65] // 55=NFe, 65=NFCe
  proximo_numero_nf integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  csc_id text // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  csc_token text // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  tipo_emissao_padrao integer [default: 1] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  tipo_impressao_default integer [default: 1] // 1=Retrato, 2=Paisagem
  data_ultima_emissao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_emissoes
  tratamento_desconto varchar(20) [not null, default: 'RATEIO'] // RATEIO ou DESTAQUE
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: configuracoes_emissoes.empresa_id > empresas.id
Ref: configuracoes_emissoes.certificado_id > certificados_digitais.id

Table saas_cupons_desconto {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  codigo varchar(50) [unique, not null] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo_desconto varchar(50) // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor_desconto numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_inicio date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_cupons_desconto
  data_fim date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_cupons_desconto
  quantidade_total integer // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  quantidade_utilizada integer [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

// ==========================================
// MÓDULO: NÚCLEO E SAAS (WORKSPACE / TENANT)
// ==========================================

// Comunicados e Enquetes Globais da Plataforma SaaS
Table saas_comunicados {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  titulo varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  mensagem text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo varchar(50) [not null, default: 'aviso'] // 'aviso' ou 'enquete'
  modulo_alvo varchar(100) // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas (Ex: 'ERP', 'Todos')
  data_expiracao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
}

Table saas_comunicados_opcoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  comunicado_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio
  texto varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
}
Ref: saas_comunicados_opcoes.comunicado_id > saas_comunicados.id

Table saas_comunicados_leituras {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  comunicado_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  opcao_selecionada_id uuid // Nulo se for apenas um aviso (sem opções)
  data_leitura timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}
Ref: saas_comunicados_leituras.comunicado_id > saas_comunicados.id
Ref: saas_comunicados_leituras.conta_id > contas.id
Ref: saas_comunicados_leituras.usuario_id > usuarios.id
Ref: saas_comunicados_leituras.opcao_selecionada_id > saas_comunicados_opcoes.id
// A Conta é o Workspace Titular. Uma Conta pode englobar Múltiplas Empresas (Matriz e Filiais).
// O plano SaaS e o faturamento da plataforma são atrelados Ã  Conta (Grupo).
Table contas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  portal_slug varchar(255) [unique, not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas
  nome varchar(255) [not null] // Ex: "Grupo Beleza Total"
  email varchar(255) [unique] // [Por que existe?] Variável de alerta ou recuperação de autenticação
  telefone varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  saas_cupom_desconto_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas
  // Endereço de cobrança da assinatura SaaS
  cep varchar(20) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  logradouro varchar(255) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  numero varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas
  complemento varchar(100) // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas
  bairro varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  cidade varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  uf varchar(2) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  pais varchar(100) [default: 'Brasil'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: contas.saas_cupom_desconto_id > saas_cupons_desconto.id

Table empresas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  razao_social varchar(255) // [Por que existe?] Identidade para exibição primária de marca/usuário
  email varchar(255) [unique] // [Por que existe?] Variável de alerta ou recuperação de autenticação
  telefone varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  cnpj varchar(20) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  cpf varchar(20) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  inscricao_estadual varchar(50) // [Por que existe?] Inscrição IE/IM demandada pela geração do XML Estadual/Sinc
  inscricao_municipal varchar(50) // [Por que existe?] Inscrição IE/IM demandada pela geração do XML Estadual/Sinc
  cnae_principal varchar(20) // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  crt integer [default: 1] // 1=Simples, 2=Simples Excesso, 3=Normal, 4=MEI
  logradouro varchar(255) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  numero varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  complemento varchar(100) // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  cep varchar(20) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  bairro varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  uf varchar(2) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  cidade varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  pais varchar(100) [default: 'Brasil'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  segmento_id integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  atividade_id integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  config_campos_customizados jsonb [default: '[]'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas.conta_id > contas.id
Ref: empresas.segmento_id > segmentos.id
Ref: empresas.atividade_id > atividades.id

// O usuario é a identidade universal de login (como um Gmail).
// NÃO vinculado a contatos_pessoas_fisicas/juridicas — essas são cadastros OFFLINE por conta.
// O cliente de um agendamento pode ser um usuario OU uma contato_pessoa_fisica —
// essa decisão é resolvida pelo campo `cliente_tipo` na tabela agendamentos.
Table usuarios {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  email varchar(255) [unique, not null] // [Por que existe?] Variável de alerta ou recuperação de autenticação
  senha_hash text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de usuarios
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  email_validado boolean [default: false] // [Por que existe?] Indica se o usuário já confirmou a posse do e-mail
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no Cloudflare R2
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Table codigos_validacao {
  id bigserial [primary key]
  email varchar(255) [not null]
  codigo varchar(10) [not null]
  tipo varchar(50) [not null] // 'validacao_email', 'recuperacao_senha', '2fa'
  expira_em timestamp [not null]
  usado boolean [default: false]
  data_inclusao timestamp [default: `now()`]
}

Table usuarios_contas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  is_titular boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de usuarios_contas
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
}

Ref: usuarios_contas.usuario_id > usuarios.id
Ref: usuarios_contas.conta_id > contas.id

Table usuarios_empresas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  grupo_id uuid // [Por que existe?] Vínculo 1:1 do usuário com o grupo de permissões dinâmico da empresa
  perfil perfil [default: 'Recepcionista'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de usuarios_empresas (MANTIDO POR COMPATIBILIDADE)
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: usuarios_empresas.usuario_id > usuarios.id
Ref: usuarios_empresas.empresa_id > empresas.id
Ref: usuarios_empresas.grupo_id > grupos_usuarios.id

// ==========================================
// MÓDULO: PERMISSÕES DINÃ‚MICAS E RBAC
// ==========================================

// Tabela onde a Empresa cria seus próprios Grupos de Usuários
Table grupos_usuarios {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Cada empresa define e gerencia seus próprios grupos
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária (Ex: 'Recepcionistas', 'Gerentes')
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  is_admin boolean [default: false] // [Por que existe?] Flag que dá superpoderes Ã quele grupo para ver botão de engrenagem e todas as telas
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: grupos_usuarios.empresa_id > empresas.id

// Tabela para definir as Telas e Ações/Botões (Populado dinamicamente pelo Frontend via FormShow)
Table recursos_sistema {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  chave varchar(100) [unique, not null] // [Por que existe?] Chave programática da interface (Ex: 'frm_vendas', 'btn_excluir_cliente')
  tipo varchar(50) [not null] // [Por que existe?] Classifica o tipo de recurso ('tela', 'botao', 'aba')
  nome varchar(255) [not null] // [Por que existe?] Nome humano para aparecer na engrenagem do Admin (Ex: 'Tela de Vendas', 'Excluir Cliente')
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  modulo varchar(100) // [Por que existe?] Agrupamento visual para a tela de configurações (Ex: 'Financeiro', 'Cadastros')
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

// Tabela que liga o Grupo aos Recursos (Criada quando o Admin clica na engrenagem e dá permissão)
Table grupos_permissoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  grupo_id uuid [not null] // [Por que existe?] O grupo que está recebendo o poder
  recurso_id uuid [not null] // [Por que existe?] O botão ou tela que foi liberado
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
}

Ref: grupos_permissoes.grupo_id > grupos_usuarios.id
Ref: grupos_permissoes.recurso_id > recursos_sistema.id


// ==========================================
// MÓDULO: VENDEDORES E CARTEIRA DE CLIENTES
// ==========================================
Table tipos_vendedor {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária (Ex: 'Interno', 'Representante')
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: tipos_vendedor.conta_id > contas.id

Table vendedores {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  usuario_id uuid // [Por que existe?] Vínculo com a pessoa que fará login no sistema (opcional)
  tipo_vendedor_id uuid [not null] // [Por que existe?] Agrupamento e parametrização do vendedor
  nome_fantasia varchar(255) [not null] // [Por que existe?] Nome comercial ou apelido exibido na plataforma
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: vendedores.conta_id > contas.id
Ref: vendedores.usuario_id > usuarios.id
Ref: vendedores.tipo_vendedor_id > tipos_vendedor.id

// CPF deve ser único dentro de uma conta: UNIQUE(conta_id, cpf)
// usuario_plataforma_id (nullable): link opcional para o usuario da plataforma
// caso este CPF seja encontrado como um usuario ativo da plataforma.
Table contatos_pessoas_fisicas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  cpf varchar(20) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  rg varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_fisicas
  data_nascimento date // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_fisicas
  email varchar(255) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  telefone varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  celular varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  is_cliente boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_fisicas
  is_fornecedor boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_fisicas
  is_profissional boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_fisicas
  usuario_plataforma_id uuid // Nullable — link para usuarios.id se este CPF tem conta na plataforma
  vendedor_1_id uuid // [Por que existe?] Carteira de clientes: vendedor principal responsável pelo contato
  vendedor_2_id uuid // [Por que existe?] Carteira de clientes: vendedor secundário/apoio
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: contatos_pessoas_fisicas.conta_id > contas.id
Ref: contatos_pessoas_fisicas.usuario_plataforma_id > usuarios.id
Ref: contatos_pessoas_fisicas.vendedor_1_id > vendedores.id
Ref: contatos_pessoas_fisicas.vendedor_2_id > vendedores.id

Table enderecos_pf {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  contato_pessoa_fisica_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pf
  tipo_endereco varchar(50) [default: 'PRINCIPAL'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  cep varchar(20) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  logradouro varchar(255) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  numero varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pf
  complemento varchar(100) // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pf
  bairro varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  cidade varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  uf varchar(2) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  pais varchar(100) [default: 'Brasil'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pf
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: enderecos_pf.contato_pessoa_fisica_id > contatos_pessoas_fisicas.id

// CNPJ deve ser único dentro de uma conta: UNIQUE(conta_id, cnpj)
// conta_plataforma_id (nullable): link opcional para a conta da plataforma
// caso este CNPJ seja encontrado como uma empresa/conta ativa na plataforma.
// Cada conta faz esse vínculo de forma independente — sem FK global obrigatória.
Table contatos_pessoas_juridicas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  razao_social varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  nome_fantasia varchar(255) // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_juridicas
  cnpj varchar(20) // UNIQUE dentro da conta (UNIQUE INDEX: conta_id, cnpj)
  inscricao_estadual varchar(50) // [Por que existe?] Inscrição IE/IM demandada pela geração do XML Estadual/Sinc
  email varchar(255) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  telefone varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  celular varchar(50) // [Por que existe?] Variável de alerta ou recuperação de autenticação
  is_cliente boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_juridicas
  is_fornecedor boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contatos_pessoas_juridicas
  conta_plataforma_id uuid // Nullable — link para contas.id se este CNPJ tem conta na plataforma
  vendedor_1_id uuid // [Por que existe?] Carteira de clientes: vendedor principal responsável pelo contato
  vendedor_2_id uuid // [Por que existe?] Carteira de clientes: vendedor secundário/apoio
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: contatos_pessoas_juridicas.conta_id > contas.id
Ref: contatos_pessoas_juridicas.conta_plataforma_id > contas.id
Ref: contatos_pessoas_juridicas.vendedor_1_id > vendedores.id
Ref: contatos_pessoas_juridicas.vendedor_2_id > vendedores.id

Table enderecos_pj {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  contato_pessoa_juridica_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pj
  tipo_endereco varchar(50) [default: 'PRINCIPAL'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  cep varchar(20) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  logradouro varchar(255) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  numero varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pj
  complemento varchar(100) // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pj
  bairro varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  cidade varchar(100) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  uf varchar(2) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  pais varchar(100) [default: 'Brasil'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de enderecos_pj
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: enderecos_pj.contato_pessoa_juridica_id > contatos_pessoas_juridicas.id

// ==========================================
// MÓDULO: CONFIGURAÇÕES DE PAPÉIS (CLIENTES E FORNECEDORES)
// ==========================================
// Os dados vitais (Nome/CPF) estão em contatos_pessoas_fisicas ou contatos_pessoas_juridicas. 
// Estas tabelas abaixo APENAS existiram se o indivíduo tiver parâmetros "Fixos e Exclusivos".
// Exemplo: O Cliente João TEM OBRIGATORIAMENTE um Limite de Crédito de R$10.000 e sempre paga no PIX.

Table empresas_clientes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  contato_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_clientes
  contato_tipo varchar(50) [not null] // 'contatos_pessoas_fisicas' ou 'contatos_pessoas_juridicas'
  limite_credito numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_clientes
  desconto_padrao numeric [default: 0] // [Por que existe?] Percentual de desconto padrão aplicado em vendas para este cliente
  forma_pagamento_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_clientes
  condicao_pagamento_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_clientes
  natureza_operacao_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_clientes
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_clientes.empresa_id > empresas.id
Ref: empresas_clientes.forma_pagamento_padrao_id > formas_pagamento.id
Ref: empresas_clientes.condicao_pagamento_padrao_id > condicoes_pagamento.id
Ref: empresas_clientes.natureza_operacao_padrao_id > naturezas_operacao.id

// Idem ao Cliente. Exemplo: O Fornecedor "Distribuidora Ambev" sempre usa CFOP X e Paga no Boleto.
Table empresas_fornecedores {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  contato_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_fornecedores
  contato_tipo varchar(50) [not null] // 'contatos_pessoas_fisicas' ou 'contatos_pessoas_juridicas'
  forma_pagamento_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_fornecedores
  condicao_pagamento_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_fornecedores
  natureza_operacao_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_fornecedores
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_fornecedores.empresa_id > empresas.id
Ref: empresas_fornecedores.forma_pagamento_padrao_id > formas_pagamento.id
Ref: empresas_fornecedores.condicao_pagamento_padrao_id > condicoes_pagamento.id
Ref: empresas_fornecedores.natureza_operacao_padrao_id > naturezas_operacao.id

Table empresas_transportadores {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  contato_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_transportadores
  contato_tipo varchar(50) [not null] // 'contatos_pessoas_fisicas' ou 'contatos_pessoas_juridicas'
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_transportadores.empresa_id > empresas.id

Ref: empresas_fornecedores.empresa_id > empresas.id
Ref: empresas_fornecedores.forma_pagamento_padrao_id > formas_pagamento.id
Ref: empresas_fornecedores.condicao_pagamento_padrao_id > condicoes_pagamento.id
Ref: empresas_fornecedores.natureza_operacao_padrao_id > naturezas_operacao.id

// ==========================================
// MÓDULO: VÍNCULOS ENTRE CONTAS (AUTORIZAÇÃO)
// ==========================================
// Controla o fluxo de autorização para conectar um cadastro offline
// (contatos_pessoas_juridicas / contatos_pessoas_fisicas) a uma conta/usuario real da plataforma.
//
// FLUXO:
//  1. Conta A cadastra PJ com CNPJ X.
//  2. Sistema detecta que esse CNPJ pertence a uma conta da plataforma (Conta B).
//  3. Sistema cria um registro aqui com status 'pendente' e notifica a Conta B.
//  4. Conta B aceita → conta_plataforma_id / usuario_plataforma_id são preenchidos.
//  5. Conta B rejeita → link nunca acontece. Conta A não sabe o motivo.
//
// REGRA DE PRIVACIDADE: Nenhuma conta pode vincular outra sem consentimento explícito.
// O campo conta_plataforma_id / usuario_plataforma_id SÓ é preenchido após status='aceito'.

Table solicitacoes_vinculo {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)

  // Quem está solicitando o vínculo
  conta_solicitante_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de solicitacoes_vinculo

  // O cadastro offline que originou a solicitação
  contato_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de solicitacoes_vinculo
  contato_tipo varchar(50) [not null] // 'contatos_pessoas_fisicas' ou 'contatos_pessoas_juridicas'

  // O alvo do vínculo na plataforma
  conta_alvo_id uuid   // Preenchido quando o alvo é uma conta (PJ)
  usuario_alvo_id uuid // Preenchido quando o alvo é um usuário (PF)

  // Papel proposto nessa relação (informativo para o alvo decidir)
  papel_proposto varchar(50) [not null] // 'cliente', 'fornecedor', 'profissional'
  mensagem text // Mensagem opcional da conta solicitante

  status varchar(20) [not null, default: 'pendente'] // pendente | aceito | rejeitado | cancelado

  data_resposta timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de solicitacoes_vinculo
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
}

Ref: solicitacoes_vinculo.conta_solicitante_id > contas.id
Ref: solicitacoes_vinculo.conta_alvo_id > contas.id
Ref: solicitacoes_vinculo.usuario_alvo_id > usuarios.id



Table saas_metodos_pagamento {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  tipo varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  gateway varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  gateway_customer_id varchar(255) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  gateway_payment_method_id varchar(255) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  ultimos_4_digitos varchar(4) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  bandeira varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  validade_mes integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  validade_ano integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  principal boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_metodos_pagamento
  ativo boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: saas_metodos_pagamento.conta_id > contas.id

Table saas_planos_assinaturas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  saas_plano_id integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas
  metodo_pagamento_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas
  status varchar(50) [default: 'ATIVA'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  valor numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_inicio date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas
  data_fim date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas
  proxima_fatura date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: saas_planos_assinaturas.conta_id > contas.id
Ref: saas_planos_assinaturas.saas_plano_id > saas_planos.id
Ref: saas_planos_assinaturas.metodo_pagamento_id > saas_metodos_pagamento.id

Table saas_planos_assinaturas_faturas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  assinatura_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  numero_parcela integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  status varchar(50) // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  valor numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  forma_pagamento varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  chave_pix text // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  data_vencimento date // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  data_pagamento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de saas_planos_assinaturas_faturas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: saas_planos_assinaturas_faturas.conta_id > contas.id
Ref: saas_planos_assinaturas_faturas.assinatura_id > saas_planos_assinaturas.id

// ==========================================
// MÓDULO: PROFISSIONAIS E AGENDAMENTOS
// ==========================================
// Tabela para padronizar os Cargos/Especialidades da Empresa (Ex: Médico, Mecânico, Cabeleireiro)
Table empresas_funcoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  nome varchar(100) [not null] // Ex: "Mecânico", "Pediatra"
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_funcoes.empresa_id > empresas.id

// Como o ERP é Multinicho, "Profissional" é a abstração para quem executa o serviço.
// Exemplo Locatário 1 (Clínica Médica): Profissional = Médico Oftalmologista.
// Exemplo Locatário 2 (Oficina Mecânica): Profissional = Mecânico de Suspensão.
// Exemplo Locatário 3 (Salão de Beleza): Profissional = Barbeiro.
Table empresas_profissionais {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  contato_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais
  funcao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais
  contato_tipo varchar(50) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais
  imagem varchar(100) // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  servicos_executados integer [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais
  data_inicio date // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_profissionais.empresa_id > empresas.id
Ref: empresas_profissionais.funcao_id > empresas_funcoes.id

// Define os blocos de horas trabalhadas de cada profissional durante a semana.
// Exemplo: Barbeiro João trabalha Seg-Sex das 09:00 Ã s 18:00 com pausa 12:00-13:00.
Table empresas_profissionais_horarios {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  profissional_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_segunda boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_terca boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_quarta boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_quinta boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_sexta boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_sabado boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  is_domingo boolean [default: false] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  hora_inicio time [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  hora_fim time [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  pausa_inicio time // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  pausa_fim time // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  duracao_minutos integer [default: 15] // [Por que existe?] Armazena característica pertinente e nativa do domínio de empresas_profissionais_horarios
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: empresas_profissionais_horarios.empresa_id > empresas.id
Ref: empresas_profissionais_horarios.profissional_id > empresas_profissionais.id

Table bloqueios_agenda {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  profissional_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de bloqueios_agenda
  inicio timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de bloqueios_agenda
  fim timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de bloqueios_agenda
  motivo text // [Por que existe?] Armazena característica pertinente e nativa do domínio de bloqueios_agenda
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
}

Ref: bloqueios_agenda.empresa_id > empresas.id
Ref: bloqueios_agenda.profissional_id > empresas_profissionais.id

// Coração das marcações da agenda. Casa o Cliente com um Profissional em um horário.
// PADRÃO POLIMÓRFICO: cliente_id + cliente_tipo define a origem:
//   'contatos_pessoas_fisicas'   — cliente offline cadastrado manualmente na conta
//   'contatos_pessoas_juridicas' — empresa cliente cadastrada offline na conta
//   'usuarios'          — usuário da plataforma com login (cliente online)
//   null + nome_cliente_avulso preenchido — walk-in sem cadastro
Table agendamentos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  cliente_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  cliente_tipo varchar(50) // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  profissional_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  os_id uuid // O agendamento vira uma OS quando concluído
  nome_cliente_avulso varchar(255) // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  valor_total numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  inicio timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  fim timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  situacao situacao_agendamento [default: 'pendente'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  gera_financeiro boolean [default: true] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  forma_pagamento_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  qtd_parcelas integer [default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  intervalo_dias integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  campos_customizados jsonb [default: '{}'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamentos
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: agendamentos.empresa_id > empresas.id
Ref: agendamentos.profissional_id > empresas_profissionais.id
Ref: agendamentos.forma_pagamento_id > formas_pagamento.id

// Especifica *o que* vai ser feito no Agendamento.
// Exemplo: Agendamento ID x. Serviço ID y (Troca de Óleo) com Profissional z (Mecânico Marcos).
Table agendamento_servicos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  agendamento_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamento_servicos
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  produto_variacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamento_servicos
  profissional_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamento_servicos
  duracao_minutos int // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamento_servicos
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  valor numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: agendamento_servicos.agendamento_id > agendamentos.id
Ref: agendamento_servicos.produto_variacao_id > produto_variacoes.id
Ref: agendamento_servicos.profissional_id > empresas_profissionais.id

Table servico_desconto 
{
id uuid [primary key, default: `gen_random_uuid()`]
empresa_id uuid [not null]
agendamento_servicos_id uuid [not null]
status boolean [default: true]
valor numeric [not null]
percentual decimal
data_inicio timestamp [default: `now()`]
data_fim timestamp 
data_inclusao timestamp [default: `now()`]
operador_inclusao uuid
data_ultima_atualizacao timestamp [default: `now()`]
}

Ref: servico_desconto.empresa_id > empresas.id
Ref: servico_desconto.agendamento_servicos_id > agendamento_servicos.id

Table agendamento_config {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null, unique] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  texto_whatsapp_agendamento text // [Por que existe?] Armazena característica pertinente e nativa do domínio de agendamento_config
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: agendamento_config.empresa_id - empresas.id


Table feriados {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  data date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de feriados
  nome text [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  uf char(2) // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  tipo varchar(50) [default: 'nacional'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de feriados
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Table auditoria_registros {
  id bigserial [primary key] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid // [Por que existe?] Isolamento multi-tenant da base master
  schema_name text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  table_name text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  user_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  action_timestamp timestamp [not null, default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  action text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  record_id text // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  old_values jsonb // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  new_values jsonb // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
  query text // [Por que existe?] Armazena característica pertinente e nativa do domínio de auditoria_registros
}

Table documentos_legais {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_legais
  versao text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_legais
  titulo text // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_legais
  texto text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_legais
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table documentos_aceites {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  documento_legal_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_aceites
  data_aceite timestamp [not null, default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de documentos_aceites
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: documentos_aceites.usuario_id > usuarios.id
Ref: documentos_aceites.documento_legal_id > documentos_legais.id

// ==========================================
// MÓDULO DE PRODUTOS, SERVIÇOS, ESTOQUE E PREÇOS
// ==========================================

Table produto_categorias {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: produto_categorias.conta_id > contas.id

Table marcas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: marcas.conta_id > contas.id

// PONTO CHAVE: No ERP, "Produto" engloba Física (Mercadoria) e Abstrata (Serviços).
// A coluna `tipo_item` ditará as regras fiscais.
// Exemplo 1: Produto "Pneu Aro 15" -> tipo_item: 'mercadoria'
// Exemplo 2: Produto "Alinhamento 3D" -> tipo_item: 'servico'
Table produtos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  categoria_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  marca_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo_item varchar(20) [default: 'mercadoria'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  codigo_anvisa varchar(13) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  dias_validade_maxima integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  controla_lote boolean [default: false] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  cnpj_fabricante varchar(14) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  ncm varchar(8) // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cest varchar(7) // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  origem integer [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  cfop_padrao varchar(4) // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_padrao varchar(4) // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_ipi_padrao varchar(3) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  codigo_enquadramento_ipi varchar(3) [default: '999'] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  modalidade_base_calculo_icms integer [default: 3] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produtos
  cst_pis_padrao varchar(2) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_cofins_padrao varchar(2) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_pis numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_cofins numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: produtos.conta_id > contas.id
Ref: produtos.categoria_id > produto_categorias.id
Ref: produtos.marca_id > marcas.id

// Variante atrelada ao Produto base (A estrutura Master/Detail do Catálogo).
// Se Produto = "Corte de Cabelo" -> Variação 1: "Degradê", Variação 2: "Máquina Zero".
// Se Produto = "Amoxilina" -> Variação 1: "Comprimido", Variação 2: "Xarope".
Table produto_variacoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  produto_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  sku varchar(50) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  codigo_barras varchar(14) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  codigo_interno varchar(60) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  unidade_medida varchar(6) [default: 'UN'] // [Por que existe?] Padrão de multiplicação metrológica de pesos/unidades
  unidade_tributavel varchar(6) // [Por que existe?] Padrão de multiplicação metrológica de pesos/unidades
  fator_conversao numeric [default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  lote_multiplo numeric [default: 1] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  peso_liquido numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  peso_bruto numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  altura_cm numeric // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  largura_cm numeric // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  profundidade_cm numeric // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  estoque_minimo numeric [default: 0] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  estoque_maximo numeric [default: 0] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  lead_time_dias integer [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  tempo_minutos integer [default: 30] // [Por que existe?] Armazena característica pertinente e nativa do domínio de produto_variacoes
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: produto_variacoes.produto_id > produtos.id

Table contextos_preco {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(255) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contextos_preco.conta_id > contas.id

Table precos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  produto_variacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de precos
  contexto_preco_id uuid [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  percentual_comissao numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de precos
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: precos.produto_variacao_id > produto_variacoes.id
Ref: precos.contexto_preco_id > contextos_preco.id

Table areas_estoque {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: areas_estoque.empresa_id > empresas.id

Table locais_estoque {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  area_estoque_id uuid [not null] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  codigo_visual varchar(50) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  tipo varchar(30) [default: 'pulmao'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de locais_estoque
  prioridade_de_saida integer [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de locais_estoque
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: locais_estoque.area_estoque_id > areas_estoque.id
Ref: locais_estoque.empresa_id > empresas.id

Table estoque_saldo {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  produto_variacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_saldo
  local_estoque_id uuid [not null] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  lote varchar(100) // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  data_validade date // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_saldo
  data_fabricacao date // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_saldo
  cnpj_fabricante varchar(14) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  quantidade_atual numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_entrada timestamp [default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_saldo
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [default: 'disponivel'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: estoque_saldo.produto_variacao_id > produto_variacoes.id
Ref: estoque_saldo.local_estoque_id > locais_estoque.id

Table estoque_movimentacoes {
  id bigserial [primary key] // [Por que existe?] Identificador PK universal (imutável)
  estoque_saldo_id uuid [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  tipo_movimentacao varchar(50) [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  quantidade numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  motivo text // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_movimentacoes
  lote varchar(100) // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  data_validade date // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_movimentacoes
  data_fabricacao date // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_movimentacoes
  cnpj_fabricante varchar(14) // [Por que existe?] Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B
  usuario_responsavel_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_movimentacoes
  data_movimentacao timestamp [default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de estoque_movimentacoes
}

Ref: estoque_movimentacoes.estoque_saldo_id > estoque_saldo.id
Ref: estoque_movimentacoes.usuario_responsavel_id > usuarios.id

// ==========================================
// MÓDULO DE VENDAS E CAIXA (PDV)
// ==========================================

Table formas_pagamento {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  tipo varchar(50) [default: 'dinheiro'] // pix, cartao_credito, etc
  taxa_percentual numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de formas_pagamento
  dias_recebimento integer [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de formas_pagamento
  status boolean [default: true] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: formas_pagamento.conta_id > contas.id

Table controle_caixa {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_abertura timestamp [default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de controle_caixa
  data_fechamento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de controle_caixa
  saldo_inicial numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  saldo_final numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  status varchar(50) [default: 'aberto'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  observacao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
}

Ref: controle_caixa.empresa_id > empresas.id
Ref: controle_caixa.usuario_id > usuarios.id

Table vendas {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  caixa_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de vendas
  cliente_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de vendas
  cliente_tipo varchar(50) // 'contatos_pessoas_fisicas', 'contatos_pessoas_juridicas', 'usuarios'
  valor_subtotal numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_desconto numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  status varchar(50) [default: 'concluida'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_venda timestamp [default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de vendas
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
}

Ref: vendas.empresa_id > empresas.id
Ref: vendas.caixa_id > controle_caixa.id
Ref: vendas.agendamento_id > agendamentos.id

Table venda_itens {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  venda_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de venda_itens
  produto_variacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de venda_itens
  profissional_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de venda_itens
  quantidade numeric [default: 1] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_unitario numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_desconto numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
}

Ref: venda_itens.venda_id > vendas.id
Ref: venda_itens.produto_variacao_id > produto_variacoes.id
Ref: venda_itens.profissional_id > empresas_profissionais.id

// ==========================================
// MÓDULO FINANCEIRO
// ==========================================

Table contas_receber {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  cliente_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_receber
  cliente_tipo varchar(50) // 'contatos_pessoas_fisicas', 'contatos_pessoas_juridicas', 'usuarios'
  origem_id uuid // ID da venda, agendamento, etc
  origem_tipo varchar(50) // 'venda', 'agendamento', etc.
  forma_pagamento_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_receber
  descricao varchar(255) // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  numero_parcela integer [default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_receber
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_vencimento date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_receber
  data_pagamento date // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_receber
  situacao situacao_conta [default: 'pendente'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: contas_receber.empresa_id > empresas.id
Ref: contas_receber.forma_pagamento_id > formas_pagamento.id

Table contas_pagar {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  fornecedor_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_pagar
  fornecedor_tipo varchar(50) // 'contatos_pessoas_fisicas', 'contatos_pessoas_juridicas'
  origem_id uuid // ID de um pedido de compra futuro
  origem_tipo varchar(50) // 'pedido_compra', 'avulso', etc.
  descricao varchar(255) [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  numero_parcela integer [default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_pagar
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_vencimento date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_pagar
  data_pagamento date // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_pagar
  situacao situacao_conta [default: 'pendente'] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: contas_pagar.empresa_id > empresas.id

# Definição do Novo Aplicativo

## 1. Visão Geral
* **Nome do Aplicativo:** Andreense
* **Objetivo Principal:** Ser um super APP com alternativa gratuita para rotinas de agendamento e ERP 
* **Público-Alvo:** População Geral

// ==========================================
// TABELAS COMPLEMENTARES (CONTEXTO ERP COMPLETO)
// Importadas de outro projeto de referência — já migradas para conta_id / empresa_id.
// ==========================================

Table contas_bancarias {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  nome text [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  banco_codigo varchar(10) [not null] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  banco_nome text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  agencia varchar(10) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  agencia_digito varchar(2), // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  conta varchar(20) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  conta_digito varchar(2), // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  carteira varchar(10), // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  convenio varchar(20), // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_bancarias
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_bancarias.empresa_id > empresas.id

Table contas_a_receber_comissao {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_a_receber_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  movimentacao_pagamento_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  origem_id uuid // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_tipo text, // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  beneficiario_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  beneficiario_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  valor_base_calculo numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total_deducoes numeric [not null, default: 0.00] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_liquido_calculo numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  percentual_comissao numeric [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  valor_final_comissao numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_pagamento_comissao date // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  observacoes text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'pendente'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_receber_comissao.conta_a_receber_id > contas_a_receber.id
Ref: contas_a_receber_comissao.movimentacao_pagamento_id > contas_a_receber_movimentacao.id

Table comissao_deducao_regra {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo_deducao text [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: comissao_deducao_regra.conta_id > contas.id

Table contas_a_receber_comissao_deducao {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  comissao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_comissao_deducao
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo_deducao text [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_calculado_deducao numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_receber_comissao_deducao.comissao_id > contas_a_receber_comissao.id

// Crédito disponível a pagar. O beneficiário (usuario_id + usuario_tipo) pode ser:
//   'contatos_pessoas_fisicas'   — fornecedor/cliente offline
//   'contatos_pessoas_juridicas' — empresa cadastrada offline
//   'usuarios'          — usuário da plataforma (com login)
Table conta_a_pagar_credito {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  criador_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_pagar_credito
  criador_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_pagar_credito
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  usuario_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_pagar_credito
  origem_id uuid // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_tipo text // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor_original numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  saldo_disponivel numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  data_expiracao date // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_pagar_credito
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'disponivel'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: conta_a_pagar_credito.empresa_id > empresas.id

// Crédito disponível a receber. Mesmo padrão polimórfico de conta_a_pagar_credito.
Table conta_a_receber_credito {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  criador_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_receber_credito
  criador_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_receber_credito
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  usuario_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_receber_credito
  origem_id uuid // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_tipo text // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor_original numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  saldo_disponivel numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  data_expiracao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de conta_a_receber_credito
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'disponivel'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: conta_a_receber_credito.empresa_id > empresas.id

Table contas_a_pagar {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  devedor_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  devedor_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  credor_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  credor_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  origem_id uuid // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_tipo text, // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  numero_nota_fiscal text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  valor_original numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  juros_multa numeric [default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  desconto numeric [default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  valor_pago numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_emissao date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  data_vencimento date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  data_pagamento date // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  conta_bancaria_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  forma_pagamento text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  observacoes text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'pendente'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_pagar.empresa_id > empresas.id

Table contas_a_pagar_movimentacao {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_a_pagar_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar_movimentacao
  tipo_movimentacao text [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_movimentacao date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar_movimentacao
  descricao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  usuario_responsavel_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_pagar_movimentacao
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_pagar_movimentacao.conta_a_pagar_id > contas_a_pagar.id

Table contas_a_receber {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  credor_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  credor_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  devedor_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  devedor_tipo text [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  descricao text [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  numero_nota_fiscal text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  numero_parcela integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  total_parcelas integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  valor_original numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  juros_multa numeric [default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  desconto numeric [default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  valor_pago numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_emissao timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  data_vencimento timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  data_pagamento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  conta_bancaria_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  forma_pagamento text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  observacoes text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber
  origem_id uuid // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_tipo text, // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'pendente'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_receber.empresa_id > empresas.id

Table contas_a_receber_movimentacao {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_a_receber_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_movimentacao
  tipo_movimentacao text [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_movimentacao timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_movimentacao
  descricao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  usuario_responsavel_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de contas_a_receber_movimentacao
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: contas_a_receber_movimentacao.conta_a_receber_id > contas_a_receber.id

Table notas_fiscais {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  empresa_id uuid [not null]
  origem_tipo varchar(50) [not null] // 'pedido_venda', 'pedido_compra', 'ordem_servico', 'venda'
  origem_id uuid [not null] // ID do documento que originou a nota
  tipo_nota varchar(10) [not null] // 'NFe', 'NFCe', 'NFSe'
  embarque_id uuid // Opcional, para NF-e de venda de produto
  numero_nota_fiscal text
  serie_nota_fiscal text
  chave_acesso text
  status_sefaz text
  xml_url text
  data_emissao timestamp [default: `now()`]
  valor_total numeric [not null, default: 0]
  tipo_operacao varchar(20) [not null, default: 'SAIDA']
  chave_acesso_referenciada varchar(44)
  natureza_operacao text [not null, default: 'VENDA']
  tipo_pagamento_sefaz varchar(2) [default: '99']
  finalidade_emissao integer [default: 1]
  valor_seguro numeric [default: 0]
  valor_outras_despesas numeric [default: 0]
  valor_total_base_icms numeric [default: 0]
  valor_total_icms numeric [default: 0]
  valor_total_icms_deson numeric [default: 0]
  valor_total_base_st numeric [default: 0]
  valor_total_st numeric [default: 0]
  valor_total_fcp numeric [default: 0]
  valor_total_fcp_st numeric [default: 0]
  valor_total_fcp_st_ret numeric [default: 0]
  valor_total_ipi numeric [default: 0]
  valor_total_ipi_devol numeric [default: 0]
  valor_total_pis numeric [default: 0]
  valor_total_cofins numeric [default: 0]
  valor_total_ii numeric [default: 0]
  valor_total_tributos numeric [default: 0]
  valor_total_is numeric [default: 0]
  valor_frete numeric [default: 0]
  valor_desconto numeric [default: 0]
  valor_subtotal numeric [default: 0]
  observacao text
  motivo_rejeicao text
  url_imagem text
  status text [not null, default: 'ativo']
  ordem integer [not null, default: 0]
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
  data_exclusao timestamp
}

Ref: notas_fiscais.empresa_id > empresas.id
Ref: notas_fiscais.embarque_id > embarques.id

Table nota_fiscal_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  nota_fiscal_id uuid [not null]
  origem_item_id uuid // ID do item no pedido_venda_itens, ordem_servico_itens, etc.
  embarque_item_id uuid
  produto_variacao_id uuid
  natureza_operacao_id uuid
  nome_produto varchar(255) [not null]
  quantidade numeric [not null]
  preco_unitario numeric [not null]
  valor_total_item numeric [default: 0]
  valor_desconto numeric [default: 0]
  percentual_desconto numeric [default: 0]
  compoe_valor_total integer [not null, default: 1]
  cfop varchar(4)
  cst_icms varchar(4)
  aliquota_icms numeric [default: 0]
  valor_base_icms numeric [default: 0]
  valor_icms numeric [default: 0]
  valor_base_icms_st numeric [default: 0]
  aliquota_icms_st numeric [default: 0]
  valor_icms_st numeric [default: 0]
  aliquota_mva_st numeric [default: 0]
  percentual_reducao_base_st numeric [default: 0]
  percentual_reducao_base numeric [default: 0]
  cst_ipi varchar(3)
  codigo_enquadramento_ipi varchar(3)
  aliquota_ipi numeric [default: 0]
  valor_base_ipi numeric [default: 0]
  valor_ipi numeric [default: 0]
  cst_pis varchar(3)
  aliquota_pis numeric [default: 0]
  valor_base_pis numeric [default: 0]
  valor_pis numeric [default: 0]
  cst_cofins varchar(3)
  aliquota_cofins numeric [default: 0]
  valor_base_cofins numeric [default: 0]
  valor_cofins numeric [default: 0]
  cst_ibs_cbs varchar(4)
  valor_base_ibs numeric [default: 0]
  aliquota_ibs numeric [default: 0]
  valor_ibs numeric [default: 0]
  valor_base_cbs numeric [default: 0]
  aliquota_cbs numeric [default: 0]
  valor_cbs numeric [default: 0]
  cst_is varchar(4)
  valor_base_is numeric [default: 0]
  aliquota_is numeric [default: 0]
  valor_is numeric [default: 0]
  lote varchar(100)
  data_validade date
  data_fabricacao date
  cnpj_fabricante varchar(14)
  url_imagem text
  status text [not null, default: 'ativo']
  ordem integer [not null, default: 0]
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
  data_exclusao timestamp
}

Ref: nota_fiscal_itens.nota_fiscal_id > notas_fiscais.id
Ref: nota_fiscal_itens.produto_variacao_id > produto_variacoes.id
Ref: nota_fiscal_itens.natureza_operacao_id > naturezas_operacao.id

Table embarques {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  pedido_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de embarques
  usuario_separador_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de embarques
  data_separacao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de embarques
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'pendente'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: embarques.pedido_id > pedidos.id

Table embarque_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  embarque_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de embarque_itens
  pedido_item_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de embarque_itens
  estoque_saldo_id uuid // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  quantidade numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: embarque_itens.embarque_id > embarques.id
Ref: embarque_itens.pedido_item_id > pedido_itens.id
Ref: embarque_itens.estoque_saldo_id > estoque_saldo.id

Table naturezas_operacao {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  descricao varchar(100) [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  descricao_fiscal varchar(60), // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  cfop_padrao varchar(4) [not null] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  indicador_total_padrao integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  finalidade_n_fe integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  soma_ipi_no_total boolean [default: FALSE] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  soma_icms_st_no_total boolean [default: FALSE] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  soma_is_no_total boolean [default: FALSE] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  incide_ibs_cbs boolean [default: TRUE] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  gera_financeiro boolean [default: TRUE] // [Por que existe?] Armazena característica pertinente e nativa do domínio de naturezas_operacao
  movimenta_estoque boolean [default: TRUE] // [Por que existe?] Controle granular para contábil, validades ANVISA ou avarias
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: naturezas_operacao.conta_id > contas.id

Table regras_fiscais {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  natureza_operacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  ncm varchar(8), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  uf_destino CHAR(2), // [Por que existe?] Requisito base do pacote geográfico de emissão de NF
  tipo_contribuinte integer [default: 0] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_mercadoria integer [default: 0] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  cfop varchar(4) [not null] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_icms varchar(4) [not null] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  modalidade_base_calculo_icms integer [default: 3] // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  aliquota_icms numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  reducao_base_icms numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  aliquota_fcp numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_mva numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_icms_interna numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  pauta_fiscal numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  reducao_base_icms_st numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  aliquota_fcp_st numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_ipi varchar(3) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_ipi numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_pis varchar(3) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_pis numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_cofins varchar(3) [default: '99'] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_cofins numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  codigo_beneficio_fiscal varchar(15), // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  motivo_desoneracao_icms integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de regras_fiscais
  cst_ibs_cbs varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_ibs numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_cbs numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_is varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_is numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_pauta_is numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  observacao_legal text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: regras_fiscais.natureza_operacao_id > naturezas_operacao.id
Ref: regras_fiscais.empresa_id > empresas.id

// Versão FISCAL de formas de pagamento — com codigo_sefaz para XML (TAG <tPag>).
// A versão simplificada do PDV é a tabela `formas_pagamento` acima.
Table formas_pagamento_fiscal {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(50) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  codigo_sefaz varchar(2), // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: formas_pagamento_fiscal.conta_id > contas.id

Table condicoes_pagamento {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  conta_id uuid [not null] // [Por que existe?] Isolamento multi-tenant da base master
  nome varchar(100) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: condicoes_pagamento.conta_id > contas.id

Table condicao_pagamento_parcelas {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  condicao_pagamento_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de condicao_pagamento_parcelas
  dias_para_vencimento integer [not null, default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de condicao_pagamento_parcelas
  percentual numeric [not null, default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de condicao_pagamento_parcelas
  forma_pagamento_padrao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de condicao_pagamento_parcelas
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: condicao_pagamento_parcelas.condicao_pagamento_id > condicoes_pagamento.id

Table orcamentos {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  cliente_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  cliente_tipo text [not null, default: 'usuario'] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  profissional_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  documento_gerado_tipo varchar(50) // 'pedido_venda', 'ordem_servico'
  documento_gerado_id uuid // ID do pedido de venda ou da OS gerada
  natureza_operacao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  data_validade date [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  valor_subtotal numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_frete numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total_ipi numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total_icms_st numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_total numeric [not null, default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  observacoes text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamentos
  observacao_interna text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  observacao_cliente text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'RASCUNHO'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: orcamentos.empresa_id > empresas.id
Ref: orcamentos.profissional_id > empresas_profissionais.id

Table orcamento_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  orcamento_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  produto_variacao_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  natureza_operacao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  quantidade numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  preco_unitario numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_desconto numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  percentual_desconto numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  total numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  observacao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  ncm varchar(10), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cfop varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  cst_icms varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_icms numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_base_icms numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_icms numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_icms_st numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_base_icms_st numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  aliquota_icms_st numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_mva_st numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  percentual_reducao_base_st numeric [default: 0] // [Por que existe?] Armazena característica pertinente e nativa do domínio de orcamento_itens
  cst_ipi varchar(3), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  codigo_enquadramento_ipi varchar(3) [default: '999'] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  aliquota_ipi numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_base_ipi numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_ipi numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  cst_pis varchar(3), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_pis numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_base_pis numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_pis numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  cst_cofins varchar(3), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_cofins numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_base_cofins numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_cofins numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  cst_ibs_cbs varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_ibs numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_ibs numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  aliquota_cbs numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_cbs numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  cst_is varchar(4), // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  aliquota_is numeric [default: 0] // [Por que existe?] Amparo fiscal mandatório mapeado da Substituição e Alíquotas
  valor_is numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Ref: orcamento_itens.orcamento_id > orcamentos.id
Ref: orcamento_itens.produto_variacao_id > produto_variacoes.id

Table pedidos_venda {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  codigo_pedido_sequence integer [not null, default: `nextval('public.pedidos_venda_codigoPedidoSequence_seq')`]
  empresa_id uuid [not null]
  cliente_id uuid [not null]
  cliente_tipo text [not null, default: 'usuario']
  profissional_id uuid
  orcamento_origem_id uuid
  natureza_operacao_id uuid
  status varchar(50) [not null, default: 'RASCUNHO']
  valor_produtos numeric [not null, default: 0]
  valor_frete numeric [default: 0]
  valor_desconto numeric [default: 0]
  valor_total numeric [not null, default: 0]
  valor_total_ipi numeric [default: 0]
  valor_total_icms_st numeric [default: 0]
  endereco_entrega_id uuid
  tipo_frete text
  modalidade_frete integer [not null, default: 9]
  delivery_tempo_estimado_minutos integer
  delivery_taxa_entrega numeric
  indicador_consumidor_final integer [not null, default: 1]
  indicador_presenca integer [not null, default: 1]
  observacao_interna text
  observacao_cliente text
  url_imagem text
  ordem integer [not null, default: 0]
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
  data_exclusao timestamp
}

Ref: pedidos_venda.empresa_id > empresas.id
Ref: pedidos_venda.profissional_id > empresas_profissionais.id

Table pedido_venda_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  pedido_venda_id uuid [not null]
  produto_variacao_id uuid [not null]
  estoque_saldo_id uuid
  natureza_operacao_id uuid
  quantidade numeric [not null]
  preco_unitario numeric [not null]
  valor_desconto numeric [default: 0]
  percentual_desconto numeric [default: 0]
  subtotal numeric [default: 0]
  observacao text
  cfop varchar(4)
  cst_icms varchar(4)
  aliquota_icms numeric [default: 0]
  valor_base_icms numeric [default: 0]
  valor_icms numeric [default: 0]
  valor_icms_st numeric [default: 0]
  valor_base_icms_st numeric [default: 0]
  aliquota_icms_st numeric [default: 0]
  aliquota_mva_st numeric [default: 0]
  percentual_reducao_base_st numeric [default: 0]
  percentual_reducao_base numeric [default: 0]
  cst_ipi varchar(3)
  codigo_enquadramento_ipi varchar(3) [default: '999']
  aliquota_ipi numeric [default: 0]
  valor_base_ipi numeric [default: 0]
  valor_ipi numeric [default: 0]
  cst_pis varchar(3)
  aliquota_pis numeric [default: 0]
  valor_base_pis numeric [default: 0]
  valor_pis numeric [default: 0]
  cst_cofins varchar(3)
  aliquota_cofins numeric [default: 0]
  valor_base_cofins numeric [default: 0]
  valor_cofins numeric [default: 0]
  cst_ibs_cbs varchar(4)
  aliquota_ibs numeric [default: 0]
  valor_ibs numeric [default: 0]
  valor_base_ibs numeric [default: 0]
  aliquota_cbs numeric [default: 0]
  valor_cbs numeric [default: 0]
  valor_base_cbs numeric [default: 0]
  cst_is varchar(4)
  aliquota_is numeric [default: 0]
  valor_is numeric [default: 0]
  valor_base_is numeric [default: 0]
  url_imagem text
  status text [not null, default: 'ativo']
  ordem integer [not null, default: 0]
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
  data_exclusao timestamp
}

Ref: pedido_venda_itens.pedido_venda_id > pedidos_venda.id
Ref: pedido_venda_itens.produto_variacao_id > produto_variacoes.id

Table pedido_venda_parcelas {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  pedido_venda_id uuid [not null]
  meio_pagamento_id uuid
  numero_parcela integer [not null]
  data_vencimento date [not null]
  valor numeric [not null]
  url_imagem text
  status text [not null, default: 'ativo']
  ordem integer [not null, default: 0]
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
  data_exclusao timestamp
}

Ref: pedido_venda_parcelas.pedido_venda_id > pedidos_venda.id

// ==========================================
// PEDIDOS DE COMPRA
// ==========================================

Table pedidos_compra {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  codigo_pedido_sequence integer [not null, default: `nextval('public.pedidos_compra_codigoPedidoSequence_seq')`]
  empresa_id uuid [not null]
  fornecedor_id uuid [not null]
  fornecedor_tipo varchar(50) [not null] // 'contatos_pessoas_fisicas' ou 'contatos_pessoas_juridicas'
  status varchar(50) [not null, default: 'RASCUNHO']
  valor_produtos numeric [not null, default: 0]
  valor_frete numeric [default: 0]
  valor_desconto numeric [default: 0]
  valor_total numeric [not null, default: 0]
  xml_chave_acesso varchar(44)
  observacao_interna text
  data_previsao_entrega date
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
}

Ref: pedidos_compra.empresa_id > empresas.id

Table pedido_compra_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  pedido_compra_id uuid [not null]
  produto_variacao_id uuid [not null]
  quantidade numeric [not null]
  preco_unitario numeric [not null]
  valor_desconto numeric [default: 0]
  subtotal numeric [default: 0]
  data_inclusao timestamp [not null, default: `now()`]
}

Ref: pedido_compra_itens.pedido_compra_id > pedidos_compra.id
Ref: pedido_compra_itens.produto_variacao_id > produto_variacoes.id

Table pedido_compra_parcelas {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  pedido_compra_id uuid [not null]
  meio_pagamento_id uuid
  numero_parcela integer [not null]
  data_vencimento date [not null]
  valor numeric [not null]
  data_inclusao timestamp [not null, default: `now()`]
}

Ref: pedido_compra_parcelas.pedido_compra_id > pedidos_compra.id

// ==========================================
// ORDENS DE SERVIÇO (OS)
// ==========================================

Table ordens_servico {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  codigo_os_sequence integer [not null, default: `nextval('public.ordens_servico_codigoOsSequence_seq')`]
  empresa_id uuid [not null]
  cliente_id uuid [not null]
  cliente_tipo text [not null, default: 'usuario']
  profissional_id uuid
  orcamento_origem_id uuid
  status varchar(50) [not null, default: 'ABERTA'] // ABERTA, EM_ANDAMENTO, CONCLUIDA, CANCELADA
  valor_servicos numeric [not null, default: 0]
  valor_produtos numeric [not null, default: 0]
  valor_desconto numeric [default: 0]
  valor_total numeric [not null, default: 0]
  equipamento_descricao text // Ex: Placa do carro, modelo do celular
  defeito_relatado text
  laudo_tecnico text
  observacao_interna text
  data_inicio timestamp
  data_fim timestamp
  data_inclusao timestamp [not null, default: `now()`]
  data_ultima_atualizacao timestamp
}

Ref: ordens_servico.empresa_id > empresas.id
Ref: ordens_servico.profissional_id > empresas_profissionais.id

Table ordem_servico_itens {
  id uuid [primary key, not null, default: `gen_random_uuid()`]
  ordem_servico_id uuid [not null]
  produto_variacao_id uuid [not null]
  quantidade numeric [not null]
  preco_unitario numeric [not null]
  valor_desconto numeric [default: 0]
  subtotal numeric [default: 0]
  profissional_executor_id uuid // Quem executou especificamente este serviço
  data_inclusao timestamp [not null, default: `now()`]
}

Ref: ordem_servico_itens.ordem_servico_id > ordens_servico.id
Ref: ordem_servico_itens.produto_variacao_id > produto_variacoes.id
Ref: ordem_servico_itens.profissional_executor_id > empresas_profissionais.id

// ==========================================
// MÓDULO APP (ANDREENSE — ASSINATURAS DO APP MOBILE)
// Estas tabelas são específicas para a plataforma do App Andreense.
// [TODO: confirmar se devem estar no mesmo banco do ERP ou em banco separado]
// ==========================================

Table planos_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  nome varchar(150) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  intervalo varchar(50) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de planos_app
  intervalo_contagem integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de planos_app
  recursos_config jsonb, // [Por que existe?] Armazena característica pertinente e nativa do domínio de planos_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status varchar(50) [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 99] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table assinaturas_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  gateway_id varchar(255), // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  plano_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  status varchar(50) [not null] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_inicio timestamp [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  data_ciclo_atual_inicio timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  data_ciclo_atual_fim timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  data_cancelamento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de assinaturas_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table servicos_disponiveis_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  codigo_servico varchar(100) [not null] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  nome varchar(150) [not null] // [Por que existe?] Identidade para exibição primária de marca/usuário
  descricao text, // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  recursos_config jsonb, // [Por que existe?] Armazena característica pertinente e nativa do domínio de servicos_disponiveis_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status varchar(50) [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table servico_contratacoes_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  gateway_id varchar(255), // [Por que existe?] Armazena característica pertinente e nativa do domínio de servico_contratacoes_app
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  servico_id uuid [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de servico_contratacoes_app
  valor_cobrado numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  metadata jsonb, // [Por que existe?] Armazena característica pertinente e nativa do domínio de servico_contratacoes_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status varchar(50) [not null] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table pagamentos_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  gateway_pagamento varchar(50), // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  gateway_id varchar(255), // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  assinatura_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  servico_contratacao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  valor numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  metodo_pagamento varchar(50), // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  boleto_url text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  boleto_linha_digitavel text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  checkout_url text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  qr_code_pix text, // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  data_vencimento_pix timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  data_vencimento_boleto timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  data_vencimento_checkout timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  data_pagamento timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status varchar(50) [not null] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table pagamentos_logs_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  pagamento_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_logs_app
  servico_contratacao_id uuid // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_logs_app
  etapa varchar(100) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_logs_app
  conteudo_json jsonb [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de pagamentos_logs_app
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Table configuracoes_taxas_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  metodo_pagamento varchar(50) [not null] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_taxas_app
  descricao varchar(255) [not null] // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  tipo_parcelamento varchar(50) [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  taxa_percentual numeric [not null, default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_taxas_app
  taxa_fixa numeric [not null, default: 0.00] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_taxas_app
  numero_maximo_parcelas integer [not null, default: 1] // [Por que existe?] Armazena característica pertinente e nativa do domínio de configuracoes_taxas_app
  valor_minimo_parcela numeric [not null, default: 5.00] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}

Table usuario_recursos_app {
  id uuid [primary key, not null, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  usuario_id uuid [not null] // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  origem_tipo varchar(50) [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  origem_id uuid [not null] // [Por que existe?] Chave seletora ou polimórfica para condicional de rotas
  codigo_recurso varchar(100) [not null] // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  valor_limite varchar(255) [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_expiracao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de usuario_recursos_app
  url_imagem text, // [Por que existe?] Referência visual rápida e cacheada no S3 para B2C e PDV
  status text [not null, default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [not null, default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [not null, default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp // [Por que existe?] Auditoria: log de alteração
  data_exclusao timestamp // [Por que existe?] Auditoria: soft-delete e manutenção do histórico fiscal
}


// ==========================================
// MÓDULO: FIDELIDADE E CASHBACK
// ==========================================

Table cashback_regras {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  titulo varchar(100) [not null] // [Por que existe?] Identidade para exibição primária (Ex: 'Cashback Black Friday')
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  percentual numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  dias_validade integer // [Por que existe?] Armazena característica pertinente e nativa do domínio de cashback_regras
  data_inicio_vigencia timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de cashback_regras
  data_fim_vigencia timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de cashback_regras
  valor_maximo_unidade numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_maximo_total numeric // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  tem_referencia boolean [default: false] // [Por que existe?] Se ativado, exige um código de venda/referência no crédito
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no Cloudflare R2
  status varchar(20) [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: cashback_regras.empresa_id > empresas.id

Table cashback_creditos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  usuario_id uuid [not null] // [Por que existe?] O consumidor que recebeu o crédito
  regra_id uuid // [Por que existe?] Regra que originou o benefício
  valor_inicial numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  valor_utilizado numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  codigo_referencia varchar(100) // [Por que existe?] Vínculo com a venda/cupom fiscal de origem
  status varchar(20) [default: 'disponivel'] // disponivel, esgotado, expirado
  data_liberacao timestamp [default: `now()`] // [Por que existe?] Armazena característica pertinente e nativa do domínio de cashback_creditos
  data_expiracao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de cashback_creditos
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: cashback_creditos.empresa_id > empresas.id
Ref: cashback_creditos.usuario_id > usuarios.id
Ref: cashback_creditos.regra_id > cashback_regras.id

Table cashback_saldos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  usuario_id uuid [not null] // [Por que existe?] O consumidor dono do saldo
  saldo_disponivel numeric [default: 0] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

// Chave única para o par Empresa x Usuário
// Create Unique Index: empresa_id, usuario_id

Ref: cashback_saldos.empresa_id > empresas.id
Ref: cashback_saldos.usuario_id > usuarios.id

Table cashback_utilizacoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  credito_id uuid [not null] // [Por que existe?] Qual crédito específico foi consumido (Lógica FIFO)
  valor_utilizado numeric [not null] // [Por que existe?] Motor numérico da operação ou grandeza contábil física
  venda_id uuid // [Por que existe?] Vínculo com a venda onde o cashback foi usado como pagamento
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: cashback_utilizacoes.credito_id > cashback_creditos.id

Table cashback_tokens {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  token varchar(255) [unique, not null] // [Por que existe?] Código seguro do QR Code
  empresa_id uuid [not null] // [Por que existe?] Empresa que gerou o QR Code
  valor_compra numeric // [Por que existe?] Se for um crédito pré-definido pelo lojista
  tipo varchar(20) [not null] // credito, debito
  status varchar(20) [default: 'disponivel'] // disponivel, usado, expirado
  data_expiracao timestamp [not null] // [Por que existe?] Geralmente 60 a 300 segundos
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: cashback_tokens.empresa_id > empresas.id


Table fidelidade_cartoes {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  empresa_id uuid [not null] // [Por que existe?] Isolamento operacional por filial (Master/Detail)
  titulo varchar(100) [not null] // [Por que existe?] Identidade para exibição primária (Ex: 'Cartão Pizza 10')
  descricao text // [Por que existe?] Campo semântico para metadados atípicos do backoffice
  pontos_necessarios integer [not null] // [Por que existe?] Meta de carimbos para ganhar a recompensa
  recompensa varchar(255) [not null] // [Por que existe?] Descrição do prêmio (Ex: '1 Pizza Grátis')
  regra_texto text // [Por que existe?] Termos e condições específicos desta campanha
  dias_validade_carimbo integer // [Por que existe?] Expiração individual de cada selo ganho
  dias_inatividade_cartao integer // [Por que existe?] Se o cliente não carimbar em X dias, perde o progresso
  data_inicio_vigencia timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de fidelidade_cartoes
  data_fim_vigencia timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de fidelidade_cartoes
  tem_referencia boolean [default: false] // [Por que existe?] Se ativado, exige um código de venda no carimbo
  url_imagem text // [Por que existe?] Referência visual rápida e cacheada no Cloudflare R2
  status varchar(20) [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  ordem integer [default: 0] // [Por que existe?] Propriedade de customização de ranking em grids e list cards na UI
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  operador_inclusao uuid // [Por que existe?] Auditoria: qual usuário executou a ação do registro
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: fidelidade_cartoes.empresa_id > empresas.id

Table fidelidade_saldos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  cartao_id uuid [not null] // [Por que existe?] Vínculo com a campanha de fidelidade
  usuario_id uuid [not null] // [Por que existe?] O consumidor dono do cartão
  pontos_acumulados integer [default: 0] // [Por que existe?] Carimbos válidos atualmente
  recompensas_acumuladas integer [default: 0] // [Por que existe?] Quantas vezes o cliente completou o cartão e não resgatou
  status varchar(20) [default: 'ativo'] // [Por que existe?] Controle da máquina de estados e inativação temporal sem delete
  data_resgate_ultima timestamp // [Por que existe?] Histórico do último prêmio retirado
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: fidelidade_saldos.cartao_id > fidelidade_cartoes.id
Ref: fidelidade_saldos.usuario_id > usuarios.id

Table fidelidade_carimbos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  saldo_id uuid [not null] // [Por que existe?] Vínculo com o cartão do cliente
  status varchar(20) [default: 'valido'] // valido, utilizado, expirado
  codigo_referencia varchar(100) // [Por que existe?] Vínculo com a venda/cupom de origem
  data_expiracao timestamp // [Por que existe?] Armazena característica pertinente e nativa do domínio de fidelidade_carimbos
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: fidelidade_carimbos.saldo_id > fidelidade_saldos.id

Table fidelidade_logs {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  saldo_id uuid [not null] // [Por que existe?] Vínculo com o cartão do cliente
  tipo_evento varchar(50) [not null] // GANHOU_CARIMBO, RESGATOU_RECOMPENSA, EXPIRADO
  descricao text // [Por que existe?] Detalhamento do evento para o cliente
  codigo_referencia varchar(100) // [Por que existe?] Metadado chave ou código de barras de leitura em PDV/Bipes rápido
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: fidelidade_logs.saldo_id > fidelidade_saldos.id

Table fidelidade_tokens {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  token varchar(255) [unique, not null] // [Por que existe?] Código seguro do QR Code
  cartao_id uuid [not null] // [Por que existe?] Qual cartão o lojista está oferecendo para carimbar
  empresa_id uuid [not null] // [Por que existe?] Empresa que gerou o QR Code
  status varchar(20) [default: 'disponivel'] // disponivel, usado, expirado
  data_expiracao timestamp [not null] // [Por que existe?] Geralmente 60 a 300 segundos
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
}

Ref: fidelidade_tokens.cartao_id > fidelidade_cartoes.id


Table cidade_achados_perdidos {
  id uuid [primary key, default: `gen_random_uuid()`] // [Por que existe?] Identificador PK universal (imutável)
  usuario_id uuid [not null] // [Por que existe?] Quem publicou o item
  tipo varchar(20) [not null] // 'achado' ou 'perdido'
  titulo varchar(255) [not null] // [Por que existe?] Título curto do item
  descricao text // [Por que existe?] Detalhes do item (cor, marca, local)
  local_visto varchar(255) // [Por que existe?] Onde foi achado ou perdido
  data_evento date // [Por que existe?] Quando ocorreu o fato
  contato_info varchar(255) // [Por que existe?] Telefone/Email para contato (pode ser escondido)
  esconder_contato boolean [default: true] // [Por que existe?] Se true, o contato só aparece via chat/solicitação
  url_imagem text // [Por que existe?] Foto do item no Cloudflare R2
  status varchar(20) [default: 'ativo'] // 'ativo', 'resolvido', 'cancelado'
  data_inclusao timestamp [default: `now()`] // [Por que existe?] Auditoria: data de criação exata
  data_ultima_atualizacao timestamp [default: `now()`] // [Por que existe?] Auditoria: log de alteração
}

Ref: cidade_achados_perdidos.usuario_id > usuarios.id


// ==========================================
// ðŸ›‘ OMISSÕES PROPOSITAIS (Por que certas colunas NÃO existem)
// ==========================================
// Em modelagem rica, o que ocultamos é tão importante quanto o que mantemos:
// 1. Falta de `valor_total` em Cabeçalhos (quando calculado): Algumas tabelas mantêm totais redundantes de leitura (Performance Cache), 
//    outras propositalmente carecem deles para forçar re-agrupamento puro de `itens`.
// 2. Falta de `usuario_tipo` solto em Históricos de Estoque: Tabelas como `estoque_movimentacoes` só ganham IDs de login explícito (`usuarios`), 
//    porque clientes Pessoas Físicas/B2C *nunca* irão manipular diretamente volumes.
// 3. Ausência de `endereço` em Vendas transacionais (PDV): O endereço já figura na raiz do Cliente (onde ele mora/sede), ele só
//    é puxado localmente quando é uma `endereco_entrega_id` destacada (para fretes de e-commerce).
// 4. Ausência de hard-delete (`ON DELETE CASCADE` implícito): Nossa base é fiscal imutável. Soft-Deletes via `status` e `data_exclusao` 
//    preservam integridade para log de auditoria retroativo (Tudo é Big Data).
// ==========================================


