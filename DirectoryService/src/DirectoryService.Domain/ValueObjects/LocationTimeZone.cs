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

        if (!timeZone.Contains('/'))
        {
            return "Timezone должен быть в IANA формате (Area/Location).";
        }

        return new LocationTimeZone(timeZone);
    }

    public static LocationTimeZone FromDb(string timeZone)
    {
        return new LocationTimeZone(timeZone);
    }
}