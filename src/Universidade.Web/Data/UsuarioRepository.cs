using Npgsql;
using NpgsqlTypes;
using Universidade.Web.Models;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>CRUD da tabela universidade.usuario.</summary>
public class UsuarioRepository(Db db)
{
    public const string SqlListar = """
        SELECT cpf, nome, data_nascimento, email, telefone, login, senha
        FROM universidade.usuario
        WHERE @busca IS NULL
           OR nome ILIKE @busca
           OR login ILIKE @busca
           OR cpf::text LIKE @busca
        ORDER BY nome, cpf
        """;

    private const string SqlBuscarPorCpf = """
        SELECT cpf, nome, data_nascimento, email, telefone, login, senha
        FROM universidade.usuario
        WHERE cpf = @cpf
        """;

    private const string SqlInserir = """
        INSERT INTO universidade.usuario (cpf, nome, data_nascimento, email, telefone, login, senha)
        VALUES (@cpf, @nome, @data_nascimento, @email, @telefone, @login, @senha)
        """;

    // senha = COALESCE(@senha, senha): se a senha vier em branco, mantém a atual
    private const string SqlAtualizar = """
        UPDATE universidade.usuario
        SET cpf = @cpf,
            nome = @nome,
            data_nascimento = @data_nascimento,
            email = @email,
            telefone = @telefone,
            login = @login,
            senha = COALESCE(@senha, senha)
        WHERE cpf = @cpf_original
        """;

    private const string SqlExcluir = """
        DELETE FROM universidade.usuario
        WHERE cpf = @cpf
        """;

    private const string SqlDependentes = """
        SELECT (SELECT mat_professor FROM universidade.professor WHERE cpf = @cpf) AS mat_professor,
               (SELECT mat_estudante FROM universidade.estudante WHERE cpf = @cpf) AS mat_estudante
        """;

    public Task<List<Usuario>> ListarAsync(string? busca) =>
        db.ConsultarAsync(SqlListar, Mapear,
            P("busca", string.IsNullOrWhiteSpace(busca) ? null : $"%{busca.Trim()}%", NpgsqlDbType.Text));

    public async Task<Usuario?> BuscarAsync(long cpf) =>
        (await db.ConsultarAsync(SqlBuscarPorCpf, Mapear, PCpf("cpf", cpf))).FirstOrDefault();

    public Task<ComandoExecutado> InserirAsync(Usuario u) =>
        db.ExecutarAsync(SqlInserir, ParametrosDados(u));

    public Task<ComandoExecutado> AtualizarAsync(long cpfOriginal, Usuario u) =>
        db.ExecutarAsync(SqlAtualizar, [.. ParametrosDados(u), PCpf("cpf_original", cpfOriginal)]);

    public Task<ComandoExecutado> ExcluirAsync(long cpf) =>
        db.ExecutarAsync(SqlExcluir, PCpf("cpf", cpf));

    /// <summary>Matrícula de professor/estudante ligados a esse CPF (para avisar efeitos das FKs).</summary>
    public async Task<(string? Professor, string? Estudante)> DependentesAsync(long cpf) =>
        (await db.ConsultarAsync(SqlDependentes,
            r => (r.IsDBNull(0) ? null : r.GetString(0), r.IsDBNull(1) ? null : r.GetString(1)),
            PCpf("cpf", cpf))).First();

    private static NpgsqlParameter[] ParametrosDados(Usuario u) =>
    [
        PCpf("cpf", u.Cpf),
        P("nome", u.Nome, NpgsqlDbType.Varchar),
        P("data_nascimento", u.DataNascimento, NpgsqlDbType.Date),
        P("email", u.Email, NpgsqlDbType.Array | NpgsqlDbType.Varchar),
        P("telefone", u.Telefone, NpgsqlDbType.Array | NpgsqlDbType.Varchar),
        P("login", u.Login, NpgsqlDbType.Varchar),
        P("senha", u.Senha, NpgsqlDbType.Varchar)
    ];

    // tipo_cpf é NUMERIC(13)
    private static NpgsqlParameter PCpf(string nome, long cpf) => P(nome, (decimal)cpf, NpgsqlDbType.Numeric);

    private static Usuario Mapear(NpgsqlDataReader r) => new()
    {
        Cpf = (long)r.GetDecimal(0),
        Nome = r.GetString(1),
        DataNascimento = r.IsDBNull(2) ? null : r.GetDateTime(2),
        Email = r.IsDBNull(3) ? null : r.GetFieldValue<string[]>(3),
        Telefone = r.IsDBNull(4) ? null : r.GetFieldValue<string[]>(4),
        Login = r.IsDBNull(5) ? null : r.GetString(5),
        Senha = r.IsDBNull(6) ? null : r.GetString(6)
    };
}
