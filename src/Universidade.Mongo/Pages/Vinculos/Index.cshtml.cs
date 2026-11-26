using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Vinculos;

public class IndexModel(VinculoRepository repo) : PageModel
{
    public List<VinculoDoc> Vinculos { get; private set; } = [];
    public ComandoMongo? UltimoComando { get; private set; }
    public string? Destaque { get; private set; }

    public async Task OnGetAsync()
    {
        Vinculos = await repo.ListarAsync();
        UltimoComando = TempData.LerComando();
        Destaque = TempData["Destaque"] as string;
    }
}
