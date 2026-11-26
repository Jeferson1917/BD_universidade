using MongoDB.Bson;
using MongoDB.Driver;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Data;

/// <summary>CRUD da collection "estudantes".</summary>
public class EstudanteRepository(MongoDb db)
{
    public async Task<List<EstudanteDoc>> ListarAsync(string? busca)
    {
        var filtro = string.IsNullOrWhiteSpace(busca)
            ? Builders<EstudanteDoc>.Filter.Empty
            : Builders<EstudanteDoc>.Filter.Or(
                Builders<EstudanteDoc>.Filter.Regex(e => e.MatEstudante, new BsonRegularExpression(busca, "i")),
                Builders<EstudanteDoc>.Filter.Regex(e => e.Cpf, new BsonRegularExpression(busca, "i")));

        return await db.Estudantes.Find(filtro).SortBy(e => e.MatEstudante).ToListAsync();
    }

    public async Task<EstudanteDoc?> BuscarAsync(string mat) =>
        await db.Estudantes.Find(e => e.MatEstudante == mat).FirstOrDefaultAsync();

    public async Task<ComandoMongo> InserirAsync(EstudanteDoc e)
    {
        var comando = ComandoMongo.De("estudantes", "insertOne", documento: e);
        try
        {
            await db.Estudantes.InsertOneAsync(e);
            comando.LinhasAfetadas = 1;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = "Já existe um estudante com essa matrícula.";
        }
        return comando;
    }

    public async Task<ComandoMongo> AtualizarAsync(string matOriginal, EstudanteDoc e)
    {
        if (e.MatEstudante != matOriginal)
            return await TrocarChaveAsync(matOriginal, e);

        var filtro = Builders<EstudanteDoc>.Filter.Eq(x => x.MatEstudante, matOriginal);
        var comando = ComandoMongo.De("estudantes", "replaceOne", new { _id = matOriginal }, e);
        var resultado = await db.Estudantes.ReplaceOneAsync(filtro, e);
        comando.LinhasAfetadas = resultado.ModifiedCount;
        return comando;
    }

    private async Task<ComandoMongo> TrocarChaveAsync(string matOriginal, EstudanteDoc novo)
    {
        var comando = ComandoMongo.De("estudantes", "deleteOne + insertOne (troca de _id)",
            new { _id = matOriginal }, novo);
        try
        {
            await db.Estudantes.InsertOneAsync(novo);
            var exclusao = await db.Estudantes.DeleteOneAsync(e => e.MatEstudante == matOriginal);
            comando.LinhasAfetadas = exclusao.DeletedCount;
            comando.Observacao =
                "O _id (matrícula) não pode ser alterado com um replaceOne: o documento foi reinserido com a " +
                "nova chave. Isso NÃO propaga para vínculos que referenciam a matrícula antiga.";
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = "Já existe um estudante com essa nova matrícula.";
        }
        return comando;
    }

    public async Task<ComandoMongo> ExcluirAsync(string mat)
    {
        var comando = ComandoMongo.De("estudantes", "deleteOne", new { _id = mat });
        var resultado = await db.Estudantes.DeleteOneAsync(e => e.MatEstudante == mat);
        comando.LinhasAfetadas = resultado.DeletedCount;
        return comando;
    }

    /// <summary>Quantidade de vínculos ligados a essa matrícula (para avisar o efeito da exclusão).</summary>
    public Task<long> ContarVinculosAsync(string mat) =>
        db.Vinculos.CountDocumentsAsync(v => v.MatEstudante == mat);
}
