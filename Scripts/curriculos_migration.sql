-- ============================================================
-- MÓDULO DE CURRÍCULO
-- ============================================================

CREATE TABLE IF NOT EXISTS curriculos (
    id                  UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    conta_id            UUID            NOT NULL,
    usuario_id          UUID            NOT NULL,
    cargo_desejado      VARCHAR(200)    NOT NULL DEFAULT '',
    area_atuacao        VARCHAR(100)    NOT NULL DEFAULT '',
    nivel               VARCHAR(50)     NOT NULL DEFAULT 'pleno',
    modalidade          VARCHAR(50)     NOT NULL DEFAULT 'clt',
    pretensao_salarial  NUMERIC(12,2)   NULL,
    disponibilidade     VARCHAR(50)     NOT NULL DEFAULT 'imediata',
    aceita_viagem       BOOLEAN         NOT NULL DEFAULT FALSE,
    aceita_mudanca      BOOLEAN         NOT NULL DEFAULT FALSE,
    resumo_profissional TEXT            NULL,
    cnh_categorias      VARCHAR(20)     NULL,
    pcd                 BOOLEAN         NOT NULL DEFAULT FALSE,
    tipo_deficiencia    VARCHAR(200)    NULL,
    visivel             BOOLEAN         NOT NULL DEFAULT TRUE,
    status              VARCHAR(20)     NOT NULL DEFAULT 'ativo',
    data_inclusao       TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    data_atualizacao    TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_curriculos_usuario ON curriculos(conta_id, usuario_id);
CREATE INDEX IF NOT EXISTS ix_curriculos_conta ON curriculos(conta_id);
CREATE INDEX IF NOT EXISTS ix_curriculos_area ON curriculos(conta_id, area_atuacao);

-- ============================================================

CREATE TABLE IF NOT EXISTS curriculo_experiencias (
    id              UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    curriculo_id    UUID            NOT NULL REFERENCES curriculos(id) ON DELETE CASCADE,
    conta_id        UUID            NOT NULL,
    empresa         VARCHAR(200)    NOT NULL DEFAULT '',
    cargo           VARCHAR(200)    NOT NULL DEFAULT '',
    data_inicio     DATE            NOT NULL,
    data_fim        DATE            NULL,
    em_andamento    BOOLEAN         NOT NULL DEFAULT FALSE,
    descricao       TEXT            NULL,
    cidade          VARCHAR(100)    NULL,
    data_inclusao   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_experiencias_curriculo ON curriculo_experiencias(curriculo_id);

-- ============================================================

CREATE TABLE IF NOT EXISTS curriculo_formacoes (
    id              UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    curriculo_id    UUID            NOT NULL REFERENCES curriculos(id) ON DELETE CASCADE,
    conta_id        UUID            NOT NULL,
    instituicao     VARCHAR(200)    NOT NULL DEFAULT '',
    curso           VARCHAR(200)    NOT NULL DEFAULT '',
    grau            VARCHAR(50)     NOT NULL DEFAULT 'graduacao',
    data_inicio     DATE            NOT NULL,
    data_fim        DATE            NULL,
    em_andamento    BOOLEAN         NOT NULL DEFAULT FALSE,
    data_inclusao   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_formacoes_curriculo ON curriculo_formacoes(curriculo_id);

-- ============================================================

CREATE TABLE IF NOT EXISTS curriculo_cursos (
    id              UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    curriculo_id    UUID            NOT NULL REFERENCES curriculos(id) ON DELETE CASCADE,
    conta_id        UUID            NOT NULL,
    nome            VARCHAR(300)    NOT NULL DEFAULT '',
    instituicao     VARCHAR(200)    NULL,
    carga_horaria   INT             NULL,
    ano_conclusao   SMALLINT        NULL,
    data_inclusao   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_cursos_curriculo ON curriculo_cursos(curriculo_id);

-- ============================================================

CREATE TABLE IF NOT EXISTS curriculo_idiomas (
    id              UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    curriculo_id    UUID            NOT NULL REFERENCES curriculos(id) ON DELETE CASCADE,
    conta_id        UUID            NOT NULL,
    idioma          VARCHAR(100)    NOT NULL DEFAULT '',
    nivel           VARCHAR(50)     NOT NULL DEFAULT 'basico'
);

CREATE INDEX IF NOT EXISTS ix_idiomas_curriculo ON curriculo_idiomas(curriculo_id);

-- ============================================================

CREATE TABLE IF NOT EXISTS curriculo_habilidades (
    id              UUID            PRIMARY KEY DEFAULT gen_random_uuid(),
    curriculo_id    UUID            NOT NULL REFERENCES curriculos(id) ON DELETE CASCADE,
    conta_id        UUID            NOT NULL,
    habilidade      VARCHAR(200)    NOT NULL DEFAULT ''
);

CREATE INDEX IF NOT EXISTS ix_habilidades_curriculo ON curriculo_habilidades(curriculo_id);
