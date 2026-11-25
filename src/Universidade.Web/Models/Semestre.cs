using System.ComponentModel.DataAnnotations;

namespace Universidade.Web.Models;

/// <summary>Linha de universidade.semestre (a coluna de fim se chama data_fom no esquema).</summary>
public class Semestre
{
    public short Ano { get; set; }
    public short Periodo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public long Turmas { get; set; }

    public string Chave => $"{Ano}-{Periodo}";
}

public class SemestreForm
{
    [Required(ErrorMessage = "Informe o ano.")]
    [Range(1900, 2100, ErrorMessage = "Ano inválido.")]
    public short? Ano { get; set; }

    [Required(ErrorMessage = "Informe o semestre.")]
    [Range(1, 9, ErrorMessage = "Semestre inválido.")]
    [Display(Name = "Semestre")]
    public short? Periodo { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de início")]
    public DateTime? DataInicio { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de fim")]
    public DateTime? DataFim { get; set; }

    public Semestre ParaSemestre() => new()
    {
        Ano = Ano!.Value,
        Periodo = Periodo!.Value,
        DataInicio = DataInicio,
        DataFim = DataFim
    };

    public static SemestreForm De(Semestre s) => new()
    {
        Ano = s.Ano,
        Periodo = s.Periodo,
        DataInicio = s.DataInicio,
        DataFim = s.DataFim
    };
}
