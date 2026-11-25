using Npgsql;
using NpgsqlTypes;
using Universidade.Web.Models;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>CRUD da tabela universidade.turma.</summary>
public class TurmaRepository(Db db)
{
    public const string SqlListar = """
        SELECT t.id_turma, t.cod_disc, d.nome, t.numero, t.ano, t.semestre,
               (SELECT string_agg(l.mat_professor, ', ' ORDER BY l.mat_professor)
                  FROM universidade.leciona l WHERE l.id_turma = t.id_turma) AS professores,
               (SELECT count(*) FROM universidade.cursa c WHERE c.id_turma = t.id_turma) AS alunos
        FROM universidade.turma t
        JOIN universidade.disciplina d ON d.cod_disc = t.cod_disc
        WHERE @busca IS NULL
           OR t.cod_disc ILIKE @busca
           OR d.nome ILIKE @busca
           OR t.id_turma::text LIKE @busca
        ORDER BY t.id_turma
        """;

    private const string SqlBuscar = """
        SELECT t.id_turma, t.cod_disc, d.nome, t.numero, t.ano, t.semestre,
               (SELECT string_agg(l.mat_professor, ', ' ORDER BY l.mat_professor)
                  FROM universidade.leciona l WHERE l.id_turma = t.id_turma) AS professores,
               (SELECT count(*) FROM universidade.cursa c WHERE c.id_turma = t.id_turma) AS alunos
        FROM universidade.turma t
        JOIN universidade.disciplina d ON d.cod_disc = t.cod_disc
        WHERE t.id_turma = @id_turma
        """;

    // id_turma não é informado: o DEFAULT nextval('universidade.seq_turma') gera o valor
    private const string SqlInserir = """
        INSERT INTO universidade.turma (cod_disc, numero, ano, semestre)
        VALUES (@cod_disc, @numero, @ano, @semestre)
        RETURNING id_turma
        """;

    private const string SqlAtualizar = """
        UPDATE universidade.turma
        SET cod_disc = @cod_disc,
            numero = @numero,
            ano = @ano,
            semestre = @semestre
        WHERE id_turma = @id_turma
        """;

    private const string SqlExcluir = """
        DELETE FROM universidade.turma
        WHERE id_turma = @id_turma
        """;

    private const string SqlDependentes = """
        SELECT (SELECT count(*) FROM universidade.leciona WHERE id_turma = @id_turma),
               (SELECT count(*) FROM universidade.cursa WHERE id_turma = @id_turma)
        """;

    public Task<List<Turma>> ListarAsync(string? busca) =>
        db.ConsultarAsync(SqlListar, Mapear,
            P("busca", string.IsNullOrWhiteSpace(busca) ? null : $"%{busca.Trim()}%", NpgsqlDbType.Text));

    public async Task<Turma?> BuscarAsync(int id) =>
        (await db.ConsultarAsync(SqlBuscar, Mapear, PId(id))).FirstOrDefault();

    /// <summary>Insere e devolve o id_turma gerado pela sequência.</summary>
    public Task<(ComandoExecutado Comando, int? Id)> InserirAsync(Turma t) =>
        db.ExecutarComRetornoAsync<int?>(SqlInserir, ParametrosDados(t));

    public Task<ComandoExecutado> AtualizarAsync(Turma t) =>
        db.ExecutarAsync(SqlAtualizar, [.. ParametrosDados(t), PId(t.IdTurma)]);

    public Task<ComandoExecutado> ExcluirAsync(int id) =>
        db.ExecutarAsync(SqlExcluir, PId(id));

    public async Task<TurmaDependentes> DependentesAsync(int id) =>
        (await db.ConsultarAsync(SqlDependentes,
            r => new TurmaDependentes(r.GetInt64(0), r.GetInt64(1)), PId(id))).First();

    private static NpgsqlParameter[] ParametrosDados(Turma t) =>
    [
        P("cod_disc", t.CodDisc, NpgsqlDbType.Varchar),
        P("numero", t.Numero, NpgsqlDbType.Integer),
        P("ano", t.Ano, NpgsqlDbType.Smallint),
        P("semestre", t.Semestre, NpgsqlDbType.Smallint)
    ];

    private static NpgsqlParameter PId(int id) => P("id_turma", id, NpgsqlDbType.Integer);

    private static Turma Mapear(NpgsqlDataReader r) => new()
    {
        IdTurma = r.GetInt32(0),
        CodDisc = r.GetString(1),
        NomeDisciplina = r.IsDBNull(2) ? null : r.GetString(2),
        Numero = r.IsDBNull(3) ? null : r.GetInt32(3),
        Ano = r.IsDBNull(4) ? null : r.GetInt16(4),
        Semestre = r.IsDBNull(5) ? null : r.GetInt16(5),
        Professores = r.IsDBNull(6) ? null : r.GetString(6),
        Alunos = r.GetInt64(7)
    };
}
