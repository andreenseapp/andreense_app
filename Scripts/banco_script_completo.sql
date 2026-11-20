-- Script de Inicialização - ERP DB
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE SCHEMA IF NOT EXISTS erp;
CREATE SCHEMA IF NOT EXISTS servicos;
CREATE SCHEMA IF NOT EXISTS cliente;

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.data_ultima_atualizacao = now();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE SEQUENCE IF NOT EXISTS public.pedidos_venda_codigoPedidoSequence_seq;
CREATE SEQUENCE IF NOT EXISTS public.pedidos_compra_codigoPedidoSequence_seq;
CREATE SEQUENCE IF NOT EXISTS public.ordens_servico_codigoOsSequence_seq;

DROP TYPE IF EXISTS public.situacao_agendamento CASCADE;
CREATE TYPE public.situacao_agendamento AS ENUM ('pendente', 'confirmado', 'concluido', 'cancelado', 'bloqueado');
DROP TYPE IF EXISTS public.perfil CASCADE;
CREATE TYPE public.perfil AS ENUM ('Administrador', 'Recepcionista', 'Vendas', 'Profissional', 'Financeiro');
DROP TYPE IF EXISTS public.situacao_conta CASCADE;
CREATE TYPE public.situacao_conta AS ENUM ('pendente', 'paga', 'cancelada', 'atrasada');

CREATE TABLE IF NOT EXISTS public.segmentos (
  id integer PRIMARY KEY,
  nome varchar(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.atividades (
  id integer PRIMARY KEY,
  nome varchar(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.saas_planos (
  id integer PRIMARY KEY,
  nome varchar(255) NOT NULL,
  descricao text,
  valor_mensal numeric,
  limite_usuarios integer,
  limite_empresas integer,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_saas_planos_updated_at ON public.saas_planos;
CREATE TRIGGER update_saas_planos_updated_at BEFORE UPDATE ON public.saas_planos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.config_responsavel_tecnico (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  cnpj varchar(14) NOT NULL,
  x_contato varchar(60) NOT NULL,
  email varchar(60),
  fone varchar(14),
  id_csrt varchar(2),
  hash_csrt varchar(28),
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_config_responsavel_tecnico_updated_at ON public.config_responsavel_tecnico;
CREATE TRIGGER update_config_responsavel_tecnico_updated_at BEFORE UPDATE ON public.config_responsavel_tecnico FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.certificados_digitais (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  nome_arquivo text,
  pfx_base64 text NOT NULL,
  senha text NOT NULL,
  data_vencimento timestamp,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_certificados_digitais_updated_at ON public.certificados_digitais;
CREATE TRIGGER update_certificados_digitais_updated_at BEFORE UPDATE ON public.certificados_digitais FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.configuracoes_emissoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  certificado_id uuid,
  ambiente integer NOT NULL DEFAULT 2,
  serie integer NOT NULL DEFAULT 1,
  modelo integer NOT NULL DEFAULT 65,
  proximo_numero_nf integer NOT NULL DEFAULT 1,
  csc_id text,
  csc_token text,
  tipo_emissao_padrao integer DEFAULT 1,
  tipo_impressao_default integer DEFAULT 1,
  data_ultima_emissao timestamp,
  tratamento_desconto varchar(20) NOT NULL DEFAULT 'RATEIO',
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_configuracoes_emissoes_updated_at ON public.configuracoes_emissoes;
CREATE TRIGGER update_configuracoes_emissoes_updated_at BEFORE UPDATE ON public.configuracoes_emissoes FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.saas_cupons_desconto (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  codigo varchar(50) UNIQUE NOT NULL,
  descricao text,
  tipo_desconto varchar(50),
  valor_desconto numeric,
  data_inicio date,
  data_fim date,
  quantidade_total integer,
  quantidade_utilizada integer DEFAULT 0,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_saas_cupons_desconto_updated_at ON public.saas_cupons_desconto;
CREATE TRIGGER update_saas_cupons_desconto_updated_at BEFORE UPDATE ON public.saas_cupons_desconto FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.saas_comunicados (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  titulo varchar(255) NOT NULL,
  mensagem text NOT NULL,
  tipo varchar(50) NOT NULL DEFAULT 'aviso',
  modulo_alvo varchar(100),
  data_expiracao timestamp,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid
);

CREATE TABLE IF NOT EXISTS public.saas_comunicados_opcoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  comunicado_id uuid NOT NULL,
  texto varchar(255) NOT NULL,
  ordem integer NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS public.saas_comunicados_leituras (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  comunicado_id uuid NOT NULL,
  conta_id uuid NOT NULL,
  usuario_id uuid NOT NULL,
  opcao_selecionada_id uuid,
  data_leitura timestamp NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.contas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  portal_slug varchar(255) UNIQUE NOT NULL,
  nome varchar(255) NOT NULL,
  email varchar(255) UNIQUE,
  telefone varchar(50),
  saas_cupom_desconto_id uuid,
  cep varchar(20),
  logradouro varchar(255),
  numero varchar(50),
  complemento varchar(100),
  bairro varchar(100),
  cidade varchar(100),
  uf varchar(2),
  pais varchar(100) DEFAULT 'Brasil',
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_contas_updated_at ON public.contas;
CREATE TRIGGER update_contas_updated_at BEFORE UPDATE ON public.contas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(255) NOT NULL,
  razao_social varchar(255),
  email varchar(255) UNIQUE,
  telefone varchar(50),
  cnpj varchar(20),
  cpf varchar(20),
  inscricao_estadual varchar(50),
  inscricao_municipal varchar(50),
  cnae_principal varchar(20),
  crt integer DEFAULT 1,
  logradouro varchar(255),
  numero varchar(50),
  complemento varchar(100),
  cep varchar(20),
  bairro varchar(100),
  uf varchar(2),
  cidade varchar(100),
  pais varchar(100) DEFAULT 'Brasil',
  segmento_id integer,
  atividade_id integer,
  config_campos_customizados jsonb DEFAULT '[]',
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_updated_at ON public.empresas;
CREATE TRIGGER update_empresas_updated_at BEFORE UPDATE ON public.empresas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.usuarios (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nome varchar(255) NOT NULL,
  email varchar(255) UNIQUE NOT NULL,
  senha_hash text NOT NULL,
  ativo boolean DEFAULT true,
  email_validado boolean DEFAULT false,
  url_imagem text,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_usuarios_updated_at ON public.usuarios;
CREATE TRIGGER update_usuarios_updated_at BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.codigos_validacao (
  id bigserial PRIMARY KEY,
  email varchar(255) NOT NULL,
  codigo varchar(10) NOT NULL,
  tipo varchar(50) NOT NULL,
  expira_em timestamp NOT NULL,
  usado boolean DEFAULT false,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.usuarios_contas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  conta_id uuid NOT NULL,
  is_titular boolean DEFAULT false,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid
);

CREATE TABLE IF NOT EXISTS public.usuarios_empresas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  empresa_id uuid NOT NULL,
  grupo_id uuid,
  perfil public.perfil DEFAULT 'Recepcionista',
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_usuarios_empresas_updated_at ON public.usuarios_empresas;
CREATE TRIGGER update_usuarios_empresas_updated_at BEFORE UPDATE ON public.usuarios_empresas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.grupos_usuarios (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  is_admin boolean DEFAULT false,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_grupos_usuarios_updated_at ON public.grupos_usuarios;
CREATE TRIGGER update_grupos_usuarios_updated_at BEFORE UPDATE ON public.grupos_usuarios FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.recursos_sistema (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  chave varchar(100) UNIQUE NOT NULL,
  tipo varchar(50) NOT NULL,
  nome varchar(255) NOT NULL,
  descricao text,
  modulo varchar(100),
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.grupos_permissoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  grupo_id uuid NOT NULL,
  recurso_id uuid NOT NULL,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid
);

CREATE TABLE IF NOT EXISTS public.tipos_vendedor (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_tipos_vendedor_updated_at ON public.tipos_vendedor;
CREATE TRIGGER update_tipos_vendedor_updated_at BEFORE UPDATE ON public.tipos_vendedor FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.vendedores (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  usuario_id uuid,
  tipo_vendedor_id uuid NOT NULL,
  nome_fantasia varchar(255) NOT NULL,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_vendedores_updated_at ON public.vendedores;
CREATE TRIGGER update_vendedores_updated_at BEFORE UPDATE ON public.vendedores FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contatos_pessoas_fisicas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(255) NOT NULL,
  cpf varchar(20),
  rg varchar(50),
  data_nascimento date,
  email varchar(255),
  telefone varchar(50),
  celular varchar(50),
  is_cliente boolean DEFAULT false,
  is_fornecedor boolean DEFAULT false,
  is_profissional boolean DEFAULT false,
  usuario_plataforma_id uuid,
  vendedor_1_id uuid,
  vendedor_2_id uuid,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_contatos_pessoas_fisicas_updated_at ON public.contatos_pessoas_fisicas;
CREATE TRIGGER update_contatos_pessoas_fisicas_updated_at BEFORE UPDATE ON public.contatos_pessoas_fisicas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.enderecos_pf (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  contato_pessoa_fisica_id uuid NOT NULL,
  tipo_endereco varchar(50) DEFAULT 'PRINCIPAL',
  cep varchar(20),
  logradouro varchar(255),
  numero varchar(50),
  complemento varchar(100),
  bairro varchar(100),
  cidade varchar(100),
  uf varchar(2),
  pais varchar(100) DEFAULT 'Brasil',
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.contatos_pessoas_juridicas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  razao_social varchar(255) NOT NULL,
  nome_fantasia varchar(255),
  cnpj varchar(20),
  inscricao_estadual varchar(50),
  email varchar(255),
  telefone varchar(50),
  celular varchar(50),
  is_cliente boolean DEFAULT false,
  is_fornecedor boolean DEFAULT false,
  conta_plataforma_id uuid,
  vendedor_1_id uuid,
  vendedor_2_id uuid,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_contatos_pessoas_juridicas_updated_at ON public.contatos_pessoas_juridicas;
CREATE TRIGGER update_contatos_pessoas_juridicas_updated_at BEFORE UPDATE ON public.contatos_pessoas_juridicas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.enderecos_pj (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  contato_pessoa_juridica_id uuid NOT NULL,
  tipo_endereco varchar(50) DEFAULT 'PRINCIPAL',
  cep varchar(20),
  logradouro varchar(255),
  numero varchar(50),
  complemento varchar(100),
  bairro varchar(100),
  cidade varchar(100),
  uf varchar(2),
  pais varchar(100) DEFAULT 'Brasil',
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.empresas_clientes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  limite_credito numeric DEFAULT 0,
  desconto_padrao numeric DEFAULT 0,
  forma_pagamento_padrao_id uuid,
  condicao_pagamento_padrao_id uuid,
  natureza_operacao_padrao_id uuid,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_clientes_updated_at ON public.empresas_clientes;
CREATE TRIGGER update_empresas_clientes_updated_at BEFORE UPDATE ON public.empresas_clientes FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas_fornecedores (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  forma_pagamento_padrao_id uuid,
  condicao_pagamento_padrao_id uuid,
  natureza_operacao_padrao_id uuid,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_fornecedores_updated_at ON public.empresas_fornecedores;
CREATE TRIGGER update_empresas_fornecedores_updated_at BEFORE UPDATE ON public.empresas_fornecedores FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas_transportadores (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_transportadores_updated_at ON public.empresas_transportadores;
CREATE TRIGGER update_empresas_transportadores_updated_at BEFORE UPDATE ON public.empresas_transportadores FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.solicitacoes_vinculo (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_solicitante_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  conta_alvo_id uuid,
  usuario_alvo_id uuid,
  papel_proposto varchar(50) NOT NULL,
  mensagem text,
  status varchar(20) NOT NULL DEFAULT 'pendente',
  data_resposta timestamp,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp
);
DROP TRIGGER IF EXISTS update_solicitacoes_vinculo_updated_at ON public.solicitacoes_vinculo;
CREATE TRIGGER update_solicitacoes_vinculo_updated_at BEFORE UPDATE ON public.solicitacoes_vinculo FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.saas_metodos_pagamento (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  tipo varchar(50),
  gateway varchar(50),
  gateway_customer_id varchar(255),
  gateway_payment_method_id varchar(255),
  ultimos_4_digitos varchar(4),
  bandeira varchar(50),
  validade_mes integer,
  validade_ano integer,
  principal boolean DEFAULT false,
  ativo boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_saas_metodos_pagamento_updated_at ON public.saas_metodos_pagamento;
CREATE TRIGGER update_saas_metodos_pagamento_updated_at BEFORE UPDATE ON public.saas_metodos_pagamento FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.saas_planos_assinaturas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  saas_plano_id integer,
  metodo_pagamento_id uuid,
  status varchar(50) DEFAULT 'ATIVA',
  valor numeric,
  data_inicio date,
  data_fim date,
  proxima_fatura date,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_saas_planos_assinaturas_updated_at ON public.saas_planos_assinaturas;
CREATE TRIGGER update_saas_planos_assinaturas_updated_at BEFORE UPDATE ON public.saas_planos_assinaturas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.saas_planos_assinaturas_faturas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  assinatura_id uuid,
  numero_parcela integer,
  status varchar(50),
  valor numeric,
  forma_pagamento varchar(50),
  chave_pix text,
  data_vencimento date,
  data_pagamento timestamp,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_saas_planos_assinaturas_faturas_updated_at ON public.saas_planos_assinaturas_faturas;
CREATE TRIGGER update_saas_planos_assinaturas_faturas_updated_at BEFORE UPDATE ON public.saas_planos_assinaturas_faturas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas_funcoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_funcoes_updated_at ON public.empresas_funcoes;
CREATE TRIGGER update_empresas_funcoes_updated_at BEFORE UPDATE ON public.empresas_funcoes FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas_profissionais (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  funcao_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  imagem varchar(100),
  servicos_executados integer DEFAULT 0,
  data_inicio date,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_profissionais_updated_at ON public.empresas_profissionais;
CREATE TRIGGER update_empresas_profissionais_updated_at BEFORE UPDATE ON public.empresas_profissionais FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.empresas_profissionais_horarios (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  profissional_id uuid NOT NULL,
  is_segunda boolean DEFAULT false,
  is_terca boolean DEFAULT false,
  is_quarta boolean DEFAULT false,
  is_quinta boolean DEFAULT false,
  is_sexta boolean DEFAULT false,
  is_sabado boolean DEFAULT false,
  is_domingo boolean DEFAULT false,
  hora_inicio time NOT NULL,
  hora_fim time NOT NULL,
  pausa_inicio time,
  pausa_fim time,
  duracao_minutos integer DEFAULT 15,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_empresas_profissionais_horarios_updated_at ON public.empresas_profissionais_horarios;
CREATE TRIGGER update_empresas_profissionais_horarios_updated_at BEFORE UPDATE ON public.empresas_profissionais_horarios FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.bloqueios_agenda (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  profissional_id uuid,
  inicio timestamp NOT NULL,
  fim timestamp NOT NULL,
  motivo text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid
);

CREATE TABLE IF NOT EXISTS public.agendamentos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  cliente_id uuid,
  cliente_tipo varchar(50),
  profissional_id uuid,
  os_id uuid,
  nome_cliente_avulso varchar(255),
  valor_total numeric,
  inicio timestamp NOT NULL,
  fim timestamp NOT NULL,
  situacao public.situacao_agendamento DEFAULT 'pendente',
  status boolean DEFAULT true,
  observacao text,
  gera_financeiro boolean DEFAULT true,
  forma_pagamento_id uuid,
  qtd_parcelas integer DEFAULT 1,
  intervalo_dias integer,
  campos_customizados jsonb DEFAULT '{}',
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_agendamentos_updated_at ON public.agendamentos;
CREATE TRIGGER update_agendamentos_updated_at BEFORE UPDATE ON public.agendamentos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.agendamento_servicos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  agendamento_id uuid NOT NULL,
  url_imagem text,
  produto_variacao_id uuid NOT NULL,
  profissional_id uuid,
  duracao_minutos int,
  observacao text,
  valor numeric,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.servico_desconto (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  agendamento_servicos_id uuid NOT NULL,
  status boolean DEFAULT true,
  valor numeric NOT NULL,
  percentual decimal,
  data_inicio timestamp DEFAULT now(),
  data_fim timestamp,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_servico_desconto_updated_at ON public.servico_desconto;
CREATE TRIGGER update_servico_desconto_updated_at BEFORE UPDATE ON public.servico_desconto FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.agendamento_config (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL UNIQUE,
  texto_whatsapp_agendamento text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_agendamento_config_updated_at ON public.agendamento_config;
CREATE TRIGGER update_agendamento_config_updated_at BEFORE UPDATE ON public.agendamento_config FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.feriados (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  data date NOT NULL,
  nome text NOT NULL,
  uf char(2),
  tipo varchar(50) DEFAULT 'nacional',
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_feriados_updated_at ON public.feriados;
CREATE TRIGGER update_feriados_updated_at BEFORE UPDATE ON public.feriados FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.auditoria_registros (
  id bigserial PRIMARY KEY,
  conta_id uuid,
  schema_name text NOT NULL,
  table_name text NOT NULL,
  user_id uuid,
  action_timestamp timestamp NOT NULL DEFAULT now(),
  action text NOT NULL,
  record_id text,
  old_values jsonb,
  new_values jsonb,
  query text
);

CREATE TABLE IF NOT EXISTS public.documentos_legais (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  tipo text NOT NULL,
  versao text NOT NULL,
  titulo text,
  texto text NOT NULL,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_documentos_legais_updated_at ON public.documentos_legais;
CREATE TRIGGER update_documentos_legais_updated_at BEFORE UPDATE ON public.documentos_legais FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.documentos_aceites (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  documento_legal_id uuid NOT NULL,
  data_aceite timestamp NOT NULL DEFAULT now(),
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_documentos_aceites_updated_at ON public.documentos_aceites;
CREATE TRIGGER update_documentos_aceites_updated_at BEFORE UPDATE ON public.documentos_aceites FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.produto_categorias (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_produto_categorias_updated_at ON public.produto_categorias;
CREATE TRIGGER update_produto_categorias_updated_at BEFORE UPDATE ON public.produto_categorias FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.marcas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_marcas_updated_at ON public.marcas;
CREATE TRIGGER update_marcas_updated_at BEFORE UPDATE ON public.marcas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.produtos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  categoria_id uuid,
  marca_id uuid,
  nome varchar(255) NOT NULL,
  descricao text,
  tipo_item varchar(20) DEFAULT 'mercadoria',
  codigo_anvisa varchar(13),
  dias_validade_maxima integer,
  controla_lote boolean DEFAULT false,
  cnpj_fabricante varchar(14),
  ncm varchar(8),
  cest varchar(7),
  origem integer DEFAULT 0,
  cfop_padrao varchar(4),
  cst_padrao varchar(4),
  cst_ipi_padrao varchar(3) DEFAULT '99',
  codigo_enquadramento_ipi varchar(3) DEFAULT '999',
  modalidade_base_calculo_icms integer DEFAULT 3,
  cst_pis_padrao varchar(2) DEFAULT '99',
  cst_cofins_padrao varchar(2) DEFAULT '99',
  aliquota_pis numeric DEFAULT 0,
  aliquota_cofins numeric DEFAULT 0,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_produtos_updated_at ON public.produtos;
CREATE TRIGGER update_produtos_updated_at BEFORE UPDATE ON public.produtos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.produto_variacoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  produto_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  sku varchar(50),
  codigo_barras varchar(14),
  codigo_interno varchar(60),
  unidade_medida varchar(6) DEFAULT 'UN',
  unidade_tributavel varchar(6),
  fator_conversao numeric DEFAULT 1,
  lote_multiplo numeric DEFAULT 1,
  peso_liquido numeric DEFAULT 0,
  peso_bruto numeric DEFAULT 0,
  altura_cm numeric,
  largura_cm numeric,
  profundidade_cm numeric,
  estoque_minimo numeric DEFAULT 0,
  estoque_maximo numeric DEFAULT 0,
  lead_time_dias integer DEFAULT 0,
  tempo_minutos integer DEFAULT 30,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_produto_variacoes_updated_at ON public.produto_variacoes;
CREATE TRIGGER update_produto_variacoes_updated_at BEFORE UPDATE ON public.produto_variacoes FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contextos_preco (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(255) NOT NULL,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contextos_preco_updated_at ON public.contextos_preco;
CREATE TRIGGER update_contextos_preco_updated_at BEFORE UPDATE ON public.contextos_preco FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.precos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  produto_variacao_id uuid NOT NULL,
  contexto_preco_id uuid NOT NULL,
  valor numeric NOT NULL,
  percentual_comissao numeric DEFAULT 0,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_precos_updated_at ON public.precos;
CREATE TRIGGER update_precos_updated_at BEFORE UPDATE ON public.precos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.areas_estoque (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_areas_estoque_updated_at ON public.areas_estoque;
CREATE TRIGGER update_areas_estoque_updated_at BEFORE UPDATE ON public.areas_estoque FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.locais_estoque (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  area_estoque_id uuid NOT NULL,
  empresa_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  codigo_visual varchar(50),
  tipo varchar(30) DEFAULT 'pulmao',
  prioridade_de_saida integer DEFAULT 0,
  url_imagem text,
  status text DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_locais_estoque_updated_at ON public.locais_estoque;
CREATE TRIGGER update_locais_estoque_updated_at BEFORE UPDATE ON public.locais_estoque FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.estoque_saldo (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  produto_variacao_id uuid NOT NULL,
  local_estoque_id uuid NOT NULL,
  lote varchar(100),
  data_validade date,
  data_fabricacao date,
  cnpj_fabricante varchar(14),
  quantidade_atual numeric DEFAULT 0,
  data_entrada timestamp DEFAULT now(),
  url_imagem text,
  status text DEFAULT 'disponivel',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_estoque_saldo_updated_at ON public.estoque_saldo;
CREATE TRIGGER update_estoque_saldo_updated_at BEFORE UPDATE ON public.estoque_saldo FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.estoque_movimentacoes (
  id bigserial PRIMARY KEY,
  estoque_saldo_id uuid NOT NULL,
  tipo_movimentacao varchar(50) NOT NULL,
  quantidade numeric NOT NULL,
  motivo text,
  lote varchar(100),
  data_validade date,
  data_fabricacao date,
  cnpj_fabricante varchar(14),
  usuario_responsavel_id uuid,
  data_movimentacao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.formas_pagamento (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  tipo varchar(50) DEFAULT 'dinheiro',
  taxa_percentual numeric DEFAULT 0,
  dias_recebimento integer DEFAULT 0,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.controle_caixa (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  usuario_id uuid NOT NULL,
  data_abertura timestamp DEFAULT now(),
  data_fechamento timestamp,
  saldo_inicial numeric DEFAULT 0,
  saldo_final numeric,
  status varchar(50) DEFAULT 'aberto',
  observacao text
);

CREATE TABLE IF NOT EXISTS public.vendas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  caixa_id uuid,
  cliente_id uuid,
  cliente_tipo varchar(50),
  valor_subtotal numeric DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  valor_total numeric DEFAULT 0,
  status varchar(50) DEFAULT 'concluida',
  data_venda timestamp DEFAULT now(),
  operador_inclusao uuid
);

CREATE TABLE IF NOT EXISTS public.venda_itens (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  venda_id uuid NOT NULL,
  produto_variacao_id uuid NOT NULL,
  profissional_id uuid,
  quantidade numeric DEFAULT 1,
  valor_unitario numeric NOT NULL,
  valor_desconto numeric DEFAULT 0,
  valor_total numeric NOT NULL
);

CREATE TABLE IF NOT EXISTS public.contas_receber (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  cliente_id uuid,
  cliente_tipo varchar(50),
  origem_id uuid,
  origem_tipo varchar(50),
  forma_pagamento_id uuid,
  descricao varchar(255),
  numero_parcela integer DEFAULT 1,
  valor numeric NOT NULL,
  data_vencimento date NOT NULL,
  data_pagamento date,
  situacao public.situacao_conta DEFAULT 'pendente',
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.contas_pagar (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  fornecedor_id uuid,
  fornecedor_tipo varchar(50),
  origem_id uuid,
  origem_tipo varchar(50),
  descricao varchar(255) NOT NULL,
  numero_parcela integer DEFAULT 1,
  valor numeric NOT NULL,
  data_vencimento date NOT NULL,
  data_pagamento date,
  situacao public.situacao_conta DEFAULT 'pendente',
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.contas_bancarias (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  nome text NOT NULL,
  banco_codigo varchar(10) NOT NULL,
  banco_nome text NOT NULL,
  agencia varchar(10) NOT NULL,
  agencia_digito varchar(2),
  conta varchar(20) NOT NULL,
  conta_digito varchar(2),
  carteira varchar(10),
  convenio varchar(20),
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_bancarias_updated_at ON public.contas_bancarias;
CREATE TRIGGER update_contas_bancarias_updated_at BEFORE UPDATE ON public.contas_bancarias FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_receber_comissao (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_a_receber_id uuid NOT NULL,
  movimentacao_pagamento_id uuid NOT NULL,
  origem_id uuid,
  origem_tipo text,
  beneficiario_id uuid NOT NULL,
  beneficiario_tipo text NOT NULL,
  valor_base_calculo numeric NOT NULL,
  valor_total_deducoes numeric NOT NULL DEFAULT 0.00,
  valor_liquido_calculo numeric NOT NULL,
  percentual_comissao numeric NOT NULL,
  valor_final_comissao numeric NOT NULL,
  data_pagamento_comissao date,
  observacoes text,
  url_imagem text,
  status text NOT NULL DEFAULT 'pendente',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_receber_comissao_updated_at ON public.contas_a_receber_comissao;
CREATE TRIGGER update_contas_a_receber_comissao_updated_at BEFORE UPDATE ON public.contas_a_receber_comissao FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.comissao_deducao_regra (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  descricao text NOT NULL,
  tipo_deducao text NOT NULL,
  valor numeric NOT NULL,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_comissao_deducao_regra_updated_at ON public.comissao_deducao_regra;
CREATE TRIGGER update_comissao_deducao_regra_updated_at BEFORE UPDATE ON public.comissao_deducao_regra FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_receber_comissao_deducao (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  comissao_id uuid NOT NULL,
  descricao text NOT NULL,
  tipo_deducao text NOT NULL,
  valor numeric NOT NULL,
  valor_calculado_deducao numeric NOT NULL,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_receber_comissao_deducao_updated_at ON public.contas_a_receber_comissao_deducao;
CREATE TRIGGER update_contas_a_receber_comissao_deducao_updated_at BEFORE UPDATE ON public.contas_a_receber_comissao_deducao FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.conta_a_pagar_credito (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  criador_id uuid NOT NULL,
  criador_tipo text NOT NULL,
  usuario_id uuid NOT NULL,
  usuario_tipo text NOT NULL,
  origem_id uuid,
  origem_tipo text,
  valor_original numeric NOT NULL,
  saldo_disponivel numeric NOT NULL,
  descricao text NOT NULL,
  data_expiracao date,
  url_imagem text,
  status text NOT NULL DEFAULT 'disponivel',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_conta_a_pagar_credito_updated_at ON public.conta_a_pagar_credito;
CREATE TRIGGER update_conta_a_pagar_credito_updated_at BEFORE UPDATE ON public.conta_a_pagar_credito FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.conta_a_receber_credito (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  criador_id uuid NOT NULL,
  criador_tipo text NOT NULL,
  usuario_id uuid NOT NULL,
  usuario_tipo text NOT NULL,
  origem_id uuid,
  origem_tipo text,
  valor_original numeric NOT NULL,
  saldo_disponivel numeric NOT NULL,
  descricao text NOT NULL,
  data_expiracao timestamp,
  url_imagem text,
  status text NOT NULL DEFAULT 'disponivel',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_conta_a_receber_credito_updated_at ON public.conta_a_receber_credito;
CREATE TRIGGER update_conta_a_receber_credito_updated_at BEFORE UPDATE ON public.conta_a_receber_credito FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_pagar (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  devedor_id uuid NOT NULL,
  devedor_tipo text NOT NULL,
  credor_id uuid NOT NULL,
  credor_tipo text NOT NULL,
  origem_id uuid,
  origem_tipo text,
  descricao text NOT NULL,
  numero_nota_fiscal text,
  valor_original numeric NOT NULL,
  juros_multa numeric DEFAULT 0.00,
  desconto numeric DEFAULT 0.00,
  valor_pago numeric,
  data_emissao date NOT NULL,
  data_vencimento date NOT NULL,
  data_pagamento date,
  conta_bancaria_id uuid,
  forma_pagamento text,
  observacoes text,
  url_imagem text,
  status text NOT NULL DEFAULT 'pendente',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_pagar_updated_at ON public.contas_a_pagar;
CREATE TRIGGER update_contas_a_pagar_updated_at BEFORE UPDATE ON public.contas_a_pagar FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_pagar_movimentacao (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_a_pagar_id uuid NOT NULL,
  tipo_movimentacao text NOT NULL,
  valor numeric NOT NULL,
  data_movimentacao date NOT NULL,
  descricao text,
  usuario_responsavel_id uuid,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_pagar_movimentacao_updated_at ON public.contas_a_pagar_movimentacao;
CREATE TRIGGER update_contas_a_pagar_movimentacao_updated_at BEFORE UPDATE ON public.contas_a_pagar_movimentacao FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_receber (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  credor_id uuid NOT NULL,
  credor_tipo text NOT NULL,
  devedor_id uuid NOT NULL,
  devedor_tipo text NOT NULL,
  descricao text NOT NULL,
  numero_nota_fiscal text,
  numero_parcela integer NOT NULL DEFAULT 1,
  total_parcelas integer NOT NULL DEFAULT 1,
  valor_original numeric NOT NULL,
  juros_multa numeric DEFAULT 0.00,
  desconto numeric DEFAULT 0.00,
  valor_pago numeric,
  data_emissao timestamp NOT NULL,
  data_vencimento timestamp NOT NULL,
  data_pagamento timestamp,
  conta_bancaria_id uuid,
  forma_pagamento text,
  observacoes text,
  origem_id uuid,
  origem_tipo text,
  url_imagem text,
  status text NOT NULL DEFAULT 'pendente',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_receber_updated_at ON public.contas_a_receber;
CREATE TRIGGER update_contas_a_receber_updated_at BEFORE UPDATE ON public.contas_a_receber FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.contas_a_receber_movimentacao (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_a_receber_id uuid NOT NULL,
  tipo_movimentacao text NOT NULL,
  valor numeric NOT NULL,
  data_movimentacao timestamp NOT NULL,
  descricao text,
  usuario_responsavel_id uuid,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_contas_a_receber_movimentacao_updated_at ON public.contas_a_receber_movimentacao;
CREATE TRIGGER update_contas_a_receber_movimentacao_updated_at BEFORE UPDATE ON public.contas_a_receber_movimentacao FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.notas_fiscais (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  origem_tipo varchar(50) NOT NULL,
  origem_id uuid NOT NULL,
  tipo_nota varchar(10) NOT NULL,
  embarque_id uuid,
  numero_nota_fiscal text,
  serie_nota_fiscal text,
  chave_acesso text,
  status_sefaz text,
  xml_url text,
  data_emissao timestamp DEFAULT now(),
  valor_total numeric NOT NULL DEFAULT 0,
  tipo_operacao varchar(20) NOT NULL DEFAULT 'SAIDA',
  chave_acesso_referenciada varchar(44),
  natureza_operacao text NOT NULL DEFAULT 'VENDA',
  tipo_pagamento_sefaz varchar(2) DEFAULT '99',
  finalidade_emissao integer DEFAULT 1,
  valor_seguro numeric DEFAULT 0,
  valor_outras_despesas numeric DEFAULT 0,
  valor_total_base_icms numeric DEFAULT 0,
  valor_total_icms numeric DEFAULT 0,
  valor_total_icms_deson numeric DEFAULT 0,
  valor_total_base_st numeric DEFAULT 0,
  valor_total_st numeric DEFAULT 0,
  valor_total_fcp numeric DEFAULT 0,
  valor_total_fcp_st numeric DEFAULT 0,
  valor_total_fcp_st_ret numeric DEFAULT 0,
  valor_total_ipi numeric DEFAULT 0,
  valor_total_ipi_devol numeric DEFAULT 0,
  valor_total_pis numeric DEFAULT 0,
  valor_total_cofins numeric DEFAULT 0,
  valor_total_ii numeric DEFAULT 0,
  valor_total_tributos numeric DEFAULT 0,
  valor_total_is numeric DEFAULT 0,
  valor_frete numeric DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  valor_subtotal numeric DEFAULT 0,
  observacao text,
  motivo_rejeicao text,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_notas_fiscais_updated_at ON public.notas_fiscais;
CREATE TRIGGER update_notas_fiscais_updated_at BEFORE UPDATE ON public.notas_fiscais FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.nota_fiscal_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  nota_fiscal_id uuid NOT NULL,
  origem_item_id uuid,
  embarque_item_id uuid,
  produto_variacao_id uuid,
  natureza_operacao_id uuid,
  nome_produto varchar(255) NOT NULL,
  quantidade numeric NOT NULL,
  preco_unitario numeric NOT NULL,
  valor_total_item numeric DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  percentual_desconto numeric DEFAULT 0,
  compoe_valor_total integer NOT NULL DEFAULT 1,
  cfop varchar(4),
  cst_icms varchar(4),
  aliquota_icms numeric DEFAULT 0,
  valor_base_icms numeric DEFAULT 0,
  valor_icms numeric DEFAULT 0,
  valor_base_icms_st numeric DEFAULT 0,
  aliquota_icms_st numeric DEFAULT 0,
  valor_icms_st numeric DEFAULT 0,
  aliquota_mva_st numeric DEFAULT 0,
  percentual_reducao_base_st numeric DEFAULT 0,
  percentual_reducao_base numeric DEFAULT 0,
  cst_ipi varchar(3),
  codigo_enquadramento_ipi varchar(3),
  aliquota_ipi numeric DEFAULT 0,
  valor_base_ipi numeric DEFAULT 0,
  valor_ipi numeric DEFAULT 0,
  cst_pis varchar(3),
  aliquota_pis numeric DEFAULT 0,
  valor_base_pis numeric DEFAULT 0,
  valor_pis numeric DEFAULT 0,
  cst_cofins varchar(3),
  aliquota_cofins numeric DEFAULT 0,
  valor_base_cofins numeric DEFAULT 0,
  valor_cofins numeric DEFAULT 0,
  cst_ibs_cbs varchar(4),
  valor_base_ibs numeric DEFAULT 0,
  aliquota_ibs numeric DEFAULT 0,
  valor_ibs numeric DEFAULT 0,
  valor_base_cbs numeric DEFAULT 0,
  aliquota_cbs numeric DEFAULT 0,
  valor_cbs numeric DEFAULT 0,
  cst_is varchar(4),
  valor_base_is numeric DEFAULT 0,
  aliquota_is numeric DEFAULT 0,
  valor_is numeric DEFAULT 0,
  lote varchar(100),
  data_validade date,
  data_fabricacao date,
  cnpj_fabricante varchar(14),
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_nota_fiscal_itens_updated_at ON public.nota_fiscal_itens;
CREATE TRIGGER update_nota_fiscal_itens_updated_at BEFORE UPDATE ON public.nota_fiscal_itens FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.embarques (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pedido_id uuid NOT NULL,
  usuario_separador_id uuid,
  data_separacao timestamp,
  url_imagem text,
  status text NOT NULL DEFAULT 'pendente',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_embarques_updated_at ON public.embarques;
CREATE TRIGGER update_embarques_updated_at BEFORE UPDATE ON public.embarques FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.embarque_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  embarque_id uuid NOT NULL,
  pedido_item_id uuid NOT NULL,
  estoque_saldo_id uuid,
  quantidade numeric NOT NULL,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_embarque_itens_updated_at ON public.embarque_itens;
CREATE TRIGGER update_embarque_itens_updated_at BEFORE UPDATE ON public.embarque_itens FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.naturezas_operacao (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  descricao varchar(100) NOT NULL,
  descricao_fiscal varchar(60),
  cfop_padrao varchar(4) NOT NULL,
  indicador_total_padrao integer NOT NULL DEFAULT 1,
  finalidade_n_fe integer NOT NULL DEFAULT 1,
  soma_ipi_no_total boolean DEFAULT FALSE,
  soma_icms_st_no_total boolean DEFAULT FALSE,
  soma_is_no_total boolean DEFAULT FALSE,
  incide_ibs_cbs boolean DEFAULT TRUE,
  gera_financeiro boolean DEFAULT TRUE,
  movimenta_estoque boolean DEFAULT TRUE,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_naturezas_operacao_updated_at ON public.naturezas_operacao;
CREATE TRIGGER update_naturezas_operacao_updated_at BEFORE UPDATE ON public.naturezas_operacao FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.regras_fiscais (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  natureza_operacao_id uuid NOT NULL,
  ncm varchar(8),
  uf_destino CHAR(2),
  tipo_contribuinte integer DEFAULT 0,
  origem_mercadoria integer DEFAULT 0,
  cfop varchar(4) NOT NULL,
  cst_icms varchar(4) NOT NULL,
  modalidade_base_calculo_icms integer DEFAULT 3,
  aliquota_icms numeric DEFAULT 0,
  reducao_base_icms numeric DEFAULT 0,
  aliquota_fcp numeric DEFAULT 0,
  aliquota_mva numeric DEFAULT 0,
  aliquota_icms_interna numeric DEFAULT 0,
  pauta_fiscal numeric DEFAULT 0,
  reducao_base_icms_st numeric DEFAULT 0,
  aliquota_fcp_st numeric DEFAULT 0,
  cst_ipi varchar(3) DEFAULT '99',
  aliquota_ipi numeric DEFAULT 0,
  cst_pis varchar(3) DEFAULT '99',
  aliquota_pis numeric DEFAULT 0,
  cst_cofins varchar(3) DEFAULT '99',
  aliquota_cofins numeric DEFAULT 0,
  codigo_beneficio_fiscal varchar(15),
  motivo_desoneracao_icms integer,
  cst_ibs_cbs varchar(4),
  aliquota_ibs numeric DEFAULT 0,
  aliquota_cbs numeric DEFAULT 0,
  cst_is varchar(4),
  aliquota_is numeric DEFAULT 0,
  valor_pauta_is numeric DEFAULT 0,
  observacao_legal text,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_regras_fiscais_updated_at ON public.regras_fiscais;
CREATE TRIGGER update_regras_fiscais_updated_at BEFORE UPDATE ON public.regras_fiscais FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.formas_pagamento_fiscal (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(50) NOT NULL,
  codigo_sefaz varchar(2),
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_formas_pagamento_fiscal_updated_at ON public.formas_pagamento_fiscal;
CREATE TRIGGER update_formas_pagamento_fiscal_updated_at BEFORE UPDATE ON public.formas_pagamento_fiscal FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.condicoes_pagamento (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  conta_id uuid NOT NULL,
  nome varchar(100) NOT NULL,
  descricao text,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_condicoes_pagamento_updated_at ON public.condicoes_pagamento;
CREATE TRIGGER update_condicoes_pagamento_updated_at BEFORE UPDATE ON public.condicoes_pagamento FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.condicao_pagamento_parcelas (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  condicao_pagamento_id uuid NOT NULL,
  dias_para_vencimento integer NOT NULL DEFAULT 0,
  percentual numeric NOT NULL DEFAULT 0,
  forma_pagamento_padrao_id uuid,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_condicao_pagamento_parcelas_updated_at ON public.condicao_pagamento_parcelas;
CREATE TRIGGER update_condicao_pagamento_parcelas_updated_at BEFORE UPDATE ON public.condicao_pagamento_parcelas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.orcamentos (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  cliente_id uuid NOT NULL,
  cliente_tipo text NOT NULL DEFAULT 'usuario',
  profissional_id uuid,
  documento_gerado_tipo varchar(50),
  documento_gerado_id uuid,
  natureza_operacao_id uuid,
  data_validade date NOT NULL,
  valor_subtotal numeric DEFAULT 0,
  valor_frete numeric DEFAULT 0,
  valor_total_ipi numeric DEFAULT 0,
  valor_total_icms_st numeric DEFAULT 0,
  valor_total numeric NOT NULL DEFAULT 0,
  observacoes text,
  observacao_interna text,
  observacao_cliente text,
  url_imagem text,
  status text NOT NULL DEFAULT 'RASCUNHO',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_orcamentos_updated_at ON public.orcamentos;
CREATE TRIGGER update_orcamentos_updated_at BEFORE UPDATE ON public.orcamentos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.orcamento_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  orcamento_id uuid NOT NULL,
  produto_variacao_id uuid NOT NULL,
  natureza_operacao_id uuid,
  quantidade numeric NOT NULL,
  preco_unitario numeric NOT NULL,
  valor_desconto numeric DEFAULT 0,
  percentual_desconto numeric DEFAULT 0,
  total numeric DEFAULT 0,
  observacao text,
  ncm varchar(10),
  cfop varchar(4),
  cst_icms varchar(4),
  aliquota_icms numeric DEFAULT 0,
  valor_base_icms numeric DEFAULT 0,
  valor_icms numeric DEFAULT 0,
  valor_icms_st numeric DEFAULT 0,
  valor_base_icms_st numeric DEFAULT 0,
  aliquota_icms_st numeric DEFAULT 0,
  aliquota_mva_st numeric DEFAULT 0,
  percentual_reducao_base_st numeric DEFAULT 0,
  cst_ipi varchar(3),
  codigo_enquadramento_ipi varchar(3) DEFAULT '999',
  aliquota_ipi numeric DEFAULT 0,
  valor_base_ipi numeric DEFAULT 0,
  valor_ipi numeric DEFAULT 0,
  cst_pis varchar(3),
  aliquota_pis numeric DEFAULT 0,
  valor_base_pis numeric DEFAULT 0,
  valor_pis numeric DEFAULT 0,
  cst_cofins varchar(3),
  aliquota_cofins numeric DEFAULT 0,
  valor_base_cofins numeric DEFAULT 0,
  valor_cofins numeric DEFAULT 0,
  cst_ibs_cbs varchar(4),
  aliquota_ibs numeric DEFAULT 0,
  valor_ibs numeric DEFAULT 0,
  aliquota_cbs numeric DEFAULT 0,
  valor_cbs numeric DEFAULT 0,
  cst_is varchar(4),
  aliquota_is numeric DEFAULT 0,
  valor_is numeric DEFAULT 0,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_orcamento_itens_updated_at ON public.orcamento_itens;
CREATE TRIGGER update_orcamento_itens_updated_at BEFORE UPDATE ON public.orcamento_itens FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pedidos_venda (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  codigo_pedido_sequence integer NOT NULL DEFAULT nextval('public.pedidos_venda_codigoPedidoSequence_seq'),
  empresa_id uuid NOT NULL,
  cliente_id uuid NOT NULL,
  cliente_tipo text NOT NULL DEFAULT 'usuario',
  profissional_id uuid,
  orcamento_origem_id uuid,
  natureza_operacao_id uuid,
  status varchar(50) NOT NULL DEFAULT 'RASCUNHO',
  valor_produtos numeric NOT NULL DEFAULT 0,
  valor_frete numeric DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  valor_total numeric NOT NULL DEFAULT 0,
  valor_total_ipi numeric DEFAULT 0,
  valor_total_icms_st numeric DEFAULT 0,
  endereco_entrega_id uuid,
  tipo_frete text,
  modalidade_frete integer NOT NULL DEFAULT 9,
  delivery_tempo_estimado_minutos integer,
  delivery_taxa_entrega numeric,
  indicador_consumidor_final integer NOT NULL DEFAULT 1,
  indicador_presenca integer NOT NULL DEFAULT 1,
  observacao_interna text,
  observacao_cliente text,
  url_imagem text,
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_pedidos_venda_updated_at ON public.pedidos_venda;
CREATE TRIGGER update_pedidos_venda_updated_at BEFORE UPDATE ON public.pedidos_venda FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pedido_venda_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pedido_venda_id uuid NOT NULL,
  produto_variacao_id uuid NOT NULL,
  estoque_saldo_id uuid,
  natureza_operacao_id uuid,
  quantidade numeric NOT NULL,
  preco_unitario numeric NOT NULL,
  valor_desconto numeric DEFAULT 0,
  percentual_desconto numeric DEFAULT 0,
  subtotal numeric DEFAULT 0,
  observacao text,
  cfop varchar(4),
  cst_icms varchar(4),
  aliquota_icms numeric DEFAULT 0,
  valor_base_icms numeric DEFAULT 0,
  valor_icms numeric DEFAULT 0,
  valor_icms_st numeric DEFAULT 0,
  valor_base_icms_st numeric DEFAULT 0,
  aliquota_icms_st numeric DEFAULT 0,
  aliquota_mva_st numeric DEFAULT 0,
  percentual_reducao_base_st numeric DEFAULT 0,
  percentual_reducao_base numeric DEFAULT 0,
  cst_ipi varchar(3),
  codigo_enquadramento_ipi varchar(3) DEFAULT '999',
  aliquota_ipi numeric DEFAULT 0,
  valor_base_ipi numeric DEFAULT 0,
  valor_ipi numeric DEFAULT 0,
  cst_pis varchar(3),
  aliquota_pis numeric DEFAULT 0,
  valor_base_pis numeric DEFAULT 0,
  valor_pis numeric DEFAULT 0,
  cst_cofins varchar(3),
  aliquota_cofins numeric DEFAULT 0,
  valor_base_cofins numeric DEFAULT 0,
  valor_cofins numeric DEFAULT 0,
  cst_ibs_cbs varchar(4),
  aliquota_ibs numeric DEFAULT 0,
  valor_ibs numeric DEFAULT 0,
  valor_base_ibs numeric DEFAULT 0,
  aliquota_cbs numeric DEFAULT 0,
  valor_cbs numeric DEFAULT 0,
  valor_base_cbs numeric DEFAULT 0,
  cst_is varchar(4),
  aliquota_is numeric DEFAULT 0,
  valor_is numeric DEFAULT 0,
  valor_base_is numeric DEFAULT 0,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_pedido_venda_itens_updated_at ON public.pedido_venda_itens;
CREATE TRIGGER update_pedido_venda_itens_updated_at BEFORE UPDATE ON public.pedido_venda_itens FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pedido_venda_parcelas (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pedido_venda_id uuid NOT NULL,
  meio_pagamento_id uuid,
  numero_parcela integer NOT NULL,
  data_vencimento date NOT NULL,
  valor numeric NOT NULL,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_pedido_venda_parcelas_updated_at ON public.pedido_venda_parcelas;
CREATE TRIGGER update_pedido_venda_parcelas_updated_at BEFORE UPDATE ON public.pedido_venda_parcelas FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pedidos_compra (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  codigo_pedido_sequence integer NOT NULL DEFAULT nextval('public.pedidos_compra_codigoPedidoSequence_seq'),
  empresa_id uuid NOT NULL,
  fornecedor_id uuid NOT NULL,
  fornecedor_tipo varchar(50) NOT NULL,
  status varchar(50) NOT NULL DEFAULT 'RASCUNHO',
  valor_produtos numeric NOT NULL DEFAULT 0,
  valor_frete numeric DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  valor_total numeric NOT NULL DEFAULT 0,
  xml_chave_acesso varchar(44),
  observacao_interna text,
  data_previsao_entrega date,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp
);
DROP TRIGGER IF EXISTS update_pedidos_compra_updated_at ON public.pedidos_compra;
CREATE TRIGGER update_pedidos_compra_updated_at BEFORE UPDATE ON public.pedidos_compra FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pedido_compra_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pedido_compra_id uuid NOT NULL,
  produto_variacao_id uuid NOT NULL,
  quantidade numeric NOT NULL,
  preco_unitario numeric NOT NULL,
  valor_desconto numeric DEFAULT 0,
  subtotal numeric DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.pedido_compra_parcelas (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pedido_compra_id uuid NOT NULL,
  meio_pagamento_id uuid,
  numero_parcela integer NOT NULL,
  data_vencimento date NOT NULL,
  valor numeric NOT NULL,
  data_inclusao timestamp NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.ordens_servico (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  codigo_os_sequence integer NOT NULL DEFAULT nextval('public.ordens_servico_codigoOsSequence_seq'),
  empresa_id uuid NOT NULL,
  cliente_id uuid NOT NULL,
  cliente_tipo text NOT NULL DEFAULT 'usuario',
  profissional_id uuid,
  orcamento_origem_id uuid,
  status varchar(50) NOT NULL DEFAULT 'ABERTA',
  valor_servicos numeric NOT NULL DEFAULT 0,
  valor_produtos numeric NOT NULL DEFAULT 0,
  valor_desconto numeric DEFAULT 0,
  valor_total numeric NOT NULL DEFAULT 0,
  equipamento_descricao text,
  defeito_relatado text,
  laudo_tecnico text,
  observacao_interna text,
  data_inicio timestamp,
  data_fim timestamp,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp
);
DROP TRIGGER IF EXISTS update_ordens_servico_updated_at ON public.ordens_servico;
CREATE TRIGGER update_ordens_servico_updated_at BEFORE UPDATE ON public.ordens_servico FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.ordem_servico_itens (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  ordem_servico_id uuid NOT NULL,
  produto_variacao_id uuid NOT NULL,
  quantidade numeric NOT NULL,
  preco_unitario numeric NOT NULL,
  valor_desconto numeric DEFAULT 0,
  subtotal numeric DEFAULT 0,
  profissional_executor_id uuid,
  data_inclusao timestamp NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.planos_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  nome varchar(150) NOT NULL,
  descricao text,
  valor numeric NOT NULL,
  intervalo varchar(50) NOT NULL,
  intervalo_contagem integer NOT NULL DEFAULT 1,
  recursos_config jsonb,
  url_imagem text,
  status varchar(50) NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 99,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_planos_app_updated_at ON public.planos_app;
CREATE TRIGGER update_planos_app_updated_at BEFORE UPDATE ON public.planos_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.assinaturas_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  gateway_id varchar(255),
  usuario_id uuid NOT NULL,
  plano_id uuid NOT NULL,
  status varchar(50) NOT NULL,
  data_inicio timestamp NOT NULL,
  data_ciclo_atual_inicio timestamp,
  data_ciclo_atual_fim timestamp,
  data_cancelamento timestamp,
  url_imagem text,
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_assinaturas_app_updated_at ON public.assinaturas_app;
CREATE TRIGGER update_assinaturas_app_updated_at BEFORE UPDATE ON public.assinaturas_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.servicos_disponiveis_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  codigo_servico varchar(100) NOT NULL,
  nome varchar(150) NOT NULL,
  descricao text,
  valor numeric NOT NULL,
  recursos_config jsonb,
  url_imagem text,
  status varchar(50) NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_servicos_disponiveis_app_updated_at ON public.servicos_disponiveis_app;
CREATE TRIGGER update_servicos_disponiveis_app_updated_at BEFORE UPDATE ON public.servicos_disponiveis_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.servico_contratacoes_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  gateway_id varchar(255),
  usuario_id uuid NOT NULL,
  servico_id uuid NOT NULL,
  valor_cobrado numeric NOT NULL,
  metadata jsonb,
  url_imagem text,
  status varchar(50) NOT NULL,
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_servico_contratacoes_app_updated_at ON public.servico_contratacoes_app;
CREATE TRIGGER update_servico_contratacoes_app_updated_at BEFORE UPDATE ON public.servico_contratacoes_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pagamentos_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  gateway_pagamento varchar(50),
  gateway_id varchar(255),
  assinatura_id uuid,
  servico_contratacao_id uuid,
  valor numeric NOT NULL,
  metodo_pagamento varchar(50),
  boleto_url text,
  boleto_linha_digitavel text,
  checkout_url text,
  qr_code_pix text,
  data_vencimento_pix timestamp,
  data_vencimento_boleto timestamp,
  data_vencimento_checkout timestamp,
  data_pagamento timestamp,
  url_imagem text,
  status varchar(50) NOT NULL,
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_pagamentos_app_updated_at ON public.pagamentos_app;
CREATE TRIGGER update_pagamentos_app_updated_at BEFORE UPDATE ON public.pagamentos_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.pagamentos_logs_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  pagamento_id uuid,
  servico_contratacao_id uuid,
  etapa varchar(100) NOT NULL,
  conteudo_json jsonb NOT NULL,
  data_inclusao timestamp NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.configuracoes_taxas_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  metodo_pagamento varchar(50) NOT NULL,
  descricao varchar(255) NOT NULL,
  tipo_parcelamento varchar(50) NOT NULL,
  taxa_percentual numeric NOT NULL DEFAULT 0.00,
  taxa_fixa numeric NOT NULL DEFAULT 0.00,
  numero_maximo_parcelas integer NOT NULL DEFAULT 1,
  valor_minimo_parcela numeric NOT NULL DEFAULT 5.00,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_configuracoes_taxas_app_updated_at ON public.configuracoes_taxas_app;
CREATE TRIGGER update_configuracoes_taxas_app_updated_at BEFORE UPDATE ON public.configuracoes_taxas_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.usuario_recursos_app (
  id uuid PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  origem_tipo varchar(50) NOT NULL,
  origem_id uuid NOT NULL,
  codigo_recurso varchar(100) NOT NULL,
  valor_limite varchar(255) NOT NULL,
  data_expiracao timestamp,
  url_imagem text,
  status text NOT NULL DEFAULT 'ativo',
  ordem integer NOT NULL DEFAULT 0,
  data_inclusao timestamp NOT NULL DEFAULT now(),
  data_ultima_atualizacao timestamp,
  data_exclusao timestamp
);
DROP TRIGGER IF EXISTS update_usuario_recursos_app_updated_at ON public.usuario_recursos_app;
CREATE TRIGGER update_usuario_recursos_app_updated_at BEFORE UPDATE ON public.usuario_recursos_app FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.cashback_regras (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  titulo varchar(100) NOT NULL,
  descricao text,
  percentual numeric NOT NULL,
  dias_validade integer,
  data_inicio_vigencia timestamp,
  data_fim_vigencia timestamp,
  valor_maximo_unidade numeric,
  valor_maximo_total numeric,
  tem_referencia boolean DEFAULT false,
  url_imagem text,
  status varchar(20) DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_cashback_regras_updated_at ON public.cashback_regras;
CREATE TRIGGER update_cashback_regras_updated_at BEFORE UPDATE ON public.cashback_regras FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.cashback_creditos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  usuario_id uuid NOT NULL,
  regra_id uuid,
  valor_inicial numeric NOT NULL,
  valor_utilizado numeric DEFAULT 0,
  descricao text,
  codigo_referencia varchar(100),
  status varchar(20) DEFAULT 'disponivel',
  data_liberacao timestamp DEFAULT now(),
  data_expiracao timestamp,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.cashback_saldos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  usuario_id uuid NOT NULL,
  saldo_disponivel numeric DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_cashback_saldos_updated_at ON public.cashback_saldos;
CREATE TRIGGER update_cashback_saldos_updated_at BEFORE UPDATE ON public.cashback_saldos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.cashback_utilizacoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  credito_id uuid NOT NULL,
  valor_utilizado numeric NOT NULL,
  venda_id uuid,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.cashback_tokens (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  token varchar(255) UNIQUE NOT NULL,
  empresa_id uuid NOT NULL,
  valor_compra numeric,
  tipo varchar(20) NOT NULL,
  status varchar(20) DEFAULT 'disponivel',
  data_expiracao timestamp NOT NULL,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.fidelidade_cartoes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  titulo varchar(100) NOT NULL,
  descricao text,
  pontos_necessarios integer NOT NULL,
  recompensa varchar(255) NOT NULL,
  regra_texto text,
  dias_validade_carimbo integer,
  dias_inatividade_cartao integer,
  data_inicio_vigencia timestamp,
  data_fim_vigencia timestamp,
  tem_referencia boolean DEFAULT false,
  url_imagem text,
  status varchar(20) DEFAULT 'ativo',
  ordem integer DEFAULT 0,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_fidelidade_cartoes_updated_at ON public.fidelidade_cartoes;
CREATE TRIGGER update_fidelidade_cartoes_updated_at BEFORE UPDATE ON public.fidelidade_cartoes FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.fidelidade_saldos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  cartao_id uuid NOT NULL,
  usuario_id uuid NOT NULL,
  pontos_acumulados integer DEFAULT 0,
  recompensas_acumuladas integer DEFAULT 0,
  status varchar(20) DEFAULT 'ativo',
  data_resgate_ultima timestamp,
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_fidelidade_saldos_updated_at ON public.fidelidade_saldos;
CREATE TRIGGER update_fidelidade_saldos_updated_at BEFORE UPDATE ON public.fidelidade_saldos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

CREATE TABLE IF NOT EXISTS public.fidelidade_carimbos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  saldo_id uuid NOT NULL,
  status varchar(20) DEFAULT 'valido',
  codigo_referencia varchar(100),
  data_expiracao timestamp,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.fidelidade_logs (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  saldo_id uuid NOT NULL,
  tipo_evento varchar(50) NOT NULL,
  descricao text,
  codigo_referencia varchar(100),
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.fidelidade_tokens (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  token varchar(255) UNIQUE NOT NULL,
  cartao_id uuid NOT NULL,
  empresa_id uuid NOT NULL,
  status varchar(20) DEFAULT 'disponivel',
  data_expiracao timestamp NOT NULL,
  data_inclusao timestamp DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.cidade_achados_perdidos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  tipo varchar(20) NOT NULL,
  titulo varchar(255) NOT NULL,
  descricao text,
  local_visto varchar(255),
  data_evento date,
  contato_info varchar(255),
  esconder_contato boolean DEFAULT true,
  url_imagem text,
  status varchar(20) DEFAULT 'ativo',
  data_inclusao timestamp DEFAULT now(),
  data_ultima_atualizacao timestamp DEFAULT now()
);
DROP TRIGGER IF EXISTS update_cidade_achados_perdidos_updated_at ON public.cidade_achados_perdidos;
CREATE TRIGGER update_cidade_achados_perdidos_updated_at BEFORE UPDATE ON public.cidade_achados_perdidos FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

ALTER TABLE public.certificados_digitais DROP CONSTRAINT IF EXISTS fk_certificados_digitais_empresa_id;
ALTER TABLE public.certificados_digitais ADD CONSTRAINT fk_certificados_digitais_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.configuracoes_emissoes DROP CONSTRAINT IF EXISTS fk_configuracoes_emissoes_empresa_id;
ALTER TABLE public.configuracoes_emissoes ADD CONSTRAINT fk_configuracoes_emissoes_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.configuracoes_emissoes DROP CONSTRAINT IF EXISTS fk_configuracoes_emissoes_certificado_id;
ALTER TABLE public.configuracoes_emissoes ADD CONSTRAINT fk_configuracoes_emissoes_certificado_id FOREIGN KEY (certificado_id) REFERENCES public.certificados_digitais(id);
ALTER TABLE public.saas_comunicados_opcoes DROP CONSTRAINT IF EXISTS fk_saas_comunicados_opcoes_comunicado_id;
ALTER TABLE public.saas_comunicados_opcoes ADD CONSTRAINT fk_saas_comunicados_opcoes_comunicado_id FOREIGN KEY (comunicado_id) REFERENCES public.saas_comunicados(id);
ALTER TABLE public.saas_comunicados_leituras DROP CONSTRAINT IF EXISTS fk_saas_comunicados_leituras_comunicado_id;
ALTER TABLE public.saas_comunicados_leituras ADD CONSTRAINT fk_saas_comunicados_leituras_comunicado_id FOREIGN KEY (comunicado_id) REFERENCES public.saas_comunicados(id);
ALTER TABLE public.saas_comunicados_leituras DROP CONSTRAINT IF EXISTS fk_saas_comunicados_leituras_conta_id;
ALTER TABLE public.saas_comunicados_leituras ADD CONSTRAINT fk_saas_comunicados_leituras_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.saas_comunicados_leituras DROP CONSTRAINT IF EXISTS fk_saas_comunicados_leituras_usuario_id;
ALTER TABLE public.saas_comunicados_leituras ADD CONSTRAINT fk_saas_comunicados_leituras_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.saas_comunicados_leituras DROP CONSTRAINT IF EXISTS fk_saas_comunicados_leituras_opcao_selecionada_id;
ALTER TABLE public.saas_comunicados_leituras ADD CONSTRAINT fk_saas_comunicados_leituras_opcao_selecionada_id FOREIGN KEY (opcao_selecionada_id) REFERENCES public.saas_comunicados_opcoes(id);
ALTER TABLE public.contas DROP CONSTRAINT IF EXISTS fk_contas_saas_cupom_desconto_id;
ALTER TABLE public.contas ADD CONSTRAINT fk_contas_saas_cupom_desconto_id FOREIGN KEY (saas_cupom_desconto_id) REFERENCES public.saas_cupons_desconto(id);
ALTER TABLE public.empresas DROP CONSTRAINT IF EXISTS fk_empresas_conta_id;
ALTER TABLE public.empresas ADD CONSTRAINT fk_empresas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.empresas DROP CONSTRAINT IF EXISTS fk_empresas_segmento_id;
ALTER TABLE public.empresas ADD CONSTRAINT fk_empresas_segmento_id FOREIGN KEY (segmento_id) REFERENCES public.segmentos(id);
ALTER TABLE public.empresas DROP CONSTRAINT IF EXISTS fk_empresas_atividade_id;
ALTER TABLE public.empresas ADD CONSTRAINT fk_empresas_atividade_id FOREIGN KEY (atividade_id) REFERENCES public.atividades(id);
ALTER TABLE public.usuarios_contas DROP CONSTRAINT IF EXISTS fk_usuarios_contas_usuario_id;
ALTER TABLE public.usuarios_contas ADD CONSTRAINT fk_usuarios_contas_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.usuarios_contas DROP CONSTRAINT IF EXISTS fk_usuarios_contas_conta_id;
ALTER TABLE public.usuarios_contas ADD CONSTRAINT fk_usuarios_contas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.usuarios_empresas DROP CONSTRAINT IF EXISTS fk_usuarios_empresas_usuario_id;
ALTER TABLE public.usuarios_empresas ADD CONSTRAINT fk_usuarios_empresas_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.usuarios_empresas DROP CONSTRAINT IF EXISTS fk_usuarios_empresas_empresa_id;
ALTER TABLE public.usuarios_empresas ADD CONSTRAINT fk_usuarios_empresas_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.usuarios_empresas DROP CONSTRAINT IF EXISTS fk_usuarios_empresas_grupo_id;
ALTER TABLE public.usuarios_empresas ADD CONSTRAINT fk_usuarios_empresas_grupo_id FOREIGN KEY (grupo_id) REFERENCES public.grupos_usuarios(id);
ALTER TABLE public.grupos_usuarios DROP CONSTRAINT IF EXISTS fk_grupos_usuarios_empresa_id;
ALTER TABLE public.grupos_usuarios ADD CONSTRAINT fk_grupos_usuarios_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.grupos_permissoes DROP CONSTRAINT IF EXISTS fk_grupos_permissoes_grupo_id;
ALTER TABLE public.grupos_permissoes ADD CONSTRAINT fk_grupos_permissoes_grupo_id FOREIGN KEY (grupo_id) REFERENCES public.grupos_usuarios(id);
ALTER TABLE public.grupos_permissoes DROP CONSTRAINT IF EXISTS fk_grupos_permissoes_recurso_id;
ALTER TABLE public.grupos_permissoes ADD CONSTRAINT fk_grupos_permissoes_recurso_id FOREIGN KEY (recurso_id) REFERENCES public.recursos_sistema(id);
ALTER TABLE public.tipos_vendedor DROP CONSTRAINT IF EXISTS fk_tipos_vendedor_conta_id;
ALTER TABLE public.tipos_vendedor ADD CONSTRAINT fk_tipos_vendedor_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.vendedores DROP CONSTRAINT IF EXISTS fk_vendedores_conta_id;
ALTER TABLE public.vendedores ADD CONSTRAINT fk_vendedores_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.vendedores DROP CONSTRAINT IF EXISTS fk_vendedores_usuario_id;
ALTER TABLE public.vendedores ADD CONSTRAINT fk_vendedores_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.vendedores DROP CONSTRAINT IF EXISTS fk_vendedores_tipo_vendedor_id;
ALTER TABLE public.vendedores ADD CONSTRAINT fk_vendedores_tipo_vendedor_id FOREIGN KEY (tipo_vendedor_id) REFERENCES public.tipos_vendedor(id);
ALTER TABLE public.contatos_pessoas_fisicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_fisicas_conta_id;
ALTER TABLE public.contatos_pessoas_fisicas ADD CONSTRAINT fk_contatos_pessoas_fisicas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.contatos_pessoas_fisicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_fisicas_usuario_plataforma_id;
ALTER TABLE public.contatos_pessoas_fisicas ADD CONSTRAINT fk_contatos_pessoas_fisicas_usuario_plataforma_id FOREIGN KEY (usuario_plataforma_id) REFERENCES public.usuarios(id);
ALTER TABLE public.contatos_pessoas_fisicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_fisicas_vendedor_1_id;
ALTER TABLE public.contatos_pessoas_fisicas ADD CONSTRAINT fk_contatos_pessoas_fisicas_vendedor_1_id FOREIGN KEY (vendedor_1_id) REFERENCES public.vendedores(id);
ALTER TABLE public.contatos_pessoas_fisicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_fisicas_vendedor_2_id;
ALTER TABLE public.contatos_pessoas_fisicas ADD CONSTRAINT fk_contatos_pessoas_fisicas_vendedor_2_id FOREIGN KEY (vendedor_2_id) REFERENCES public.vendedores(id);
ALTER TABLE public.enderecos_pf DROP CONSTRAINT IF EXISTS fk_enderecos_pf_contato_pessoa_fisica_id;
ALTER TABLE public.enderecos_pf ADD CONSTRAINT fk_enderecos_pf_contato_pessoa_fisica_id FOREIGN KEY (contato_pessoa_fisica_id) REFERENCES public.contatos_pessoas_fisicas(id);
ALTER TABLE public.contatos_pessoas_juridicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_juridicas_conta_id;
ALTER TABLE public.contatos_pessoas_juridicas ADD CONSTRAINT fk_contatos_pessoas_juridicas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.contatos_pessoas_juridicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_juridicas_conta_plataforma_id;
ALTER TABLE public.contatos_pessoas_juridicas ADD CONSTRAINT fk_contatos_pessoas_juridicas_conta_plataforma_id FOREIGN KEY (conta_plataforma_id) REFERENCES public.contas(id);
ALTER TABLE public.contatos_pessoas_juridicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_juridicas_vendedor_1_id;
ALTER TABLE public.contatos_pessoas_juridicas ADD CONSTRAINT fk_contatos_pessoas_juridicas_vendedor_1_id FOREIGN KEY (vendedor_1_id) REFERENCES public.vendedores(id);
ALTER TABLE public.contatos_pessoas_juridicas DROP CONSTRAINT IF EXISTS fk_contatos_pessoas_juridicas_vendedor_2_id;
ALTER TABLE public.contatos_pessoas_juridicas ADD CONSTRAINT fk_contatos_pessoas_juridicas_vendedor_2_id FOREIGN KEY (vendedor_2_id) REFERENCES public.vendedores(id);
ALTER TABLE public.enderecos_pj DROP CONSTRAINT IF EXISTS fk_enderecos_pj_contato_pessoa_juridica_id;
ALTER TABLE public.enderecos_pj ADD CONSTRAINT fk_enderecos_pj_contato_pessoa_juridica_id FOREIGN KEY (contato_pessoa_juridica_id) REFERENCES public.contatos_pessoas_juridicas(id);
ALTER TABLE public.empresas_clientes DROP CONSTRAINT IF EXISTS fk_empresas_clientes_empresa_id;
ALTER TABLE public.empresas_clientes ADD CONSTRAINT fk_empresas_clientes_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_clientes DROP CONSTRAINT IF EXISTS fk_empresas_clientes_forma_pagamento_padrao_id;
ALTER TABLE public.empresas_clientes ADD CONSTRAINT fk_empresas_clientes_forma_pagamento_padrao_id FOREIGN KEY (forma_pagamento_padrao_id) REFERENCES public.formas_pagamento(id);
ALTER TABLE public.empresas_clientes DROP CONSTRAINT IF EXISTS fk_empresas_clientes_condicao_pagamento_padrao_id;
ALTER TABLE public.empresas_clientes ADD CONSTRAINT fk_empresas_clientes_condicao_pagamento_padrao_id FOREIGN KEY (condicao_pagamento_padrao_id) REFERENCES public.condicoes_pagamento(id);
ALTER TABLE public.empresas_clientes DROP CONSTRAINT IF EXISTS fk_empresas_clientes_natureza_operacao_padrao_id;
ALTER TABLE public.empresas_clientes ADD CONSTRAINT fk_empresas_clientes_natureza_operacao_padrao_id FOREIGN KEY (natureza_operacao_padrao_id) REFERENCES public.naturezas_operacao(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_empresa_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_forma_pagamento_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_forma_pagamento_padrao_id FOREIGN KEY (forma_pagamento_padrao_id) REFERENCES public.formas_pagamento(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_condicao_pagamento_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_condicao_pagamento_padrao_id FOREIGN KEY (condicao_pagamento_padrao_id) REFERENCES public.condicoes_pagamento(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_natureza_operacao_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_natureza_operacao_padrao_id FOREIGN KEY (natureza_operacao_padrao_id) REFERENCES public.naturezas_operacao(id);
ALTER TABLE public.empresas_transportadores DROP CONSTRAINT IF EXISTS fk_empresas_transportadores_empresa_id;
ALTER TABLE public.empresas_transportadores ADD CONSTRAINT fk_empresas_transportadores_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_empresa_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_forma_pagamento_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_forma_pagamento_padrao_id FOREIGN KEY (forma_pagamento_padrao_id) REFERENCES public.formas_pagamento(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_condicao_pagamento_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_condicao_pagamento_padrao_id FOREIGN KEY (condicao_pagamento_padrao_id) REFERENCES public.condicoes_pagamento(id);
ALTER TABLE public.empresas_fornecedores DROP CONSTRAINT IF EXISTS fk_empresas_fornecedores_natureza_operacao_padrao_id;
ALTER TABLE public.empresas_fornecedores ADD CONSTRAINT fk_empresas_fornecedores_natureza_operacao_padrao_id FOREIGN KEY (natureza_operacao_padrao_id) REFERENCES public.naturezas_operacao(id);
ALTER TABLE public.solicitacoes_vinculo DROP CONSTRAINT IF EXISTS fk_solicitacoes_vinculo_conta_solicitante_id;
ALTER TABLE public.solicitacoes_vinculo ADD CONSTRAINT fk_solicitacoes_vinculo_conta_solicitante_id FOREIGN KEY (conta_solicitante_id) REFERENCES public.contas(id);
ALTER TABLE public.solicitacoes_vinculo DROP CONSTRAINT IF EXISTS fk_solicitacoes_vinculo_conta_alvo_id;
ALTER TABLE public.solicitacoes_vinculo ADD CONSTRAINT fk_solicitacoes_vinculo_conta_alvo_id FOREIGN KEY (conta_alvo_id) REFERENCES public.contas(id);
ALTER TABLE public.solicitacoes_vinculo DROP CONSTRAINT IF EXISTS fk_solicitacoes_vinculo_usuario_alvo_id;
ALTER TABLE public.solicitacoes_vinculo ADD CONSTRAINT fk_solicitacoes_vinculo_usuario_alvo_id FOREIGN KEY (usuario_alvo_id) REFERENCES public.usuarios(id);
ALTER TABLE public.saas_metodos_pagamento DROP CONSTRAINT IF EXISTS fk_saas_metodos_pagamento_conta_id;
ALTER TABLE public.saas_metodos_pagamento ADD CONSTRAINT fk_saas_metodos_pagamento_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.saas_planos_assinaturas DROP CONSTRAINT IF EXISTS fk_saas_planos_assinaturas_conta_id;
ALTER TABLE public.saas_planos_assinaturas ADD CONSTRAINT fk_saas_planos_assinaturas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.saas_planos_assinaturas DROP CONSTRAINT IF EXISTS fk_saas_planos_assinaturas_saas_plano_id;
ALTER TABLE public.saas_planos_assinaturas ADD CONSTRAINT fk_saas_planos_assinaturas_saas_plano_id FOREIGN KEY (saas_plano_id) REFERENCES public.saas_planos(id);
ALTER TABLE public.saas_planos_assinaturas DROP CONSTRAINT IF EXISTS fk_saas_planos_assinaturas_metodo_pagamento_id;
ALTER TABLE public.saas_planos_assinaturas ADD CONSTRAINT fk_saas_planos_assinaturas_metodo_pagamento_id FOREIGN KEY (metodo_pagamento_id) REFERENCES public.saas_metodos_pagamento(id);
ALTER TABLE public.saas_planos_assinaturas_faturas DROP CONSTRAINT IF EXISTS fk_saas_planos_assinaturas_faturas_conta_id;
ALTER TABLE public.saas_planos_assinaturas_faturas ADD CONSTRAINT fk_saas_planos_assinaturas_faturas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.saas_planos_assinaturas_faturas DROP CONSTRAINT IF EXISTS fk_saas_planos_assinaturas_faturas_assinatura_id;
ALTER TABLE public.saas_planos_assinaturas_faturas ADD CONSTRAINT fk_saas_planos_assinaturas_faturas_assinatura_id FOREIGN KEY (assinatura_id) REFERENCES public.saas_planos_assinaturas(id);
ALTER TABLE public.empresas_funcoes DROP CONSTRAINT IF EXISTS fk_empresas_funcoes_empresa_id;
ALTER TABLE public.empresas_funcoes ADD CONSTRAINT fk_empresas_funcoes_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_profissionais DROP CONSTRAINT IF EXISTS fk_empresas_profissionais_empresa_id;
ALTER TABLE public.empresas_profissionais ADD CONSTRAINT fk_empresas_profissionais_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_profissionais DROP CONSTRAINT IF EXISTS fk_empresas_profissionais_funcao_id;
ALTER TABLE public.empresas_profissionais ADD CONSTRAINT fk_empresas_profissionais_funcao_id FOREIGN KEY (funcao_id) REFERENCES public.empresas_funcoes(id);
ALTER TABLE public.empresas_profissionais_horarios DROP CONSTRAINT IF EXISTS fk_empresas_profissionais_horarios_empresa_id;
ALTER TABLE public.empresas_profissionais_horarios ADD CONSTRAINT fk_empresas_profissionais_horarios_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.empresas_profissionais_horarios DROP CONSTRAINT IF EXISTS fk_empresas_profissionais_horarios_profissional_id;
ALTER TABLE public.empresas_profissionais_horarios ADD CONSTRAINT fk_empresas_profissionais_horarios_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.bloqueios_agenda DROP CONSTRAINT IF EXISTS fk_bloqueios_agenda_empresa_id;
ALTER TABLE public.bloqueios_agenda ADD CONSTRAINT fk_bloqueios_agenda_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.bloqueios_agenda DROP CONSTRAINT IF EXISTS fk_bloqueios_agenda_profissional_id;
ALTER TABLE public.bloqueios_agenda ADD CONSTRAINT fk_bloqueios_agenda_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.agendamentos DROP CONSTRAINT IF EXISTS fk_agendamentos_empresa_id;
ALTER TABLE public.agendamentos ADD CONSTRAINT fk_agendamentos_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.agendamentos DROP CONSTRAINT IF EXISTS fk_agendamentos_profissional_id;
ALTER TABLE public.agendamentos ADD CONSTRAINT fk_agendamentos_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.agendamentos DROP CONSTRAINT IF EXISTS fk_agendamentos_forma_pagamento_id;
ALTER TABLE public.agendamentos ADD CONSTRAINT fk_agendamentos_forma_pagamento_id FOREIGN KEY (forma_pagamento_id) REFERENCES public.formas_pagamento(id);
ALTER TABLE public.agendamento_servicos DROP CONSTRAINT IF EXISTS fk_agendamento_servicos_agendamento_id;
ALTER TABLE public.agendamento_servicos ADD CONSTRAINT fk_agendamento_servicos_agendamento_id FOREIGN KEY (agendamento_id) REFERENCES public.agendamentos(id);
ALTER TABLE public.agendamento_servicos DROP CONSTRAINT IF EXISTS fk_agendamento_servicos_produto_variacao_id;
ALTER TABLE public.agendamento_servicos ADD CONSTRAINT fk_agendamento_servicos_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.agendamento_servicos DROP CONSTRAINT IF EXISTS fk_agendamento_servicos_profissional_id;
ALTER TABLE public.agendamento_servicos ADD CONSTRAINT fk_agendamento_servicos_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.servico_desconto DROP CONSTRAINT IF EXISTS fk_servico_desconto_empresa_id;
ALTER TABLE public.servico_desconto ADD CONSTRAINT fk_servico_desconto_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.servico_desconto DROP CONSTRAINT IF EXISTS fk_servico_desconto_agendamento_servicos_id;
ALTER TABLE public.servico_desconto ADD CONSTRAINT fk_servico_desconto_agendamento_servicos_id FOREIGN KEY (agendamento_servicos_id) REFERENCES public.agendamento_servicos(id);
ALTER TABLE public.agendamento_config DROP CONSTRAINT IF EXISTS fk_agendamento_config_empresa_id;
ALTER TABLE public.agendamento_config ADD CONSTRAINT fk_agendamento_config_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.documentos_aceites DROP CONSTRAINT IF EXISTS fk_documentos_aceites_usuario_id;
ALTER TABLE public.documentos_aceites ADD CONSTRAINT fk_documentos_aceites_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.documentos_aceites DROP CONSTRAINT IF EXISTS fk_documentos_aceites_documento_legal_id;
ALTER TABLE public.documentos_aceites ADD CONSTRAINT fk_documentos_aceites_documento_legal_id FOREIGN KEY (documento_legal_id) REFERENCES public.documentos_legais(id);
ALTER TABLE public.produto_categorias DROP CONSTRAINT IF EXISTS fk_produto_categorias_conta_id;
ALTER TABLE public.produto_categorias ADD CONSTRAINT fk_produto_categorias_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.marcas DROP CONSTRAINT IF EXISTS fk_marcas_conta_id;
ALTER TABLE public.marcas ADD CONSTRAINT fk_marcas_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.produtos DROP CONSTRAINT IF EXISTS fk_produtos_conta_id;
ALTER TABLE public.produtos ADD CONSTRAINT fk_produtos_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.produtos DROP CONSTRAINT IF EXISTS fk_produtos_categoria_id;
ALTER TABLE public.produtos ADD CONSTRAINT fk_produtos_categoria_id FOREIGN KEY (categoria_id) REFERENCES public.produto_categorias(id);
ALTER TABLE public.produtos DROP CONSTRAINT IF EXISTS fk_produtos_marca_id;
ALTER TABLE public.produtos ADD CONSTRAINT fk_produtos_marca_id FOREIGN KEY (marca_id) REFERENCES public.marcas(id);
ALTER TABLE public.produto_variacoes DROP CONSTRAINT IF EXISTS fk_produto_variacoes_produto_id;
ALTER TABLE public.produto_variacoes ADD CONSTRAINT fk_produto_variacoes_produto_id FOREIGN KEY (produto_id) REFERENCES public.produtos(id);
ALTER TABLE public.contextos_preco DROP CONSTRAINT IF EXISTS fk_contextos_preco_conta_id;
ALTER TABLE public.contextos_preco ADD CONSTRAINT fk_contextos_preco_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.precos DROP CONSTRAINT IF EXISTS fk_precos_produto_variacao_id;
ALTER TABLE public.precos ADD CONSTRAINT fk_precos_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.precos DROP CONSTRAINT IF EXISTS fk_precos_contexto_preco_id;
ALTER TABLE public.precos ADD CONSTRAINT fk_precos_contexto_preco_id FOREIGN KEY (contexto_preco_id) REFERENCES public.contextos_preco(id);
ALTER TABLE public.areas_estoque DROP CONSTRAINT IF EXISTS fk_areas_estoque_empresa_id;
ALTER TABLE public.areas_estoque ADD CONSTRAINT fk_areas_estoque_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.locais_estoque DROP CONSTRAINT IF EXISTS fk_locais_estoque_area_estoque_id;
ALTER TABLE public.locais_estoque ADD CONSTRAINT fk_locais_estoque_area_estoque_id FOREIGN KEY (area_estoque_id) REFERENCES public.areas_estoque(id);
ALTER TABLE public.locais_estoque DROP CONSTRAINT IF EXISTS fk_locais_estoque_empresa_id;
ALTER TABLE public.locais_estoque ADD CONSTRAINT fk_locais_estoque_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.estoque_saldo DROP CONSTRAINT IF EXISTS fk_estoque_saldo_produto_variacao_id;
ALTER TABLE public.estoque_saldo ADD CONSTRAINT fk_estoque_saldo_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.estoque_saldo DROP CONSTRAINT IF EXISTS fk_estoque_saldo_local_estoque_id;
ALTER TABLE public.estoque_saldo ADD CONSTRAINT fk_estoque_saldo_local_estoque_id FOREIGN KEY (local_estoque_id) REFERENCES public.locais_estoque(id);
ALTER TABLE public.estoque_movimentacoes DROP CONSTRAINT IF EXISTS fk_estoque_movimentacoes_estoque_saldo_id;
ALTER TABLE public.estoque_movimentacoes ADD CONSTRAINT fk_estoque_movimentacoes_estoque_saldo_id FOREIGN KEY (estoque_saldo_id) REFERENCES public.estoque_saldo(id);
ALTER TABLE public.estoque_movimentacoes DROP CONSTRAINT IF EXISTS fk_estoque_movimentacoes_usuario_responsavel_id;
ALTER TABLE public.estoque_movimentacoes ADD CONSTRAINT fk_estoque_movimentacoes_usuario_responsavel_id FOREIGN KEY (usuario_responsavel_id) REFERENCES public.usuarios(id);
ALTER TABLE public.formas_pagamento DROP CONSTRAINT IF EXISTS fk_formas_pagamento_conta_id;
ALTER TABLE public.formas_pagamento ADD CONSTRAINT fk_formas_pagamento_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.controle_caixa DROP CONSTRAINT IF EXISTS fk_controle_caixa_empresa_id;
ALTER TABLE public.controle_caixa ADD CONSTRAINT fk_controle_caixa_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.controle_caixa DROP CONSTRAINT IF EXISTS fk_controle_caixa_usuario_id;
ALTER TABLE public.controle_caixa ADD CONSTRAINT fk_controle_caixa_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.vendas DROP CONSTRAINT IF EXISTS fk_vendas_empresa_id;
ALTER TABLE public.vendas ADD CONSTRAINT fk_vendas_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.vendas DROP CONSTRAINT IF EXISTS fk_vendas_caixa_id;
ALTER TABLE public.vendas ADD CONSTRAINT fk_vendas_caixa_id FOREIGN KEY (caixa_id) REFERENCES public.controle_caixa(id);
ALTER TABLE public.vendas DROP CONSTRAINT IF EXISTS fk_vendas_agendamento_id;
ALTER TABLE public.vendas ADD CONSTRAINT fk_vendas_agendamento_id FOREIGN KEY (agendamento_id) REFERENCES public.agendamentos(id);
ALTER TABLE public.venda_itens DROP CONSTRAINT IF EXISTS fk_venda_itens_venda_id;
ALTER TABLE public.venda_itens ADD CONSTRAINT fk_venda_itens_venda_id FOREIGN KEY (venda_id) REFERENCES public.vendas(id);
ALTER TABLE public.venda_itens DROP CONSTRAINT IF EXISTS fk_venda_itens_produto_variacao_id;
ALTER TABLE public.venda_itens ADD CONSTRAINT fk_venda_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.venda_itens DROP CONSTRAINT IF EXISTS fk_venda_itens_profissional_id;
ALTER TABLE public.venda_itens ADD CONSTRAINT fk_venda_itens_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.contas_receber DROP CONSTRAINT IF EXISTS fk_contas_receber_empresa_id;
ALTER TABLE public.contas_receber ADD CONSTRAINT fk_contas_receber_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_receber DROP CONSTRAINT IF EXISTS fk_contas_receber_forma_pagamento_id;
ALTER TABLE public.contas_receber ADD CONSTRAINT fk_contas_receber_forma_pagamento_id FOREIGN KEY (forma_pagamento_id) REFERENCES public.formas_pagamento(id);
ALTER TABLE public.contas_pagar DROP CONSTRAINT IF EXISTS fk_contas_pagar_empresa_id;
ALTER TABLE public.contas_pagar ADD CONSTRAINT fk_contas_pagar_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_bancarias DROP CONSTRAINT IF EXISTS fk_contas_bancarias_empresa_id;
ALTER TABLE public.contas_bancarias ADD CONSTRAINT fk_contas_bancarias_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_a_receber_comissao DROP CONSTRAINT IF EXISTS fk_contas_a_receber_comissao_conta_a_receber_id;
ALTER TABLE public.contas_a_receber_comissao ADD CONSTRAINT fk_contas_a_receber_comissao_conta_a_receber_id FOREIGN KEY (conta_a_receber_id) REFERENCES public.contas_a_receber(id);
ALTER TABLE public.contas_a_receber_comissao DROP CONSTRAINT IF EXISTS fk_contas_a_receber_comissao_movimentacao_pagamento_id;
ALTER TABLE public.contas_a_receber_comissao ADD CONSTRAINT fk_contas_a_receber_comissao_movimentacao_pagamento_id FOREIGN KEY (movimentacao_pagamento_id) REFERENCES public.contas_a_receber_movimentacao(id);
ALTER TABLE public.comissao_deducao_regra DROP CONSTRAINT IF EXISTS fk_comissao_deducao_regra_conta_id;
ALTER TABLE public.comissao_deducao_regra ADD CONSTRAINT fk_comissao_deducao_regra_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.contas_a_receber_comissao_deducao DROP CONSTRAINT IF EXISTS fk_contas_a_receber_comissao_deducao_comissao_id;
ALTER TABLE public.contas_a_receber_comissao_deducao ADD CONSTRAINT fk_contas_a_receber_comissao_deducao_comissao_id FOREIGN KEY (comissao_id) REFERENCES public.contas_a_receber_comissao(id);
ALTER TABLE public.conta_a_pagar_credito DROP CONSTRAINT IF EXISTS fk_conta_a_pagar_credito_empresa_id;
ALTER TABLE public.conta_a_pagar_credito ADD CONSTRAINT fk_conta_a_pagar_credito_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.conta_a_receber_credito DROP CONSTRAINT IF EXISTS fk_conta_a_receber_credito_empresa_id;
ALTER TABLE public.conta_a_receber_credito ADD CONSTRAINT fk_conta_a_receber_credito_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_a_pagar DROP CONSTRAINT IF EXISTS fk_contas_a_pagar_empresa_id;
ALTER TABLE public.contas_a_pagar ADD CONSTRAINT fk_contas_a_pagar_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_a_pagar_movimentacao DROP CONSTRAINT IF EXISTS fk_contas_a_pagar_movimentacao_conta_a_pagar_id;
ALTER TABLE public.contas_a_pagar_movimentacao ADD CONSTRAINT fk_contas_a_pagar_movimentacao_conta_a_pagar_id FOREIGN KEY (conta_a_pagar_id) REFERENCES public.contas_a_pagar(id);
ALTER TABLE public.contas_a_receber DROP CONSTRAINT IF EXISTS fk_contas_a_receber_empresa_id;
ALTER TABLE public.contas_a_receber ADD CONSTRAINT fk_contas_a_receber_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.contas_a_receber_movimentacao DROP CONSTRAINT IF EXISTS fk_contas_a_receber_movimentacao_conta_a_receber_id;
ALTER TABLE public.contas_a_receber_movimentacao ADD CONSTRAINT fk_contas_a_receber_movimentacao_conta_a_receber_id FOREIGN KEY (conta_a_receber_id) REFERENCES public.contas_a_receber(id);
ALTER TABLE public.notas_fiscais DROP CONSTRAINT IF EXISTS fk_notas_fiscais_empresa_id;
ALTER TABLE public.notas_fiscais ADD CONSTRAINT fk_notas_fiscais_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.notas_fiscais DROP CONSTRAINT IF EXISTS fk_notas_fiscais_embarque_id;
ALTER TABLE public.notas_fiscais ADD CONSTRAINT fk_notas_fiscais_embarque_id FOREIGN KEY (embarque_id) REFERENCES public.embarques(id);
ALTER TABLE public.nota_fiscal_itens DROP CONSTRAINT IF EXISTS fk_nota_fiscal_itens_nota_fiscal_id;
ALTER TABLE public.nota_fiscal_itens ADD CONSTRAINT fk_nota_fiscal_itens_nota_fiscal_id FOREIGN KEY (nota_fiscal_id) REFERENCES public.notas_fiscais(id);
ALTER TABLE public.nota_fiscal_itens DROP CONSTRAINT IF EXISTS fk_nota_fiscal_itens_produto_variacao_id;
ALTER TABLE public.nota_fiscal_itens ADD CONSTRAINT fk_nota_fiscal_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.nota_fiscal_itens DROP CONSTRAINT IF EXISTS fk_nota_fiscal_itens_natureza_operacao_id;
ALTER TABLE public.nota_fiscal_itens ADD CONSTRAINT fk_nota_fiscal_itens_natureza_operacao_id FOREIGN KEY (natureza_operacao_id) REFERENCES public.naturezas_operacao(id);
ALTER TABLE public.embarque_itens DROP CONSTRAINT IF EXISTS fk_embarque_itens_embarque_id;
ALTER TABLE public.embarque_itens ADD CONSTRAINT fk_embarque_itens_embarque_id FOREIGN KEY (embarque_id) REFERENCES public.embarques(id);
ALTER TABLE public.embarque_itens DROP CONSTRAINT IF EXISTS fk_embarque_itens_estoque_saldo_id;
ALTER TABLE public.embarque_itens ADD CONSTRAINT fk_embarque_itens_estoque_saldo_id FOREIGN KEY (estoque_saldo_id) REFERENCES public.estoque_saldo(id);
ALTER TABLE public.naturezas_operacao DROP CONSTRAINT IF EXISTS fk_naturezas_operacao_conta_id;
ALTER TABLE public.naturezas_operacao ADD CONSTRAINT fk_naturezas_operacao_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.regras_fiscais DROP CONSTRAINT IF EXISTS fk_regras_fiscais_natureza_operacao_id;
ALTER TABLE public.regras_fiscais ADD CONSTRAINT fk_regras_fiscais_natureza_operacao_id FOREIGN KEY (natureza_operacao_id) REFERENCES public.naturezas_operacao(id);
ALTER TABLE public.regras_fiscais DROP CONSTRAINT IF EXISTS fk_regras_fiscais_empresa_id;
ALTER TABLE public.regras_fiscais ADD CONSTRAINT fk_regras_fiscais_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.formas_pagamento_fiscal DROP CONSTRAINT IF EXISTS fk_formas_pagamento_fiscal_conta_id;
ALTER TABLE public.formas_pagamento_fiscal ADD CONSTRAINT fk_formas_pagamento_fiscal_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.condicoes_pagamento DROP CONSTRAINT IF EXISTS fk_condicoes_pagamento_conta_id;
ALTER TABLE public.condicoes_pagamento ADD CONSTRAINT fk_condicoes_pagamento_conta_id FOREIGN KEY (conta_id) REFERENCES public.contas(id);
ALTER TABLE public.condicao_pagamento_parcelas DROP CONSTRAINT IF EXISTS fk_condicao_pagamento_parcelas_condicao_pagamento_id;
ALTER TABLE public.condicao_pagamento_parcelas ADD CONSTRAINT fk_condicao_pagamento_parcelas_condicao_pagamento_id FOREIGN KEY (condicao_pagamento_id) REFERENCES public.condicoes_pagamento(id);
ALTER TABLE public.orcamentos DROP CONSTRAINT IF EXISTS fk_orcamentos_empresa_id;
ALTER TABLE public.orcamentos ADD CONSTRAINT fk_orcamentos_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.orcamentos DROP CONSTRAINT IF EXISTS fk_orcamentos_profissional_id;
ALTER TABLE public.orcamentos ADD CONSTRAINT fk_orcamentos_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.orcamento_itens DROP CONSTRAINT IF EXISTS fk_orcamento_itens_orcamento_id;
ALTER TABLE public.orcamento_itens ADD CONSTRAINT fk_orcamento_itens_orcamento_id FOREIGN KEY (orcamento_id) REFERENCES public.orcamentos(id);
ALTER TABLE public.orcamento_itens DROP CONSTRAINT IF EXISTS fk_orcamento_itens_produto_variacao_id;
ALTER TABLE public.orcamento_itens ADD CONSTRAINT fk_orcamento_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.pedidos_venda DROP CONSTRAINT IF EXISTS fk_pedidos_venda_empresa_id;
ALTER TABLE public.pedidos_venda ADD CONSTRAINT fk_pedidos_venda_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.pedidos_venda DROP CONSTRAINT IF EXISTS fk_pedidos_venda_profissional_id;
ALTER TABLE public.pedidos_venda ADD CONSTRAINT fk_pedidos_venda_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.pedido_venda_itens DROP CONSTRAINT IF EXISTS fk_pedido_venda_itens_pedido_venda_id;
ALTER TABLE public.pedido_venda_itens ADD CONSTRAINT fk_pedido_venda_itens_pedido_venda_id FOREIGN KEY (pedido_venda_id) REFERENCES public.pedidos_venda(id);
ALTER TABLE public.pedido_venda_itens DROP CONSTRAINT IF EXISTS fk_pedido_venda_itens_produto_variacao_id;
ALTER TABLE public.pedido_venda_itens ADD CONSTRAINT fk_pedido_venda_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.pedido_venda_parcelas DROP CONSTRAINT IF EXISTS fk_pedido_venda_parcelas_pedido_venda_id;
ALTER TABLE public.pedido_venda_parcelas ADD CONSTRAINT fk_pedido_venda_parcelas_pedido_venda_id FOREIGN KEY (pedido_venda_id) REFERENCES public.pedidos_venda(id);
ALTER TABLE public.pedidos_compra DROP CONSTRAINT IF EXISTS fk_pedidos_compra_empresa_id;
ALTER TABLE public.pedidos_compra ADD CONSTRAINT fk_pedidos_compra_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.pedido_compra_itens DROP CONSTRAINT IF EXISTS fk_pedido_compra_itens_pedido_compra_id;
ALTER TABLE public.pedido_compra_itens ADD CONSTRAINT fk_pedido_compra_itens_pedido_compra_id FOREIGN KEY (pedido_compra_id) REFERENCES public.pedidos_compra(id);
ALTER TABLE public.pedido_compra_itens DROP CONSTRAINT IF EXISTS fk_pedido_compra_itens_produto_variacao_id;
ALTER TABLE public.pedido_compra_itens ADD CONSTRAINT fk_pedido_compra_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.pedido_compra_parcelas DROP CONSTRAINT IF EXISTS fk_pedido_compra_parcelas_pedido_compra_id;
ALTER TABLE public.pedido_compra_parcelas ADD CONSTRAINT fk_pedido_compra_parcelas_pedido_compra_id FOREIGN KEY (pedido_compra_id) REFERENCES public.pedidos_compra(id);
ALTER TABLE public.ordens_servico DROP CONSTRAINT IF EXISTS fk_ordens_servico_empresa_id;
ALTER TABLE public.ordens_servico ADD CONSTRAINT fk_ordens_servico_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.ordens_servico DROP CONSTRAINT IF EXISTS fk_ordens_servico_profissional_id;
ALTER TABLE public.ordens_servico ADD CONSTRAINT fk_ordens_servico_profissional_id FOREIGN KEY (profissional_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.ordem_servico_itens DROP CONSTRAINT IF EXISTS fk_ordem_servico_itens_ordem_servico_id;
ALTER TABLE public.ordem_servico_itens ADD CONSTRAINT fk_ordem_servico_itens_ordem_servico_id FOREIGN KEY (ordem_servico_id) REFERENCES public.ordens_servico(id);
ALTER TABLE public.ordem_servico_itens DROP CONSTRAINT IF EXISTS fk_ordem_servico_itens_produto_variacao_id;
ALTER TABLE public.ordem_servico_itens ADD CONSTRAINT fk_ordem_servico_itens_produto_variacao_id FOREIGN KEY (produto_variacao_id) REFERENCES public.produto_variacoes(id);
ALTER TABLE public.ordem_servico_itens DROP CONSTRAINT IF EXISTS fk_ordem_servico_itens_profissional_executor_id;
ALTER TABLE public.ordem_servico_itens ADD CONSTRAINT fk_ordem_servico_itens_profissional_executor_id FOREIGN KEY (profissional_executor_id) REFERENCES public.empresas_profissionais(id);
ALTER TABLE public.cashback_regras DROP CONSTRAINT IF EXISTS fk_cashback_regras_empresa_id;
ALTER TABLE public.cashback_regras ADD CONSTRAINT fk_cashback_regras_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.cashback_creditos DROP CONSTRAINT IF EXISTS fk_cashback_creditos_empresa_id;
ALTER TABLE public.cashback_creditos ADD CONSTRAINT fk_cashback_creditos_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.cashback_creditos DROP CONSTRAINT IF EXISTS fk_cashback_creditos_usuario_id;
ALTER TABLE public.cashback_creditos ADD CONSTRAINT fk_cashback_creditos_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.cashback_creditos DROP CONSTRAINT IF EXISTS fk_cashback_creditos_regra_id;
ALTER TABLE public.cashback_creditos ADD CONSTRAINT fk_cashback_creditos_regra_id FOREIGN KEY (regra_id) REFERENCES public.cashback_regras(id);
ALTER TABLE public.cashback_saldos DROP CONSTRAINT IF EXISTS fk_cashback_saldos_empresa_id;
ALTER TABLE public.cashback_saldos ADD CONSTRAINT fk_cashback_saldos_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.cashback_saldos DROP CONSTRAINT IF EXISTS fk_cashback_saldos_usuario_id;
ALTER TABLE public.cashback_saldos ADD CONSTRAINT fk_cashback_saldos_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.cashback_utilizacoes DROP CONSTRAINT IF EXISTS fk_cashback_utilizacoes_credito_id;
ALTER TABLE public.cashback_utilizacoes ADD CONSTRAINT fk_cashback_utilizacoes_credito_id FOREIGN KEY (credito_id) REFERENCES public.cashback_creditos(id);
ALTER TABLE public.cashback_tokens DROP CONSTRAINT IF EXISTS fk_cashback_tokens_empresa_id;
ALTER TABLE public.cashback_tokens ADD CONSTRAINT fk_cashback_tokens_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.fidelidade_cartoes DROP CONSTRAINT IF EXISTS fk_fidelidade_cartoes_empresa_id;
ALTER TABLE public.fidelidade_cartoes ADD CONSTRAINT fk_fidelidade_cartoes_empresa_id FOREIGN KEY (empresa_id) REFERENCES public.empresas(id);
ALTER TABLE public.fidelidade_saldos DROP CONSTRAINT IF EXISTS fk_fidelidade_saldos_cartao_id;
ALTER TABLE public.fidelidade_saldos ADD CONSTRAINT fk_fidelidade_saldos_cartao_id FOREIGN KEY (cartao_id) REFERENCES public.fidelidade_cartoes(id);
ALTER TABLE public.fidelidade_saldos DROP CONSTRAINT IF EXISTS fk_fidelidade_saldos_usuario_id;
ALTER TABLE public.fidelidade_saldos ADD CONSTRAINT fk_fidelidade_saldos_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);
ALTER TABLE public.fidelidade_carimbos DROP CONSTRAINT IF EXISTS fk_fidelidade_carimbos_saldo_id;
ALTER TABLE public.fidelidade_carimbos ADD CONSTRAINT fk_fidelidade_carimbos_saldo_id FOREIGN KEY (saldo_id) REFERENCES public.fidelidade_saldos(id);
ALTER TABLE public.fidelidade_logs DROP CONSTRAINT IF EXISTS fk_fidelidade_logs_saldo_id;
ALTER TABLE public.fidelidade_logs ADD CONSTRAINT fk_fidelidade_logs_saldo_id FOREIGN KEY (saldo_id) REFERENCES public.fidelidade_saldos(id);
ALTER TABLE public.fidelidade_tokens DROP CONSTRAINT IF EXISTS fk_fidelidade_tokens_cartao_id;
ALTER TABLE public.fidelidade_tokens ADD CONSTRAINT fk_fidelidade_tokens_cartao_id FOREIGN KEY (cartao_id) REFERENCES public.fidelidade_cartoes(id);
ALTER TABLE public.cidade_achados_perdidos DROP CONSTRAINT IF EXISTS fk_cidade_achados_perdidos_usuario_id;
ALTER TABLE public.cidade_achados_perdidos ADD CONSTRAINT fk_cidade_achados_perdidos_usuario_id FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);

-- Modificação: Novo papel de funcionário no cadastro de contatos
ALTER TABLE contatos_pessoas_fisicas ADD COLUMN IF NOT EXISTS is_funcionario boolean DEFAULT false;
ALTER TABLE contatos_pessoas_juridicas ADD COLUMN IF NOT EXISTS is_funcionario boolean DEFAULT false;

CREATE TABLE IF NOT EXISTS public.empresas_funcionarios (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  empresa_id uuid NOT NULL,
  contato_id uuid NOT NULL,
  contato_tipo varchar(50) NOT NULL,
  observacao text,
  status boolean DEFAULT true,
  data_inclusao timestamp DEFAULT now(),
  operador_inclusao uuid,
  data_ultima_atualizacao timestamp DEFAULT now()
);

-- Configurações Fiscais: garante unicidade de configuração por empresa (necessário para upsert via ON CONFLICT)
ALTER TABLE public.configuracoes_emissoes
  DROP CONSTRAINT IF EXISTS uq_configuracoes_emissoes_empresa_id;
ALTER TABLE public.configuracoes_emissoes
  ADD CONSTRAINT uq_configuracoes_emissoes_empresa_id UNIQUE (empresa_id);


ALTER TABLE public.contatos_pessoas_fisicas ADD COLUMN IF NOT EXISTS envia_email_agendamento BOOLEAN NOT NULL DEFAULT TRUE;
ALTER TABLE public.contatos_pessoas_juridicas ADD COLUMN IF NOT EXISTS envia_email_agendamento BOOLEAN NOT NULL DEFAULT TRUE;
-- Script correto para a criação das tabelas do Módulo de Veículos

CREATE TABLE veiculos (
    id UUID PRIMARY KEY,
    conta_id UUID NOT NULL,
    usuario_id UUID NOT NULL,
    marca VARCHAR(50) NOT NULL,
    modelo VARCHAR(50) NOT NULL,
    versao VARCHAR(100) NULL,
    ano_fabricacao INT NOT NULL,
    ano_modelo INT NOT NULL,
    quilometragem INT NOT NULL,
    combustivel INT NOT NULL,
    cambio INT NOT NULL,
    cor VARCHAR(50) NULL,
    placa VARCHAR(10) NULL,
    tipo_veiculo INT NOT NULL,
    numero_portas INT NULL,
    data_inclusao TIMESTAMP NOT NULL DEFAULT NOW(),
    data_ultima_atualizacao TIMESTAMP NULL
);

CREATE TABLE veiculos_anuncios (
    id UUID PRIMARY KEY,
    veiculo_id UUID NOT NULL REFERENCES veiculos(id) ON DELETE CASCADE,
    conta_id UUID NOT NULL,
    usuario_id UUID NOT NULL,
    titulo VARCHAR(150) NOT NULL,
    descricao TEXT NULL,
    preco DECIMAL(18,2) NOT NULL,
    aceita_troca BOOLEAN NOT NULL DEFAULT FALSE,
    contato_whatsapp VARCHAR(20) NULL,
    status TEXT NOT NULL DEFAULT 'ativo',
    eh_destaque BOOLEAN NOT NULL DEFAULT FALSE,
    codigo_publico VARCHAR(50) NOT NULL,
    data_expiracao TIMESTAMP NULL,
    data_inclusao TIMESTAMP NOT NULL DEFAULT NOW(),
    data_ultima_atualizacao TIMESTAMP NULL
);

CREATE TABLE veiculos_fotos (
    id UUID PRIMARY KEY,
    anuncio_veiculo_id UUID NOT NULL REFERENCES veiculos_anuncios(id) ON DELETE CASCADE,
    url VARCHAR(255) NOT NULL,
    ordem INT NOT NULL DEFAULT 0,
    data_inclusao TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE veiculos_perfis_vendedor (
    id UUID PRIMARY KEY,
    conta_id UUID NOT NULL UNIQUE,
    nome_exibicao VARCHAR(100) NOT NULL,
    descricao TEXT NULL,
    foto_logo_url VARCHAR(255) NULL,
    whatsapp VARCHAR(20) NULL,
    url_site VARCHAR(255) NULL,
    instagram VARCHAR(50) NULL,
    data_inclusao TIMESTAMP NOT NULL DEFAULT NOW(),
    data_atualizacao TIMESTAMP NULL
);

CREATE TABLE veiculos_favoritos (
    usuario_id UUID NOT NULL,
    anuncio_veiculo_id UUID NOT NULL REFERENCES veiculos_anuncios(id) ON DELETE CASCADE,
    data_criacao TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (usuario_id, anuncio_veiculo_id)
);

CREATE TABLE veiculos_buscas_recentes (
    id UUID PRIMARY KEY,
    usuario_id UUID NOT NULL,
    chave_filtro VARCHAR(50) NULL,
    filtro_json TEXT NULL,
    rotulo VARCHAR(100) NULL,
    data_inclusao TIMESTAMP NOT NULL DEFAULT NOW(),
    data_ultima_busca TIMESTAMP NOT NULL DEFAULT NOW()
);

