using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;

namespace Universidade.Mongo.Data;

/// <summary>Consultas que alimentam as listas de seleção (dropdowns) dos formulários.</summary>
public class OpcoesRepository(MongoDb db)
{
    public async Task<List<SelectListItem>> EstudantesAsync()
    {
        var estudantes = await db.Estudantes.Find(Builders<Models.EstudanteDoc>.Filter.Empty)
            .SortBy(e => e.MatEstudante)
            .ToListAsync();

        return [.. estudantes.Select(e => new SelectListItem($"{e.MatEstudante} — {e.Cpf ?? "(sem usuário)"}", e.MatEstudante))];
    }

    public async Task<List<SelectListItem>> CursosAsync()
    {
        var cursos = await db.Cursos.Find(Builders<Models.CursoDoc>.Filter.Empty)
            .SortBy(c => c.Id)
            .ToListAsync();

        return [.. cursos.Select(c => new SelectListItem($"{c.Id} — {c.Nome} ({c.Turno})", c.Id.ToString()))];
    }
}
