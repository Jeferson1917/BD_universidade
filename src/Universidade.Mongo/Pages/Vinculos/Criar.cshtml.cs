using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Vinculos;

public class CriarModel(VinculoRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty]
    public VinculoForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public async Task OnGetAsync() => await CarregarOpcoesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var (comando, id) = await repo.InserirAsync(Form);

        TempData.GuardarComando(comando);
        TempData["Destaque"] = id.ToString();
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Estudantes"] = await opcoes.EstudantesAsync();
        ViewData["Cursos"] = await opcoes.CursosAsync();
    }
}
