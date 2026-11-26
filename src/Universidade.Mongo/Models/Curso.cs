using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson.Serialization.Attributes;

namespace Universidade.Mongo.Models;

/// <summary>Documento da collection "cursos" (mapeada de universidade.curso).</summary>
[BsonIgnoreExtraElements]
public class CursoDoc
{
    /// <summary>idCurso original (gerado via a collection "contadores"), usado como _id.</summary>
    [BsonId]
    public int Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = "";

    [BsonElement("grau")]
    public string? Grau { get; set; }

    [BsonElement("turno")]
    public string Turno { get; set; } = "";

    [BsonElement("campus")]
    public string? Campus { get; set; }

    [BsonElement("nivel")]
    public string? Nivel { get; set; }
}

public class CursoForm
{
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres.")]
    public string Nome { get; set; } = "";

    [Display(Name = "Grau")]
    public string? Grau { get; set; }

    [Required(ErrorMessage = "Escolha o turno.")]
    [Display(Name = "Turno")]
    public string Turno { get; set; } = "";

    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres.")]
    public string? Campus { get; set; }

    [Display(Name = "Nível")]
    public string? Nivel { get; set; }

    public CursoDoc ParaDocumento(int id = 0) => new()
    {
        Id = id,
        Nome = Nome.Trim(),
        Grau = Vazio(Grau),
        Turno = Turno,
        Campus = Vazio(Campus),
        Nivel = Vazio(Nivel)
    };

    public static CursoForm De(CursoDoc c) => new()
    {
        Nome = c.Nome,
        Grau = c.Grau,
        Turno = c.Turno,
        Campus = c.Campus,
        Nivel = c.Nivel
    };

    private static string? Vazio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;
}

public static class DominioSelectExtensions
{
    public static List<SelectListItem> ParaSelect(this string[] valores, string? selecionado = null) =>
        [.. valores.Select(v => new SelectListItem(v, v, v == selecionado))];
}
