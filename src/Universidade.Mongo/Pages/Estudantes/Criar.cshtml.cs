using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Estudantes;

public class CriarModel(EstudanteRepository repo) : PageModel
{
    [BindProperty]
    public EstudanteForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var estudante = Form.ParaDocumento();
        var comando = await repo.InserirAsync(estudante);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = estudante.MatEstudante;
        return RedirectToPage("Index");
    }
}
