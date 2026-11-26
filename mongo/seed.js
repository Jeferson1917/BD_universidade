// Povoa o banco "universidade" no MongoDB com os mesmos dados de sql/universidade.sql,
// já no formato das collections definidas em mongo/schema.js (ver docs/mapeamento-mongodb.md).
// Rode schema.js antes deste.
//
// Uso: mongosh "<connection-string>" mongo/seed.js

db = db.getSiblingDB("universidade");

function data(iso) { return iso ? new Date(iso) : null; }

// ---------------------------------------------------------------------------
// departamentos
// ---------------------------------------------------------------------------
db.departamentos.insertMany([
    { _id: "DCOMP", nome: "Departamento de Computação", chefe: null, orcamento: 10000, comissal: 1000 },
    { _id: "DCOM2", nome: "Departamento de Computação", chefe: null, orcamento: 5000, comissal: 0 },
    { _id: "DMA", nome: "Departamento de Matemática", chefe: null, orcamento: 20000, comissal: 1000 },
    { _id: "DFI", nome: "Departamento de Física", chefe: null, orcamento: 50000, comissal: 1000 },
    { _id: "DECAT", nome: "Departamento de Estatística e Ciências Atuarias", chefe: null, orcamento: 10000, comissal: 1000 },
    { _id: "DEL", nome: "Departamento de Engenharia Elétrica", chefe: null, orcamento: null, comissal: 0 }
]);

// ---------------------------------------------------------------------------
// usuarios + professores (pares do script original)
// ---------------------------------------------------------------------------
const professores = [
    { cpf: "11111111100", nome: "Prof A", nasc: "1980-03-05", email: ["profA@email.com"], tel: ["99998888", "88889999"], login: "profa", senha: "senha1", mat: "P100", dep: "DCOMP", form: "Doutorado", jornada: "20h", sal: 2000 },
    { cpf: "11111111101", nome: "Prof B", nasc: "1980-07-23", email: ["profB@email.com", "pB@mail.com"], tel: null, login: "profb", senha: "senha2", mat: "P200", dep: "DCOMP", form: "Doutorado", jornada: "20h", sal: 2000 },
    { cpf: "11111111102", nome: "Prof C", nasc: "1987-05-12", email: ["profc@email.com"], tel: ["99998885", "84389999"], login: "profc", senha: "senha3", mat: "P300", dep: "DCOMP", form: "Doutorado", jornada: "40h", sal: 4000 },
    { cpf: "11111111103", nome: "Prof D", nasc: "1975-01-22", email: null, tel: null, login: "profd", senha: "senha4", mat: "P400", dep: "DCOMP", form: "Mestrado", jornada: "20h", sal: 2000 },
    { cpf: "11111111104", nome: "Prof E", nasc: "1980-04-25", email: ["profE@email.com"], tel: ["99298888", "88889999"], login: "profe", senha: "senha5", mat: "P500", dep: "DCOMP", form: "Mestrado", jornada: "40h", sal: 4000 },
    { cpf: "11111111105", nome: "Prof F", nasc: "1960-03-05", email: ["profF@email.com"], tel: ["89998888", "98889999"], login: "proff", senha: "senha6", mat: "P600", dep: "DMA", form: "Especialização", jornada: "20h", sal: 1500 },
    { cpf: "11111111106", nome: "Prof G", nasc: "1981-03-05", email: ["profG@email.com"], tel: ["99978888", "86889999"], login: "profg", senha: "senha7", mat: "P700", dep: "DFI", form: "Mestrado", jornada: "40h", sal: 4000 },
    { cpf: "11111111107", nome: "Prof H", nasc: "1972-08-03", email: ["profh@email.com"], tel: ["90998888", "88889999"], login: "profh", senha: "senha8", mat: "P800", dep: "DCOMP", form: "Mestrado", jornada: "40h", sal: 2000 },
    { cpf: "11111111108", nome: "Prof A", nasc: "1986-04-01", email: ["profA2@email.com"], tel: ["90998088", "81889999"], login: "profam", senha: "senha9", mat: "P900", dep: "DCOMP", form: "Mestrado", jornada: "DE", sal: 2000 },
    { cpf: "11111111109", nome: "Prof I", nasc: "1980-03-05", email: ["profI@email.com"], tel: null, login: "profi", senha: "sennha10", mat: "P1000", dep: "DCOMP", form: "Doutorado", jornada: "DE", sal: 6000 },
    { cpf: "11111111110", nome: "Prof F", nasc: "1981-09-05", email: ["profFF@email.com"], tel: ["99668888", "88329999"], login: "proffd", senha: "senha12", mat: "P1100", dep: "DECAT", form: "Graduação", jornada: "20h", sal: 1000 },
    { cpf: "11111111111", nome: "Prof K", nasc: "1980-03-04", email: ["profK@email.com"], tel: ["79998888", "97889999"], login: "profk", senha: "senha11", mat: "P1200", dep: "DMA", form: "Mestrado", jornada: "20h", sal: 2000 },
    { cpf: "11111111112", nome: "Prof I", nasc: "1980-03-05", email: ["profI@email.com"], tel: ["93998888", "92889999"], login: "profim", senha: "senha13", mat: "P1300", dep: "DMA", form: "Doutorado", jornada: "20h", sal: 2500 },
    { cpf: "11111111113", nome: "Prof G", nasc: "1954-06-15", email: ["profGA@email.com"], tel: ["97698888", "88129999"], login: "profgf", senha: "senha14", mat: "P1400", dep: "DFI", form: "Doutorado", jornada: "40h", sal: 4000 },
    { cpf: "11111111114", nome: "Prof P", nasc: "1989-11-05", email: ["profP@email.com"], tel: ["99128888", "88339999"], login: "profp", senha: "senha15", mat: "P1500", dep: "DCOM2", form: "Doutorado", jornada: "DE", sal: 6000 }
];

professores.forEach(p => {
    db.usuarios.insertOne({
        _id: p.cpf, nome: p.nome, data_nascimento: data(p.nasc),
        email: p.email, telefone: p.tel, login: p.login, senha: p.senha
    });
    db.professores.insertOne({
        _id: p.mat, cpf: p.cpf, departamento: p.dep, formacao: p.form,
        data_admissao: null, tipo_jornada_trabalho: p.jornada, salario: p.sal
    });
});

db.departamentos.updateOne({ _id: "DCOMP" }, { $set: { chefe: "P100" } });
db.departamentos.updateOne({ _id: "DMA" }, { $set: { chefe: "P600" } });
db.departamentos.updateOne({ _id: "DECAT" }, { $set: { chefe: "P1100" } });
db.departamentos.updateOne({ _id: "DFI" }, { $set: { chefe: "P1400" } });

// ---------------------------------------------------------------------------
// usuarios + estudantes
// ---------------------------------------------------------------------------
const estudantes = [
    { cpf: "22222222201", nome: "Steve Jobs", nasc: "1990-03-05", email: ["steve@email.com", "steve@apple.com"], tel: null, login: "steve", senha: "s1", mat: "E101", mc: 7.0, ano: 2021 },
    { cpf: "22222222202", nome: "Paul Bell", nasc: "1999-09-15", email: ["bell@email.com"], tel: null, login: "paul", senha: "s2", mat: "E102", mc: 8.3, ano: 2021 },
    { cpf: "22222222203", nome: "Alan Turing", nasc: "1912-07-23", email: null, tel: null, login: "alan", senha: "s3", mat: "E103", mc: 6.7, ano: 2021 },
    { cpf: "22222222204", nome: "John Hopcroft", nasc: "1939-10-07", email: ["hopcroft@lfc.com"], tel: null, login: "john", senha: "s4", mat: "E104", mc: 0, ano: 2021 },
    { cpf: "22222222205", nome: "Ada Lovelace", nasc: "1985-11-27", email: null, tel: null, login: "ada", senha: "s5", mat: "E105", mc: 9, ano: 2022 },
    { cpf: "22222222206", nome: "Grace Hooper", nasc: "1996-12-10", email: ["hooper@linguagens.com"], tel: null, login: "grace", senha: "s5", mat: "E106", mc: 7.7, ano: 2022 },
    { cpf: "22222222207", nome: "Charles Babbage", nasc: "1971-12-26", email: null, tel: null, login: "charles", senha: "s6", mat: "E107", mc: 5.5, ano: 2022 },
    { cpf: "22222222208", nome: "Musa al-Khwarizmi", nasc: "1950-12-26", email: null, tel: null, login: "musa", senha: "s7", mat: "E108", mc: 6.5, ano: 2023 },
    { cpf: "22222222209", nome: "Cesar Lattes", nasc: "1924-06-11", email: ["cesar@cnpq.com", "lattes@curriculo.com"], tel: null, login: "lattes", senha: "s8", mat: "E109", mc: 6.0, ano: 2023 },
    { cpf: "22222222210", nome: "Donald Knuth", nasc: "1938-01-10", email: ["knuth@algorithms.com"], tel: null, login: "knuth", senha: "s9", mat: "E110", mc: 2.1, ano: 2023 },
    { cpf: "22222222211", nome: "Abraham Silberschatz", nasc: "1956-01-10", email: ["silberchatz@sgbd.com"], tel: null, login: "abraham", senha: "s10", mat: "E111", mc: 3.3, ano: 2023 },
    { cpf: "22222222212", nome: "Elmasri Navathe", nasc: "1944-03-24", email: null, tel: null, login: "elmasri", senha: "s11", mat: "E112", mc: 4.5, ano: 2024 },
    { cpf: "22222222213", nome: "Ramakrishnam Raghu", nasc: "1965-08-22", email: null, tel: null, login: "raghu", senha: "s12", mat: "E113", mc: 8.1, ano: 2024 }
];

// universidade.plano: agrupado por mat_estudante e embutido em estudantes[].planos
const planosPorEstudante = {
    E103: [{ id_projeto: 1, mat_professor: "P100", ano: 2018 }],
    E105: [{ id_projeto: 2, mat_professor: "P200", ano: 2018 }],
    E106: [{ id_projeto: 3, mat_professor: "P300", ano: 2018 }],
    E107: [{ id_projeto: 4, mat_professor: "P500", ano: 2018 }],
    E108: [{ id_projeto: 5, mat_professor: "P600", ano: 2018 }],
    E109: [{ id_projeto: 1, mat_professor: "P100", ano: 2018 }]
};

estudantes.forEach(e => {
    db.usuarios.insertOne({
        _id: e.cpf, nome: e.nome, data_nascimento: data(e.nasc),
        email: e.email, telefone: e.tel, login: e.login, senha: e.senha
    });
    db.estudantes.insertOne({
        _id: e.mat, cpf: e.cpf, mc: e.mc, ano_ingresso: e.ano,
        planos: planosPorEstudante[e.mat] ?? []
    });
});

// ---------------------------------------------------------------------------
// projetos
// ---------------------------------------------------------------------------
db.projetos.insertMany([1, 2, 3, 4, 5].map(n => ({ _id: n, descricao: "Projeto " + n })));

// ---------------------------------------------------------------------------
// cursos (via contador, como o app faria)
// ---------------------------------------------------------------------------
const cursosSeed = [
    { nome: "Ciência da Computação", grau: "Bacharelado", turno: "Vespertino", campus: "São Cristóvão", nivel: "Graduação" },
    { nome: "Sistemas de Informação", grau: "Bacharelado", turno: "Noturno", campus: "São Cristóvão", nivel: "Graduação" },
    { nome: "Sistemas de Informação", grau: "Bacharelado", turno: "Matutino", campus: "Itabaiana", nivel: "Graduação" },
    { nome: "Engenharia de Computação", grau: "Bacharelado", turno: "Vespertino", campus: "São Cristóvão", nivel: "Graduação" },
    { nome: "Inteligência Artificial", grau: "Bacharelado", turno: "Vespertino", campus: "São Cristóvão", nivel: "Graduação" }
];
// os ids batem com universidade.sql (1 a 5) pra manter as referências de "vinculo" simples
cursosSeed.forEach((c, i) => db.cursos.insertOne({ _id: i + 1, ...c }));
db.contadores.insertOne({ _id: "curso", valor: cursosSeed.length });

// ---------------------------------------------------------------------------
// vinculos
// ---------------------------------------------------------------------------
const vinculos = [
    [1, "E101", 3], [2, "E102", 2], [3, "E103", 1], [4, "E104", 4], [5, "E105", 1],
    [6, "E106", 1], [7, "E107", 1], [8, "E108", 5], [9, "E109", 1], [10, "E110", 5],
    [11, "E111", 4], [12, "E112", 2], [13, "E113", 2]
];
db.vinculos.insertMany(vinculos.map(([id, mat, curso]) => ({
    _id: id, mat_estudante: mat, curso, data_entrada: null, status: "Ativo", data_saida: null
})));
db.contadores.insertOne({ _id: "vinculo", valor: vinculos.length });

// ---------------------------------------------------------------------------
// disciplinas
// ---------------------------------------------------------------------------
db.disciplinas.insertMany([
    { _id: "COMP0196", nome: "Fundamentos da Computação", pre_req: null, creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0197", nome: "Programação Imperativa", pre_req: null, creditos: 6, depto_responsavel: "DCOMP" },
    { _id: "COMP0198", nome: "Programação Orientada à Objetos", pre_req: "COMP0197", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0199", nome: "Programação Declarativa", pre_req: "COMP0197", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0212", nome: "Estrutura de Dados I", pre_req: "COMP0197", creditos: 6, depto_responsavel: "DCOMP" },
    { _id: "COMP0213", nome: "Estrutura de Dados II", pre_req: "COMP0212", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0222", nome: "Inteligência Artificial", pre_req: "COMP0199", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0233", nome: "Lógica para Computação", pre_req: null, creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0279", nome: "Desenvolvimento de Software I", pre_req: "COMP0197", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0280", nome: "Desenvolvimento de Software II", pre_req: "COMP0279", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0281", nome: "Desenvolvimento de Software III", pre_req: "COMP0280", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0298", nome: "Redes de Computadores", pre_req: null, creditos: 4, depto_responsavel: null },
    { _id: "COMP0311", nome: "Banco de dados", pre_req: "COMP0213", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "COMP0326", nome: "Sistemas Distribuídos", pre_req: "COMP0298", creditos: 4, depto_responsavel: "DCOMP" },
    { _id: "MAT0064", nome: "Cálculo I", pre_req: null, creditos: 6, depto_responsavel: "DMA" },
    { _id: "MAT0065", nome: "Cálculo II", pre_req: null, creditos: 6, depto_responsavel: "DMA" },
    { _id: "MAT0096", nome: "Cálculo Numérico", pre_req: null, creditos: 4, depto_responsavel: "DMA" },
    { _id: "FISI0050", nome: "Física A", pre_req: null, creditos: 4, depto_responsavel: "DFI" },
    { _id: "FISI0051", nome: "Física B", pre_req: "FISI0050", creditos: 4, depto_responsavel: "DFI" }
]);

// ---------------------------------------------------------------------------
// semestres
// ---------------------------------------------------------------------------
db.semestres.insertMany([
    { _id: { ano: 2017, semestre: 2 }, data_inicio: null, data_fim: null },
    { _id: { ano: 2019, semestre: 1 }, data_inicio: null, data_fim: null }
]);

// ---------------------------------------------------------------------------
// turmas (+ universidade.leciona embutido em professores[])
// salas/horarios/alocacao não têm dados em universidade.sql, então alocacoes fica vazio.
// ---------------------------------------------------------------------------
const turmas = [
    [1, "COMP0212", 1, 2017, 2], [2, "COMP0213", 1, 2017, 2], [3, "COMP0311", 1, 2017, 2],
    [4, "COMP0198", 1, 2017, 2], [5, "COMP0199", 1, 2017, 2], [6, "COMP0233", 1, 2017, 2],
    [7, "COMP0196", 1, 2017, 2], [8, "COMP0197", 1, 2017, 2], [9, "COMP0279", 1, 2017, 2],
    [10, "COMP0280", 1, 2017, 2], [11, "COMP0281", 1, 2017, 2], [12, "COMP0298", 1, 2017, 2],
    [13, "COMP0326", 2, 2017, 2], [14, "MAT0096", 1, 2017, 2], [15, "MAT0064", 1, 2017, 2],
    [16, "MAT0096", 2, 2017, 2], [17, "MAT0096", 3, 2017, 2], [18, "MAT0065", 1, 2017, 2],
    [19, "FISI0050", 1, 2017, 2], [20, "COMP0199", 2, 2017, 2], [21, "COMP0233", 2, 2017, 2],
    [22, "COMP0222", 1, 2017, 2], [23, "MAT0065", 1, 2019, 1], [24, "FISI0050", 1, 2019, 1],
    [25, "COMP0199", 2, 2019, 1], [26, "COMP0233", 2, 2019, 1], [27, "COMP0222", 1, 2019, 1]
];

const lecionaPorTurma = {
    1: ["P100"], 2: ["P100"], 3: ["P100"], 4: ["P200"], 5: ["P200"], 6: ["P300"],
    7: ["P300"], 8: ["P300"], 9: ["P400"], 10: ["P400"], 11: ["P400"], 12: ["P500"],
    13: ["P500"], 14: ["P600"], 15: ["P600"], 16: ["P900"], 17: ["P1300"], 18: ["P1200"],
    19: ["P700"], 20: ["P1500"], 21: ["P1500"], 22: ["P100"]
};

db.turmas.insertMany(turmas.map(([id, cod, numero, ano, semestre]) => ({
    _id: id, cod_disc: cod, numero, ano, semestre,
    professores: lecionaPorTurma[id] ?? [],
    alocacoes: []
})));

// ---------------------------------------------------------------------------
// cursa (universidade.cursa) — collection própria, não embutida (ver mapeamento)
// ---------------------------------------------------------------------------
const cursa = [
    ["E112", 7, 4.1], ["E102", 7, 10], ["E104", 7, 7], ["E109", 7, 8],
    ["E112", 8, 5.5], ["E102", 8, 10], ["E107", 8, 9], ["E108", 8, 8.5], ["E109", 8, 10],
    ["E104", 5, 8.7], ["E109", 5, null],
    ["E110", 20, 5.5], ["E111", 20, 10.0],
    ["E101", 2, 10.0], ["E107", 2, 10.0], ["E106", 2, 10.0], ["E104", 2, 10.0], ["E105", 2, 10.0],
    ["E101", 22, null], ["E111", 22, null], ["E106", 22, 9.5], ["E108", 22, 9.5], ["E109", 22, 9.5], ["E110", 22, 8.1],
    ["E112", 6, null], ["E102", 6, null], ["E104", 6, null], ["E106", 6, null], ["E107", 6, null],
    ["E108", 21, null], ["E109", 21, null], ["E111", 21, null],
    ["E106", 9, null], ["E110", 9, null], ["E111", 9, null], ["E108", 9, null],
    ["E108", 11, null], ["E107", 11, null], ["E110", 11, null],
    ["E101", 12, 9.5], ["E111", 12, 7.6], ["E110", 12, 7.7],
    ["E106", 3, 9.0], ["E110", 3, null], ["E101", 3, 7.0], ["E102", 3, 2.5], ["E107", 3, 3.2],
    ["E101", 13, 8.0], ["E111", 13, 8.0], ["E109", 13, 4.0],
    ["E101", 15, 4.0], ["E111", 15, 3.5], ["E107", 15, 2.0], ["E102", 15, 8.0], ["E112", 15, 6.0],
    ["E106", 18, 6.0], ["E111", 18, 7.5], ["E109", 18, 6.0], ["E110", 18, 9.0],
    ["E102", 14, 7.5], ["E104", 14, 6.0],
    ["E106", 16, 10.0], ["E107", 16, 0.0], ["E108", 16, 9.0],
    ["E110", 17, 8.5],
    ["E101", 23, 8.0], ["E105", 23, 8.0], ["E109", 23, 4.0],
    ["E101", 24, 4.0], ["E110", 24, 3.5], ["E107", 24, 2.0], ["E102", 24, 8.0], ["E111", 24, 6.0],
    ["E106", 25, 6.0], ["E111", 25, 7.5], ["E109", 25, 6.0], ["E112", 25, 9.0]
];
db.cursa.insertMany(cursa.map(([mat, turma, nota]) => ({ mat_estudante: mat, id_turma: turma, nota })));

print("Seed concluído: " +
    db.usuarios.countDocuments() + " usuarios, " +
    db.professores.countDocuments() + " professores, " +
    db.estudantes.countDocuments() + " estudantes, " +
    db.cursos.countDocuments() + " cursos, " +
    db.vinculos.countDocuments() + " vinculos, " +
    db.turmas.countDocuments() + " turmas, " +
    db.cursa.countDocuments() + " cursa.");
