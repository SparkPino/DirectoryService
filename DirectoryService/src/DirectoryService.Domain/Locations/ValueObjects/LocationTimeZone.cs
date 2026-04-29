using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationTimeZone
{
    public string TimeZone { get; }

    private LocationTimeZone(string timeZone)
    {
        TimeZone = timeZone;
    }

    public static Result<LocationTimeZone, Error> Create(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
        {
            var message = "Timezone не может быть пустой";
            return Error.Validation("location.timezone", message, nameof(TimeZone));
        }


        timeZone = timeZone.Trim();

        if (!TryNormalize(timeZone, out var ianaTimeZone))
        {
            var message = $"Timezone '{timeZone}' не существует. Используй IANA формат, например 'Europe/Warsaw'";
            return Error.Validation("location.timezone", message, nameof(TimeZone));
        }


        return new LocationTimeZone(ianaTimeZone!);
    }

    private static bool TryNormalize(string timeZone, out string? ianaId)
    {
        ianaId = null;

        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);


            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(tz.Id, out var converted))
                ianaId = converted;
            else
                ianaId = tz.Id;

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