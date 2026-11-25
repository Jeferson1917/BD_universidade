# BD Universidade

Trabalho prático de Banco de Dados — Parte 1: CRUD relacional (PostgreSQL) para as tabelas `usuario`, `professor`, `leciona` e `turma`, usando ASP.NET Core Razor Pages.

## Requisitos

- .NET SDK 8.0
- PostgreSQL (o trabalho usa uma instância hospedada na AWS RDS)

## 1. Criar o banco

Rode o script `sql/universidade.sql` no banco PostgreSQL (ele já começa com `DROP SCHEMA IF EXISTS universidade CASCADE`, então pode ser executado várias vezes).

## 2. Configurar a connection string

Dentro de `src/Universidade.Web`, configure a connection string via `user-secrets` (ela não é commitada):

```bash
cd src/Universidade.Web
dotnet user-secrets set "ConnectionStrings:Universidade" "Host=<endpoint-rds>;Port=5432;Database=<nome-do-banco>;Username=<usuario>;Password=<senha>;SSL Mode=Require"
```

## 3. Rodar a aplicação

```bash
cd src/Universidade.Web
dotnet run
```

A aplicação abre em `http://localhost:5254` (ou `https://localhost:7070`).

## Estrutura

- `sql/universidade.sql` — script de criação do esquema `universidade` e dados iniciais.
- `src/Universidade.Web/Data` — acesso ao banco (Npgsql) e repositórios de CRUD.
- `src/Universidade.Web/Models` — modelos e formulários de cada entidade.
- `src/Universidade.Web/Pages` — páginas Razor (Criar/Editar/Excluir/Index) de Usuários, Professores, Leciona e Turmas.
