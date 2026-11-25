using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Semestres;

public class ExcluirModel(SemestreRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public short Ano { get; set; }

    [BindProperty(SupportsGet = true)]
    public short Semestre { get; set; }

    public Models.Semestre? Registro { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Registro = await repo.BuscarAsync(Ano, Semestre);
        return Registro is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var atual = await repo.BuscarAsync(Ano, Semestre);
        var comando = await repo.ExcluirAsync(Ano, Semestre);

        // fk_semestre é ON DELETE NO ACTION: o banco recusa se houver turmas
        if (comando.CodigoErro == "23503")
            comando.Erro =
                $"Não é possível excluir {Ano}/{Semestre}: existem {atual?.Turmas} turma(s) nesse semestre " +
                "e a chave estrangeira fk_semestre é ON DELETE NO ACTION.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
