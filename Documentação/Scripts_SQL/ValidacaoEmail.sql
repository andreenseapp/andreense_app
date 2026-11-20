-- SEGURANÇA: VALIDAÇÃO DE E-MAIL E 2FA

ALTER TABLE public.usuarios ADD COLUMN email_validado BOOLEAN DEFAULT FALSE;

CREATE TABLE public.codigos_validacao (
    id BIGSERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    codigo VARCHAR(10) NOT NULL,
    tipo VARCHAR(50) NOT NULL, -- 'validacao_email', 'recuperacao_senha', '2fa'
    expira_em TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE,
    data_inclusao TIMESTAMP DEFAULT now()
);

CREATE INDEX idx_codigos_validacao_email ON public.codigos_validacao(email);
CREATE INDEX idx_codigos_validacao_codigo ON public.codigos_validacao(codigo);
