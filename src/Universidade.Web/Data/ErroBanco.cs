using Npgsql;

namespace Universidade.Web.Data;

/// <summary>
/// Traduz os erros do PostgreSQL (SQLSTATE) em mensagens legíveis.
/// </summary>
public static class ErroBanco
{
    public static string Traduzir(PostgresException ex) => ex.SqlState switch
    {
        // unique_violation
        "23505" => ex.ConstraintName switch
        {
            "pk_usuario" => "Já existe um usuário com esse CPF.",
            "usuario_login_key" => "Esse login já está em uso por outro usuário.",
            "pk_professor" => "Já existe um professor com essa matrícula.",
            "professor_cpf_key" => "Esse usuário já está vinculado a outro professor.",
            "uq_turma" => "Já existe uma turma dessa disciplina com esse número no mesmo ano/semestre.",
            "pk_leciona" => "Esse professor já está associado a essa turma.",
            "pk_semestre" => "Esse semestre já está cadastrado.",
            _ => $"Valor duplicado (restrição {ex.ConstraintName})."
        },
        // foreign_key_violation
        "23503" => $"Violação de chave estrangeira ({ex.ConstraintName}).",
        // not_null_violation
        "23502" => $"O campo '{ex.ColumnName}' é obrigatório.",
        // check_violation
        "23514" => $"Valor fora do permitido (restrição {ex.ConstraintName}).",
        // string_data_right_truncation
        "22001" => "Algum texto ultrapassa o tamanho máximo da coluna.",
        // numeric_value_out_of_range
        "22003" => "Número fora do intervalo permitido pela coluna.",
        // invalid_text_representation (ex.: valor inválido para ENUM)
        "22P02" => "Valor em formato inválido para o tipo da coluna.",
        _ => $"Erro do banco ({ex.SqlState}): {ex.MessageText}"
    };
}
