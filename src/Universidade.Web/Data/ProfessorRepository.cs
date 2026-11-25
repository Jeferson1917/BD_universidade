using Npgsql;
using NpgsqlTypes;
using Universidade.Web.Models;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>CRUD da tabela universidade.professor.</summary>
public class ProfessorRepository(Db db)
{
    // ENUMs lidos como texto (::text); length() ordena P100, P200... antes de P1000
    public const string SqlListar = """
        SELECT p.mat_professor, p.cpf, u.nome, p.departamento, d.nome,
               p.formacao::text, p.data_admissao, p.tipo_jornada_trabalho::text, p.salario
        FROM universidade.professor p
        LEFT JOIN universidade.usuario u ON u.cpf = p.cpf
        LEFT JOIN universidade.departamento d ON d.cod_depto = p.departamento
        WHERE @busca IS NULL
           OR p.mat_professor ILIKE @busca
           OR u.nome ILIKE @busca
           OR p.departamento ILIKE @busca
        ORDER BY length(p.mat_professor), p.mat_professor
        """;

    private const string SqlBuscar = """
        SELECT p.mat_professor, p.cpf, u.nome, p.departamento, d.nome,
               p.formacao::text, p.data_admissao, p.tipo_jornada_trabalho::text, p.salario
        FROM universidade.professor p
        LEFT JOIN universidade.usuario u ON u.cpf = p.cpf
        LEFT JOIN universidade.departamento d ON d.cod_depto = p.departamento
        WHERE p.mat_professor = @mat_professor
        """;

    // Os ENUMs chegam como texto e são convertidos com cast explícito
    private const string SqlInserir = """
        INSERT INTO universidade.professor
            (mat_professor, cpf, departamento, formacao, data_admissao, tipo_jornada_trabalho, salario)
        VALUES
            (@mat_professor, @cpf, @departamento, @formacao::universidade.tipo_formacao,
             @data_admissao, @tipo_jornada_trabalho::universidade.tipo_jornada, @salario)
        """;

    private const string SqlAtualizar = """
        UPDATE universidade.professor
        SET mat_professor = @mat_professor,
            cpf = @cpf,
            departamento = @departamento,
            formacao = @formacao::universidade.tipo_formacao,
            data_admissao = @data_admissao,
            tipo_jornada_trabalho = @tipo_jornada_trabalho::universidade.tipo_jornada,
            salario = @salario
        WHERE mat_professor = @mat_original
        """;

    private const string SqlExcluir = """
        DELETE FROM universidade.professor
        WHERE mat_professor = @mat_professor
        """;

    private const string SqlDependentes = """
        SELECT (SELECT count(*) FROM universidade.leciona WHERE mat_professor = @mat_professor),
               (SELECT string_agg(cod_depto, ', ') FROM universidade.departamento WHERE chefe = @mat_professor),
               (SELECT count(*) FROM universidade.plano WHERE mat_professor = @mat_professor)
        """;

    public Task<List<Professor>> ListarAsync(string? busca) =>
        db.ConsultarAsync(SqlListar, Mapear,
            P("busca", string.IsNullOrWhiteSpace(busca) ? null : $"%{busca.Trim()}%", NpgsqlDbType.Text));

    public async Task<Professor?> BuscarAsync(string mat) =>
        (await db.ConsultarAsync(SqlBuscar, Mapear, PMat("mat_professor", mat))).FirstOrDefault();

    public Task<ComandoExecutado> InserirAsync(Professor p) =>
        db.ExecutarAsync(SqlInserir, ParametrosDados(p));

    public Task<ComandoExecutado> AtualizarAsync(string matOriginal, Professor p) =>
        db.ExecutarAsync(SqlAtualizar, [.. ParametrosDados(p), PMat("mat_original", matOriginal)]);

    public Task<ComandoExecutado> ExcluirAsync(string mat) =>
        db.ExecutarAsync(SqlExcluir, PMat("mat_professor", mat));

    public async Task<ProfessorDependentes> DependentesAsync(string mat) =>
        (await db.ConsultarAsync(SqlDependentes,
            r => new ProfessorDependentes(r.GetInt64(0), r.IsDBNull(1) ? null : r.GetString(1), r.GetInt64(2)),
            PMat("mat_professor", mat))).First();

    private static NpgsqlParameter[] ParametrosDados(Professor p) =>
    [
        PMat("mat_professor", p.MatProfessor),
        P("cpf", p.Cpf is null ? null : (decimal)p.Cpf, NpgsqlDbType.Numeric),
        P("departamento", p.Departamento, NpgsqlDbType.Varchar),
        P("formacao", p.Formacao, NpgsqlDbType.Text),
        P("data_admissao", p.DataAdmissao, NpgsqlDbType.Date),
        P("tipo_jornada_trabalho", p.TipoJornada, NpgsqlDbType.Text),
        P("salario", p.Salario, NpgsqlDbType.Double)
    ];

    private static NpgsqlParameter PMat(string nome, string mat) => P(nome, mat, NpgsqlDbType.Varchar);

    private static Professor Mapear(NpgsqlDataReader r) => new()
    {
        MatProfessor = r.GetString(0),
        Cpf = r.IsDBNull(1) ? null : (long)r.GetDecimal(1),
        NomeUsuario = r.IsDBNull(2) ? null : r.GetString(2),
        Departamento = r.IsDBNull(3) ? null : r.GetString(3),
        NomeDepartamento = r.IsDBNull(4) ? null : r.GetString(4),
        Formacao = r.IsDBNull(5) ? null : r.GetString(5),
        DataAdmissao = r.IsDBNull(6) ? null : r.GetDateTime(6),
        TipoJornada = r.IsDBNull(7) ? null : r.GetString(7),
        Salario = r.IsDBNull(8) ? null : r.GetDouble(8)
    };
}
