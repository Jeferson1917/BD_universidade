using MongoDB.Bson;
using MongoDB.Driver;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Data;

/// <summary>CRUD da collection "cursos".</summary>
public class CursoRepository(MongoDb db)
{
    public async Task<List<CursoDoc>> ListarAsync(string? busca)
    {
        var filtro = string.IsNullOrWhiteSpace(busca)
            ? Builders<CursoDoc>.Filter.Empty
            : Builders<CursoDoc>.Filter.Regex(c => c.Nome, new BsonRegularExpression(busca, "i"));

        return await db.Cursos.Find(filtro).SortBy(c => c.Id).ToListAsync();
    }

    public async Task<CursoDoc?> BuscarAsync(int id) =>
        await db.Cursos.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<(ComandoMongo Comando, int Id)> InserirAsync(CursoForm form)
    {
        var id = await db.ProximoIdAsync("curso");
        var curso = form.ParaDocumento(id);
        var comando = ComandoMongo.De("cursos", "insertOne", documento: curso);
        try
        {
            await db.Cursos.InsertOneAsync(curso);
            comando.LinhasAfetadas = 1;
            comando.Observacao = $"_id = {id}, gerado pela collection \"contadores\" (equivalente à SEQUENCE do Postgres).";
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = $"Valor duplicado: {ex.WriteError.Message}";
        }
        return (comando, id);
    }

    public async Task<ComandoMongo> AtualizarAsync(CursoDoc c)
    {
        var filtro = Builders<CursoDoc>.Filter.Eq(x => x.Id, c.Id);
        var comando = ComandoMongo.De("cursos", "replaceOne", new { _id = c.Id }, c);
        var resultado = await db.Cursos.ReplaceOneAsync(filtro, c);
        comando.LinhasAfetadas = resultado.ModifiedCount;
        return comando;
    }

    public async Task<ComandoMongo> ExcluirAsync(int id)
    {
        var comando = ComandoMongo.De("cursos", "deleteOne", new { _id = id });
        var resultado = await db.Cursos.DeleteOneAsync(c => c.Id == id);
        comando.LinhasAfetadas = resultado.DeletedCount;
        return comando;
    }

    /// <summary>Quantidade de vínculos ligados a esse curso (para avisar o efeito da exclusão).</summary>
    public Task<long> ContarVinculosAsync(int id) =>
        db.Vinculos.CountDocumentsAsync(v => v.Curso == id);
}
