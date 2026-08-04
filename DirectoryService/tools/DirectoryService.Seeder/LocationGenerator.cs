using Bogus;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryService.Seeder;

public static class LocationGenerator
{
    private static readonly string[] TimeZones =
    [
        "Europe/Warsaw", "Europe/Kyiv", "Europe/London", "Europe/Berlin", "Europe/Paris",
        "America/New_York", "America/Los_Angeles", "America/Chicago",
        "Asia/Tokyo", "Asia/Singapore", "Asia/Dubai", "Australia/Sydney",
    ];

    public static List<Location> Generate(Faker faker, int count)
    {
        var locations = new List<Location>(count);

        for (var i = 0; i < count; i++)
        {
            var name = $"{faker.Address.City()} Office";
            var timeZone = faker.PickRandom(TimeZones);
            var apartment = faker.Random.Bool(0.3f) ? faker.Address.SecondaryAddress() : null;

            var addressResult = Address.Create(
                faker.Address.Country(),
                faker.Address.City(),
                faker.Address.StreetName(),
                faker.Address.ZipCode(),
                faker.Address.BuildingNumber(),
                apartment);

            if (addressResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать адрес для '{name}': {addressResult.Error.Message}");
            }

            var nameResult = LocationName.Create(name);
            if (nameResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать название локации '{name}': " +
                    string.Join(", ", nameResult.Error.Select(e => e.Message)));
            }

            var timeZoneResult = LocationTimeZone.Create(timeZone);
            if (timeZoneResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать таймзону '{timeZone}': {timeZoneResult.Error.Message}");
            }

            var location = Location.Create(nameResult.Value, addressResult.Value, timeZoneResult.Value);
            locations.Add(location);
        }

        return locations;
    }
}