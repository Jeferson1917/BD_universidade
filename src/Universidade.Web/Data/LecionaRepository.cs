using Npgsql;
using NpgsqlTypes;
using Universidade.Web.Models;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>
/// CRUD da tabela universidade.leciona.
/// A chave primária é composta (id_turma, mat_professor) e não há outras colunas:
/// "atualizar" significa trocar a turma e/ou o professor da associação.
/// </summary>
public class LecionaRepository(Db db)
{
    public const string SqlListar = """
        SELECT l.id_turma, l.mat_professor, t.cod_disc, d.nome, t.numero,
               t.ano || '/' || t.semestre AS semestre, u.nome
        FROM universidade.leciona l
        JOIN universidade.turma t ON t.id_turma = l.id_turma
        JOIN universidade.disciplina d ON d.cod_disc = t.cod_disc
        JOIN universidade.professor p ON p.mat_professor = l.mat_professor
        LEFT JOIN universidade.usuario u ON u.cpf = p.cpf
        WHERE @busca IS NULL
           OR l.mat_professor ILIKE @busca
           OR u.nome ILIKE @busca
           OR t.cod_disc ILIKE @busca
           OR d.nome ILIKE @busca
        ORDER BY l.id_turma, l.mat_professor
        """;

    private const string SqlBuscar = """
        SELECT l.id_turma, l.mat_professor, t.cod_disc, d.nome, t.numero,
               t.ano || '/' || t.semestre AS semestre, u.nome
        FROM universidade.leciona l
        JOIN universidade.turma t ON t.id_turma = l.id_turma
        JOIN universidade.disciplina d ON d.cod_disc = t.cod_disc
        JOIN universidade.professor p ON p.mat_professor = l.mat_professor
        LEFT JOIN universidade.usuario u ON u.cpf = p.cpf
        WHERE l.id_turma = @id_turma AND l.mat_professor = @mat_professor
        """;

    private const string SqlInserir = """
        INSERT INTO universidade.leciona (id_turma, mat_professor)
        VALUES (@id_turma, @mat_professor)
        """;

    private const string SqlAtualizar = """
        UPDATE universidade.leciona
        SET id_turma = @novo_id_turma,
            mat_professor = @novo_mat_professor
        WHERE id_turma = @id_turma AND mat_professor = @mat_professor
        """;

    private const string SqlExcluir = """
        DELETE FROM universidade.leciona
        WHERE id_turma = @id_turma AND mat_professor = @mat_professor
        """;

    public Task<List<Leciona>> ListarAsync(string? busca) =>
        db.ConsultarAsync(SqlListar, Mapear,
            P("busca", string.IsNullOrWhiteSpace(busca) ? null : $"%{busca.Trim()}%", NpgsqlDbType.Text));

    public async Task<Leciona?> BuscarAsync(int idTurma, string mat) =>
        (await db.ConsultarAsync(SqlBuscar, Mapear, Chave("", idTurma, mat))).FirstOrDefault();

    public Task<ComandoExecutado> InserirAsync(int idTurma, string mat) =>
        db.ExecutarAsync(SqlInserir, Chave("", idTurma, mat));

    public Task<ComandoExecutado> AtualizarAsync(int idTurma, string mat, int novoIdTurma, string novoMat) =>
        db.ExecutarAsync(SqlAtualizar, [.. Chave("novo_", novoIdTurma, novoMat), .. Chave("", idTurma, mat)]);

    public Task<ComandoExecutado> ExcluirAsync(int idTurma, string mat) =>
        db.ExecutarAsync(SqlExcluir, Chave("", idTurma, mat));

    private static NpgsqlParameter[] Chave(string prefixo, int idTurma, string mat) =>
    [
        P(prefixo + "id_turma", idTurma, NpgsqlDbType.Integer),
        P(prefixo + "mat_professor", mat, NpgsqlDbType.Varchar)
    ];

    private static Leciona Mapear(NpgsqlDataReader r) => new()
    {
        IdTurma = r.GetInt32(0),
        MatProfessor = r.GetString(1),
        CodDisc = r.GetString(2),
        NomeDisciplina = r.IsDBNull(3) ? null : r.GetString(3),
        Numero = r.IsDBNull(4) ? null : r.GetInt32(4),
        SemestreTexto = r.IsDBNull(5) ? null : r.GetString(5),
        NomeProfessor = r.IsDBNull(6) ? null : r.GetString(6)
    };
}
