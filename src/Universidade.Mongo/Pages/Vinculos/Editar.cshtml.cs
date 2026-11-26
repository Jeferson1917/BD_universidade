using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Vinculos;

public class EditarModel(VinculoRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public VinculoForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var vinculo = await repo.BuscarAsync(Id);
        if (vinculo is null) return NotFound();

        Form = VinculoForm.De(vinculo);
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

        var comando = await repo.AtualizarAsync(Form.ParaDocumento(Id));

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhum documento foi alterado: o vínculo não existe mais.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = Id.ToString();
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Estudantes"] = await opcoes.EstudantesAsync();
        ViewData["Cursos"] = await opcoes.CursosAsync();
    }
}
