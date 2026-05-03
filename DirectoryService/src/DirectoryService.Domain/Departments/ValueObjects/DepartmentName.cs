using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentName
{
    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 150;

    public string Value { get; }

    private DepartmentName(string value) => Value = value;

    public override string ToString() => $"{Value}";

    public static Result<DepartmentName, Errors> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<DepartmentName>.For(name)
                .IsNullOrWhiteSpace()
                .LengthMinMax(NAME_MIN_LENGTH, NAME_MAX_LENGTH)
                .IsValid(out List<Error>? errorMessage))
        {
            return new Errors(errorMessage!);
        }


        return new DepartmentName(name);
    }

    public static DepartmentName FromDb(string name) => new DepartmentName(name);
}