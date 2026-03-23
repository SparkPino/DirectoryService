using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public record LocationName
{
    public string Name { get; }

    private LocationName(string name) => Name = name;

    public static Result<LocationName, string> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<LocationName>.For(name)
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 120)
                .IsValid(out string errorMessage))
        {
            return errorMessage!;
        }

        return new LocationName(name);
    }

    public static LocationName FromDb(string value)
    {
        return new LocationName(value);
    }
}