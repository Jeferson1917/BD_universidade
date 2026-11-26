// Cria as collections do banco "universidade" no MongoDB com validador de schema
// ($jsonSchema) e índices — a recriação das restrições de chave/domínio/NOT NULL
// que o Postgres garantia de graça. Ver docs/mapeamento-mongodb.md para a explicação
// completa do mapeamento. Integridade referencial (FK) não é imposta aqui: fica a
// cargo da aplicação (ver *Repository.cs em src/Universidade.Mongo/Data).
//
// Uso: mongosh "<connection-string>" mongo/schema.js

db = db.getSiblingDB("universidade");

const colecoes = [
    "usuarios", "professores", "departamentos", "cursos", "estudantes",
    "projetos", "disciplinas", "semestres", "salas", "horarios",
    "turmas", "vinculos", "cursa", "contadores"
];
colecoes.forEach(c => db[c].drop());

// ---------------------------------------------------------------------------
// usuarios  (universidade.usuario)
// ---------------------------------------------------------------------------
db.createCollection("usuarios", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "nome"],
            properties: {
                _id: { bsonType: "string", pattern: "^[0-9]{11}$", description: "CPF, 11 dígitos" },
                nome: { bsonType: "string", maxLength: 100 },
                data_nascimento: { bsonType: ["date", "null"] },
                email: { bsonType: ["array", "null"], items: { bsonType: "string" } },
                telefone: { bsonType: ["array", "null"], items: { bsonType: "string" } },
                login: { bsonType: ["string", "null"], maxLength: 45 },
                senha: { bsonType: ["string", "null"], maxLength: 32 }
            }
        }
    }
});
db.usuarios.createIndex({ login: 1 }, { unique: true, partialFilterExpression: { login: { $type: "string" } } });

// ---------------------------------------------------------------------------
// departamentos  (universidade.departamento)
// ---------------------------------------------------------------------------
db.createCollection("departamentos", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "nome"],
            properties: {
                _id: { bsonType: "string", maxLength: 5, description: "cod_depto" },
                nome: { bsonType: "string", maxLength: 50 },
                chefe: { bsonType: ["string", "null"], description: "ref professores._id" },
                orcamento: { bsonType: ["double", "int", "null"], exclusiveMinimum: true, minimum: 0 },
                comissal: { bsonType: ["double", "int", "null"] }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// professores  (universidade.professor)
// ---------------------------------------------------------------------------
db.createCollection("professores", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: { bsonType: "string", maxLength: 7, description: "mat_professor" },
                cpf: { bsonType: ["string", "null"], description: "ref usuarios._id" },
                departamento: { bsonType: ["string", "null"], description: "ref departamentos._id" },
                formacao: { enum: ["Graduação", "Especialização", "Mestrado", "Doutorado", null] },
                data_admissao: { bsonType: ["date", "null"] },
                tipo_jornada_trabalho: { enum: ["20h", "40h", "DE", null] },
                salario: { bsonType: ["double", "int", "null"] }
            }
        }
    }
});
db.professores.createIndex({ cpf: 1 }, { unique: true, partialFilterExpression: { cpf: { $type: "string" } } });

// ---------------------------------------------------------------------------
// cursos  (universidade.curso)
// ---------------------------------------------------------------------------
db.createCollection("cursos", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "nome", "turno"],
            properties: {
                _id: { bsonType: "int", description: "idCurso, gerado via \"contadores\"" },
                nome: { bsonType: "string", maxLength: 100 },
                grau: { enum: ["Bacharelado", "Licenciatura Plena", null] },
                turno: { enum: ["Matutino", "Vespertino", "Noturno", "Turno Indefinido"] },
                campus: { bsonType: ["string", "null"], maxLength: 100 },
                nivel: { enum: ["Graduação", "Mestrado", "Doutorado", "Lato", null] }
            }
        }
    }
});
db.cursos.createIndex({ nome: 1, turno: 1, campus: 1, nivel: 1 }, { unique: true });

// ---------------------------------------------------------------------------
// estudantes  (universidade.estudante + universidade.plano embutido)
// ---------------------------------------------------------------------------
db.createCollection("estudantes", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: { bsonType: "string", maxLength: 7, description: "mat_estudante" },
                cpf: { bsonType: ["string", "null"], description: "ref usuarios._id" },
                mc: { bsonType: ["double", "int", "null"], minimum: 0, maximum: 10 },
                ano_ingresso: { bsonType: ["int", "null"] },
                planos: {
                    bsonType: "array",
                    description: "universidade.plano embutido (1 aluno : poucos planos, sempre lido junto)",
                    items: {
                        bsonType: "object",
                        properties: {
                            id_projeto: { bsonType: ["int", "null"], description: "ref projetos._id" },
                            mat_professor: { bsonType: ["string", "null"], description: "ref professores._id" },
                            ano: { bsonType: ["int", "null"] }
                        }
                    }
                }
            }
        }
    }
});
db.estudantes.createIndex({ cpf: 1 }, { unique: true, partialFilterExpression: { cpf: { $type: "string" } } });

// ---------------------------------------------------------------------------
// projetos  (universidade.projeto)
// ---------------------------------------------------------------------------
db.createCollection("projetos", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: { bsonType: "int", description: "id_projeto" },
                descricao: { bsonType: ["string", "null"] }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// disciplinas  (universidade.disciplina)
// ---------------------------------------------------------------------------
db.createCollection("disciplinas", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "nome"],
            properties: {
                _id: { bsonType: "string", maxLength: 8, description: "cod_disc" },
                nome: { bsonType: "string", maxLength: 40 },
                pre_req: { bsonType: ["string", "null"], description: "auto-referência: ref disciplinas._id" },
                creditos: { bsonType: ["int", "null"], minimum: 1, maximum: 11 },
                depto_responsavel: { bsonType: ["string", "null"], description: "ref departamentos._id" }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// semestres  (universidade.semestre) — chave composta natural como _id
// ---------------------------------------------------------------------------
db.createCollection("semestres", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: {
                    bsonType: "object",
                    required: ["ano", "semestre"],
                    properties: {
                        ano: { bsonType: "int" },
                        semestre: { bsonType: "int" }
                    }
                },
                data_inicio: { bsonType: ["date", "null"] },
                data_fim: { bsonType: ["date", "null"] }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// salas / horarios  (universidade.sala / universidade.horario)
// Tabelas de referência pequenas; não há dados no script original para elas.
// ---------------------------------------------------------------------------
db.createCollection("salas", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: { bsonType: "int", description: "id_sala" },
                descricao: { bsonType: ["string", "null"] }
            }
        }
    }
});

db.createCollection("horarios", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "dia", "slot"],
            properties: {
                _id: { bsonType: "int", description: "id_horario" },
                dia: { bsonType: "string", maxLength: 15 },
                slot: { bsonType: "int" }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// turmas  (universidade.turma + leciona/alocacao embutidos)
// ---------------------------------------------------------------------------
db.createCollection("turmas", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id", "cod_disc"],
            properties: {
                _id: { bsonType: "int", description: "id_turma" },
                cod_disc: { bsonType: "string", description: "ref disciplinas._id" },
                numero: { bsonType: ["int", "null"] },
                ano: { bsonType: ["int", "null"] },
                semestre: { bsonType: ["int", "null"] },
                professores: {
                    bsonType: "array",
                    description: "universidade.leciona embutido: refs a professores._id",
                    items: { bsonType: "string" }
                },
                alocacoes: {
                    bsonType: "array",
                    description: "universidade.alocacao embutido",
                    items: {
                        bsonType: "object",
                        properties: {
                            id_horario: { bsonType: "int", description: "ref horarios._id" },
                            id_sala: { bsonType: "int", description: "ref salas._id" }
                        }
                    }
                }
            }
        }
    }
});
db.turmas.createIndex({ cod_disc: 1, numero: 1, semestre: 1, ano: 1 }, { unique: true });

// ---------------------------------------------------------------------------
// vinculos  (universidade.vinculo) — exigido como collection própria (CRUD da Parte 2)
// ---------------------------------------------------------------------------
db.createCollection("vinculos", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["_id"],
            properties: {
                _id: { bsonType: "int", description: "idVinculo, gerado via \"contadores\"" },
                mat_estudante: { bsonType: ["string", "null"], description: "ref estudantes._id" },
                curso: { bsonType: ["int", "null"], description: "ref cursos._id" },
                data_entrada: { bsonType: ["date", "null"] },
                status: { enum: ["Ativo", "Cancelada", "Formando", "Graduado", null] },
                data_saida: { bsonType: ["date", "null"] }
            }
        }
    }
});

// ---------------------------------------------------------------------------
// cursa  (universidade.cursa) — N:N estudante<->turma com nota; cresce sem limite
// nos dois lados, por isso fica como collection própria (não embutida).
// ---------------------------------------------------------------------------
db.createCollection("cursa", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["mat_estudante", "id_turma"],
            properties: {
                mat_estudante: { bsonType: "string", description: "ref estudantes._id" },
                id_turma: { bsonType: "int", description: "ref turmas._id" },
                nota: { bsonType: ["double", "int", "null"] }
            }
        }
    }
});
db.cursa.createIndex({ mat_estudante: 1, id_turma: 1 }, { unique: true });

// ---------------------------------------------------------------------------
// contadores — substitui as SEQUENCEs do Postgres (idCurso, idVinculo, ...),
// já que o MongoDB não tem autoincremento nativo (ver MongoDb.ProximoIdAsync).
// ---------------------------------------------------------------------------
db.createCollection("contadores");

print("Schema criado: " + colecoes.length + " collections.");
