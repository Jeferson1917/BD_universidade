using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Professores;

public class CriarModel(ProfessorRepository repo, OpcoesRepository opcoes) : PageModel
{
    [BindProperty]
    public ProfessorForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task OnGetAsync() => await CarregarOpcoesAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync();
            return Page();
        }

        var professor = Form.ParaProfessor();
        var comando = await repo.InserirAsync(professor);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync();
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = professor.MatProfessor;
        return RedirectToPage("Index");
    }

    private async Task CarregarOpcoesAsync()
    {
        ViewData["Usuarios"] = (await opcoes.UsuariosSemProfessorAsync(null)).ParaSelect();
        ViewData["Departamentos"] = (await opcoes.DepartamentosAsync()).ParaSelect();
        ViewData["Formacoes"] = (await opcoes.ValoresEnumAsync(TipoEnum.Formacao)).ParaSelect();
        ViewData["Jornadas"] = (await opcoes.ValoresEnumAsync(TipoEnum.Jornada)).ParaSelect();
    }
}
