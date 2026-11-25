using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Semestres;

public class CriarModel(SemestreRepository repo) : PageModel
{
    [BindProperty]
    public SemestreForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var semestre = Form.ParaSemestre();
        var comando = await repo.InserirAsync(semestre);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = semestre.Chave;
        return RedirectToPage("Index");
    }
}
