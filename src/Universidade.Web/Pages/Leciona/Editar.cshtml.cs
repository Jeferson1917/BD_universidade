using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Leciona;

public class EditarModel(LecionaRepository repo, OpcoesRepository opcoes) : PageModel
{
    // Chave primária original (id_turma, mat_professor)
    [BindProperty(SupportsGet = true)]
    public int IdTurma { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    [BindProperty]
    public LecionaForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var registro = await repo.BuscarAsync(IdTurma, Mat);
        if (registro is null) return NotFound();

        Form = new LecionaForm { IdTurma = registro.IdTurma, MatProfessor = registro.MatProfessor };
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

        var comando = await repo.AtualizarAsync(IdTurma, Mat, Form.IdTurma!.Value, Form.MatProfessor!);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync();
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhuma linha foi alterada: a associação não existe mais.";

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
