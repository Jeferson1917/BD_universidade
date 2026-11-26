using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Universidade.Mongo.Data;

/// <summary>
/// Registro de uma operação enviada ao MongoDB, para exibir na tela
/// o que foi executado (collection, operação, filtro/documento) e qual foi o efeito.
/// </summary>
public class ComandoMongo
{
    public string Colecao { get; set; } = "";
    public string Operacao { get; set; } = "";
    public string? Filtro { get; set; }
    public string? Documento { get; set; }
    public long LinhasAfetadas { get; set; }
    public string? Erro { get; set; }
    public string? Observacao { get; set; }
    public DateTime Momento { get; set; } = DateTime.Now;

    public bool Sucesso => Erro is null;

    public static ComandoMongo De(string colecao, string operacao, object? filtro = null, object? documento = null) => new()
    {
        Colecao = colecao,
        Operacao = operacao,
        Filtro = filtro is null ? null : ParaJson(filtro),
        Documento = documento is null ? null : ParaJson(documento)
    };

    // valor.ToJson() usaria o ObjectSerializer polimórfico (tipo estático "object"), que o driver
    // bloqueia por segurança sem uma lista de tipos permitidos. Serializar pelo tipo concreto evita isso.
    private static string ParaJson(object valor) => valor switch
    {
        BsonDocument doc => doc.ToJson(),
        _ => valor.ToBsonDocument(valor.GetType()).ToJson()
    };
}

public static class TempDataComandoMongoExtensions
{
    private const string Chave = "ComandoMongo";

    public static void GuardarComando(this ITempDataDictionary tempData, ComandoMongo comando) =>
        tempData[Chave] = JsonSerializer.Serialize(comando);

    public static ComandoMongo? LerComando(this ITempDataDictionary tempData) =>
        tempData[Chave] is string json ? JsonSerializer.Deserialize<ComandoMongo>(json) : null;
}
