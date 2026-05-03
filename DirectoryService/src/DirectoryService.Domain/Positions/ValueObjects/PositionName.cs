using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Positions.ValueObjects;

public sealed record PositionName
{
    public string Name { get; }

    private PositionName(string name) => Name = name;

    public static Result<PositionName, Errors> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<PositionName>.For(name, nameof(PositionName))
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 100)
                .IsValid(out List<Error> errorMessage))
        {
            return new Errors(errorMessage!);
        }

        return new PositionName(name);
    }

    public static PositionName FromDb(string name) => new PositionName(name);
}