-- Esquema estrela da Parte 3 (data warehouse).
-- Roda num banco novo no mesmo servidor RDS da Parte 1, separado do banco "universidade"
-- (ex.: criar antes com `CREATE DATABASE universidade_dw;` conectado como admin).

CREATE SCHEMA IF NOT EXISTS dw;

-- Dimensao departamento: dimensao conformada, referenciada direto pelo fato
-- (departamento responsavel pela disciplina da turma).
CREATE TABLE dw.dim_departamento (
    sk_departamento SERIAL PRIMARY KEY,
    cod_depto       VARCHAR(20),
    nome            VARCHAR(100) NOT NULL,
    sistema_origem  VARCHAR(20) NOT NULL
);

-- Dimensao professor: atributos de departamento de lotacao ficam achatados aqui
-- (dimensao denormalizada, estilo Kimball), sem FK pra dim_departamento.
CREATE TABLE dw.dim_professor (
    sk_professor               SERIAL PRIMARY KEY,
    mat_professor              VARCHAR(20),
    nome                       VARCHAR(150) NOT NULL,
    tipo_jornada_trabalho      VARCHAR(10),
    formacao                   VARCHAR(30),
    cod_departamento_lotacao   VARCHAR(20),
    nome_departamento_lotacao  VARCHAR(100),
    sistema_origem             VARCHAR(20) NOT NULL
);

-- Dimensao disciplina: mesma logica, departamento responsavel achatado aqui tambem.
CREATE TABLE dw.dim_disciplina (
    sk_disciplina                  SERIAL PRIMARY KEY,
    cod_disc                       VARCHAR(20),
    nome                           VARCHAR(150) NOT NULL,
    cr_total                       SMALLINT,
    cod_departamento_responsavel   VARCHAR(20),
    nome_departamento_responsavel  VARCHAR(100),
    sistema_origem                 VARCHAR(20) NOT NULL
);

-- Dimensao semestre: chave natural (ano, periodo), sem motivo pra surrogate alem do sk.
CREATE TABLE dw.dim_semestre (
    sk_semestre SERIAL PRIMARY KEY,
    ano         SMALLINT NOT NULL,
    periodo     SMALLINT NOT NULL,
    UNIQUE (ano, periodo)
);

-- Fato: grao = uma linha por turma + professor (cobre turmas com mais de um professor,
-- via leciona N:N na origem relacional). Turma sem professor usa o membro "Nao informado"
-- (sk_professor = 0) em vez de FK nula.
CREATE TABLE dw.fato_turma (
    sk_fato_turma     BIGSERIAL PRIMARY KEY,
    id_turma_origem   VARCHAR(30),
    numero_turma      INT,
    sk_professor      INT NOT NULL REFERENCES dw.dim_professor(sk_professor),
    sk_disciplina     INT NOT NULL REFERENCES dw.dim_disciplina(sk_disciplina),
    sk_departamento   INT NOT NULL REFERENCES dw.dim_departamento(sk_departamento),
    sk_semestre       INT NOT NULL REFERENCES dw.dim_semestre(sk_semestre),
    qtd_matriculados  INT,
    media_notas       NUMERIC(4,2),
    qtd_aprovados     INT,
    qtd_reprovados    INT,
    sistema_origem    VARCHAR(20) NOT NULL
);

-- Membros "Nao informado" (sk = 0), usados quando a origem nao tem a associacao
-- (ex.: turma sem leciona). Mantem as FKs do fato sempre NOT NULL.
INSERT INTO dw.dim_departamento (sk_departamento, cod_depto, nome, sistema_origem)
VALUES (0, NULL, 'Nao informado', 'dw');

INSERT INTO dw.dim_professor (sk_professor, mat_professor, nome, tipo_jornada_trabalho, formacao, cod_departamento_lotacao, nome_departamento_lotacao, sistema_origem)
VALUES (0, NULL, 'Nao informado', NULL, NULL, NULL, NULL, 'dw');

INSERT INTO dw.dim_disciplina (sk_disciplina, cod_disc, nome, cr_total, cod_departamento_responsavel, nome_departamento_responsavel, sistema_origem)
VALUES (0, NULL, 'Nao informado', NULL, NULL, NULL, 'dw');

-- Ajusta as sequences pra nao colidir com os sk = 0 inseridos a mao acima.
SELECT setval(pg_get_serial_sequence('dw.dim_departamento', 'sk_departamento'), 1, false);
SELECT setval(pg_get_serial_sequence('dw.dim_professor', 'sk_professor'), 1, false);
SELECT setval(pg_get_serial_sequence('dw.dim_disciplina', 'sk_disciplina'), 1, false);
