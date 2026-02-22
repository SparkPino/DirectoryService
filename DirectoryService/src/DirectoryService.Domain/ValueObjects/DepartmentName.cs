using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentName
{
    public string Value { get; }

    private DepartmentName(string value) => Value = value;

    public static Result<DepartmentName, string> Create(string value)
    {
        value = value.Trim();

        if (!StringValidator<DepartmentName>.For(value)
            .IsNullOrWhiteSpace()
            .LengthMinMax(3,150)
            .IsValid(out string errorMessage))
        {
            return errorMessage!;
        }

        return new DepartmentName(value);
    }
}