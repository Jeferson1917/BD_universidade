using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Universidade.Mongo.Models;

/// <summary>
/// Subdocumento de universidade.plano, embutido no estudante: histórico de orientação
/// (poucos itens por aluno, sempre consultado junto do estudante). Ver docs/mapeamento-mongodb.md.
/// </summary>
public class PlanoEmbed
{
    [BsonElement("id_projeto")]
    public int? IdProjeto { get; set; }

    [BsonElement("mat_professor")]
    public string? MatProfessor { get; set; }

    [BsonElement("ano")]
    public int? Ano { get; set; }
}

/// <summary>Documento da collection "estudantes" (mapeada de universidade.estudante).</summary>
[BsonIgnoreExtraElements]
public class EstudanteDoc
{
    /// <summary>Matrícula do estudante, usada como _id.</summary>
    [BsonId]
    public string MatEstudante { get; set; } = "";

    /// <summary>CPF do usuário (referência a "usuarios"), pode ser nulo (ON DELETE SET NULL no relacional).</summary>
    [BsonElement("cpf")]
    public string? Cpf { get; set; }

    [BsonElement("mc")]
    public double? Mc { get; set; }

    [BsonElement("ano_ingresso")]
    public int? AnoIngresso { get; set; }

    /// <summary>universidade.plano embutido (ver docs/mapeamento-mongodb.md).</summary>
    [BsonElement("planos")]
    public List<PlanoEmbed> Planos { get; set; } = [];
}

public class EstudanteForm
{
    [Required(ErrorMessage = "Informe a matrícula.")]
    [StringLength(7, ErrorMessage = "Máximo de 7 caracteres.")]
    [Display(Name = "Matrícula")]
    public string MatEstudante { get; set; } = "";

    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
    [Display(Name = "Usuário (CPF)")]
    public string? Cpf { get; set; }

    [Range(0, 10, ErrorMessage = "Média deve estar entre 0 e 10.")]
    [Display(Name = "Média de conclusão (MC)")]
    public double? Mc { get; set; }

    [Display(Name = "Ano de ingresso")]
    public int? AnoIngresso { get; set; }

    public EstudanteDoc ParaDocumento(List<PlanoEmbed>? planosExistentes = null) => new()
    {
        MatEstudante = MatEstudante.Trim(),
        Cpf = string.IsNullOrWhiteSpace(Cpf) ? null : Cpf.Trim(),
        Mc = Mc,
        AnoIngresso = AnoIngresso,
        Planos = planosExistentes ?? []
    };

    public static EstudanteForm De(EstudanteDoc e) => new()
    {
        MatEstudante = e.MatEstudante,
        Cpf = e.Cpf,
        Mc = e.Mc,
        AnoIngresso = e.AnoIngresso
    };
}
