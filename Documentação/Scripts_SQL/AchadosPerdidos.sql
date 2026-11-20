-- MÓDULO: CIDADE - ACHADOS E PERDIDOS

CREATE TABLE public.cidade_achados_perdidos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID NOT NULL,
    tipo VARCHAR(20) NOT NULL, -- 'achado', 'perdido'
    titulo VARCHAR(255) NOT NULL,
    descricao TEXT,
    local_visto VARCHAR(255),
    data_evento DATE,
    contato_info VARCHAR(255),
    esconder_contato BOOLEAN DEFAULT TRUE,
    url_imagem TEXT,
    status VARCHAR(20) DEFAULT 'ativo', -- 'ativo', 'resolvido', 'cancelado'
    data_inclusao TIMESTAMP DEFAULT now(),
    data_ultima_atualizacao TIMESTAMP DEFAULT now(),
    CONSTRAINT fk_achados_perdidos_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id)
);

CREATE INDEX idx_achados_perdidos_tipo ON public.cidade_achados_perdidos(tipo);
CREATE INDEX idx_achados_perdidos_status ON public.cidade_achados_perdidos(status);
