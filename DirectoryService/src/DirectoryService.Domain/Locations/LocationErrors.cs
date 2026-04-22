using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Locations;

public class LocationErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("location.not_found", $"Локация с id {id} не найдена");

    public static readonly Error Database =
        Error.Failure("location.database", "Не удалось сохранить локацию");

    public static readonly Error Concurrency =
        Error.Failure("location.concurrency", "Локация была изменена другим пользователем");
}