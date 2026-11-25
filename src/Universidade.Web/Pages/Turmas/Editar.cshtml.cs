using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Turmas;

public class EditarModel(TurmaRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public TurmaForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var turma = await repo.BuscarAsync(Id);
        if (turma is null) return NotFound();

        Form = TurmaForm.De(turma);
        await CarregarOpcoesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var comando = await repo.AtualizarAsync(Form.ParaTurma(Id));

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync();
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhuma linha foi alterada: a turma não existe mais.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = Id.ToString();
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Disciplinas"] = (await opcoes.DisciplinasAsync()).ParaSelect();
        ViewData["Semestres"] = (await opcoes.SemestresAsync()).ParaSelect();
    }
}
