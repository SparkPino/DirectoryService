using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentName
{
    public string Name { get; }

    private DepartmentName(string name) => Name = name;

    public override string ToString() => $"{Name}";

    public static Result<DepartmentName, Errors> Create(string name)
    {
        name = name.Trim();

        if (!StringValidator<DepartmentName>.For(name)
                .IsNullOrWhiteSpace()
                .LengthMinMax(3, 150)
                .IsValid(out List<Error> errorMessage))
        {
            return new Errors(errorMessage!);
        }


        return new DepartmentName(name);
    }

    public static DepartmentName FromDb(string value)
    {
        return new DepartmentName(value);
    }
}