using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationName
{
    public string Name { get; }

    private LocationName(string name) => Name = name;

    public static Result<LocationName, Errors> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<LocationName>.For(name,nameof(LocationName))
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 120)
                .IsValid(out List<Error>? errorMessage))
        {
            return new Errors(errorMessage!);
        }

        return new LocationName(name);
    }

    public static LocationName FromDb(string value)
    {
        return new LocationName(value);
    }
}