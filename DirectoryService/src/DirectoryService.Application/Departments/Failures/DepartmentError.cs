using Shared;

namespace DirectoryService.Application.Departments.Failures;

public class DepartmentError
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("department.not_found", $"Department с id: {id} не найдена");

    public static readonly Error Database =
        Error.Failure("department.database", "Не удалось сохранить локацию");

    public static readonly Error Concurrency =
        Error.Failure("department.concurrency", "Department был изменен другим пользователем");
}