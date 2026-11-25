using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Leciona;

public class CriarModel(LecionaRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty]
    public LecionaForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task OnGetAsync() => await CarregarOpcoesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var comando = await repo.InserirAsync(Form.IdTurma!.Value, Form.MatProfessor!);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync();
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = $"{Form.IdTurma}|{Form.MatProfessor}";
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Turmas"] = (await opcoes.TurmasAsync()).ParaSelect();
        ViewData["Professores"] = (await opcoes.ProfessoresAsync()).ParaSelect();
    }
}
