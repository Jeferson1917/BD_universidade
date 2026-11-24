using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Usuarios;

public class IndexModel(UsuarioRepository repo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Busca { get; set; }

    public List<Usuario> Usuarios { get; private set; } = [];
    public ComandoExecutado? UltimoComando { get; private set; }
    public string? Destaque { get; private set; }

    public async Task OnGetAsync()
    {
        Usuarios = await repo.ListarAsync(Busca);
        UltimoComando = TempData.LerComando();
        Destaque = TempData["Destaque"] as string;
    }
}
