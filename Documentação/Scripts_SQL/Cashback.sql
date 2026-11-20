-- MÓDULO: FIDELIDADE E CASHBACK

CREATE TABLE public.cashback_regras (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    empresa_id UUID NOT NULL,
    titulo VARCHAR(100) NOT NULL,
    descricao TEXT,
    percentual NUMERIC NOT NULL,
    dias_validade INTEGER,
    data_inicio_vigencia TIMESTAMP,
    data_fim_vigencia TIMESTAMP,
    valor_maximo_unidade NUMERIC,
    valor_maximo_total NUMERIC,
    tem_referencia BOOLEAN DEFAULT FALSE,
    url_imagem TEXT,
    status VARCHAR(20) DEFAULT 'ativo',
    ordem INTEGER DEFAULT 0,
    data_inclusao TIMESTAMP DEFAULT now(),
    operador_inclusao UUID,
    data_ultima_atualizacao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_cashback_regras_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id)
);

CREATE TABLE public.cashback_creditos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    empresa_id UUID NOT NULL,
    usuario_id UUID NOT NULL,
    regra_id UUID,
    valor_inicial NUMERIC NOT NULL,
    valor_utilizado NUMERIC DEFAULT 0,
    descricao TEXT,
    codigo_referencia VARCHAR(100),
    status VARCHAR(20) DEFAULT 'disponivel',
    data_liberacao TIMESTAMP DEFAULT now(),
    data_expiracao TIMESTAMP,
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_cashback_creditos_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id),
    CONSTRAINT fk_cashback_creditos_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id),
    CONSTRAINT fk_cashback_creditos_regra FOREIGN KEY (regra_id) REFERENCES public.cashback_regras(id)
);

CREATE TABLE public.cashback_saldos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    empresa_id UUID NOT NULL,
    usuario_id UUID NOT NULL,
    saldo_disponivel NUMERIC DEFAULT 0,
    data_inclusao TIMESTAMP DEFAULT now(),
    data_ultima_atualizacao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_cashback_saldos_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id),
    CONSTRAINT fk_cashback_saldos_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id),
    CONSTRAINT uk_cashback_saldos_empresa_usuario UNIQUE (empresa_id, usuario_id)
);

CREATE TABLE public.cashback_utilizacoes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    credito_id UUID NOT NULL,
    valor_utilizado NUMERIC NOT NULL,
    venda_id UUID,
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_cashback_utilizacoes_credito FOREIGN KEY (credito_id) REFERENCES public.cashback_creditos(id)
);

CREATE TABLE public.cashback_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    token VARCHAR(255) UNIQUE NOT NULL,
    empresa_id UUID NOT NULL,
    valor_compra NUMERIC,
    tipo VARCHAR(20) NOT NULL, -- 'credito', 'debito'
    status VARCHAR(20) DEFAULT 'disponivel', -- 'disponivel', 'usado', 'expirado'
    data_expiracao TIMESTAMP NOT NULL,
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_cashback_tokens_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id)
);
