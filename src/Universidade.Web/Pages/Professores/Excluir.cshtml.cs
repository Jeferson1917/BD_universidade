using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Professores;

public class ExcluirModel(ProfessorRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    public Professor? Professor { get; private set; }
    public ProfessorDependentes? Dependentes { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Professor = await repo.BuscarAsync(Mat);
        if (Professor is null) return NotFound();

        Dependentes = await repo.DependentesAsync(Mat);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var d = await repo.DependentesAsync(Mat);
        var comando = await repo.ExcluirAsync(Mat);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && d.Algum)
            comando.Observacao = "Efeitos das chaves estrangeiras: " + string.Join("; ", new[]
            {
                d.Turmas > 0 ? $"{d.Turmas} registro(s) de leciona apagado(s) (ON DELETE CASCADE)" : null,
                d.ChefeDe is not null ? $"departamento(s) {d.ChefeDe} ficaram sem chefe (ON DELETE SET NULL)" : null,
                d.Planos > 0 ? $"{d.Planos} plano(s) ficaram com mat_professor = NULL (ON DELETE SET NULL)" : null
            }.OfType<string>()) + ".";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
