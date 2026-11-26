using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Usuarios;

public class ExcluirModel(UsuarioRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Cpf { get; set; } = "";

    public UsuarioDoc? Usuario { get; private set; }
    public string? MatEstudante { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Usuario = await repo.BuscarAsync(Cpf);
        if (Usuario is null) return NotFound();

        MatEstudante = await repo.DependenteEstudanteAsync(Cpf);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var estudante = await repo.DependenteEstudanteAsync(Cpf);
        var comando = await repo.ExcluirAsync(Cpf);

        if (comando.Sucesso && comando.LinhasAfetadas > 0 && estudante is not null)
            comando.Observacao =
                $"O estudante {estudante} continua existindo, mas agora referencia um CPF que não existe mais " +
                "em \"usuarios\" — o MongoDB não tem ON DELETE SET NULL automático.";

        TempData.GuardarComando(comando);
        return RedirectToPage("Index");
    }
}
