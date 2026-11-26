using MongoDB.Driver;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Data;

/// <summary>CRUD da collection "vinculos".</summary>
public class VinculoRepository(MongoDb db)
{
    public Task<List<VinculoDoc>> ListarAsync() =>
        db.Vinculos.Find(Builders<VinculoDoc>.Filter.Empty).SortBy(v => v.Id).ToListAsync();

    public async Task<VinculoDoc?> BuscarAsync(int id) =>
        await db.Vinculos.Find(v => v.Id == id).FirstOrDefaultAsync();

    public async Task<(ComandoMongo Comando, int Id)> InserirAsync(VinculoForm form)
    {
        var id = await db.ProximoIdAsync("vinculo");
        var vinculo = form.ParaDocumento(id);
        var comando = ComandoMongo.De("vinculos", "insertOne", documento: vinculo);
        await db.Vinculos.InsertOneAsync(vinculo);
        comando.LinhasAfetadas = 1;
        comando.Observacao = $"_id = {id}, gerado pela collection \"contadores\" (equivalente à SEQUENCE do Postgres).";
        return (comando, id);
    }

    public async Task<ComandoMongo> AtualizarAsync(VinculoDoc v)
    {
        var filtro = Builders<VinculoDoc>.Filter.Eq(x => x.Id, v.Id);
        var comando = ComandoMongo.De("vinculos", "replaceOne", new { _id = v.Id }, v);
        var resultado = await db.Vinculos.ReplaceOneAsync(filtro, v);
        comando.LinhasAfetadas = resultado.ModifiedCount;
        return comando;
    }

    public async Task<ComandoMongo> ExcluirAsync(int id)
    {
        var comando = ComandoMongo.De("vinculos", "deleteOne", new { _id = id });
        var resultado = await db.Vinculos.DeleteOneAsync(v => v.Id == id);
        comando.LinhasAfetadas = resultado.DeletedCount;
        return comando;
    }
}
