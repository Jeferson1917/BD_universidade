using Microsoft.AspNetCore.Mvc.Rendering;
using NpgsqlTypes;
using static Universidade.Web.Data.Db;

namespace Universidade.Web.Data;

/// <summary>Item de uma lista de seleção (dropdown) do formulário.</summary>
public record Opcao(string Valor, string Texto);

/// <summary>Consultas que alimentam as listas de seleção dos formulários.</summary>
public class OpcoesRepository(Db db)
{
    private static Opcao Ler(Npgsql.NpgsqlDataReader r) => new(r.GetString(0), r.GetString(1));

    public Task<List<Opcao>> DepartamentosAsync() => db.ConsultarAsync("""
        SELECT cod_depto, cod_depto || ' — ' || nome
        FROM universidade.departamento
        ORDER BY cod_depto
        """, Ler);

    public Task<List<Opcao>> DisciplinasAsync() => db.ConsultarAsync("""
        SELECT cod_disc, cod_disc || ' — ' || nome
        FROM universidade.disciplina
        ORDER BY cod_disc
        """, Ler);

    /// <summary>Valor no formato "ano-semestre" (ex.: "2017-2").</summary>
    public Task<List<Opcao>> SemestresAsync() => db.ConsultarAsync("""
        SELECT ano || '-' || semestre, ano || '/' || semestre
        FROM universidade.semestre
        ORDER BY ano, semestre
        """, Ler);

    public Task<List<Opcao>> TurmasAsync() => db.ConsultarAsync("""
        SELECT t.id_turma::text,
               t.id_turma || ' — ' || t.cod_disc || ' ' || d.nome
               || ' (turma ' || COALESCE(t.numero::text, '?') || ', '
               || COALESCE(t.ano || '/' || t.semestre, 'sem semestre') || ')'
        FROM universidade.turma t
        JOIN universidade.disciplina d ON d.cod_disc = t.cod_disc
        ORDER BY t.id_turma
        """, Ler);

    public Task<List<Opcao>> ProfessoresAsync() => db.ConsultarAsync("""
        SELECT p.mat_professor, p.mat_professor || ' — ' || COALESCE(u.nome, '(sem usuário)')
        FROM universidade.professor p
        LEFT JOIN universidade.usuario u ON u.cpf = p.cpf
        ORDER BY length(p.mat_professor), p.mat_professor
        """, Ler);

    /// <summary>
    /// Usuários que ainda não são professores (cpf é UNIQUE em professor),
    /// mais o usuário atual do professor sendo editado.
    /// </summary>
    public Task<List<Opcao>> UsuariosSemProfessorAsync(long? cpfAtual) => db.ConsultarAsync("""
        SELECT u.cpf::text, lpad(u.cpf::text, 11, '0') || ' — ' || u.nome
        FROM universidade.usuario u
        WHERE NOT EXISTS (SELECT 1 FROM universidade.professor p WHERE p.cpf = u.cpf)
           OR u.cpf = @cpf_atual
        ORDER BY u.nome
        """, Ler, P("cpf_atual", cpfAtual is null ? null : (decimal)cpfAtual, NpgsqlDbType.Numeric));

    /// <summary>Valores de um tipo ENUM, lidos do próprio banco.</summary>
    public Task<List<Opcao>> ValoresEnumAsync(TipoEnum tipo)
    {
        var nome = tipo switch
        {
            TipoEnum.Formacao => "tipo_formacao",
            TipoEnum.Jornada => "tipo_jornada",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
        return db.ConsultarAsync(
            $"SELECT v::text, v::text FROM unnest(enum_range(NULL::universidade.{nome})) AS v",
            Ler);
    }
}

public enum TipoEnum { Formacao, Jornada }

public static class OpcaoExtensions
{
    public static List<SelectListItem> ParaSelect(this IEnumerable<Opcao> opcoes) =>
        opcoes.Select(o => new SelectListItem(o.Texto, o.Valor)).ToList();
}
