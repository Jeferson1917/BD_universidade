using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Semestres;

public class EditarModel(SemestreRepository repo) : PageModel
{
    // Chave primária original (ano, semestre)
    [BindProperty(SupportsGet = true)]
    public short Ano { get; set; }

    [BindProperty(SupportsGet = true)]
    public short Semestre { get; set; }

    [BindProperty]
    public SemestreForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var semestre = await repo.BuscarAsync(Ano, Semestre);
        if (semestre is null) return NotFound();

        Form = SemestreForm.De(semestre);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var atual = await repo.BuscarAsync(Ano, Semestre);
        var novo = Form.ParaSemestre();
        var comando = await repo.AtualizarAsync(Ano, Semestre, novo);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhuma linha foi alterada: o semestre não existe mais.";
        else if (novo.Chave != $"{Ano}-{Semestre}" && atual is { Turmas: > 0 })
            comando.Observacao =
                $"A chave mudou de {Ano}/{Semestre} para {novo.Ano}/{novo.Periodo}. Por causa do ON UPDATE CASCADE, " +
                $"as {atual.Turmas} turma(s) desse semestre foram atualizadas junto.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = novo.Chave;
        return RedirectToPage("Index");
    }
}
