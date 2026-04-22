using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public record LocationTimeZone
{
    public string Value { get; }

    private LocationTimeZone(string value)
    {
        Value = value;
    }

    public static Result<LocationTimeZone, string> Create(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
            return "Timezone не может быть пустой";

        timeZone = timeZone.Trim();

        if (!TryNormalize(timeZone, out var ianaTimeZone))
            return $"Timezone '{timeZone}' не существует. Используй IANA формат, например 'Europe/Warsaw'";

        return new LocationTimeZone(ianaTimeZone!);
    }

    private static bool TryNormalize(string timeZone, out string? ianaId)
    {
        ianaId = null;

        // Спроба 1: вже є валідний IANA або Windows ID
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            // Конвертуємо Windows → IANA якщо потрібно
            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(tz.Id, out var converted))
                ianaId = converted;
            else
                ianaId = tz.Id; // вже IANA

            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }

    public static LocationTimeZone FromDb(string timeZone)
    {
        return new LocationTimeZone(timeZone);
    }
}