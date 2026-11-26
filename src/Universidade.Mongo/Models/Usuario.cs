using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Universidade.Mongo.Models;

/// <summary>Documento da collection "usuarios" (mapeada de universidade.usuario).</summary>
[BsonIgnoreExtraElements]
public class UsuarioDoc
{
    /// <summary>CPF com 11 dígitos, usado como _id (chave primária original).</summary>
    [BsonId]
    public string Cpf { get; set; } = "";

    [BsonElement("nome")]
    public string Nome { get; set; } = "";

    [BsonElement("data_nascimento")]
    public DateTime? DataNascimento { get; set; }

    [BsonElement("email")]
    public List<string>? Email { get; set; }

    [BsonElement("telefone")]
    public List<string>? Telefone { get; set; }

    [BsonElement("login")]
    public string? Login { get; set; }

    [BsonElement("senha")]
    public string? Senha { get; set; }
}

public class UsuarioForm
{
    [Required(ErrorMessage = "Informe o CPF.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
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

    public UsuarioDoc ParaDocumento() => new()
    {
        Cpf = Cpf.Trim(),
        Nome = Nome.Trim(),
        DataNascimento = DataNascimento,
        Email = ParaLista(Emails),
        Telefone = ParaLista(Telefones),
        Login = string.IsNullOrWhiteSpace(Login) ? null : Login.Trim(),
        Senha = string.IsNullOrEmpty(Senha) ? null : Senha
    };

    public static UsuarioForm De(UsuarioDoc u) => new()
    {
        Cpf = u.Cpf,
        Nome = u.Nome,
        DataNascimento = u.DataNascimento,
        Emails = u.Email is null ? null : string.Join(", ", u.Email),
        Telefones = u.Telefone is null ? null : string.Join(", ", u.Telefone),
        Login = u.Login
        // senha não é preenchida na edição: em branco = manter a atual
    };

    private static List<string>? ParaLista(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;
        var itens = texto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return itens.Length == 0 ? null : [.. itens];
    }
}
