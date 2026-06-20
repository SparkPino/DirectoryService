using Shared;

namespace DirectoryService.Application.Positions.Failures;

public static class PositionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("position.not_found", $"Должность с id: {id} не найдена");

    public static readonly Error InvalidId =
        Error.Validation("position.invalid_id", "Идентификатор должности не может быть пустым");
}