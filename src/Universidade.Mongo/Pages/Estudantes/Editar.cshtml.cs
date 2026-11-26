using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Estudantes;

public class EditarModel(EstudanteRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    [BindProperty]
    public EstudanteForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var estudante = await repo.BuscarAsync(Mat);
        if (estudante is null) return NotFound();

        Form = EstudanteForm.De(estudante);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var atual = await repo.BuscarAsync(Mat);
        var estudante = Form.ParaDocumento(atual?.Planos);
        var comando = await repo.AtualizarAsync(Mat, estudante);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        if (comando.LinhasAfetadas == 0 && comando.Observacao is null)
            comando.Observacao = "Nenhum documento foi alterado: o estudante não existe mais.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = estudante.MatEstudante;
        return RedirectToPage("Index");
    }
}
