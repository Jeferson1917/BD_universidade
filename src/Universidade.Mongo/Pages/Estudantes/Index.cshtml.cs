using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Estudantes;

public class IndexModel(EstudanteRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Busca { get; set; }

    public List<EstudanteDoc> Estudantes { get; private set; } = [];
    public ComandoMongo? UltimoComando { get; private set; }
    public string? Destaque { get; private set; }

    public async Task OnGetAsync()
    {
        Estudantes = await repo.ListarAsync(Busca);
        UltimoComando = TempData.LerComando();
        Destaque = TempData["Destaque"] as string;
    }
}
