using System.ComponentModel.DataAnnotations;

namespace Universidade.Web.Models;

/// <summary>Linha de universidade.turma (com nome da disciplina, professores e nº de alunos).</summary>
public class Turma
{
    public int IdTurma { get; set; }
    public string CodDisc { get; set; } = "";
    public string? NomeDisciplina { get; set; }
    public int? Numero { get; set; }
    public short? Ano { get; set; }
    public short? Semestre { get; set; }
    public string? Professores { get; set; }
    public long Alunos { get; set; }

    public string SemestreTexto => Ano is null ? "" : $"{Ano}/{Semestre}";
    public string? SemestreValor => Ano is null ? null : $"{Ano}-{Semestre}";
}

/// <summary>Registros que dependem de uma turma (para avisar os efeitos das FKs).</summary>
public record TurmaDependentes(long Professores, long Alunos)
{
    public bool Algum => Professores > 0 || Alunos > 0;
}

public class TurmaForm
{
    [Required(ErrorMessage = "Escolha a disciplina.")]
    [Display(Name = "Disciplina")]
    public string CodDisc { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Número inválido.")]
    [Display(Name = "Número da turma")]
    public int? Numero { get; set; }

    /// <summary>"ano-semestre", ex.: "2017-2".</summary>
    [Display(Name = "Semestre")]
    public string? Semestre { get; set; }

    public Turma ParaTurma(int idTurma = 0)
    {
        short? ano = null, sem = null;
        if (!string.IsNullOrEmpty(Semestre))
        {
            var partes = Semestre.Split('-');
            ano = short.Parse(partes[0]);
            sem = short.Parse(partes[1]);
        }
        return new() { IdTurma = idTurma, CodDisc = CodDisc, Numero = Numero, Ano = ano, Semestre = sem };
    }

    public static TurmaForm De(Turma t) => new()
    {
        CodDisc = t.CodDisc,
        Numero = t.Numero,
        Semestre = t.SemestreValor
    };
}
