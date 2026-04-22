using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public record PositionName
{
    public string Name { get; }

    private PositionName(string name) => Name = name;

    public static Result<PositionName, string> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<PositionName>.For(name)
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 100)
                .IsValid(out string errorMessage))
        {
            return errorMessage!;
        }

        return new PositionName(name);
    }

    public static PositionName FromDb(string positionName)
    {
        return new PositionName(positionName);
    }
}