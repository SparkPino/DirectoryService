using Shared;

namespace DirectoryService.Application.Departments.Failures;

public static class DepartmentError
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("department.not.found", $"Department с id: {id} не найдена");

    public static readonly Error InvalidId =
        Error.Validation("invalid.department.id", "Идентификатор департамента не может быть пустым");

    public static Error LocationNotAttached(Guid locationId) =>
        Error.Conflict("department.location.not_attached", $"Локация с id: {locationId} не прикреплена к данному департаменту");

    public static Error PositionNotAttached(Guid positionId) =>
        Error.Conflict("department.position.not_attached", $"Должность с id: {positionId} не прикреплена к данному департаменту");
}