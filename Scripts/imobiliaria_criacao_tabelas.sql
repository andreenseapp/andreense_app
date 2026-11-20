-- ============================================================
-- Hub Imobiliário — Tabelas do Módulo
-- ============================================================

CREATE TABLE IF NOT EXISTS imobiliaria_imoveis (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conta_id UUID NOT NULL REFERENCES contas(id),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    tipo_imovel VARCHAR(50) NOT NULL,
    area_total DECIMAL(10,2),
    quartos SMALLINT NOT NULL DEFAULT 0,
    banheiros SMALLINT NOT NULL DEFAULT 0,
    vagas_garagem SMALLINT NOT NULL DEFAULT 0,
    andar SMALLINT,
    total_andares SMALLINT,
    tem_elevador BOOLEAN NOT NULL DEFAULT FALSE,
    tem_piscina BOOLEAN NOT NULL DEFAULT FALSE,
    aceita_pet BOOLEAN NOT NULL DEFAULT FALSE,
    tem_churrasqueira BOOLEAN NOT NULL DEFAULT FALSE,
    tem_salao_festas BOOLEAN NOT NULL DEFAULT FALSE,
    tem_quadra BOOLEAN NOT NULL DEFAULT FALSE,
    tem_academia BOOLEAN NOT NULL DEFAULT FALSE,
    tem_portaria_24h BOOLEAN NOT NULL DEFAULT FALSE,
    tem_varanda BOOLEAN NOT NULL DEFAULT FALSE,
    tem_playground BOOLEAN NOT NULL DEFAULT FALSE,
    tem_agua_separada BOOLEAN NOT NULL DEFAULT FALSE,
    tem_luz_separada BOOLEAN NOT NULL DEFAULT FALSE,
    tem_entrada_separada BOOLEAN NOT NULL DEFAULT FALSE,
    tem_cobertura BOOLEAN NOT NULL DEFAULT FALSE,
    logradouro VARCHAR(255),
    numero VARCHAR(20),
    complemento VARCHAR(100),
    bairro VARCHAR(100) NOT NULL,
    cidade VARCHAR(100) NOT NULL DEFAULT 'Santo André',
    cep VARCHAR(9),
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_ultima_atualizacao TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS imobiliaria_anuncios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    imovel_id UUID NOT NULL REFERENCES imobiliaria_imoveis(id),
    conta_id UUID NOT NULL REFERENCES contas(id),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    tipo_negocio VARCHAR(20) NOT NULL,
    valor_principal DECIMAL(12,2) NOT NULL,
    valor_condominio DECIMAL(12,2) NOT NULL DEFAULT 0,
    valor_iptu DECIMAL(12,2) NOT NULL DEFAULT 0,
    titulo VARCHAR(200) NOT NULL,
    descricao TEXT,
    contato_whatsapp VARCHAR(20),
    mostrar_logradouro BOOLEAN NOT NULL DEFAULT TRUE,
    mostrar_numero BOOLEAN NOT NULL DEFAULT TRUE,
    status VARCHAR(30) NOT NULL DEFAULT 'ativo',
    data_expiracao TIMESTAMPTZ,
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_ultima_atualizacao TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS imobiliaria_fotos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    anuncio_id UUID NOT NULL REFERENCES imobiliaria_anuncios(id) ON DELETE CASCADE,
    url VARCHAR(500) NOT NULL,
    ordem SMALLINT NOT NULL DEFAULT 0,
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_status        ON imobiliaria_anuncios(status);
CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_tipo_negocio   ON imobiliaria_anuncios(tipo_negocio);
CREATE INDEX IF NOT EXISTS idx_imobiliaria_imoveis_bairro          ON imobiliaria_imoveis(bairro);
CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_usuario_id     ON imobiliaria_anuncios(usuario_id);
CREATE INDEX IF NOT EXISTS idx_imobiliaria_fotos_anuncio_id        ON imobiliaria_fotos(anuncio_id);
CREATE INDEX IF NOT EXISTS idx_imobiliaria_anuncios_data_inclusao  ON imobiliaria_anuncios(data_inclusao DESC);

CREATE TABLE IF NOT EXISTS imobiliaria_buscas_recentes (
    id UUID PRIMARY KEY,
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    chave_filtro VARCHAR(64) NOT NULL,
    filtro_json JSONB NOT NULL,
    rotulo VARCHAR(200) NOT NULL,
    data_inclusao TIMESTAMP NOT NULL,
    data_ultima_busca TIMESTAMP NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_imobiliaria_buscas_recentes_usuario_chave
    ON imobiliaria_buscas_recentes (usuario_id, chave_filtro);

CREATE INDEX IF NOT EXISTS ix_imobiliaria_buscas_recentes_usuario_data
    ON imobiliaria_buscas_recentes (usuario_id, data_ultima_busca DESC);

ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS aceita_pet BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_churrasqueira BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_salao_festas BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_quadra BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_academia BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_portaria_24h BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_varanda BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_playground BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_agua_separada BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_luz_separada BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_entrada_separada BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE imobiliaria_imoveis ADD COLUMN IF NOT EXISTS tem_cobertura BOOLEAN NOT NULL DEFAULT FALSE;
