namespace Universidade.Mongo.Models;

/// <summary>
/// Valores dos domínios/ENUMs do esquema relacional, recriados aqui porque o MongoDB
/// não tem tipo ENUM nativo: a validação vira um `enum` no validador de schema da
/// collection (ver mongo/schema.js) e, no formulário, a lista de opções abaixo.
/// </summary>
public static class Dominios
{
    public static readonly string[] GrauCurso = ["Bacharelado", "Licenciatura Plena"];
    public static readonly string[] TurnoCurso = ["Matutino", "Vespertino", "Noturno", "Turno Indefinido"];
    public static readonly string[] NivelCurso = ["Graduação", "Mestrado", "Doutorado", "Lato"];
    public static readonly string[] StatusVinculo = ["Ativo", "Cancelada", "Formando", "Graduado"];
}
