using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Professores;

public class IndexModel(ProfessorRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Busca { get; set; }

    public List<Professor> Professores { get; private set; } = [];
    public ComandoExecutado? UltimoComando { get; private set; }
    public string? Destaque { get; private set; }

    public async Task OnGetAsync()
    {
        Professores = await repo.ListarAsync(Busca);
        UltimoComando = TempData.LerComando();
        Destaque = TempData["Destaque"] as string;
    }
}
