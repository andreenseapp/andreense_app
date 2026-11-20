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
    status INT NOT NULL DEFAULT 1,
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
