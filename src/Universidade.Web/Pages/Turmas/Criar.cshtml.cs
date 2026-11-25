using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Turmas;

public class CriarModel(TurmaRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty]
    public TurmaForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task OnGetAsync() => await CarregarOpcoesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var (comando, id) = await repo.InserirAsync(Form.ParaTurma());

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync();
            return Page();
        }

        comando.Observacao = $"id_turma = {id}, gerado pela sequência universidade.seq_turma (RETURNING id_turma).";
        TempData.GuardarComando(comando);
        TempData["Destaque"] = id?.ToString();
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Disciplinas"] = (await opcoes.DisciplinasAsync()).ParaSelect();
        ViewData["Semestres"] = (await opcoes.SemestresAsync()).ParaSelect();
    }
}
