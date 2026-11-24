using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Usuarios;

public class ExcluirModel(UsuarioRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public long Cpf { get; set; }

    public Usuario? Usuario { get; private set; }
    public string? MatProfessor { get; private set; }
    public string? MatEstudante { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Usuario = await repo.BuscarAsync(Cpf);
        if (Usuario is null) return NotFound();

        (MatProfessor, MatEstudante) = await repo.DependentesAsync(Cpf);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var (professor, estudante) = await repo.DependentesAsync(Cpf);
        var comando = await repo.ExcluirAsync(Cpf);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && (professor is not null || estudante is not null))
            comando.Observacao =
                "Efeito do ON DELETE SET NULL: " +
                string.Join(" e ", new[] { professor is null ? null : $"o professor {professor}", estudante is null ? null : $"o estudante {estudante}" }.OfType<string>()) +
                " continua(m) existindo, mas agora com cpf = NULL.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
