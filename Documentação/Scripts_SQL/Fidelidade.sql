-- MÓDULO: FIDELIDADE E CARTÃO FIDELIDADE

CREATE TABLE public.fidelidade_cartoes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    empresa_id UUID NOT NULL,
    titulo VARCHAR(100) NOT NULL,
    descricao TEXT,
    pontos_necessarios INTEGER NOT NULL,
    recompensa VARCHAR(255) NOT NULL,
    regra_texto TEXT,
    dias_validade_carimbo INTEGER,
    dias_inatividade_cartao INTEGER,
    data_inicio_vigencia TIMESTAMP,
    data_fim_vigencia TIMESTAMP,
    tem_referencia BOOLEAN DEFAULT FALSE,
    url_imagem TEXT,
    status VARCHAR(20) DEFAULT 'ativo',
    ordem INTEGER DEFAULT 0,
    data_inclusao TIMESTAMP DEFAULT now(),
    operador_inclusao UUID,
    data_ultima_atualizacao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_fidelidade_cartoes_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id)
);

CREATE TABLE public.fidelidade_saldos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cartao_id UUID NOT NULL,
    usuario_id UUID NOT NULL,
    pontos_acumulados INTEGER DEFAULT 0,
    recompensas_acumuladas INTEGER DEFAULT 0,
    status VARCHAR(20) DEFAULT 'ativo',
    data_resgate_ultima TIMESTAMP,
    data_inclusao TIMESTAMP DEFAULT now(),
    data_ultima_atualizacao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_fidelidade_saldos_cartao FOREIGN KEY (cartao_id) REFERENCES public.fidelidade_cartoes(id),
    CONSTRAINT fk_fidelidade_saldos_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id),
    CONSTRAINT uk_fidelidade_saldos_cartao_usuario UNIQUE (cartao_id, usuario_id)
);

CREATE TABLE public.fidelidade_carimbos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    saldo_id UUID NOT NULL,
    status VARCHAR(20) DEFAULT 'valido', -- 'valido', 'utilizado', 'expirado'
    codigo_referencia VARCHAR(100),
    data_expiracao TIMESTAMP,
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_fidelidade_carimbos_saldo FOREIGN KEY (saldo_id) REFERENCES public.fidelidade_saldos(id)
);

CREATE TABLE public.fidelidade_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    saldo_id UUID NOT NULL,
    tipo_evento VARCHAR(50) NOT NULL, -- 'GANHOU_CARIMBO', 'RESGATOU_RECOMPENSA', 'EXPIRADO'
    descricao TEXT,
    codigo_referencia VARCHAR(100),
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_fidelidade_logs_saldo FOREIGN KEY (saldo_id) REFERENCES public.fidelidade_saldos(id)
);

CREATE TABLE public.fidelidade_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    token VARCHAR(255) UNIQUE NOT NULL,
    cartao_id UUID NOT NULL,
    empresa_id UUID NOT NULL,
    status VARCHAR(20) DEFAULT 'disponivel',
    data_expiracao TIMESTAMP NOT NULL,
    data_inclusao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_fidelidade_tokens_cartao FOREIGN KEY (cartao_id) REFERENCES public.fidelidade_cartoes(id),
    CONSTRAINT fk_fidelidade_tokens_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id)
);
