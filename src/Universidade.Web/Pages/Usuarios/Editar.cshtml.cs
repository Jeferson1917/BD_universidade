using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Universidade.Web.Data;
using Universidade.Web.Models;

namespace Universidade.Web.Pages.Usuarios;

public class EditarModel(UsuarioRepository repo) : PageModel
{
    /// <summary>CPF antes da edição (a chave primária também pode ser alterada).</summary>
    [BindProperty(SupportsGet = true)]
    public long Cpf { get; set; }

    [BindProperty]
    public UsuarioForm Form { get; set; } = new();

    public ComandoExecutado? UltimoComando { get; private set; }

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

        var usuario = Form.ParaUsuario();
        var (professor, estudante) = await repo.DependentesAsync(Cpf);
        var comando = await repo.AtualizarAsync(Cpf, usuario);

        if (!comando.Sucesso)
        {
            UltimoComando = comando;
            ModelState.AddModelError(string.Empty, comando.Erro!);
            return Page();
        }

        if (comando.LinhasAfetadas == 0)
            comando.Observacao = "Nenhuma linha foi alterada: o usuário não existe mais.";
        else if (usuario.Cpf != Cpf && (professor is not null || estudante is not null))
            comando.Observacao =
                "O CPF (chave primária) foi alterado. Por causa do ON UPDATE CASCADE, o novo CPF foi propagado para " +
                string.Join(" e ", new[] { professor is null ? null : $"professor {professor}", estudante is null ? null : $"estudante {estudante}" }.OfType<string>()) + ".";

        TempData.GuardarComando(comando);
        TempData["Destaque"] = usuario.CpfFormatado;
        return RedirectToPage("Index");
    }
}
