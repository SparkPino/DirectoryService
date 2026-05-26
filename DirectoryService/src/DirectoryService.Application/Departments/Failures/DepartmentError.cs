using Shared;

namespace DirectoryService.Application.Departments.Failures;

public class DepartmentError
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("department.not_found", $"Локация с id: {id} не найдена");

    public static readonly Error Database =
        Error.Failure("department.database", "Не удалось сохранить локацию");

    public static readonly Error Concurrency =
        Error.Failure("department.concurrency", "Локация была изменена другим пользователем");
}