using System.ComponentModel.DataAnnotations;

namespace Universidade.Web.Models;

/// <summary>Linha da tabela universidade.usuario.</summary>
public class Usuario
{
    public long Cpf { get; set; }
    public string Nome { get; set; } = "";
    public DateTime? DataNascimento { get; set; }
    public string[]? Email { get; set; }
    public string[]? Telefone { get; set; }
    public string? Login { get; set; }
    public string? Senha { get; set; }

    /// <summary>CPF com 11 dígitos (NUMERIC perde os zeros à esquerda).</summary>
    public string CpfFormatado => Cpf.ToString("00000000000");
}

/// <summary>Dados do formulário de usuário (arrays digitados separados por vírgula).</summary>
public class UsuarioForm
{
    [Required(ErrorMessage = "Informe o CPF.")]
    [RegularExpression(@"^\d{1,13}$", ErrorMessage = "CPF deve ter apenas números (até 13 dígitos).")]
    [Display(Name = "CPF")]
    public string Cpf { get; set; } = "";

    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres.")]
    public string Nome { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateTime? DataNascimento { get; set; }

    [Display(Name = "E-mails (separados por vírgula)")]
    public string? Emails { get; set; }

    [Display(Name = "Telefones (separados por vírgula)")]
    public string? Telefones { get; set; }

    [StringLength(45, ErrorMessage = "Máximo de 45 caracteres.")]
    public string? Login { get; set; }

    [StringLength(32, ErrorMessage = "Máximo de 32 caracteres.")]
    public string? Senha { get; set; }

    public Usuario ParaUsuario() => new()
    {
        Cpf = long.Parse(Cpf),
        Nome = Nome.Trim(),
        DataNascimento = DataNascimento,
        Email = ParaArray(Emails),
        Telefone = ParaArray(Telefones),
        Login = string.IsNullOrWhiteSpace(Login) ? null : Login.Trim(),
        Senha = string.IsNullOrEmpty(Senha) ? null : Senha
    };

    public static UsuarioForm De(Usuario u) => new()
    {
        Cpf = u.CpfFormatado,
        Nome = u.Nome,
        DataNascimento = u.DataNascimento,
        Emails = u.Email is null ? null : string.Join(", ", u.Email),
        Telefones = u.Telefone is null ? null : string.Join(", ", u.Telefone),
        Login = u.Login
        // senha não é preenchida na edição: em branco = manter a atual
    };

    private static string[]? ParaArray(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;
        var itens = texto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return itens.Length == 0 ? null : itens;
    }
}
