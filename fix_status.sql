ALTER TABLE veiculos_anuncios ALTER COLUMN status DROP DEFAULT;
ALTER TABLE veiculos_anuncios ALTER COLUMN status TYPE TEXT USING status::TEXT;
ALTER TABLE veiculos_anuncios ALTER COLUMN status SET DEFAULT 'ativo';
