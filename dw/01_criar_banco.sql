-- Passo 1 da Parte 3. Roda conectado ao banco de manutencao do RDS
-- (normalmente "postgres"), com o mesmo usuario admin usado na Parte 1.
-- CREATE DATABASE nao pode rodar enquanto conectado ao proprio banco
-- que esta sendo criado, por isso fica separado do resto.

CREATE DATABASE universidade_dw;
