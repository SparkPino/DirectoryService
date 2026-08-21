using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Failures;

public static class LocationErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("location.not_found", $"Локация с id: {id} не найдена");

    public static Error NotFoundMany(IEnumerable<Guid> ids) =>
        Error.NotFound("locations.not_found", $"Локации с id: {string.Join(", ", ids)} не найдены");

    public static Error NotFoundMany() =>
        Error.NotFound("locations.not_found", $"Локации не найдены");

    public static readonly Error InvalidId =
        Error.Validation("location.invalid_id", "Идентификатор локации не может быть пустым");

    public static Error EmptyField(string field) =>
        Error.Validation("field.is.empty", "Строка не может быть пустой", field);
}