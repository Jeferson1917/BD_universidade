using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Estudantes;

public class ExcluirModel(EstudanteRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    public EstudanteDoc? Estudante { get; private set; }
    public long Vinculos { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Estudante = await repo.BuscarAsync(Mat);
        if (Estudante is null) return NotFound();

        Vinculos = await repo.ContarVinculosAsync(Mat);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var vinculos = await repo.ContarVinculosAsync(Mat);
        var comando = await repo.ExcluirAsync(Mat);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && vinculos > 0)
            comando.Observacao =
                $"{vinculos} documento(s) em \"vinculos\" continuam referenciando essa matrícula — " +
                "o MongoDB não apaga em cascata automaticamente.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
