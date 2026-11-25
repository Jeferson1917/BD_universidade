using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Npgsql;

namespace Universidade.Web.Data;

/// <summary>
/// Registro de um comando SQL enviado ao banco, para exibir na tela
/// o que foi executado e qual foi o efeito.
/// </summary>
public class ComandoExecutado
{
    public string Sql { get; set; } = "";
    public List<ParametroExibido> Parametros { get; set; } = [];
    public int LinhasAfetadas { get; set; }
    public string? Erro { get; set; }
    public string? CodigoErro { get; set; }
    public string? ErroOriginal { get; set; }
    public string? Observacao { get; set; }
    public DateTime Momento { get; set; } = DateTime.Now;

    public bool Sucesso => Erro is null;

    public static ComandoExecutado De(string sql, IEnumerable<NpgsqlParameter> parametros) => new()
    {
        Sql = sql.Trim(),
        Parametros = parametros
            .Select(p => new ParametroExibido("@" + p.ParameterName.TrimStart('@'), Formatar(p.Value)))
            .ToList()
    };

    private static string Formatar(object? valor) => valor switch
    {
        null or DBNull => "NULL",
        string s => $"'{s}'",
        string[] arr => "{" + string.Join(", ", arr.Select(x => $"\"{x}\"")) + "}",
        DateTime d => $"'{d:yyyy-MM-dd}'",
        _ => Convert.ToString(valor, System.Globalization.CultureInfo.InvariantCulture) ?? "NULL"
    };
}

public record ParametroExibido(string Nome, string Valor);

public static class TempDataComandoExtensions
{
    private const string Chave = "ComandoExecutado";

    public static void GuardarComando(this ITempDataDictionary tempData, ComandoExecutado comando) =>
        tempData[Chave] = JsonSerializer.Serialize(comando);

    public static ComandoExecutado? LerComando(this ITempDataDictionary tempData) =>
        tempData[Chave] is string json ? JsonSerializer.Deserialize<ComandoExecutado>(json) : null;
}
