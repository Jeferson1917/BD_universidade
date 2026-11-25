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
            RegistrarErro(registro, ex);
        }
        return registro;
    }

    /// <summary>
    /// Executa um comando com RETURNING (ex.: INSERT que devolve o id gerado pela sequência).
    /// </summary>
    public async Task<(ComandoExecutado Comando, T? Retorno)> ExecutarComRetornoAsync<T>(string sql, params NpgsqlParameter[] parametros)
    {
        var registro = ComandoExecutado.De(sql, parametros);
        T? retorno = default;
        try
        {
            await using var cmd = dataSource.CreateCommand(sql);
            cmd.Parameters.AddRange(parametros);
            var valor = await cmd.ExecuteScalarAsync();
            if (valor is not null and not DBNull)
            {
                var tipo = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                retorno = (T)Convert.ChangeType(valor, tipo);
                registro.LinhasAfetadas = 1;
            }
        }
        catch (PostgresException ex)
        {
            RegistrarErro(registro, ex);
        }
        return (registro, retorno);
    }

    private static void RegistrarErro(ComandoExecutado registro, PostgresException ex)
    {
        registro.Erro = ErroBanco.Traduzir(ex);
        registro.CodigoErro = ex.SqlState;
        registro.ErroOriginal = $"{ex.SqlState}: {ex.MessageText}";
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
