using Npgsql;
using NpgsqlTypes;
using Universidade.Web.Models;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>CRUD da tabela universidade.semestre (chave primária composta ano + semestre).</summary>
public class SemestreRepository(Db db)
{
    public const string SqlListar = """
        SELECT s.ano, s.semestre, s.data_inicio, s.data_fom,
               (SELECT count(*) FROM universidade.turma t
                 WHERE t.ano = s.ano AND t.semestre = s.semestre) AS turmas
        FROM universidade.semestre s
        ORDER BY s.ano, s.semestre
        """;

    private const string SqlBuscar = """
        SELECT s.ano, s.semestre, s.data_inicio, s.data_fom,
               (SELECT count(*) FROM universidade.turma t
                 WHERE t.ano = s.ano AND t.semestre = s.semestre) AS turmas
        FROM universidade.semestre s
        WHERE s.ano = @ano AND s.semestre = @semestre
        """;

    private const string SqlInserir = """
        INSERT INTO universidade.semestre (ano, semestre, data_inicio, data_fom)
        VALUES (@ano, @semestre, @data_inicio, @data_fom)
        """;

    private const string SqlAtualizar = """
        UPDATE universidade.semestre
        SET ano = @novo_ano,
            semestre = @novo_semestre,
            data_inicio = @data_inicio,
            data_fom = @data_fom
        WHERE ano = @ano AND semestre = @semestre
        """;

    private const string SqlExcluir = """
        DELETE FROM universidade.semestre
        WHERE ano = @ano AND semestre = @semestre
        """;

    public Task<List<Semestre>> ListarAsync() => db.ConsultarAsync(SqlListar, Mapear);

    public async Task<Semestre?> BuscarAsync(short ano, short semestre) =>
        (await db.ConsultarAsync(SqlBuscar, Mapear, Chave("", ano, semestre))).FirstOrDefault();

    public Task<ComandoExecutado> InserirAsync(Semestre s) =>
        db.ExecutarAsync(SqlInserir, [.. Chave("", s.Ano, s.Periodo), .. Datas(s)]);

    public Task<ComandoExecutado> AtualizarAsync(short ano, short semestre, Semestre s) =>
        db.ExecutarAsync(SqlAtualizar, [.. Chave("novo_", s.Ano, s.Periodo), .. Datas(s), .. Chave("", ano, semestre)]);

    public Task<ComandoExecutado> ExcluirAsync(short ano, short semestre) =>
        db.ExecutarAsync(SqlExcluir, Chave("", ano, semestre));

    private static NpgsqlParameter[] Chave(string prefixo, short ano, short semestre) =>
    [
        P(prefixo + "ano", ano, NpgsqlDbType.Smallint),
        P(prefixo + "semestre", semestre, NpgsqlDbType.Smallint)
    ];

    private static NpgsqlParameter[] Datas(Semestre s) =>
    [
        P("data_inicio", s.DataInicio, NpgsqlDbType.Date),
        P("data_fom", s.DataFim, NpgsqlDbType.Date)
    ];

    private static Semestre Mapear(NpgsqlDataReader r) => new()
    {
        Ano = r.GetInt16(0),
        Periodo = r.GetInt16(1),
        DataInicio = r.IsDBNull(2) ? null : r.GetDateTime(2),
        DataFim = r.IsDBNull(3) ? null : r.GetDateTime(3),
        Turmas = r.GetInt64(4)
    };
}
