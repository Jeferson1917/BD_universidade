-- Passo 3 da Parte 3. Roda conectado ao banco "universidade_dw",
-- depois de ja ter aplicado dw/schema_estrela.sql (as tabelas do
-- schema dw precisam existir antes dos GRANTs abaixo).
-- Troque a senha antes de rodar.

CREATE ROLE etl_universidade_dw WITH LOGIN PASSWORD 'senha123';

GRANT CONNECT ON DATABASE universidade_dw TO etl_universidade_dw;
GRANT USAGE ON SCHEMA dw TO etl_universidade_dw;
GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA dw TO etl_universidade_dw;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA dw TO etl_universidade_dw;

-- Garante que tabelas novas criadas depois no schema dw ja saiam com a mesma permissao.
ALTER DEFAULT PRIVILEGES IN SCHEMA dw GRANT SELECT, INSERT, UPDATE ON TABLES TO etl_universidade_dw;
ALTER DEFAULT PRIVILEGES IN SCHEMA dw GRANT USAGE, SELECT ON SEQUENCES TO etl_universidade_dw;
 