using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Vinculos;

public class ExcluirModel(VinculoRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public VinculoDoc? Vinculo { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Vinculo = await repo.BuscarAsync(Id);
        return Vinculo is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var comando = await repo.ExcluirAsync(Id);
        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
