using Npgsql;
using NpgsqlTypes;

namespace Universidade.Web.Data;

/// <summary>
/// Acesso ao PostgreSQL com SQL puro e parâmetros.
/// </summary>
public class Db(NpgsqlDataSource dataSource)
{
    /// <summary>
    /// Executa INSERT/UPDATE/DELETE e devolve o registro do que foi executado.
    /// Erros do banco não estouram exceção: voltam em <see cref="ComandoExecutado.Erro"/>.
    /// </summary>
    public async Task<ComandoExecutado> ExecutarAsync(string sql, params NpgsqlParameter[] parametros)
    {
        var registro = ComandoExecutado.De(sql, parametros);
        try
        {
            await using var cmd = dataSource.CreateCommand(sql);
            cmd.Parameters.AddRange(parametros);
            registro.LinhasAfetadas = await cmd.ExecuteNonQueryAsync();
        }
        catch (PostgresException ex)
        {
            registro.Erro = ErroBanco.Traduzir(ex);
            registro.ErroOriginal = $"{ex.SqlState}: {ex.MessageText}";
        }
        return registro;
    }

    /// <summary>Executa um SELECT e converte cada linha com <paramref name="mapear"/>.</summary>
    public async Task<List<T>> ConsultarAsync<T>(string sql, Func<NpgsqlDataReader, T> mapear, params NpgsqlParameter[] parametros)
    {
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddRange(parametros);
        await using var reader = await cmd.ExecuteReaderAsync();

        var resultado = new List<T>();
        while (await reader.ReadAsync())
            resultado.Add(mapear(reader));
        return resultado;
    }

    /// <summary>Cria um parâmetro; null vira DBNull.</summary>
    public static NpgsqlParameter P(string nome, object? valor, NpgsqlDbType? tipo = null)
    {
        var p = new NpgsqlParameter(nome, valor ?? DBNull.Value);
        if (tipo is { } t) p.NpgsqlDbType = t;
        return p;
    }
}
