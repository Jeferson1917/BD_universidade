using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Universidade.Mongo.Models;

/// <summary>Documento da collection "vinculos" (mapeada de universidade.vinculo).</summary>
[BsonIgnoreExtraElements]
public class VinculoDoc
{
    /// <summary>idVinculo original (gerado via a collection "contadores"), usado como _id.</summary>
    [BsonId]
    public int Id { get; set; }

    /// <summary>Matrícula do estudante (referência a "estudantes").</summary>
    [BsonElement("mat_estudante")]
    public string? MatEstudante { get; set; }

    /// <summary>_id do curso (referência a "cursos").</summary>
    [BsonElement("curso")]
    public int? Curso { get; set; }

    [BsonElement("data_entrada")]
    public DateTime? DataEntrada { get; set; }

    [BsonElement("status")]
    public string? Status { get; set; }

    [BsonElement("data_saida")]
    public DateTime? DataSaida { get; set; }
}

public class VinculoForm
{
    [Required(ErrorMessage = "Informe a matrícula do estudante.")]
    [Display(Name = "Estudante (matrícula)")]
    public string MatEstudante { get; set; } = "";

    [Required(ErrorMessage = "Escolha o curso.")]
    [Display(Name = "Curso")]
    public int? Curso { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de entrada")]
    public DateTime? DataEntrada { get; set; }

    [Display(Name = "Situação")]
    public string? Status { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de saída")]
    public DateTime? DataSaida { get; set; }

    public VinculoDoc ParaDocumento(int id = 0) => new()
    {
        Id = id,
        MatEstudante = MatEstudante.Trim(),
        Curso = Curso,
        DataEntrada = DataEntrada,
        Status = string.IsNullOrWhiteSpace(Status) ? null : Status,
        DataSaida = DataSaida
    };

    public static VinculoForm De(VinculoDoc v) => new()
    {
        MatEstudante = v.MatEstudante ?? "",
        Curso = v.Curso,
        DataEntrada = v.DataEntrada,
        Status = v.Status,
        DataSaida = v.DataSaida
    };
}

