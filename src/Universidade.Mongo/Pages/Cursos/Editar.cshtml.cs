using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Cursos;

public class EditarModel(CursoRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public CursoForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var curso = await repo.BuscarAsync(Id);
        if (curso is null) return NotFound();

        Form = CursoForm.De(curso);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var comando = await repo.AtualizarAsync(Form.ParaDocumento(Id));

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhum documento foi alterado: o curso não existe mais.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = Id.ToString();
        return RedirectToPage("Index");
    }
}
