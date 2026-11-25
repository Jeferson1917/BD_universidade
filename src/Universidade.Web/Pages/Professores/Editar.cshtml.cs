using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Professores;

public class EditarModel(ProfessorRepository repo, OpcoesRepository opcoes) : PageModel
{
    /// <summary>Matrícula antes da edição (a chave primária também pode ser alterada).</summary>
    [BindProperty(SupportsGet = true)]
    public string Mat { get; set; } = "";

    [BindProperty]
    public ProfessorForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var professor = await repo.BuscarAsync(Mat);
        if (professor is null) return NotFound();

        Form = ProfessorForm.De(professor);
        await CarregarOpcoesAsync(professor.Cpf);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var atual = await repo.BuscarAsync(Mat);

        if (!ModelState.IsValid)
        {
            await CarregarOpcoesAsync(atual?.Cpf);
            return Page();
        }

        var professor = Form.ParaProfessor();
        var dependentes = await repo.DependentesAsync(Mat);
        var comando = await repo.AtualizarAsync(Mat, professor);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            await CarregarOpcoesAsync(atual?.Cpf);
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhuma linha foi alterada: o professor não existe mais.";
        else if (professor.MatProfessor != Mat && dependentes.Algum)
            comando.Observacao =
                $"A matrícula (chave primária) mudou de {Mat} para {professor.MatProfessor}. " +
                "Por causa do ON UPDATE CASCADE, a nova matrícula foi propagada para: " +
                DescreverDependentes(dependentes) + ".";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = professor.MatProfessor;
        return RedirectToPage("Index");
    }

    private static string DescreverDependentes(ProfessorDependentes d) =>
        string.Join(", ", new[]
        {
            d.Turmas > 0 ? $"{d.Turmas} registro(s) em leciona" : null,
            d.ChefeDe is not null ? $"chefia do(s) departamento(s) {d.ChefeDe}" : null,
            d.Planos > 0 ? $"{d.Planos} plano(s)" : null
        }.OfType<string>());

    private async Task CarregarOpcoesAsync(long? cpfAtual)
    {
        ViewData["Usuarios"] = (await opcoes.UsuariosSemProfessorAsync(cpfAtual)).ParaSelect();
        ViewData["Departamentos"] = (await opcoes.DepartamentosAsync()).ParaSelect();
        ViewData["Formacoes"] = (await opcoes.ValoresEnumAsync(TipoEnum.Formacao)).ParaSelect();
        ViewData["Jornadas"] = (await opcoes.ValoresEnumAsync(TipoEnum.Jornada)).ParaSelect();
    }
}
