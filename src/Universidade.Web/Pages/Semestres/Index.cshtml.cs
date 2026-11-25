using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Semestres;

public class IndexModel(SemestreRepository repo) : PageModel
{
    public List<Semestre> Semestres { get; private set; } = [];
    public ComandoExecutado? UltimoComando { get; private set; }
    public string? Destaque { get; private set; }

    public async Task OnGetAsync()
    {
        Semestres = await repo.ListarAsync();
        UltimoComando = TempData.LerComando();
        Destaque = TempData["Destaque"] as string;
    }
}
