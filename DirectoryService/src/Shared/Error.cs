using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public string Code { get; } // example "lesson.not.found" or "warning.lesson"

    public string Message { get; }

    //[JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string? code, string? message = null, string? invalidField = null) =>
        new(code ?? "value.is.invalid", message ?? "Введённые данные некорректны", ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string? message = null) =>
        new(code ?? "record.not.found", message ?? "Запись не найдена", ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string? message = null) =>
        new(code ?? "failure", message ?? "Произошла внутренняя ошибка", ErrorType.FAILURE);

    public static Error Conflict(string? code, string? message = null) =>
        new(code ?? "value.is.conflict", message ?? "Запись уже существует", ErrorType.CONFLICT);

    public static Error Authentication(string? code, string? message = null) =>
        new(code ?? "authentication.failure", message ?? "Ошибка аутентификации", ErrorType.AUTHENTICATION);

    public static Error Authorization(string? code, string? message = null) =>
        new(code ?? "authorization.failure", message ?? "Недостаточно прав доступа", ErrorType.AUTHORIZATION);

    public Errors ToErrors() => new([this]);
}