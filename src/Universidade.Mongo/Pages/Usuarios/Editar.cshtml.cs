using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Mongo.Data;
using Universidade.Mongo.Models;

namespace Universidade.Mongo.Pages.Usuarios;

public class EditarModel(UsuarioRepository repo) : PageModel
{
    /// <summary>CPF antes da edição (o _id também pode ser alterado).</summary>
    [BindProperty(SupportsGet = true)]
    public string Cpf { get; set; } = "";

    [BindProperty]
    public UsuarioForm Form { get; set; } = new();

    public ComandoMongo? UltimoComando { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var usuario = await repo.BuscarAsync(Cpf);
        if (usuario is null) return NotFound();

        Form = UsuarioForm.De(usuario);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = Form.ParaDocumento();
        var estudante = await repo.DependenteEstudanteAsync(Cpf);
        var comando = await repo.AtualizarAsync(Cpf, usuario);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        if (comando.LinhasAfetadas == 0 && comando.Observacao is null)
            comando.Observacao = "Nenhum documento foi alterado: o usuário não existe mais.";
        else if (usuario.Cpf != Cpf && estudante is not null && comando.Observacao is null)
            comando.Observacao = $"O CPF (_id) foi alterado. O estudante {estudante} ainda referencia o CPF antigo.";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = usuario.Cpf;
        return RedirectToPage("Index");
    }
}
