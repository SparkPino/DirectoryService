using Shared;

namespace DirectoryService.Application.Departments.Failures;

public static class DepartmentError
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("department.not_found", $"Department с id: {id} не найдена");

    public static readonly Error Database =
        Error.Failure("department.database", "Не удалось сохранить локацию");

    public static readonly Error Concurrency =
        Error.Failure("department.concurrency", "Department был изменен другим пользователем");

    public static readonly Error InvalidId =
        Error.Validation("invalid.department.id", "Идентификатор департамента не может быть пустым");

    public static Error LocationNotAttached(Guid locationId) =>
        Error.Conflict("department.location.not_attached", $"Локация с id: {locationId} не прикреплена к данному департаменту");
}