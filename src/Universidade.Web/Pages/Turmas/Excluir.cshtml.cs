using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Turmas;

public class ExcluirModel(TurmaRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public Turma? Turma { get; private set; }
    public TurmaDependentes? Dependentes { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Turma = await repo.BuscarAsync(Id);
        if (Turma is null) return NotFound();

        Dependentes = await repo.DependentesAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var d = await repo.DependentesAsync(Id);
        var comando = await repo.ExcluirAsync(Id);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && d.Algum)
            comando.Observacao = "Efeito do ON DELETE CASCADE: " + string.Join(" e ", new[]
            {
                d.Professores > 0 ? $"{d.Professores} registro(s) de leciona" : null,
                d.Alunos > 0 ? $"{d.Alunos} matrícula(s) em cursa" : null
            }.OfType<string>()) + " foram apagados junto com a turma.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
