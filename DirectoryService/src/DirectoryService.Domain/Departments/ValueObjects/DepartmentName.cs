using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentName
{
    public string Name { get; }

    private DepartmentName(string name) => Name = name;

    public override string ToString() => $"{Name}";

    public static Result<DepartmentName, string> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<DepartmentName>.For(name)
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 150)
                .IsValid(out string errorMessage))
        {
            return errorMessage!;
        }


        return new DepartmentName(name);
    }

    public static DepartmentName FromDb(string value)
    {
        return new DepartmentName(value);
    }
}