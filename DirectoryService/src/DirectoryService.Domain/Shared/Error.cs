using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Shared;

public record Error
{
    public static Error None => new(string.Empty, string.Empty, null, null);

    public string Code { get; } // example "lesson.not.found" or "warning.lesson"

    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType? Type { get; }

    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType? type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string? code, string message, string? invalidField = null) =>
        new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message) =>
        new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string message) => new(code ?? "failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message) =>
        new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);

    public static Error Authentication(string? code, string message) =>
        new(code ?? "failure", message, ErrorType.AUTHENTICATION);

    public static Error Authorization(string? code, string message) =>
        new(code ?? "failure", message, ErrorType.AUTHORIZATION);

    public string ToDetails() =>
        $"code: {Code}\nmessage: {Message}\ntype: {Type}";
}