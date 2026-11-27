# BD Universidade

Trabalho prático de Banco de Dados.

- **Parte 1**: CRUD relacional (PostgreSQL) para `usuario`, `professor`, `leciona` e `turma`, em ASP.NET Core Razor Pages.
- **Parte 2**: Mapeamento do esquema relacional para MongoDB e CRUD de `usuario`, `estudante`, `vinculo` e `curso`.

## Requisitos

- .NET SDK 8.0
- PostgreSQL (Parte 1, hospedado na AWS RDS)
- MongoDB (Parte 2, hospedado na AWS) e o `mongosh` para rodar os scripts

---

## Parte 1: CRUD relacional (`src/Universidade.Web`)

### 1. Criar o banco

Rode o script `sql/universidade.sql` no PostgreSQL (ele já começa com `DROP SCHEMA IF EXISTS universidade CASCADE`, então pode ser executado várias vezes).

### 2. Configurar a connection string

```bash
cd src/Universidade.Web
dotnet user-secrets set "ConnectionStrings:Universidade" "Host=<endpoint-rds>;Port=5432;Database=<nome-do-banco>;Username=<usuario>;Password=<senha>;SSL Mode=Require"
```

### 3. Rodar

```bash
cd src/Universidade.Web
dotnet run
```

Abre em `http://localhost:5254` (ou `https://localhost:7070`).

---

## Parte 2: CRUD MongoDB (`src/Universidade.Mongo`)

Veja `docs/mapeamento-mongodb.md` para a explicação completa do mapeamento de
todas as tabelas do esquema relacional para as collections do MongoDB.

### 1. Criar o schema e popular o banco

```bash
mongosh "<connection-string-do-mongo>" mongo/schema.js
mongosh "<connection-string-do-mongo>" mongo/seed.js
```

### 2. Configurar a connection string

```bash
cd src/Universidade.Mongo
dotnet user-secrets set "ConnectionStrings:Universidade" "mongodb://<usuario>:<senha>@<host>:27017/?authSource=admin"
```

### 3. Rodar

```bash
cd src/Universidade.Mongo
dotnet run
```

Abre em `http://localhost:5086` (ou `https://localhost:7242`).

---

## Estrutura

- `sql/universidade.sql`: script de criação do esquema relacional `universidade` e dados iniciais.
- `mongo/schema.js`: cria as collections do MongoDB com validadores de schema (`$jsonSchema`) e índices.
- `mongo/seed.js`: popula o MongoDB com os mesmos dados do `universidade.sql`, já no formato mapeado.
- `docs/mapeamento-mongodb.md`: relatório do mapeamento relacional para o MongoDB.
- `dw/schema_estrela.sql`: esquema estrela do data warehouse da Parte 3 (dimensões, fato e membros "Não informado").
- `src/Universidade.Web`: Parte 1 (PostgreSQL via Npgsql).
- `src/Universidade.Mongo`: Parte 2 (MongoDB via MongoDB.Driver).
