using MongoDB.Bson;
using MongoDB.Driver;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Data;

/// <summary>Acesso às collections do banco "universidade" no MongoDB.</summary>
public class MongoDb(IMongoDatabase database)
{
    public IMongoCollection<UsuarioDoc> Usuarios => database.GetCollection<UsuarioDoc>("usuarios");
    public IMongoCollection<EstudanteDoc> Estudantes => database.GetCollection<EstudanteDoc>("estudantes");
    public IMongoCollection<CursoDoc> Cursos => database.GetCollection<CursoDoc>("cursos");
    public IMongoCollection<VinculoDoc> Vinculos => database.GetCollection<VinculoDoc>("vinculos");

    /// <summary>
    /// Collection "contadores": substitui as SEQUENCEs do Postgres (ex.: idCurso, idVinculo),
    /// já que o MongoDB não tem autoincremento nativo. Um documento por sequência, com $inc atômico.
    /// </summary>
    private IMongoCollection<BsonDocument> Contadores => database.GetCollection<BsonDocument>("contadores");

    public async Task<int> ProximoIdAsync(string sequencia)
    {
        var filtro = Builders<BsonDocument>.Filter.Eq("_id", sequencia);
        var atualizacao = Builders<BsonDocument>.Update.Inc("valor", 1);
        var opcoes = new FindOneAndUpdateOptions<BsonDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };
        var resultado = await Contadores.FindOneAndUpdateAsync(filtro, atualizacao, opcoes);
        return resultado["valor"].AsInt32;
    }
}
