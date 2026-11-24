using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Usuarios;

public class CriarModel(UsuarioRepository repo) : PageModel
{
    [BindProperty]
    public UsuarioForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = Form.ParaUsuario();
        var comando = await repo.InserirAsync(usuario);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        TempData.GuardarComando(comando);
        TempData["Destaque"] = usuario.CpfFormatado;
        return RedirectToPage("Index");
    }
}
