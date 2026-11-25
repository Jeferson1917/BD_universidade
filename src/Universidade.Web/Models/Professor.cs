using System.ComponentModel.DataAnnotations;

namespace Universidade.Web.Models;

/// <summary>Linha de universidade.professor (com nome do usuário e do departamento via JOIN).</summary>
public class Professor
{
    public string MatProfessor { get; set; } = "";
    public long? Cpf { get; set; }
    public string? NomeUsuario { get; set; }
    public string? Departamento { get; set; }
    public string? NomeDepartamento { get; set; }
    public string? Formacao { get; set; }
    public DateTime? DataAdmissao { get; set; }
    public string? TipoJornada { get; set; }
    public double? Salario { get; set; }

    public string? CpfFormatado => Cpf?.ToString("00000000000");
}

/// <summary>Dependentes de um professor (para avisar os efeitos das FKs).</summary>
public record ProfessorDependentes(long Turmas, string? ChefeDe, long Planos)
{
    public bool Algum => Turmas > 0 || ChefeDe is not null || Planos > 0;
}

public class ProfessorForm
{
    [Required(ErrorMessage = "Informe a matrícula.")]
    [StringLength(7, ErrorMessage = "Máximo de 7 caracteres.")]
    [Display(Name = "Matrícula")]
    public string MatProfessor { get; set; } = "";

    [Display(Name = "Usuário (CPF)")]
    public string? Cpf { get; set; }

    [Display(Name = "Departamento")]
    public string? Departamento { get; set; }

    [Display(Name = "Formação")]
    public string? Formacao { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de admissão")]
    public DateTime? DataAdmissao { get; set; }

    [Display(Name = "Jornada de trabalho")]
    public string? TipoJornada { get; set; }

    [Range(0, 1_000_000_000, ErrorMessage = "Salário inválido.")]
    [Display(Name = "Salário")]
    public double? Salario { get; set; }

    public Professor ParaProfessor() => new()
    {
        MatProfessor = MatProfessor.Trim(),
        Cpf = string.IsNullOrEmpty(Cpf) ? null : long.Parse(Cpf),
        Departamento = Vazio(Departamento),
        Formacao = Vazio(Formacao),
        DataAdmissao = DataAdmissao,
        TipoJornada = Vazio(TipoJornada),
        Salario = Salario
    };

    public static ProfessorForm De(Professor p) => new()
    {
        MatProfessor = p.MatProfessor,
        Cpf = p.Cpf?.ToString(),
        Departamento = p.Departamento,
        Formacao = p.Formacao,
        DataAdmissao = p.DataAdmissao,
        TipoJornada = p.TipoJornada,
        Salario = p.Salario
    };

    private static string? Vazio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;
}
