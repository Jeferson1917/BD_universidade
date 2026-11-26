using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Cursos;

public class ExcluirModel(CursoRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public CursoDoc? Curso { get; private set; }
    public long Vinculos { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Curso = await repo.BuscarAsync(Id);
        if (Curso is null) return NotFound();

        Vinculos = await repo.ContarVinculosAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var vinculos = await repo.ContarVinculosAsync(Id);
        var comando = await repo.ExcluirAsync(Id);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && vinculos > 0)
            comando.Observacao =
                $"{vinculos} documento(s) em \"vinculos\" continuam referenciando esse curso — " +
                "o MongoDB não apaga/atualiza em cascata automaticamente.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
