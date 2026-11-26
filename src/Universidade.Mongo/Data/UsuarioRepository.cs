using MongoDB.Bson;
using MongoDB.Driver;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Data;

/// <summary>CRUD da collection "usuarios".</summary>
public class UsuarioRepository(MongoDb db)
{
    public async Task<List<UsuarioDoc>> ListarAsync(string? busca)
    {
        var filtro = string.IsNullOrWhiteSpace(busca)
            ? Builders<UsuarioDoc>.Filter.Empty
            : Builders<UsuarioDoc>.Filter.Or(
                Builders<UsuarioDoc>.Filter.Regex(u => u.Nome, new BsonRegularExpression(busca, "i")),
                Builders<UsuarioDoc>.Filter.Regex(u => u.Login, new BsonRegularExpression(busca, "i")),
                Builders<UsuarioDoc>.Filter.Regex(u => u.Cpf, new BsonRegularExpression(busca, "i")));

        return await db.Usuarios.Find(filtro).SortBy(u => u.Nome).ToListAsync();
    }

    public async Task<UsuarioDoc?> BuscarAsync(string cpf) =>
        await db.Usuarios.Find(u => u.Cpf == cpf).FirstOrDefaultAsync();

    public async Task<ComandoMongo> InserirAsync(UsuarioDoc u)
    {
        var comando = ComandoMongo.De("usuarios", "insertOne", documento: u);
        try
        {
            await db.Usuarios.InsertOneAsync(u);
            comando.LinhasAfetadas = 1;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = DuplicidadeKey(ex) switch
            {
                "_id" => "Já existe um usuário com esse CPF.",
                "login" => "Esse login já está em uso por outro usuário.",
                _ => $"Valor duplicado: {ex.WriteError.Message}"
            };
        }
        return comando;
    }

    public async Task<ComandoMongo> AtualizarAsync(string cpfOriginal, UsuarioDoc u)
    {
        // _id (cpf) não pode ser alterado por um replace; se mudou, apaga e reinsere.
        if (u.Cpf != cpfOriginal)
            return await TrocarChaveAsync(cpfOriginal, u);

        var filtro = Builders<UsuarioDoc>.Filter.Eq(x => x.Cpf, cpfOriginal);
        var comando = ComandoMongo.De("usuarios", "replaceOne", new { _id = cpfOriginal }, u);
        try
        {
            var resultado = await db.Usuarios.ReplaceOneAsync(filtro, u);
            comando.LinhasAfetadas = resultado.ModifiedCount;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = DuplicidadeKey(ex) == "login"
                ? "Esse login já está em uso por outro usuário."
                : $"Valor duplicado: {ex.WriteError.Message}";
        }
        return comando;
    }

    private async Task<ComandoMongo> TrocarChaveAsync(string cpfOriginal, UsuarioDoc novo)
    {
        var comando = ComandoMongo.De("usuarios", "deleteOne + insertOne (troca de _id)",
            new { _id = cpfOriginal }, novo);
        try
        {
            await db.Usuarios.InsertOneAsync(novo);
            var exclusao = await db.Usuarios.DeleteOneAsync(u => u.Cpf == cpfOriginal);
            comando.LinhasAfetadas = exclusao.DeletedCount;
            comando.Observacao =
                "O _id (CPF) não pode ser alterado com um replaceOne: o documento foi reinserido com a nova " +
                "chave e o antigo removido. Diferente do Postgres, isso NÃO propaga automaticamente para " +
                "estudantes que referenciam o CPF antigo — é preciso atualizar manualmente (sem ON UPDATE CASCADE).";
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            comando.Erro = "Já existe um usuário com esse novo CPF.";
        }
        return comando;
    }

    public async Task<ComandoMongo> ExcluirAsync(string cpf)
    {
        var comando = ComandoMongo.De("usuarios", "deleteOne", new { _id = cpf });
        var resultado = await db.Usuarios.DeleteOneAsync(u => u.Cpf == cpf);
        comando.LinhasAfetadas = resultado.DeletedCount;
        return comando;
    }

    /// <summary>Estudante ligado a esse CPF (para avisar o efeito da exclusão/alteração).</summary>
    public async Task<string?> DependenteEstudanteAsync(string cpf) =>
        (await db.Estudantes.Find(e => e.Cpf == cpf).FirstOrDefaultAsync())?.MatEstudante;

    private static string? DuplicidadeKey(MongoWriteException ex)
    {
        var msg = ex.WriteError.Message;
        if (msg.Contains("login")) return "login";
        if (msg.Contains("_id")) return "_id";
        return null;
    }
}
