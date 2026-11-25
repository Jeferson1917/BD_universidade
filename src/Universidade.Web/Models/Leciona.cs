using System.ComponentModel.DataAnnotations;

namespace Universidade.Web.Models;

/// <summary>Linha de universidade.leciona (com dados da turma e do professor via JOIN).</summary>
public class Leciona
{
    public int IdTurma { get; set; }
    public string MatProfessor { get; set; } = "";
    public string CodDisc { get; set; } = "";
    public string? NomeDisciplina { get; set; }
    public int? Numero { get; set; }
    public string? SemestreTexto { get; set; }
    public string? NomeProfessor { get; set; }

    /// <summary>Identifica a linha (chave primária composta).</summary>
    public string Chave => $"{IdTurma}|{MatProfessor}";
}

public class LecionaForm
{
    [Required(ErrorMessage = "Escolha a turma.")]
    [Display(Name = "Turma")]
    public int? IdTurma { get; set; }

    [Required(ErrorMessage = "Escolha o professor.")]
    [Display(Name = "Professor")]
    public string? MatProfessor { get; set; }
}
