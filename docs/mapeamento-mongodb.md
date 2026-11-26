# Mapeamento do esquema `universidade` para MongoDB

Este documento explica como cada tabela do modelo relacional (`sql/universidade.sql`)
foi mapeada para collections do MongoDB, e como as restrições que o PostgreSQL
garantia automaticamente (chave, domínio, `NOT NULL`, integridade referencial) são
recriadas no banco NoSQL.

## Regra geral

A chave primária original do PostgreSQL é usada como `_id` do documento
correspondente (string onde o domínio era `VARCHAR`, número onde era `SERIAL`/`INT`).
Isso preserva a correspondência 1:1 com o relacional e facilita o ETL da Parte 3.

A decisão entre **embedding** (aninhar) e **referência** (guardar o `_id` de outro
documento, como uma FK) segue um critério único: *o "filho" só existe através do
"pai", tem crescimento limitado e é sempre lido junto dele? → embedding. Senão →
referência.*

## Entidades fortes → uma collection cada (referência)

| Tabela relacional | Collection | `_id` | Observações |
|---|---|---|---|
| `usuario` | `usuarios` | `cpf` (string, 11 dígitos) | `email`/`telefone` continuam array nativo. Índice único em `login` (parcial, pois pode ser nulo). |
| `professor` | `professores` | `mat_professor` | `cpf` e `departamento` são referências. Não é embutido em `usuario` porque é 1:1 opcional e consultado independentemente. |
| `departamento` | `departamentos` | `cod_depto` | `chefe` é referência a `professores`. |
| `curso` | `cursos` | `idCurso` (int, via collection `contadores`) | Entidade independente, referenciada por `vinculo`. |
| `estudante` | `estudantes` | `mat_estudante` | `cpf` é referência a `usuarios`. |
| `projeto` | `projetos` | `id_projeto` | Referenciado pelo array `planos` embutido em `estudantes`. |
| `disciplina` | `disciplinas` | `cod_disc` | `pre_req` é auto-referência; `depto_responsavel` referencia `departamentos`. |
| `semestre` | `semestres` | `{ ano, semestre }` (objeto) | Chave composta natural, sem motivo pra ter um surrogate. |
| `sala` | `salas` | `id_sala` | Tabela de referência pequena (sem dados no script original). |
| `horario` | `horarios` | `id_horario` | Tabela de referência pequena (sem dados no script original). |
| `turma` | `turmas` | `id_turma` | `cod_disc` referencia `disciplinas`; `ano`/`semestre` espelham a chave de `semestres`. |

## Associações N:N → decisão caso a caso

| Tabela relacional | Decisão | Onde fica | Por quê |
|---|---|---|---|
| `leciona` (turma↔professor) | **Embedding** | array `professores: [mat_professor, ...]` dentro de `turma` | Uma turma tem poucos professores; a lista não cresce sem limite e é sempre lida junto da turma. |
| `alocacao` (turma↔horário+sala) | **Embedding** | array `alocacoes: [{ id_horario, id_sala }, ...]` dentro de `turma` | Mesmo raciocínio: poucos horários por turma, sempre consultados com ela. A unicidade `(id_horario, id_sala)` entre turmas diferentes vira checagem na aplicação. |
| `plano` (estudante↔professor↔projeto, por ano) | **Embedding** | array `planos: [{ id_projeto, mat_professor, ano }, ...]` dentro de `estudante` | Histórico de orientação do aluno: cresce no máximo um item por ano, e faz sentido ver junto do perfil do estudante. |
| `vinculo` (estudante↔curso) | **Collection própria** | `vinculos`, `_id = idVinculo` (via `contadores`), com `mat_estudante` e `curso` como referências | Exigido pelo enunciado como estrutura própria de CRUD: não pode ser embutido. |
| `cursa` (estudante↔turma, com nota) | **Collection própria** | `cursa`, com `mat_estudante` e `id_turma` como referências + `nota` | Os dois lados crescem sem limite claro (uma turma pode ter centenas de alunos ao longo do tempo; um aluno cursa dezenas de turmas): embutir em qualquer lado arrisca documentos grandes e updates concorrentes. |

## Restrições do relacional → como são recriadas

| Restrição no relacional | Equivalente no MongoDB |
|---|---|
| Chave primária | `_id` (unicidade automática) |
| Chave única (`login`, `professor.cpf`, `uq_curso`, `uq_turma`) | Índice único na collection (`mongo/schema.js`); parcial quando o campo pode ser nulo |
| Domínio / `ENUM` / `CHECK` (`tipo_jornada`, `creditos BETWEEN 1 AND 11`, `orcamento > 0`) | Validador de schema da collection (`$jsonSchema`, com `enum`/`minimum`/`maximum`): ver `mongo/schema.js` |
| `NOT NULL` | Campo listado em `required` no `$jsonSchema` da collection |
| Integridade referencial (FK, `ON DELETE CASCADE`/`SET NULL`) | **Não existe de graça.** A aplicação verifica dependentes antes de excluir/alterar (ex.: `UsuarioRepository.DependenteEstudanteAsync`, `CursoRepository.ContarVinculosAsync`) e avisa o efeito na tela, sem apagar/atualizar em cascata automaticamente, diferente do Postgres |
| `SEQUENCE` (autoincremento de `idCurso`, `idVinculo`) | Collection `contadores`: um documento por sequência, incrementado atomicamente com `$inc` (`MongoDb.ProximoIdAsync`) |

## CRUD implementado (Parte 2)

O CRUD pedido no enunciado (`usuario`, `estudante`, `vinculo`, `curso`) está em
`src/Universidade.Mongo`, nas collections `usuarios`, `estudantes`, `vinculos` e
`cursos`. Nenhuma dessas quatro é uma collection que recebeu embedding de outra
tabela, então o CRUD fica direto (sem precisar manipular array aninhado), com
exceção de `estudantes`, que mantém o array `planos` embutido intacto durante as
edições (o formulário não o altera).

As demais tabelas (`professor`, `departamento`, `projeto`, `disciplina`,
`semestre`, `sala`, `horario`, `leciona`, `alocacao`, `cursa`) estão representadas
no schema (`mongo/schema.js`) e povoadas com os mesmos dados do script relacional
(`mongo/seed.js`), mas sem CRUD próprio: não fazem parte do que o enunciado pediu
para esta etapa.
