using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;

namespace Universidade.Web.Pages.Leciona;

public class ExcluirModel(LecionaRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int IdTurma { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    public Models.Leciona? Registro { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Registro = await repo.BuscarAsync(IdTurma, Mat);
        return Registro is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var comando = await repo.ExcluirAsync(IdTurma, Mat);
        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
