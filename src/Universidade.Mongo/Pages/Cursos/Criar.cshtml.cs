using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Cursos;

public class CriarModel(CursoRepository repo) : PageModel
{
    [BindProperty]
    public CursoForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var (comando, id) = await repo.InserirAsync(Form);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = id.ToString();
        return RedirectToPage("Index");
    }
}
